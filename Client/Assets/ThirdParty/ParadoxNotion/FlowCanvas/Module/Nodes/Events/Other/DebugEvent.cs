using ParadoxNotion.Design;
using UnityEngine;
using System.Diagnostics;
using NodeCanvas.Framework;

namespace FlowCanvas.Nodes{

	[Description("Use to debug send a Flow Signal in PlayMode Only")]
	[Category("Events/Other")]
	public class DebugEvent : EventNode, IUpdatable {
        
        #if UNITY_EDITOR
        private FlowOutput o;
		private bool send;
        #endif

		protected override void RegisterPorts(){
			#if UNITY_EDITOR
			o = AddFlowOutput("Out");
			#else
			AddFlowOutput("Out"); //a stub
			#endif
		}

		public void Update(){
			#if UNITY_EDITOR
			if (send){
				send = false;
				var sw = new Stopwatch();
				sw.Start();
				o.Call(new Flow());
				sw.Stop();
				UnityEngine.Debug.Log(string.Format("Debug Event Elapsed Time: {0} ms.", sw.ElapsedMilliseconds));
			}
			#endif
		}

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		protected override void OnNodeInspectorGUI(){
			if (GUILayout.Button("Call")){
				if (Application.isPlaying){
					//we do this only for the debugging to show, cause it doesn if we fire the port in here (OnGUI) but it works fine otherwise
					send = true;
				} else {
					UnityEngine.Debug.LogWarning("Debug Flow Signal Event will only work in PlayMode");
				}
			}
		}

		#endif
	}
}