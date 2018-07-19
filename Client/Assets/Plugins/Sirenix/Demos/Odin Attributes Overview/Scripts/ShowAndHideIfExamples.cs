namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;
    using Sirenix.OdinInspector;

    // Note that you can also reference methods and properties. You are not limited to fields.
    public class ShowAndHideIfExamples : MonoBehaviour
    {
        [EnumToggleButtons]
        public InfoMessageType SomeEnum;

        public bool IsToggled;

        [ShowIf("SomeEnum", InfoMessageType.Info)]
        public Vector3 Info;

        [ShowIf("SomeEnum", InfoMessageType.Warning)]
        public Vector2 Warning;

        [ShowIf("SomeEnum", InfoMessageType.Error)]
        public Vector3 Error;

        [ShowIf("IsToggled")]
        public Vector2 VisibleWhenToggled;

        [HideIf("IsToggled")]
        public Vector3 HiddenWhenToggled;
    }
}