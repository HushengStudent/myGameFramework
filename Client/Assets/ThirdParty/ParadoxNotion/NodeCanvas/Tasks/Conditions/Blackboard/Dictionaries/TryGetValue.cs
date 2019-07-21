using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Conditions
{

    [Category("✫ Blackboard/Dictionaries")]
    public class TryGetValue<T> : ConditionTask
    {

        [RequiredField]
        [BlackboardOnly]
        public BBParameter<Dictionary<string, T>> targetDictionary;
        [RequiredField]
        public BBParameter<string> key;
        [BlackboardOnly]
        public BBParameter<T> saveValueAs;

        protected override string info {
            get { return string.Format("{0}.TryGetValue({1} as {2})", targetDictionary, key, saveValueAs); }
        }

        protected override bool OnCheck() {
            if ( targetDictionary.value == null ) {
                return false;
            }

            T result;
            if ( targetDictionary.value.TryGetValue(key.value, out result) ) {
                saveValueAs.value = result;
                return true;
            }

            return false;
        }
    }
}