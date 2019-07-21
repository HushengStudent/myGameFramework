using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.DialogueTrees
{

    /// Use DialogueTrees to create Dialogues between Actors
    [GraphInfo(
        packageName = "NodeCanvas",
        docsURL = "http://nodecanvas.paradoxnotion.com/documentation/",
        resourcesURL = "http://nodecanvas.paradoxnotion.com/downloads/",
        forumsURL = "http://nodecanvas.paradoxnotion.com/forums-page/"
        )]
    [CreateAssetMenu(menuName = "ParadoxNotion/NodeCanvas/Dialogue Tree Asset")]
    public class DialogueTree : Graph
    {

        ///----------------------------------------------------------------------------------------------
        [System.Serializable]
        struct DerivedSerializationData
        {
            public List<ActorParameter> actorParameters;
        }

        public override object OnDerivedDataSerialization() {
            var data = new DerivedSerializationData();
            data.actorParameters = this._actorParameters;
            return data;
        }

        public override void OnDerivedDataDeserialization(object data) {
            if ( data is DerivedSerializationData ) {
                this._actorParameters = ( (DerivedSerializationData)data ).actorParameters;
            }
        }
        ///----------------------------------------------------------------------------------------------

        [System.Serializable]
        public class ActorParameter
        {

            [SerializeField]
            private string _keyName;
            [SerializeField]
            private string _id;
            [SerializeField]
            private UnityEngine.Object _actorObject;
            [System.NonSerialized]
            private IDialogueActor _actor;

            public string name {
                get { return _keyName; }
                set { _keyName = value; }
            }

            public string ID {
                get { return string.IsNullOrEmpty(_id) ? _id = System.Guid.NewGuid().ToString() : _id; }
            }

            public IDialogueActor actor {
                get
                {
                    if ( _actor == null ) {
                        _actor = _actorObject as IDialogueActor;
                    }
                    return _actor;
                }
                set
                {
                    _actor = value;
                    _actorObject = value as UnityEngine.Object;
                }
            }

            public ActorParameter() { }
            public ActorParameter(string name) {
                this.name = name;
            }
            public ActorParameter(string name, IDialogueActor actor) {
                this.name = name;
                this.actor = actor;
            }

            public override string ToString() {
                return name;
            }
        }


        public const string INSTIGATOR_NAME = "INSTIGATOR";

        [SerializeField]
        private List<ActorParameter> _actorParameters = new List<ActorParameter>();

        private DTNode currentNode;

        public static event Action<DialogueTree> OnDialogueStarted;
        public static event Action<DialogueTree> OnDialoguePaused;
        public static event Action<DialogueTree> OnDialogueFinished;
        public static event Action<SubtitlesRequestInfo> OnSubtitlesRequest;
        public static event Action<MultipleChoiceRequestInfo> OnMultipleChoiceRequest;
        public static DialogueTree currentDialogue { get; private set; }
        public static DialogueTree previousDialogue { get; private set; }

        public override System.Type baseNodeType { get { return typeof(DTNode); } }
        public override bool requiresAgent { get { return false; } }
        public override bool requiresPrimeNode { get { return true; } }
        public override bool isTree { get { return true; } }
        public override bool useLocalBlackboard { get { return true; } }
        sealed public override bool canAcceptVariableDrops { get { return false; } }

        ///The dialogue actor parameters
        public List<ActorParameter> actorParameters {
            get { return _actorParameters; }
        }


        //A list of the defined names for the involved actor parameters
        public List<string> definedActorParameterNames {
            get
            {
                var list = actorParameters.Select(r => r.name).ToList();
                list.Insert(0, INSTIGATOR_NAME);
                return list;
            }
        }

        //Returns the ActorParameter by id
        public ActorParameter GetParameterByID(string id) {
            return actorParameters.Find(p => p.ID == id);
        }

        //Returns the ActorParameter by name
        public ActorParameter GetParameterByName(string paramName) {
            return actorParameters.Find(p => p.name == paramName);
        }

        //Returns the actor by parameter id.
        public IDialogueActor GetActorReferenceByID(string id) {
            var param = GetParameterByID(id);
            return param != null ? GetActorReferenceByName(param.name) : null;
        }

        ///Resolves and gets an actor based on the key name
        public IDialogueActor GetActorReferenceByName(string paramName) {

            //Check for INSTIGATOR selection
            if ( paramName == INSTIGATOR_NAME ) {

                //return it directly if it implements IDialogueActor
                if ( agent is IDialogueActor ) {
                    return (IDialogueActor)agent;
                }

                //Otherwise use the default actor and set name and transform from agent
                if ( agent != null ) {
                    return new ProxyDialogueActor(agent.gameObject.name, agent.transform);
                }

                return new ProxyDialogueActor("Null Instigator", null);
            }

            //Check for non INSTIGATOR selection. If there IS an actor reference return it
            var refData = actorParameters.Find(r => r.name == paramName);
            if ( refData != null && refData.actor != null ) {
                return refData.actor;
            }

            //Otherwise use the default actor and set the name to the key and null transform
            Debug.Log(string.Format("<b>DialogueTree:</b> An actor entry '{0}' on DialogueTree has no reference. A dummy Actor will be used with the entry Key for name", paramName), this);
            return new ProxyDialogueActor(paramName, null);
        }


        ///Set the target IDialogueActor for the provided key parameter name
        public void SetActorReference(string paramName, IDialogueActor actor) {
            var param = actorParameters.Find(p => p.name == paramName);
            if ( param == null ) {
                Debug.LogError(string.Format("There is no defined Actor key name '{0}'", paramName));
                return;
            }
            param.actor = actor;
        }

        ///Set all target IDialogueActors at once by provided dictionary
        public void SetActorReferences(Dictionary<string, IDialogueActor> actors) {
            foreach ( var pair in actors ) {
                var param = actorParameters.Find(p => p.name == pair.Key);
                if ( param == null ) {
                    Debug.LogWarning(string.Format("There is no defined Actor key name '{0}'. Seting actor skiped", pair.Key));
                    continue;
                }
                param.actor = pair.Value;
            }
        }


        ///Continues the DialogueTree at provided child connection index of currentNode
        public void Continue(int index = 0) {

            if ( !isRunning ) {
                return;
            }

            if ( index < 0 || index > currentNode.outConnections.Count - 1 ) {
                Stop(true);
                return;
            }

            EnterNode((DTNode)currentNode.outConnections[index].targetNode);
        }

        ///Enters the provided node
        public void EnterNode(DTNode node) {
            currentNode = node;
            currentNode.Reset(false);
            if ( currentNode.Execute(agent, blackboard) == Status.Error ) {
                Stop(false);
            }
        }




        ///Raise the OnSubtitlesRequest event
        public static void RequestSubtitles(SubtitlesRequestInfo info) {
            if ( OnSubtitlesRequest != null )
                OnSubtitlesRequest(info);
            else Debug.LogWarning("<b>DialogueTree:</b> Subtitle Request event has no subscribers. Make sure to add the default '@DialogueGUI' prefab or create your own GUI.");
        }

        ///Raise the OnMultipleChoiceRequest event
        public static void RequestMultipleChoices(MultipleChoiceRequestInfo info) {
            if ( OnMultipleChoiceRequest != null )
                OnMultipleChoiceRequest(info);
            else Debug.LogWarning("<b>DialogueTree:</b> Multiple Choice Request event has no subscribers. Make sure to add the default '@DialogueGUI' prefab or create your own GUI.");
        }



        protected override void OnGraphStarted() {

            previousDialogue = currentDialogue;
            currentDialogue = this;

            Debug.Log(string.Format("<b>DialogueTree:</b> Dialogue Started '{0}'", this.name));
            if ( OnDialogueStarted != null ) {
                OnDialogueStarted(this);
            }

            if ( !( agent is IDialogueActor ) ) {
                Debug.Log("<b>DialogueTree:</b> INSTIGATOR agent used in DialogueTree does not implement IDialogueActor. A dummy actor will be used.");
            }

            currentNode = currentNode != null ? currentNode : (DTNode)primeNode;
            EnterNode(currentNode);
        }

        protected override void OnGraphUnpaused() {
            currentNode = currentNode != null ? currentNode : (DTNode)primeNode;
            EnterNode(currentNode);

            Debug.Log(string.Format("<b>DialogueTree:</b> Dialogue Resumed '{0}'", this.name));
            if ( OnDialogueStarted != null ) {
                OnDialogueStarted(this);
            }
        }

        protected override void OnGraphStoped() {

            currentDialogue = previousDialogue;
            previousDialogue = null;

            currentNode = null;

            Debug.Log(string.Format("<b>DialogueTree:</b> Dialogue Finished '{0}'", this.name));
            if ( OnDialogueFinished != null ) {
                OnDialogueFinished(this);
            }
        }

        protected override void OnGraphPaused() {

            Debug.Log(string.Format("<b>DialogueTree:</b> Dialogue Paused '{0}'", this.name));
            if ( OnDialoguePaused != null ) {
                OnDialoguePaused(this);
            }
        }


        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        [UnityEditor.MenuItem("Tools/ParadoxNotion/NodeCanvas/Create/Dialogue Tree Object", false, 0)]
        static void Editor_CreateGraph() {
            var dt = new GameObject("DialogueTree").AddComponent<DialogueTreeController>();
            UnityEditor.Selection.activeObject = dt;
        }

#endif
    }
}