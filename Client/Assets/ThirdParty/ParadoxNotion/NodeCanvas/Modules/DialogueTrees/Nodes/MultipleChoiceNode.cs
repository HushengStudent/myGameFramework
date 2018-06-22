using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.DialogueTrees{

	[Icon("List")]
	[Name("Multiple Choice")]
	[Category("Branch")]
	[Description("Prompt a Dialogue Multiple Choice. A choice will be available if the connection's condition is true or there is no condition on that connection. The Actor selected is used for the Condition checks as well as will Say the selection if the option is checked.")]
	[Color("b3ff7f")]
	public class MultipleChoiceNode : DTNode, ISubTasksContainer{

		[System.Serializable]
		public class Choice{
			public bool isUnfolded = true;
			public Statement statement;
			public ConditionTask condition;
			public Choice(){}
			public Choice(Statement statement){
				this.statement = statement;
			}
		}


		[SliderField(0f,10f)]
		public float availableTime = 0;
		public bool saySelection = false;
		[SerializeField]
		private List<Choice> availableChoices = new List<Choice>();

		public Task[] GetSubTasks(){ return availableChoices != null? availableChoices.Select(c => c.condition).ToArray() : new Task[0]; }		
		public override int maxOutConnections{ get{return availableChoices.Count;} }
		public override bool requireActorSelection{ get {return true;} }

		protected override Status OnExecute(Component agent, IBlackboard bb){

			if (outConnections.Count == 0)
				return Error("There are no connections to the Multiple Choice Node!");

			var finalOptions = new Dictionary<IStatement, int>();

			for (var i = 0; i < availableChoices.Count; i++){
				var condition = availableChoices[i].condition;
				if (condition == null || condition.CheckCondition(finalActor.transform, bb)){
					var tempStatement = availableChoices[i].statement.BlackboardReplace(bb);
					finalOptions[tempStatement] = i;					
				}
			}

			if (finalOptions.Count == 0){
				Debug.Log("Multiple Choice Node has no available options. Dialogue Ends");
				DLGTree.Stop(false);
				return Status.Failure;
			}

			var optionsInfo = new MultipleChoiceRequestInfo(finalActor, finalOptions, availableTime, OnOptionSelected);
			optionsInfo.showLastStatement = inConnections.Count > 0 && inConnections[0].sourceNode is StatementNode;
			DialogueTree.RequestMultipleChoices( optionsInfo );
			return Status.Running;
		}

		void OnOptionSelected(int index){

			status = Status.Success;

			System.Action Finalize = ()=> { DLGTree.Continue(index); };

			if (saySelection){
				var tempStatement = availableChoices[index].statement.BlackboardReplace(graphBlackboard);
				var speechInfo = new SubtitlesRequestInfo( finalActor, tempStatement, Finalize );
				DialogueTree.RequestSubtitles(speechInfo);
			} else {
				Finalize();
			}
		}

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		//every label witin OnNodeGUI has it's alignment set to middle, but we need a left here.
		GUIStyle _leftLabelStyle;
		GUIStyle leftLabelStyle{
			get
			{
				if (_leftLabelStyle == null){
					_leftLabelStyle = new GUIStyle(GUI.skin.GetStyle("label"));
					_leftLabelStyle.alignment = TextAnchor.UpperLeft;
				}
				return _leftLabelStyle;
			}
		}

		public override void OnConnectionInspectorGUI(int i){
			DoChoiceGUI(availableChoices[i]);
		}

		public override string GetConnectionInfo(int i){
			if (i >= availableChoices.Count){
				return "NOT SET";
			}
			var text = string.Format("'{0}'", availableChoices[i].statement.text);
			if (availableChoices[i].condition == null){
				return text;
			}
			return string.Format("{0}\n{1}", text, availableChoices[i].condition.summaryInfo );
		}


		protected override void OnNodeGUI(){

			if (availableChoices.Count == 0){
				GUILayout.Label("No Options Available");
				return;
			}

			for (var i = 0; i < availableChoices.Count; i++){
				var choice = availableChoices[i];
				var connection = i < outConnections.Count? outConnections[i] : null;
				GUILayout.BeginHorizontal("box");
				GUILayout.Label(string.Format("(#{0}) {1}", connection != null? connection.targetNode.ID.ToString() : "NONE", choice.statement.text ), leftLabelStyle );
				GUILayout.EndHorizontal();
			}

			GUILayout.BeginHorizontal();
			if (availableTime > 0){
				GUILayout.Label("Choose in '" + availableTime + "' seconds");
			}
			if (saySelection){
				GUILayout.Label("Say Selection");
			}
			GUILayout.EndHorizontal();
		}

		protected override void OnNodeInspectorGUI(){

			base.OnNodeInspectorGUI();

			if (GUILayout.Button("Add Choice")){
				availableChoices.Add(new Choice(new Statement("I am a choice...")));
			}

			if (availableChoices.Count == 0){
				return;
			}

			EditorUtils.ReorderableList(availableChoices, (i, picked) =>
			{
				var choice = availableChoices[i];
				GUILayout.BeginHorizontal("box");

				var text = string.Format("{0} {1}", choice.isUnfolded? "▼ " : "► ", choice.statement.text);
				if (GUILayout.Button(text, (GUIStyle)"label", GUILayout.Width(0), GUILayout.ExpandWidth(true)) ) {
					choice.isUnfolded = !choice.isUnfolded;
				}

				if (GUILayout.Button("X", GUILayout.Width(20))){
					availableChoices.RemoveAt(i);
					if (i < outConnections.Count){
						graph.RemoveConnection(outConnections[i]);
					}
				}

				GUILayout.EndHorizontal();

				if (choice.isUnfolded){
					DoChoiceGUI(choice);
				}
			});

		}

		void DoChoiceGUI(Choice choice){
			GUILayout.BeginHorizontal();
			GUILayout.Space(10);
			GUILayout.BeginVertical("box");

			choice.statement.text = UnityEditor.EditorGUILayout.TextField(choice.statement.text);
			choice.statement.audio = UnityEditor.EditorGUILayout.ObjectField("Audio File", choice.statement.audio, typeof(AudioClip), false) as AudioClip;
			choice.statement.meta = UnityEditor.EditorGUILayout.TextField("Meta Data", choice.statement.meta);

			NodeCanvas.Editor.TaskEditor.TaskFieldMulti<ConditionTask>(choice.condition, graph, (c)=> { choice.condition = c; });

			GUILayout.EndVertical();
			GUILayout.EndHorizontal();

			GUILayout.Space(10);			
		}

		#endif
	}
}