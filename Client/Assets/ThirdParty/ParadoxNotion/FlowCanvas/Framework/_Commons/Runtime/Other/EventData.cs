namespace ParadoxNotion{

	///Used for events
	public class EventData{
		public string name{get; private set;}
		public object value{get{return GetValue();}}
		virtual protected object GetValue(){return null;}
		public EventData(string name){
			this.name = name;
		}
	}

	///Used for events with a value
	public class EventData<T> : EventData {
		new public T value{get; private set;}
		protected override object GetValue(){return value;}
		public EventData(string name, T value) : base(name){
			this.value = value;
		}
	}
}