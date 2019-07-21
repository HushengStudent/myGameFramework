using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes
{

    [Category("Time")]
    [Description("Wait for a certain amount of time before continueing")]
    public class Wait : LatentActionNode<float>
    {
        public float timeLeft { get; private set; }
        public float normalized { get; private set; }
        public override IEnumerator Invoke(float time = 1f) {
            timeLeft = time;
            while ( timeLeft > 0 ) {
                timeLeft -= Time.deltaTime;
                timeLeft = Mathf.Max(timeLeft, 0);
                normalized = timeLeft / time;
                yield return null;
            }
        }
    }


    [Category("Time")]
    [Description("Wait for one (the next) frame")]
    public class WaitForOneFrame : LatentActionNode
    {
        public override bool allowRoutineQueueing { get { return false; } }
        public override IEnumerator Invoke() {
            yield return null;
        }
    }

    [Category("Time")]
    [Description("Wait until the physics update frame")]
    public class WaitForPhysicsFrame : LatentActionNode
    {
        public override bool allowRoutineQueueing { get { return false; } }
        public override IEnumerator Invoke() {
            yield return new UnityEngine.WaitForFixedUpdate();
        }
    }

    [Category("Time")]
    [Description("Wait until the end of current frame")]
    public class WaitForEndOfFrame : LatentActionNode
    {
        public override bool allowRoutineQueueing { get { return false; } }
        public override IEnumerator Invoke() {
            yield return new UnityEngine.WaitForEndOfFrame();
        }
    }

    [Category("Time")]
    [Description("Wait Until condition value becomes true")]
    public class WaitUntil : LatentActionNode
    {
        private ValueInput<bool> condition;
        public override bool allowRoutineQueueing { get { return false; } }
        public override IEnumerator Invoke() {
            yield return new UnityEngine.WaitUntil(condition.GetValue);
        }
        //since we want to check the condition per frame, this is implementing this way instead of a parameter
        protected override void OnRegisterExtraPorts(FlowNode node) {
            condition = node.AddValueInput<bool>("Condition");
        }
    }

    [Category("Time")]
    [Description("Wait While condition value is true")]
    public class WaitWhile : LatentActionNode
    {
        private ValueInput<bool> condition;
        public override bool allowRoutineQueueing { get { return false; } }
        public override IEnumerator Invoke() {
            yield return new UnityEngine.WaitWhile(condition.GetValue);
        }
        //since we want to check the condition per frame, this is implementing this way instead of a parameter
        protected override void OnRegisterExtraPorts(FlowNode node) {
            condition = node.AddValueInput<bool>("Condition");
        }
    }

    ///----------------------------------------------------------------------------------------------

    [Name("Per Second (Float)")]
    [Category("Time")]
    [Description("Mutliply input value by Time.deltaTime and optional multiplier")]
    public class DeltaTimed : PureFunctionNode<float, float, float>
    {
        public override float Invoke(float value, float multiplier = 1) {
            return value * multiplier * Time.deltaTime;
        }
    }

    [Name("Per Second (Vector3)")]
    [Category("Time")]
    [Description("Mutliply input value by Time.deltaTime and optional multiplier")]
    public class DeltaTimedVector3 : PureFunctionNode<Vector3, Vector3, float>
    {
        public override Vector3 Invoke(Vector3 value, float multiplier = 1) {
            return value * multiplier * Time.deltaTime;
        }
    }

    ///----------------------------------------------------------------------------------------------

    [Category("Time")]
    [Description("Input values are buffered and returned in order of buffering once the buffered amount reaches the buffer size. In practice this creates a delayed value output when the node is constantly updated per-frame.\nWhen the node ceases to update, a new buffer is created.")]
    [ExposeAsDefinition]
    public class Buffer<T> : PureFunctionNode<T, T, int>
    {

        private int lastFrame = -10;
        private Queue<T> q;

        public override T Invoke(T value, int size = 60) {
            var currentFrame = Time.frameCount;

            //init
            if ( currentFrame - lastFrame > 1 ) {
                q = new Queue<T>();
            }

            lastFrame = currentFrame;
            q.Enqueue(value);
            return q.Count >= size ? q.Dequeue() : q.Peek();
        }
    }

    [Name("Damp (Float)")]
    [Category("Time")]
    [Description("Returns a smoothly interpolated value towards the input value.")]
    public class DampFloat : PureFunctionNode<float, float, float, float>
    {
        public override float Invoke(float current, float target, float damp = 1f) {
            return Mathf.Lerp(current, target, damp * Time.deltaTime);
        }
    }

    [Name("Damp (Vector3)")]
    [Category("Time")]
    [Description("Returns a smoothly interpolated value towards the input value.")]
    public class DampVector3 : PureFunctionNode<Vector3, Vector3, Vector3, float>
    {
        public override Vector3 Invoke(Vector3 current, Vector3 target, float damp = 1f) {
            return Vector3.Lerp(current, target, damp * Time.deltaTime);
        }
    }

}