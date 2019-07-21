using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Conditions
{

    [Category("✫ Utility")]
    [Description("Will return true after a specific amount of time has passed and false while still counting down")]
    public class Timeout : ConditionTask
    {

        public BBParameter<float> timeout = 1f;
        private float currentTime;

        protected override string info {
            get { return string.Format("Timeout {0}/{1}", currentTime.ToString("0.00"), timeout.ToString()); }
        }

        protected override void OnEnable() {
            currentTime = 0;
        }

        protected override bool OnCheck() {
            if ( currentTime >= timeout.value ) {
                currentTime = 0;
                return true;
            }
            currentTime += Time.deltaTime;
            return false;
        }
    }
}
