using UnityEngine;
#if CROSS_PLATFORM_INPUT
using UnityStandardAssets.CrossPlatformInput;
#endif

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityInput
{
    [TaskCategory("Basic/Input")]
    [TaskDescription("Stores the state of the specified button.")]
    public class GetButton : Action
    {
        [Tooltip("The name of the button")]
        public SharedString buttonName;
        [RequiredField]
        [Tooltip("The stored result")]
        public SharedBool storeResult;

        public override TaskStatus OnUpdate()
        {
#if CROSS_PLATFORM_INPUT
            storeResult.Value = CrossPlatformInputManager.GetButton(buttonName.Value);
#else
            storeResult.Value = Input.GetButton(buttonName.Value);
#endif
            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            buttonName = "Fire1";
            storeResult = false;
        }
    }
}