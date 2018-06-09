using UnityEngine;
using System.Linq;

namespace BehaviorDesigner.Runtime.Tasks.Basic.SharedVariables
{
    [TaskCategory("Basic/SharedVariable")]
    [TaskDescription("Returns success if the variable value is equal to the compareTo value.")]
    public class CompareSharedObjectList : Conditional
    {
        [Tooltip("The first variable to compare")]
        public SharedObjectList variable;
        [Tooltip("The variable to compare to")]
        public SharedObjectList compareTo;

        public override TaskStatus OnUpdate()
        {
            if (variable.Value == null && compareTo.Value != null)
                return TaskStatus.Failure;
            if (variable.Value == null && compareTo.Value == null)
                return TaskStatus.Success;
            if (variable.Value.Count != compareTo.Value.Count)
                return TaskStatus.Failure;

            return variable.Value.Except(compareTo.Value).Count() > 0 ? TaskStatus.Failure : TaskStatus.Success;
        }

        public override void OnReset()
        {
            variable = null;
            compareTo = null;
        }
    }
}