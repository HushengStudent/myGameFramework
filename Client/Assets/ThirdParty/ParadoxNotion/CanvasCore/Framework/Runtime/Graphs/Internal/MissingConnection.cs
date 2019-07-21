using UnityEngine;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;

namespace NodeCanvas.Framework.Internal
{

    ///Missing node types are deserialized into this on deserialization and can load back if type is found
    [DoNotList]
    sealed public class MissingConnection : Connection, IMissingRecoverable
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


        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
#if UNITY_EDITOR

        protected override void OnConnectionInspectorGUI() {
            var text = _missingType.Substring(0, _missingType.Contains("[") ? _missingType.IndexOf("[") : _missingType.Length);
            GUILayout.Label(text);
        }

#endif
    }
}