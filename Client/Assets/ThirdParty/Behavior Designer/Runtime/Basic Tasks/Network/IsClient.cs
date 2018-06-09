#if !(UNITY_4_6 || UNITY_4_7 || UNITY_5_0)
using UnityEngine;
using UnityEngine.Networking;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityNetwork
{
    public class IsClient : Conditional
    {
        public override TaskStatus OnUpdate()
        {
            return NetworkClient.active ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
#endif