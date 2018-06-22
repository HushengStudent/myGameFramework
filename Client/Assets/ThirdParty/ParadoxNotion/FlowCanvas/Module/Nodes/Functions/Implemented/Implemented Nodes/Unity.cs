using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using System.Linq;
using NavMeshAgent = UnityEngine.AI.NavMeshAgent;
using System.Collections.Generic;

namespace FlowCanvas.Nodes{

	[Category("Unity")]
	[Description("Moves a NavMeshAgent object with pathfinding to target destination")]
	public class MoveTo : LatentActionNode<NavMeshAgent, Vector3, float, float>{
		private NavMeshAgent agent;
		public override IEnumerator Invoke(NavMeshAgent agent, Vector3 destination, float speed, float stoppingDistance){
			this.agent = agent;
			agent.speed = speed;
			agent.stoppingDistance = stoppingDistance;
			
			if (agent.speed > 0){
				agent.SetDestination(destination);
			} else {
				agent.Warp(destination);
			}

			while (agent.pathPending || agent.remainingDistance > stoppingDistance){
				yield return null;
			}
		}

		public override void OnBreak(){	agent.ResetPath(); }
	}

	[Category("Unity")]
	[Description("If 'Try Get Existing' is true, then if there is an existing component of that type already attached to the gameobject, it will be returned instead of adding another instance.")]
	public class AddComponent<T> : CallableFunctionNode<T, GameObject, bool> where T:Component{
		public override T Invoke(GameObject gameObject, bool tryGetExisting){
			T component = null;
			if (gameObject != null){
				if (tryGetExisting){
					component = gameObject.GetComponent<T>();
				}
				if (component == null){
					component = gameObject.AddComponent<T>();
				}
			}
			return component;
		}
	}

	[Category("Unity")]
	[Description("Creates a new ScriptableObject instance")]
	public class NewScriptableObject<T> : CallableFunctionNode<T> where T:ScriptableObject{
		public override T Invoke(){
			return ScriptableObject.CreateInstance<T>();
		}
	}

	[Category("Unity")]
	[Description("Get a component attached on an object")]
	public class GetComponent<T> : PureFunctionNode<T, GameObject> where T:Component{
		private T _component;
		public override T Invoke(GameObject gameObject){
			if (gameObject == null) return null;
			if (_component == null || _component.gameObject != gameObject){
				_component = gameObject.GetComponent<T>();			
			}
			return _component;
		}
	}

	[Category("Unity")]
	[Description("Instantiate an object")]
	[ExposeAsDefinition]
	public class Instantiate<T> : CallableFunctionNode<T, T, Vector3, Quaternion, Transform> where T:UnityEngine.Object{
		public override T Invoke(T original, Vector3 position, Quaternion rotation, Transform parent){
			if (original == null) return null;
			return UnityEngine.Object.Instantiate<T>(original, position, rotation, parent);
		}
	}

	[Category("Unity")]
	[Description("Get all child transforms of specified parent")]
	public class GetChildTransforms : PureFunctionNode<IEnumerable<Transform>, Transform>{
		public override IEnumerable<Transform> Invoke(Transform parent){
			return parent.Cast<Transform>();
		}
	}

}