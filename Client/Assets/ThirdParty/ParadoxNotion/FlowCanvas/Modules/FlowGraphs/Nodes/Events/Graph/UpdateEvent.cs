using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes
{

    [Name("On Update", 7)]
    [Category("Events/Graph")]
    [Description("Called per-frame.\nUpdate Interval optionally determines the period in seconds every which update is called. Leave at 0 to call update per-frame as normal.")]
    [ExecutionPriority(7)]
    public class UpdateEvent : EventNode, IUpdatable
    {

        public BBParameter<float> updateInterval = 0f;
        private FlowOutput update;
        private float lastUpdatedTime;

        protected override void RegisterPorts() {
            update = AddFlowOutput("Out");
        }

        public override void OnGraphStarted() {
            lastUpdatedTime = -1;
        }

        public void Update() {
            if ( updateInterval.value <= 0 ) {
                update.Call(new Flow());
                return;
            }

            var currentTime = Time.realtimeSinceStartup;
            if ( currentTime > updateInterval.value + lastUpdatedTime ) {
                update.Call(new Flow());
                lastUpdatedTime = currentTime;
            }
        }
    }
}