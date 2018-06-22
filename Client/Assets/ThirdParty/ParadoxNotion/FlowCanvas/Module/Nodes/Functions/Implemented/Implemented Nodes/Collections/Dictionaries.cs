using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes{

	[Category("Collections/Dictionaries")]
	[ExposeAsDefinition]
	public class AddDictionaryItem<T> : CallableFunctionNode<IDictionary<string, T>, IDictionary<string, T>, string, T>{
		public override IDictionary<string, T> Invoke(IDictionary<string, T> dict, string key, T item){
			dict.Add(key, item);
			return dict;
		}
	}

	[Category("Collections/Dictionaries")]
	[ExposeAsDefinition]
	public class GetDictionaryItem<T> : CallableFunctionNode<T, IDictionary<string, T>, string>{
		public override T Invoke(IDictionary<string, T> dict, string key){
			return dict[key];
		}
	}

	[Category("Collections/Dictionaries")]
	[ExposeAsDefinition]
	public class RemoveDictionaryKey<T> : CallableFunctionNode<IDictionary<string, T>, IDictionary<string, T>, string>{
		public override IDictionary<string, T> Invoke(IDictionary<string, T> dict, string key){
			dict.Remove(key);
			return dict;
		}
	}

	[Category("Collections/Dictionaries")]
	[ExposeAsDefinition]
	public class ClearDictionary : CallableFunctionNode<IDictionary, IDictionary>{
		public override IDictionary Invoke(IDictionary dict){
			dict.Clear();
			return dict;
		}
	}

	[Category("Collections/Dictionaries")]
	[ExposeAsDefinition]
	public class TryGetValue<T> : CallableFunctionNode<T, IDictionary<string, T>, string>{

		public bool exists{get; private set;}

		public override T Invoke(IDictionary<string, T> dict, string key){
			T value = default(T);
			exists = dict.TryGetValue(key, out value);
			return value;
		}
	}

	[Category("Collections/Dictionaries")]
	[ExposeAsDefinition]
	public class DictionaryContainsKey<T> : CallableFunctionNode<bool, IDictionary<string, T>, string>{
		public override bool Invoke(IDictionary<string, T> dict, string key){
			return dict.ContainsKey(key);
		}
	}
}