#if UNITY_4_6 || UNITY_4_7
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityAnimator
{
    [TaskCategory("Basic/Animator")]
    [TaskDescription("Gets the current state hash. Returns Success.")]
    public class GetCurrentAnimatorStateNameHash : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The layer to operate on")]
        public SharedInt layerIndex;
        [Tooltip("The current state hash")]
        [RequiredField]
        public SharedInt storeValue;

        private Animator animator;
        private GameObject prevGameObject;

        public override void OnStart()
        {
            var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            if (currentGameObject != prevGameObject) {
                animator = currentGameObject.GetComponent<Animator>();
                prevGameObject = currentGameObject;
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (animator == null) {
                Debug.LogWarning("Animator is null");
                return TaskStatus.Failure;
            }

            storeValue.Value = animator.GetCurrentAnimatorStateInfo(layerIndex.Value).nameHash;

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            layerIndex = 0;
            storeValue = 0;
        }
    }
}
#endif