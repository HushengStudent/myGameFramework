using ParadoxNotion.Serialization;

namespace NodeCanvas.Framework.Internal
{

    public class MissingVariableType : Variable<object>, IMissingRecoverable
    {

        [UnityEngine.SerializeField]
        private string _missingType;
        [UnityEngine.SerializeField]
        private string _recoveryState;

        public string missingType {
            get { return _missingType; }
            set { _missingType = value; }
        }

        public string recoveryState {
            get { return _recoveryState; }
            set { _recoveryState = value; }
        }
    }
}