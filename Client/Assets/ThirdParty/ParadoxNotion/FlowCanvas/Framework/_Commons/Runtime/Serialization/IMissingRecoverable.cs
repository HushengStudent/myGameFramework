namespace ParadoxNotion.Serialization{

	//an interface used along with fsRecoveryProcessor to handle missing types and their recovery
	public interface IMissingRecoverable{
		string missingType{get;set;}
		string recoveryState{get;set;}
	}
}