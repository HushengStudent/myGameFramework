using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace FlowCanvas.Nodes{

	[DoNotList]
	[Icon(runtimeIconTypeCallback:"GetRuntimeIconType")]
	///Wraps a SimplexNode
	public class SimplexNodeWrapper<T> : FlowNode where T:SimplexNode {

		//used for [IconAttribute]
		System.Type GetRuntimeIconType(){
			return typeof(T);
		}

		[SerializeField]
		private T _simplexNode;
		private T simplexNode{
			get
			{
				if (_simplexNode == null){
					_simplexNode = (T)System.Activator.CreateInstance(typeof(T));
					if (_simplexNode != null){
						base.GatherPorts();
					}
				}
				return _simplexNode;
			}
		}

		public override string name{
			get {return simplexNode != null? simplexNode.name : "NULL";}
		}

		public override string description{
			get {return simplexNode != null? simplexNode.description : "NULL";}
		}


		public override System.Type GetNodeWildDefinitionType(){
			return typeof(T).GetFirstGenericParameterConstraintType();
		}

		public override void OnCreate(Graph assignedGraph){
			if (simplexNode != null){
				simplexNode.SetDefaultParameters(this);
			}
		}

		public override void OnGraphStarted(){
			if (simplexNode != null){
				simplexNode.OnGraphStarted();
			}
		}

		public override void OnGraphPaused(){
			if (simplexNode != null){
				simplexNode.OnGraphPaused();
			}
		}

		public override void OnGraphUnpaused(){
			if (simplexNode != null){
				simplexNode.OnGraphUnpaused();
			}			
		}

		public override void OnGraphStoped(){
			if (simplexNode != null){
				simplexNode.OnGraphStoped();
			}
		}

		protected override void RegisterPorts(){
			if (simplexNode != null){
				simplexNode.RegisterPorts(this);
			}
		}

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR
			
		//Override of right click node context menu for ability to change type
		protected override UnityEditor.GenericMenu OnContextMenu(UnityEditor.GenericMenu menu){
			
			base.OnContextMenu(menu);
			if (simplexNode == null){
				return menu;
			}

			var type = simplexNode.GetType();
			if (type.IsGenericType){
				menu = EditorUtils.GetPreferedTypesSelectionMenu(type.GetGenericTypeDefinition(), (t)=>{ this.ReplaceWith( typeof(SimplexNodeWrapper<>).MakeGenericType(t) ); }, menu, "Change Generic Type");
			}

			return menu;
		}		

		protected override void OnNodeInspectorGUI(){
			EditorUtils.ReflectedObjectInspector(simplexNode);
			base.OnNodeInspectorGUI();
		}

		#endif
		
	}
}