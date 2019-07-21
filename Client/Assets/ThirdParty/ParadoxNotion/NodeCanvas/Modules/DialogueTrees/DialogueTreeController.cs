using System;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;

namespace NodeCanvas.DialogueTrees
{

    [AddComponentMenu("NodeCanvas/Dialogue Tree Controller")]
    public class DialogueTreeController : GraphOwner<DialogueTree>, IDialogueActor
    {

        string IDialogueActor.name { get { return name; } }
        Texture2D IDialogueActor.portrait { get { return null; } }
        Sprite IDialogueActor.portraitSprite { get { return null; } }
        Color IDialogueActor.dialogueColor { get { return Color.white; } }
        Vector3 IDialogueActor.dialoguePosition { get { return Vector3.zero; } }
        Transform IDialogueActor.transform { get { return transform; } }


        ///Start the DialogueTree without an Instigator
        public void StartDialogue() {
            StartDialogue(this, null);
        }

        ///Start the DialogueTree with a callback for when its finished
        public void StartDialogue(Action<bool> callback) {
            StartDialogue(this, callback);
        }

        ///Start the DialogueTree with provided actor as Instigator
        public void StartDialogue(IDialogueActor instigator) {
            StartDialogue(instigator, null);
        }

        ///Start the DialogueTree with provded actor as instigator and callback
        public void StartDialogue(IDialogueActor instigator, Action<bool> callback) {
            graph = GetInstance(graph);
            graph.StartGraph(instigator is Component ? (Component)instigator : instigator.transform, blackboard, true, callback);
        }

        ///Pause the DialogueTree
        public void PauseDialogue() {
            graph.Pause();
        }

        ///Stop the DialogueTree
        public void StopDialogue() {
            graph.Stop();
        }

        ///Set an actor reference by parameter name
        public void SetActorReference(string paramName, IDialogueActor actor) {
            if ( behaviour != null ) {
                behaviour.SetActorReference(paramName, actor);
            }
        }

        ///Set all actor reference parameters at once
        public void SetActorReferences(Dictionary<string, IDialogueActor> actors) {
            if ( behaviour != null ) {
                behaviour.SetActorReferences(actors);
            }
        }

        ///Get the actor reference by parameter name
        public IDialogueActor GetActorReferenceByName(string paramName) {
            return behaviour != null ? behaviour.GetActorReferenceByName(paramName) : null;
        }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        new void Reset() {
            base.enableAction = EnableAction.DoNothing;
            base.disableAction = DisableAction.DoNothing;
            SetBoundGraphReference(ScriptableObject.CreateInstance<DialogueTree>());
        }

#endif
    }
}