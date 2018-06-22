using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using ParadoxNotion.Serialization.FullSerializer.Internal;

namespace ParadoxNotion.Serialization{

	///Serialized MethodInfo
	[Serializable]
	public class SerializedMethodInfo : SerializedMethodBaseInfo {

		[SerializeField]
		private string _baseInfo;
		[SerializeField]
		private string _paramsInfo;
		[SerializeField]
		private string _genericArgumentsInfo;
		
		[NonSerialized]
		private MethodInfo _method;
		[NonSerialized]
		private bool _hasChanged;

		///serialize to strings info
		public override void OnBeforeSerialize(){
			_hasChanged = false;
			if (_method != null){
				_baseInfo = string.Format("{0}|{1}|{2}", _method.RTReflectedType().FullName, _method.Name, _method.ReturnType.FullName);
				_paramsInfo = string.Join("|", _method.GetParameters().Select(p => p.ParameterType.FullName).ToArray() );
				_genericArgumentsInfo = _method.IsGenericMethod? string.Join("|", _method.GetGenericArguments().Select(a => a.FullName).ToArray() ) : null;
			}
		}

		//deserialize from strings info
		public override void OnAfterDeserialize(){
			_hasChanged = false;
			if (_baseInfo == null){
				return;
			}
			var split = _baseInfo.Split('|');
			var type = fsTypeCache.GetType(split[0], null);
			if (type == null){
				_method = null;
				return;
			}

			var name = split[1];
			var paramTypeNames = string.IsNullOrEmpty(_paramsInfo)? null : _paramsInfo.Split('|');
			var parameterTypes = paramTypeNames == null? new Type[0] : paramTypeNames.Select(n => fsTypeCache.GetType(n, null)).ToArray();
			if (parameterTypes.All(t => t != null)){
				
				//Generic Method
				if (!string.IsNullOrEmpty(_genericArgumentsInfo)){
					var genericArgumentTypes = _genericArgumentsInfo.Split('|').Select(x => fsTypeCache.GetType(x, null)).ToArray();
					_method = type.RTGetMethods().FirstOrDefault(m =>
						m.IsGenericMethod &&
						m.Name == name &&
						m.GetParameters().Length == parameterTypes.Length &&
						m.MakeGenericMethod(genericArgumentTypes).GetParameters().Select(p => p.ParameterType).SequenceEqual(parameterTypes)
					);
					if (_method != null){
						_method = _method.MakeGenericMethod( genericArgumentTypes );					
					}
				
				//Non generic method
				} else { 

					_method = type.RTGetMethod(name, parameterTypes);
					if (split.Length >= 3){ //in older serializations we didnt save return type.
						var returnTypeName = split[2];
						var returnType = fsTypeCache.GetType(returnTypeName, null);
						if (_method != null && returnType != _method.ReturnType){
							_method = null;
						}
					}
				}
			}
			
			if (_method == null){
				_hasChanged = true;
				//resolve to first method found with same name. Do it this way instead of GetMethod to avoid ambigous reference
				_method = type.RTGetMethods().FirstOrDefault(m => m.Name == name);
				if (_method.IsGenericMethodDefinition){
					var arg1 = _method.GetGenericArguments().First().GetGenericParameterConstraints().FirstOrDefault();
					_method = _method.MakeGenericMethod(arg1 != null? arg1 : typeof(object));
				}
			}
		}

		//required
		public SerializedMethodInfo(){}
		///Serialize a new MethodInfo
		public SerializedMethodInfo(MethodInfo method){
			_hasChanged = false;
			_method = method;
		}

		///Deserialize and return target MethodInfo.
		public MethodInfo Get(){
			return _method;
		}

		//MethodBase info
		public override MethodBase GetBase(){
			return Get();
		}

		//Are the original and finaly resolve methods different?
		public override bool HasChanged(){
			return _hasChanged;
		}
	
		///Returns the serialized method information.
		public override string GetMethodString(){
			return string.Format("{0} ({1})", _baseInfo.Replace("|", "."), _paramsInfo.Replace("|", ", "));
		}
	}
}