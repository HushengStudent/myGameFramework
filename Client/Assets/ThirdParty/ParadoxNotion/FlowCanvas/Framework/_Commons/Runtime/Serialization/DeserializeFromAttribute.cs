using System;

namespace ParadoxNotion.Serialization{

	///Used by the ObjectProcessor serializer to upgrade types. It's up to the fsProcessor used to make use of this attribute or not.
	public class DeserializeFromAttribute : Attribute{
		public string[] previousTypeNames;
		public DeserializeFromAttribute(params string[] previousTypeNames){
			this.previousTypeNames = previousTypeNames;
		}
	}
}