using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeCanvas.Framework{

	///A special BBParameter for the task agent used in Task.
	///This should be a nested class of Task, but WSA has a bug in doing so.
	[System.Serializable]
	public class TaskAgent : BBParameter<UnityEngine.Object>{

		new public UnityEngine.Object value{
			get
			{
				if (useBlackboard){
					var o = base.value;
					if (o == null){
						return null;
					}
					if (o is GameObject){
						return (o as GameObject).transform;
					}
					if (o is Component){
						return (Component)o;
					}
					return null;
				}
				return _value as Component;
			}
			set {_value = value;} //the linked blackboard variable is NEVER set through the TaskAgent. Instead we set the local (inherited) variable
		}

		protected override object objectValue{
			get {return this.value;}
			set {this.value = (UnityEngine.Object)value;}
		}
	}
}