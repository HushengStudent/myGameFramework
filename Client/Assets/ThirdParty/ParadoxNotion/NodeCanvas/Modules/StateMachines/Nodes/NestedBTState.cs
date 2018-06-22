#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using System.Linq;
using NodeCanvas.BehaviourTrees;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.StateMachines{

	[Name("BehaviourTree")]
	[Category("Nested")]
	[Description("Execute a Behaviour Tree OnEnter. OnExit that Behavior Tree will be stoped or paused based on the relevant specified setting. You can optionaly specify a Success Event and a Failure Event which will be sent when the BT's root node status returns either of the two. If so, use alongside with a CheckEvent on a transition.")]
	public class NestedBTState : FSMState, IGraphAssignable{

		public enum BTExecutionMode
		{
			Once,
			Repeat
		}

		public enum BTExitMode
		{
			StopAndRestart,
			PauseAndResume
		}

		[SerializeField]
		private BBParameter<BehaviourTree> _nestedBT = null;

		public BTExecutionMode executionMode = BTExecutionMode.Repeat;
		public BTExitMode exitMode = BTExitMode.StopAndRestart;
		public string successEvent;
		public string failureEvent;
	
		private Dictionary<BehaviourTree, BehaviourTree> instances = new Dictionary<BehaviourTree, BehaviourTree>();
		private BehaviourTree currentInstance = null;

		public BehaviourTree nestedBT{
			get {return _nestedBT.value;}
			set {_nestedBT.value = value;}
		}

		Graph IGraphAssignable.nestedGraph{
			get {return nestedBT;}
			set {nestedBT = (BehaviourTree)value;}
		}

		Graph[] IGraphAssignable.GetInstances(){ return instances.Values.ToArray(); }

		////

		protected override void OnEnter(){

			if (nestedBT == null){
				Finish(false);
				return;
			}

			currentInstance = CheckInstance();

			currentInstance.repeat = (executionMode == BTExecutionMode.Repeat);
			currentInstance.updateInterval = 0;
			currentInstance.StartGraph(graphAgent, graphBlackboard, false, OnFinish);
		}

		protected override void OnUpdate(){
			
			currentInstance.UpdateGraph();

			if (!string.IsNullOrEmpty(successEvent) && currentInstance.rootStatus == Status.Success){
				currentInstance.Stop(true);
			}

			if (!string.IsNullOrEmpty(failureEvent) && currentInstance.rootStatus == Status.Failure){
				currentInstance.Stop(false);
			}	
		}

		void OnFinish(bool success){
			if (this.status == Status.Running){
				if (!string.IsNullOrEmpty(successEvent) && success){
					SendEvent(new EventData(successEvent));
				}

				if (!string.IsNullOrEmpty(failureEvent) && !success){
					SendEvent(new EventData(failureEvent));
				}
				
				Finish(success);
			}
		}


		protected override void OnExit(){
			if (currentInstance != null && currentInstance.isRunning){
				if (exitMode == BTExitMode.StopAndRestart){
					currentInstance.Stop();
				} else {
					currentInstance.Pause();
				}
			}
		}

		protected override void OnPause(){
			if (currentInstance != null && currentInstance.isRunning){
				currentInstance.Pause();
			}
		}

		BehaviourTree CheckInstance(){

			if (nestedBT == currentInstance){
				return currentInstance;
			}

			BehaviourTree instance = null;
			if (!instances.TryGetValue(nestedBT, out instance)){
				instance = Graph.Clone<BehaviourTree>(nestedBT);
				instances[nestedBT] = instance;
			}

            instance.agent = graphAgent;
		    instance.blackboard = graphBlackboard;
			nestedBT = instance;
			return instance;
		}

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR
		
		protected override void OnNodeGUI(){
			
			GUILayout.Label(string.Format("Sub BT\n{0}", _nestedBT) );
			if (nestedBT == null){
				if (!Application.isPlaying && GUILayout.Button("CREATE NEW")){
					Node.CreateNested<BehaviourTree>(this);
				}
			}
		}

		protected override void OnNodeInspectorGUI(){

			ShowBaseFSMInspectorGUI();
			NodeCanvas.Editor.BBParameterEditor.ParameterField("Behaviour Tree", _nestedBT);

			if (nestedBT == null){
				return;
			}

			executionMode = (BTExecutionMode)EditorGUILayout.EnumPopup("Execution Mode", executionMode);
			exitMode = (BTExitMode)EditorGUILayout.EnumPopup("Exit Mode", exitMode);

			var alpha1 = string.IsNullOrEmpty(successEvent)? 0.5f : 1;
			var alpha2 = string.IsNullOrEmpty(failureEvent)? 0.5f : 1;
			GUILayout.BeginVertical("box");
			GUI.color = new Color(1,1,1,alpha1);
			successEvent = EditorGUILayout.TextField("Success Status Event", successEvent);
			GUI.color = new Color(1,1,1,alpha2);
			failureEvent = EditorGUILayout.TextField("Failure Status Event", failureEvent);
			GUILayout.EndVertical();
			GUI.color = Color.white;

	    	var defParams = nestedBT.GetDefinedParameters();
	    	if (defParams.Length != 0){
		    	EditorUtils.TitledSeparator("Defined Nested BT Parameters");
		    	GUI.color = Color.yellow;
		    	EditorGUILayout.LabelField("Name", "Type");
				GUI.color = Color.white;
		    	var added = new List<string>();
		    	foreach(var bbVar in defParams){
		    		if (!added.Contains(bbVar.name)){
			    		EditorGUILayout.LabelField(bbVar.name, bbVar.varType.FriendlyName());
			    		added.Add(bbVar.name);
			    	}
		    	}
		    	if (GUILayout.Button("Check/Create Blackboard Variables")){
		    		nestedBT.PromoteDefinedParametersToVariables(graphBlackboard);
		    	}
		    }
		}

		#endif
	}
}