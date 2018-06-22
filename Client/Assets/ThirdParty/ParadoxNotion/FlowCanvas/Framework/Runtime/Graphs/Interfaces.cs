namespace NodeCanvas.Framework{

	///An interface to update objects that need concurent updating apart from their normal 'Execution'.
	///It's up to the system to check for this interface
	public interface IUpdatable{
		void Update();
	}

	///Denotes that the node can be invoked in means outside of it's 'Execution' scope.
	public interface IInvokable{
		string GetInvocationID();
		object Invoke(params object[] args);
	}

	///Denotes that the node holds a nested graph.
	///Nodes are checked for this interface
	public interface IGraphAssignable{
		Graph nestedGraph {get;set;}
		Graph[] GetInstances();
	}

	///Denotes that the node can be assigned a Task and it's workings are based on that task.
	///Nodes and Connections are checked for this interface
	public interface ITaskAssignable{
		Task task{get;set;}
	}

	///Use the generic ITaskAssignable when the Task type is known
	public interface ITaskAssignable<T> : ITaskAssignable where T : Task {}

	///Used when the object contains Tasks that are not directly declared (eg wrapped within some other class). Very rarely used.
	///An ITaskAssignable doesnt need this.
	///Nodes and tasks are checked for this interface
	public interface ISubTasksContainer{
		Task[] GetSubTasks();
	}

	///Used to tell that the node or task has BBParameters not defined simply as fields. Mostly used when the node/task is a wrapper of some kind.
	public interface ISubParametersContainer{
		BBParameter[] GetSubParameters();
	}
}