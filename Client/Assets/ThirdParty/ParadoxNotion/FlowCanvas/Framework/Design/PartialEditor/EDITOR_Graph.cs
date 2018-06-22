#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;


namespace NodeCanvas.Framework {

    partial class Graph {

		private Graph _currentChildGraph;
		//EDITOR ONLY. Responsible for the breacrumb navigation
		public Graph currentChildGraph{
			get {return _currentChildGraph;}
			set
			{
				if (Application.isPlaying && value != null && EditorUtility.IsPersistent(value)){
					ParadoxNotion.Services.Logger.LogWarning("You can't view sub-graphs in play mode until they are initialized to avoid editing asset references accidentally", "Editor", this);
					return;
				}

				RecordUndo("Change View");
				if (value != null){
					value.currentChildGraph = null;
				}
				_currentChildGraph = value;
			}
		}

		///----------------------------------------------------------------------------------------------

		public GenericMenu CallbackOnCanvasContextMenu(GenericMenu menu, Vector2 canvasMousePos){ return OnCanvasContextMenu(menu, canvasMousePos); }
		public void CallbackOnDropAccepted(Object o, Vector2 canvasMousePos){ OnDropAccepted(o, canvasMousePos); }
		public void CallbackOnVariableDropInGraph(Variable variable, Vector2 canvasMousePos){ OnVariableDropInGraph(variable, canvasMousePos); }

		///Override to add extra context sensitive options in the right click canvas context menu
		virtual protected GenericMenu OnCanvasContextMenu(GenericMenu menu, Vector2 canvasMousePos){ return menu; }
		///Handles drag and drop objects in the graph
		virtual protected void OnDropAccepted(Object o, Vector2 canvasMousePos){}
		///Handle what happens when blackboard variable is drag&droped in graph
		virtual protected void OnVariableDropInGraph(Variable variable, Vector2 canvasMousePos){}

		///----------------------------------------------------------------------------------------------

	}
}

#endif
