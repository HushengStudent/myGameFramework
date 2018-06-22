using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;


namespace FlowCanvas.Nodes{

	[Name("Graph Variable", 99)]
	[Category("Variables")]
	[Description("Returns a constant or linked variable value.\nYou can alter between constant or linked at any time using the radio button.")]
	[ContextDefinedOutputs(typeof(Wild))]
	public class GetVariable<T> : VariableNode {
		
		public BBParameter<T> value;
		
		#if UNITY_EDITOR
		public override string name{
			get
			{
				var size = typeof(T).IsPrimitive && !value.useBlackboard? 20 : 12;
				return string.Format("<size={0}>{1}</size>", size.ToString(), value.ToString());
			}
		}
		#endif

		protected override void RegisterPorts(){
			AddValueOutput<T>("Value", ()=> {return value.value; });
		}

		public void SetTargetVariableName(string name){
			value.name = name;
		}

		public override void SetVariable(object o){

			if (o is Variable<T>){
				value.name = (o as Variable<T>).name;
				return;
			}

			if (o is T){
				value.value = (T)o;
				return;
			}
			
			Debug.LogError("Set Variable Error");
		}


		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		protected override void OnNodeGUI(){
			base.OnNodeGUI();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			
			if ( value.value is UnityEngine.Object ){
				var o = (UnityEngine.Object)(object)value.value;
				var prefabType = UnityEditor.PrefabUtility.GetPrefabType(o);
				if (prefabType == UnityEditor.PrefabType.PrefabInstance || prefabType == UnityEditor.PrefabType.PrefabInstance){
					o = UnityEditor.PrefabUtility.GetPrefabParent(o);
				}
				var texture = UnityEditor.AssetPreview.GetAssetPreview(o);
				if (texture != null){
					GUILayout.Box(texture, GUILayout.Width(64), GUILayout.Height(64));
				}
			}

			if ( value.value is Color ){
				var color = (Color)(object)value.value;
				GUI.color = color;
				GUILayout.Box(string.Empty, GUILayout.Width(64), GUILayout.Height(64));
				var lastRect = GUILayoutUtility.GetLastRect();
				GUI.DrawTexture(lastRect, Texture2D.whiteTexture);
				GUI.color = Color.white;
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		#endif

	}
}