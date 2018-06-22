#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace NodeCanvas.Editor {

    public class TaskEditor : EditorObjectWrapper<Task> {

		private bool isUnfolded = true;
		private EditorPropertyWrapper<TaskAgent> overrideAgentProp;
		private EditorMethodWrapper onTaskInspectorGUI;

		private Task task{get{return target;}}

		protected override void OnInit(){
			overrideAgentProp = CreatePropertyWrapper<TaskAgent>("overrideAgent");
			onTaskInspectorGUI = CreateMethodWrapper("OnTaskInspectorGUI");
		}

		///----------------------------------------------------------------------------------------------

		private static string search = string.Empty;

		///Show a Task's field without ability to add if null or add multiple tasks for form a list.
		public static void TaskFieldSingle(Task task, Action<Task> callback, bool showTitlebar = true){
			if (task != null){ ShowTaskInspectorGUI(task, callback, showTitlebar); }
		}

		///Show a Task's field. If task null allow add task. Multiple tasks can be added to form a list.
		public static void TaskFieldMulti<T>(T task, ITaskSystem ownerSystem, Action<T> callback) where T : Task{
			TaskFieldMulti(task, ownerSystem, typeof(T), (Task t)=> { callback((T)t); });
		}
		
		///Show a Task's field. If task null allow add task. Multiple tasks can be added to form a list.
		public static void TaskFieldMulti(Task task, ITaskSystem ownerSystem, Type baseType, Action<Task> callback){
			//if null simply show an assignment button
			if (task == null){
				ShowCreateTaskSelectionButton(ownerSystem, baseType, callback);
				return;
			}

			//Handle Action/ActionLists so that in GUI level a list is used only when needed
			if (baseType == typeof(ActionTask) ){
				if (!(task is ActionList)){
					ShowCreateTaskSelectionButton(ownerSystem, baseType, (t)=>
						{
							var newList = Task.Create<ActionList>(ownerSystem);
							newList.AddAction( (ActionTask)task );
							newList.AddAction( (ActionTask)t );
							callback(newList);
						});
				}

				ShowTaskInspectorGUI(task, callback);

				if (task is ActionList){
					var list = (ActionList)task;
					if (list.actions.Count == 1){
						callback(list.actions[0]);
					}
				}
				return;
			}

			//Handle Condition/ConditionLists so that in GUI level a list is used only when needed
			if (baseType == typeof(ConditionTask)){
				if (!(task is ConditionList)){
					ShowCreateTaskSelectionButton(ownerSystem, baseType, (t)=>
						{
							var newList = Task.Create<ConditionList>(ownerSystem);
							newList.AddCondition( (ConditionTask)task );
							newList.AddCondition( (ConditionTask)t );
							callback(newList);
						});
				}

				ShowTaskInspectorGUI(task, callback);

				if (task is ConditionList){
					var list = (ConditionList)task;
					if (list.conditions.Count == 1){
						callback(list.conditions[0]);
					}
				}
				return;
			}

			//in all other cases where the base type is not a base ActionTask or ConditionTask,
			//(thus lists can't be used unless the base type IS a list), simple show the inspector.
			ShowTaskInspectorGUI(task, callback);
		}

		///Show the editor inspector of target task
		static void ShowTaskInspectorGUI(Task task, Action<Task> callback, bool showTitlebar = true){
			EditorWrapperFactory.GetEditor<TaskEditor>(task).ShowInspector(callback, showTitlebar);
		}

		//Shows a button that when clicked, pops a context menu with a list of tasks deriving the base type specified. When something is selected the callback is called
		//On top of that it also shows a search field for Tasks
        public static void ShowCreateTaskSelectionButton<T>(ITaskSystem ownerSystem, Action<T> callback) where T : Task{
			ShowCreateTaskSelectionButton(ownerSystem, typeof(T), (Task t)=> { callback((T)t); });
		}

		//Shows a button that when clicked, pops a context menu with a list of tasks deriving the base type specified. When something is selected the callback is called
		//On top of that it also shows a search field for Tasks
		public static void ShowCreateTaskSelectionButton(ITaskSystem ownerSystem, Type baseType, Action<Task> callback){

			Action<Type> TaskTypeSelected = (t)=> {
				var newTask = Task.Create(t, ownerSystem);
				Undo.RecordObject(ownerSystem.contextObject, "New Task");
				callback(newTask);
			};

			Func<GenericMenu> GetMenu = ()=> {
				var menu = EditorUtils.GetTypeSelectionMenu(baseType, TaskTypeSelected);
				if (CopyBuffer.Has<Task>() && baseType.IsAssignableFrom( CopyBuffer.Peek<Task>().GetType()) ){
					menu.AddSeparator("/");
					menu.AddItem(new GUIContent(string.Format("Paste ({0})", CopyBuffer.Peek<Task>().name) ), false, ()=> { callback( CopyBuffer.Get<Task>().Duplicate(ownerSystem) ); });
				}
				return menu;				
			};

			GUI.backgroundColor = Colors.lightBlue;
			var label = "Assign " + baseType.Name.SplitCamelCase();
			if (GUILayout.Button(label)){
				GetMenu().Show(NodeCanvas.Editor.NCPrefs.useBrowser, label, typeof(Task));
			}

			GUI.backgroundColor = Color.white;
			search = EditorUtils.SearchField(search);
			if (!string.IsNullOrEmpty(search)){
				GUILayout.BeginVertical("TextField");
				var itemAdded = false;
				foreach (var taskInfo in EditorUtils.GetScriptInfosOfType(baseType)){
					if (StringUtils.SearchMatch(search, taskInfo.category + "/" + taskInfo.name)){
						itemAdded = true;
						if (GUILayout.Button(taskInfo.name)){
							search = string.Empty;
							GUIUtility.keyboardControl = 0;
							TaskTypeSelected(taskInfo.type);
						}
					}
				}
				if (!itemAdded){
					GUILayout.Label("No results to display with current search input.");
				}
				GUILayout.EndVertical();
			}
		}


		///----------------------------------------------------------------------------------------------


		//Draw the task inspector GUI
		void ShowInspector(Action<Task> callback, bool showTitlebar = true){
			if (task.ownerSystem == null){
				GUILayout.Label("<b>Owner System is null! This should have not happen! Please report a bug</b>");
				return;
			}

			//make sure TaskAgent is not null in case task defines an AgentType
			if (task.agentIsOverride && overrideAgentProp.value == null){
				overrideAgentProp.value = new TaskAgent();
			}

			UndoManager.CheckUndo(task.ownerSystem.contextObject, "Task Inspector");

			if (task.obsolete != string.Empty){
				EditorGUILayout.HelpBox(string.Format("This is an obsolete Task:\n\"{0}\"", task.obsolete), MessageType.Warning);
			}

			if (!showTitlebar || ShowTitlebar(callback) == true){

				if (NCPrefs.showNodeInfo && !string.IsNullOrEmpty(task.description)){
					EditorGUILayout.HelpBox(task.description, MessageType.None);
				}

				// ShowWarnings(task);
				SpecialCaseInspector();
				ShowAgentField();
				onTaskInspectorGUI.Invoke();
			}

			UndoManager.CheckDirty(task.ownerSystem.contextObject);
		}

		void ShowWarnings(){
			if (task.firstWarningMessage != null){
				GUILayout.BeginHorizontal("box");
				GUILayout.Box(Icons.warningIcon, GUIStyle.none, GUILayout.Width(16));
				GUILayout.Label(string.Format("<size=9>{0}</size>", task.firstWarningMessage) );
				GUILayout.EndHorizontal();
			}
		}

		//Some special cases for Action & Condition. A bit weird but better that creating a virtual method in this case
		void SpecialCaseInspector(){
			
			if (task is ActionTask){
				if (Application.isPlaying){
					if ( (task as ActionTask).elapsedTime > 0) GUI.color = Color.yellow;
					EditorGUILayout.LabelField("Elapsed Time", (task as ActionTask).elapsedTime.ToString());
					GUI.color = Color.white;
				}
			}

			if (task is ConditionTask){
				GUI.color = (task as ConditionTask).invert? Color.white : new Color(1f,1f,1f,0.5f);
				(task as ConditionTask).invert = EditorGUILayout.ToggleLeft("Invert Condition", (task as ConditionTask).invert);
				GUI.color = Color.white;
			}
		}

		//a Custom titlebar for tasks
		bool ShowTitlebar(Action<Task> callback){

			GUI.backgroundColor = new Color(1,1,1,0.8f);
			GUILayout.BeginHorizontal("box");
			GUI.backgroundColor = Color.white;

			GUILayout.Label("<b>" + (isUnfolded? "▼ " :"► ") + task.name + "</b>" + (isUnfolded? "" : "\n<i><size=10>(" + task.summaryInfo + ")</size></i>"), Styles.leftLabel );

			if (GUILayout.Button(Icons.csIcon, GUI.skin.label, GUILayout.Width(20), GUILayout.Height(20))){
				EditorUtils.OpenScriptOfType(task.GetType());
			}

			if (GUILayout.Button(Icons.gearPopupIcon, Styles.centerLabel, GUILayout.Width(20), GUILayout.Height(20))){
				GetMenu(callback).ShowAsContext();
			}

			GUILayout.EndHorizontal();
			var titleRect = GUILayoutUtility.GetLastRect();
			EditorGUIUtility.AddCursorRect(titleRect, MouseCursor.Link);

			var e = Event.current;
			if (e.type == EventType.ContextClick && titleRect.Contains(e.mousePosition)){
				GetMenu(callback).ShowAsContext();
				e.Use();
			}

			if (e.button == 0 && e.type == EventType.MouseUp && titleRect.Contains(e.mousePosition)){
				isUnfolded = !isUnfolded;
				e.Use();
			}

			return isUnfolded;
		}

		///Generate and return task menu
		GenericMenu GetMenu(Action<Task> callback){
			var menu = new GenericMenu();
			menu.AddItem(new GUIContent("Open Script"), false, ()=> { EditorUtils.OpenScriptOfType(task.GetType()); } );
			menu.AddItem(new GUIContent("Copy"), false, ()=> { CopyBuffer.Set<Task>(task); } );

			foreach(var _m in task.GetType().RTGetMethods()){
				var m = _m;
				var att = m.RTGetAttribute<ContextMenu>(true);
				if (att != null){
					menu.AddItem(new GUIContent(att.menuItem), false, ()=>{ m.Invoke(task, null); });
				}
			}

			menu.AddSeparator("/");

			menu.AddItem(new GUIContent("Delete"), false, ()=>
			{
				if (callback != null){
					callback(null);
				}
			});

			return menu;
		}

		//Shows the agent field in case an agent type is specified either with [AgentType] attribute or through the use of the generic versions of Action or Condition Task
		void ShowAgentField(){

			if (task.agentType == null){
				return;
			}

			TaskAgent taskAgent = overrideAgentProp.value;

			if (Application.isPlaying && task.agentIsOverride && taskAgent.value == null){
				GUI.color = Colors.lightRed;
				GUILayout.Label("<b>!Missing Agent Reference!</b>");
				GUI.color = Color.white;
				return;
			}


			var isMissingType = task.agent == null;
			var infoString = isMissingType? "<color=#ff5f5f>" + task.agentType.FriendlyName() + "</color>": task.agentType.FriendlyName();

			GUI.color = new Color(1f,1f,1f, task.agentIsOverride? 1f : 0.5f);
			GUI.backgroundColor = GUI.color;
			GUILayout.BeginVertical(task.agentIsOverride? "box" : "button");
			GUILayout.BeginHorizontal();


			if (task.agentIsOverride){

				if (taskAgent.useBlackboard){

					GUI.color = new Color(0.9f,0.9f,1f,1f);
					var varNames = new List<string>();
					
					//Local
					if (taskAgent.bb != null){
						varNames.AddRange(taskAgent.bb.GetVariableNames(typeof(GameObject)));
						varNames.AddRange(taskAgent.bb.GetVariableNames(typeof(Component)));
						if (task.agentType.IsInterface){
							varNames.AddRange(taskAgent.bb.GetVariableNames(task.agentType));
						}
					}

					//Globals
					foreach (var globalBB in GlobalBlackboard.allGlobals){

						varNames.Add(globalBB.name + "/");
						
						var globalVars = new List<string>();
						globalVars.AddRange( globalBB.GetVariableNames(typeof(GameObject)));
						globalVars.AddRange( globalBB.GetVariableNames(typeof(Component)));
						if (task.agentType.IsInterface){
							globalVars.AddRange( globalBB.GetVariableNames(task.agentType));
						}

						for (var i = 0; i < globalVars.Count; i++){
							globalVars[i] = globalBB.name + "/" + globalVars[i];
						}

						varNames.AddRange( globalVars );
					}

					//No Dynamic for AgentField for convenience and user error safety
					varNames.Add("(DynamicVar)");


					if (varNames.Contains(taskAgent.name) || string.IsNullOrEmpty(taskAgent.name) ){
						taskAgent.name = EditorUtils.StringPopup(taskAgent.name, varNames, false, true);
						if (taskAgent.name == "(DynamicVar)"){
							taskAgent.name = "_";
						}

					} else {
						taskAgent.name = EditorGUILayout.TextField(taskAgent.name);
					}

				} else {

					taskAgent.value = EditorGUILayout.ObjectField(taskAgent.value, task.agentType, true) as Component;
				}

			} else {

				GUILayout.BeginHorizontal();
				var icon = UserTypePrefs.GetTypeIcon(task.agentType);
				var label = string.Format("Use Self ({0})", infoString);
				var content = new GUIContent(label, icon);
				GUILayout.Label(content, GUILayout.Height(16));
				GUILayout.EndHorizontal();				
			}


			GUI.color = Color.white;

			if (task.agentIsOverride){
				if (isMissingType){
					GUILayout.Label("(" + infoString + ")", GUILayout.Height(15));
				}
				taskAgent.useBlackboard = EditorGUILayout.Toggle(taskAgent.useBlackboard, EditorStyles.radioButton, GUILayout.Width(18));
			}

			GUI.color = Color.white;
			GUI.backgroundColor = Color.white;

			if (!Application.isPlaying){
				task.agentIsOverride = EditorGUILayout.Toggle(task.agentIsOverride, GUILayout.Width(18));
			}

			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}

	}
}

#endif