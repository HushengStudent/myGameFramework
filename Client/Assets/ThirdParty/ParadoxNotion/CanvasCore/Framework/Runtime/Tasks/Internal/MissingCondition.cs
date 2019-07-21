using System.Linq;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;
using UnityEngine;


namespace NodeCanvas.Framework.Internal
{

    /// Injected when a ConditionTask is missing. Recovers back when that condition is found.
    [DoNotList]
    [Description("Please resolve the MissingTask issue by either replacing the task or importing the missing task type in the project")]
    public class MissingCondition : ConditionTask, IMissingRecoverable
    {

        [SerializeField]
        private string _missingType;
        [SerializeField]
        private string _recoveryState;

        string IMissingRecoverable.missingType {
            get { return _missingType; }
            set { _missingType = value; }
        }

        string IMissingRecoverable.recoveryState {
            get { return _recoveryState; }
            set { _recoveryState = value; }
        }

        protected override string info {
            get { return string.Format("<color=#ff6457>* {0} *</color>", _missingType.Split('.').Last()); }
        }


        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
#if UNITY_EDITOR

        protected override void OnTaskInspectorGUI() {
            GUILayout.Label(_missingType);
        }

#endif
    }
}