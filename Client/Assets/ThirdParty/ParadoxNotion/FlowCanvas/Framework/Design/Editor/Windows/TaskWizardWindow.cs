#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;


namespace NodeCanvas.Editor{

	public class TaskWizardWindow : EditorWindow {

		enum TaskType{Action, Condition}
		TaskType type = TaskType.Action;

		string taskName;
		string category;
	    string description;
		string agentType;
		string ns;

		public static void ShowWindow(){
			var window = ScriptableObject.CreateInstance(typeof(TaskWizardWindow)) as TaskWizardWindow;
			window.ShowUtility();
		}

		void OnEnable(){
	        titleContent = new GUIContent("NC Task Wizard");
		}

		void OnGUI(){
			type        = (TaskType)EditorGUILayout.EnumPopup("Task Type", type);
			taskName    = EditorGUILayout.TextField("Task Name", taskName);
			ns          = EditorGUILayout.TextField("Namespace", ns);
			category    = EditorGUILayout.TextField("Category(?)", category);
			description = EditorGUILayout.TextField("Description(?)", description);
			agentType   = EditorGUILayout.TextField("Agent Type(?)", agentType);

			if (GUILayout.Button("CREATE")){

				if (string.IsNullOrEmpty(taskName)){
					EditorUtility.DisplayDialog("Empty Task Name", "Please give the new task a name","OK");
					return;
				}

				if (type == TaskType.Action){
					CreateFile(GetActionTemplate());
				}

				if (type == TaskType.Condition){
					CreateFile(GetCoditionTemplate());
				}

				taskName = "";
				GUIUtility.hotControl = 0;
				GUIUtility.keyboardControl = 0;
			}

			if (type == TaskType.Action){
				GUILayout.Label(GetActionTemplate());
			}

			if (type == TaskType.Condition){
				GUILayout.Label(GetCoditionTemplate());
			}
		}

		void CreateFile(string template){
			
			var path = GetUniquePath();

			if (System.IO.File.Exists(path)){
				if (!EditorUtility.DisplayDialog("File Exists", "Overwrite file?","YES", "NO")){
					return;
				}
			}

			System.IO.File.WriteAllText(path, template);
			UnityEditor.AssetDatabase.Refresh();
			Debug.LogWarning("New Task is placed under: " + path);
		}

		string GetUniquePath(){
			var path = AssetDatabase.GetAssetPath (Selection.activeObject);
			if (path == ""){
			    path = "Assets";
			}
			if (System.IO.Path.GetExtension(path) != ""){
			    path = path.Replace(System.IO.Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
			}
			return AssetDatabase.GenerateUniqueAssetPath(path + "/" + taskName + ".cs");
		}

		string GetActionTemplate(){
			return
            "using NodeCanvas.Framework;\n" +
            "using ParadoxNotion.Design;\n" +
			"\n\n" +
			"namespace " + (string.IsNullOrEmpty(ns)? "NodeCanvas.Tasks.Actions" : ns) + "{\n\n" + 
			(!string.IsNullOrEmpty(category)? "\t[Category(\"" + category + "\")]\n" : "") +
            (!string.IsNullOrEmpty(description) ? "\t[Description(\"" + description + "\")]\n" : "") +
			"\tpublic class " + taskName + " : ActionTask" + (!string.IsNullOrEmpty(agentType)? ("<" + agentType + ">") : "") + "{\n\n" +
			"\t\t//Use for initialization. This is called only once in the lifetime of the task.\n" +
			"\t\t//Return null if init was successfull. Return an error string otherwise\n" +
			"\t\tprotected override string OnInit(){\n" +
			"\t\t\treturn null;\n" +
			"\t\t}\n\n" +
			"\t\t//This is called once each time the task is enabled.\n" +
			"\t\t//Call EndAction() to mark the action as finished, either in success or failure.\n" +
			"\t\t//EndAction can be called from anywhere.\n" +
			"\t\tprotected override void OnExecute(){\n" +
			"\t\t\tEndAction(true);\n" +
			"\t\t}\n\n" +
			"\t\t//Called once per frame while the action is active.\n" +
			"\t\tprotected override void OnUpdate(){\n" +
			"\t\t\t\n" + 
			"\t\t}\n\n" +
			"\t\t//Called when the task is disabled.\n" +
			"\t\tprotected override void OnStop(){\n" +
			"\t\t\t\n" +
			"\t\t}\n\n" + 
			"\t\t//Called when the task is paused.\n" +
			"\t\tprotected override void OnPause(){\n" +
			"\t\t\t\n" +
			"\t\t}\n" + 
			"\t}\n" +
			"}";
		}

		string GetCoditionTemplate(){	
			return
            "using NodeCanvas.Framework;\n" +
            "using ParadoxNotion.Design;\n" +
			"\n\n" +
			"namespace " + (string.IsNullOrEmpty(ns)? "NodeCanvas.Tasks.Conditions" : ns) + "{\n\n" + 
			(!string.IsNullOrEmpty(category)? "\t[Category(\"" + category + "\")]\n" : "") +
            (!string.IsNullOrEmpty(description) ? "\t[Description(\"" + description + "\")]\n" : "") +
			"\tpublic class " + taskName + " : ConditionTask" + (!string.IsNullOrEmpty(agentType)? ("<" + agentType + ">") : "") + "{\n\n" +
			"\t\t//Use for initialization. This is called only once in the lifetime of the task.\n" +
			"\t\t//Return null if init was successfull. Return an error string otherwise\n" +
			"\t\tprotected override string OnInit(){\n" +
			"\t\t\treturn null;\n" +
			"\t\t}\n\n" +
			"\t\t//Called once per frame while the condition is active.\n" +
			"\t\t//Return whether the condition is success or failure.\n" +
			"\t\tprotected override bool OnCheck(){\n" +
			"\t\t\treturn true;\n" +
			"\t\t}\n" + 
			"\t}\n" +
			"}";
		}
	}
}

#endif