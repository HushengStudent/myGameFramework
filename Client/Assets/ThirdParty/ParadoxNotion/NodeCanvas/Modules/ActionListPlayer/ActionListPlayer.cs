using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Serialization;

namespace NodeCanvas{

	[AddComponentMenu("NodeCanvas/Standalone Action List (Bonus)")]
	public class ActionListPlayer : MonoBehaviour, ITaskSystem, ISerializationCallbackReceiver {

		[SerializeField]
		private string _serializedList;
		[SerializeField]
		private List<UnityEngine.Object> _objectReferences;
		[System.NonSerialized]
		private ActionList _actionList;
		[SerializeField]
		private Blackboard _blackboard;

		void ISerializationCallbackReceiver.OnBeforeSerialize(){
			#if UNITY_EDITOR
			if (JSONSerializer.applicationPlaying){
				 return;
			}			
			_objectReferences = new List<UnityEngine.Object>();
			_serializedList = JSONSerializer.Serialize(typeof(ActionList), _actionList, false, _objectReferences);
			#endif
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize(){
			_actionList = JSONSerializer.Deserialize<ActionList>(_serializedList, _objectReferences);
			if (_actionList == null) _actionList = (ActionList)Task.Create(typeof(ActionList), this);
		}


		///----------------------------------------------------------------------------------------------

		public ActionList actionList{
			get {return _actionList;}
		}

		Component ITaskSystem.agent{
			get {return this;}
		}

		public IBlackboard blackboard{
			get {return _blackboard;}
			set
			{
				if ( !ReferenceEquals(_blackboard, value) ){
					_blackboard = (Blackboard)(object)value;
					SendTaskOwnerDefaults();
				}
			}
		}

		public float elapsedTime{
			get {return actionList.elapsedTime;}
		}

		Object ITaskSystem.contextObject{
			get {return this;}
		}

		public static ActionListPlayer Create(){
			return new GameObject("ActionList").AddComponent<ActionListPlayer>();
		}

		public void SendTaskOwnerDefaults(){
			actionList.SetOwnerSystem(this);
			foreach(var a in actionList.actions){
				a.SetOwnerSystem(this);
			}
		}

		void ITaskSystem.SendEvent(ParadoxNotion.EventData eventData){
			Debug.LogWarning("Sending events to action lists has no effect");
		}

		void ITaskSystem.RecordUndo(string name){
			#if UNITY_EDITOR
			if (!Application.isPlaying){
				UnityEditor.Undo.RecordObject(this, name);
			}
			#endif
		}

		void Awake(){
			SendTaskOwnerDefaults();
		}

		[ContextMenu("Play")]
		public void Play(){
			Play(this, this.blackboard, null);
		}

		public void Play(System.Action<bool> OnFinish){
			Play(this, this.blackboard, OnFinish);
		}

		public void Play(Component agent, IBlackboard blackboard, System.Action<bool> OnFinish){
			if (Application.isPlaying){
				actionList.ExecuteAction(agent, blackboard, OnFinish);
			}
		}

		public Status ExecuteAction(){
			return actionList.ExecuteAction(this, blackboard);
		}

		public Status ExecuteAction(Component agent){
			return actionList.ExecuteAction(agent, blackboard);
		}


		///----------------------------------------------------------------------------------------------
		///---------------------------------------UNITY EDITOR-------------------------------------------
		#if UNITY_EDITOR

		[UnityEditor.MenuItem("Tools/ParadoxNotion/NodeCanvas/Create/Standalone Action List")]
		static void CreateActionListPlayer(){
			UnityEditor.Selection.activeObject = Create();
		}

		void Reset(){
			var bb = GetComponent<Blackboard>();
			_blackboard = bb != null? bb : gameObject.AddComponent<Blackboard>();
			_actionList = (ActionList)Task.Create(typeof(ActionList), this);
		}

		void OnValidate(){
			if ( !Application.isPlaying && !UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode ){
				SendTaskOwnerDefaults();
			}
		}

		#endif
	}
}