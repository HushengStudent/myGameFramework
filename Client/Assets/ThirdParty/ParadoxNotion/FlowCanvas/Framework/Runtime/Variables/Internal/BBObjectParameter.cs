using System;
using UnityEngine;
using ParadoxNotion;

namespace NodeCanvas.Framework.Internal{

	///Can be set to any type in case type is unknown. Not really recomended for performance and VERY rarely used
	[Serializable]
	public class BBObjectParameter : BBParameter<object>{

		public BBObjectParameter(){
			SetType(typeof(object));
		}
		public BBObjectParameter(Type t){
			SetType(t);
		}

		[SerializeField]
		private Type _type;

		public override Type varType{
			get {return _type;}
		}

		public void SetType(Type t){
			
			if (t == null){
				t = typeof(object);
			}

			if (t != _type){
				_value = t.RTIsValueType()? Activator.CreateInstance(t) : null;
			}

			_type = t;
		}
	}
}