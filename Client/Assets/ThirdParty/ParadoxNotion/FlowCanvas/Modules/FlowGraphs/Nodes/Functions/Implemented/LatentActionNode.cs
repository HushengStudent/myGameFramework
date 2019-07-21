using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Services;

namespace FlowCanvas.Nodes
{

    ///Latent Action Nodes do not return any value and can span within multiple frames and have up to 10 parameters. They need Flow execution.
    abstract public class LatentActionNodeBase : SimplexNode
    {

        //...
        public enum InvocationMode
        {
            QueueCalls,
            FilterCalls
        }

        //enumerator - flow pair
        private struct RoutineData
        {
            public IEnumerator enumerator;
            public Flow flow;
            public RoutineData(IEnumerator enumerator, Flow flow) {
                this.enumerator = enumerator;
                this.flow = flow;
            }
        }

        public InvocationMode invocationMode;

        private FlowOutput onStart;
        private FlowOutput onUpdate;
        private FlowOutput onFinish;
        private Queue<RoutineData> routineQueue;
        private UnityEngine.Coroutine currentCoroutine;
        private bool graphStoped;
        private bool graphPaused;

        public override string name {
            get { return routineQueue != null && routineQueue.Count > 0 ? string.Format("{0} [{1}]", base.name, routineQueue.Count.ToString()) : base.name; }
        }

        ///----------------------------------------------------------------------------------------------

        sealed public override void OnGraphStarted() { graphStoped = false; }
        sealed public override void OnGraphStoped() { graphStoped = true; BreakAll(); }
        sealed public override void OnGraphPaused() { graphPaused = true; }
        sealed public override void OnGraphUnpaused() { graphPaused = false; }

        ///----------------------------------------------------------------------------------------------

        //begins a new coroutine
        protected void Begin(IEnumerator enumerator, Flow f) {

            var data = new RoutineData(enumerator, f);
            if ( currentCoroutine == null ) {
                currentCoroutine = MonoManager.current.StartCoroutine(InternalCoroutine(data));
                return;
            }

            if ( routineQueue == null ) { routineQueue = new Queue<RoutineData>(); }
            if ( allowRoutineQueueing && invocationMode == InvocationMode.QueueCalls ) {
                routineQueue.Enqueue(data);
            }
        }

        //breaks all coroutine queues
        protected void BreakAll() {
            if ( currentCoroutine != null ) {
                MonoManager.current.StopCoroutine(currentCoroutine);
                currentCoroutine = null;
                routineQueue = null;
                onFinish.parent.SetStatus(NodeCanvas.Framework.Status.Resting);
                OnBreak();
                if ( !graphStoped ) {
                    onFinish.Call(new Flow());
                }
            }
        }

        IEnumerator InternalCoroutine(RoutineData data) {
            parentNode.SetStatus(NodeCanvas.Framework.Status.Running);
            if ( onStart != null ) {
                onStart.Call(data.flow);
            }

            data.flow.BeginBreakBlock(BreakAll);
            while ( data.enumerator.MoveNext() ) {
                while ( graphPaused ) {
                    yield return null;
                }
                if ( onUpdate != null ) {
                    onUpdate.Call(data.flow);
                }
                yield return data.enumerator.Current;
            }
            data.flow.EndBreakBlock();

            parentNode.SetStatus(NodeCanvas.Framework.Status.Resting);
            onFinish.Call(data.flow);
            currentCoroutine = null;

            if ( routineQueue != null && routineQueue.Count > 0 ) {
                var next = routineQueue.Dequeue();
                currentCoroutine = MonoManager.current.StartCoroutine(InternalCoroutine(next));
            }
        }

        ///----------------------------------------------------------------------------------------------

        protected override void OnRegisterPorts(FlowNode node) {
            //to make update safe from previous version, the ID (2nd string), is same as the old version. The first string, is the display name.
            if ( allowRoutineQueueing ) {
                onStart = node.AddFlowOutput("Start", "Out");
                onUpdate = node.AddFlowOutput("Update", "Doing");
            }
            onFinish = node.AddFlowOutput("Finish", "Done");
            OnRegisterDerivedPorts(node);
            if ( allowRoutineQueueing ) {
                node.AddFlowInput("Break", (f) => { BreakAll(); });
            }
        }

        ///----------------------------------------------------------------------------------------------

        ///Register the special per type ports
        abstract protected void OnRegisterDerivedPorts(FlowNode node);
        ///Called when Break input is called.
        virtual public void OnBreak() { }
        ///Should extra ports be shown?
        virtual public bool allowRoutineQueueing {
            get { return true; }
        }
    }

    ///----------------------------------------------------------------------------------------------

    abstract public class LatentActionNode : LatentActionNodeBase
    {
        abstract public IEnumerator Invoke();
        sealed protected override void OnRegisterDerivedPorts(FlowNode node) {
            node.AddFlowInput("In", (f) => { Begin(Invoke(), f); });
        }
    }

    abstract public class LatentActionNode<T1> : LatentActionNodeBase
    {
        abstract public IEnumerator Invoke(T1 a);
        sealed protected override void OnRegisterDerivedPorts(FlowNode node) {
            var p1 = node.AddValueInput<T1>(parameters[0].Name);
            node.AddFlowInput("In", (f) => { Begin(Invoke(p1.value), f); });
        }
    }

    abstract public class LatentActionNode<T1, T2> : LatentActionNodeBase
    {
        abstract public IEnumerator Invoke(T1 a, T2 b);
        sealed protected override void OnRegisterDerivedPorts(FlowNode node) {
            var p1 = node.AddValueInput<T1>(parameters[0].Name);
            var p2 = node.AddValueInput<T2>(parameters[1].Name);
            node.AddFlowInput("In", (f) => { Begin(Invoke(p1.value, p2.value), f); });
        }
    }

    abstract public class LatentActionNode<T1, T2, T3> : LatentActionNodeBase
    {
        abstract public IEnumerator Invoke(T1 a, T2 b, T3 c);
        sealed protected override void OnRegisterDerivedPorts(FlowNode node) {
            var p1 = node.AddValueInput<T1>(parameters[0].Name);
            var p2 = node.AddValueInput<T2>(parameters[1].Name);
            var p3 = node.AddValueInput<T3>(parameters[2].Name);
            node.AddFlowInput("In", (f) => { Begin(Invoke(p1.value, p2.value, p3.value), f); });
        }
    }

    abstract public class LatentActionNode<T1, T2, T3, T4> : LatentActionNodeBase
    {
        abstract public IEnumerator Invoke(T1 a, T2 b, T3 c, T4 d);
        sealed protected override void OnRegisterDerivedPorts(FlowNode node) {
            var p1 = node.AddValueInput<T1>(parameters[0].Name);
            var p2 = node.AddValueInput<T2>(parameters[1].Name);
            var p3 = node.AddValueInput<T3>(parameters[2].Name);
            var p4 = node.AddValueInput<T4>(parameters[3].Name);
            node.AddFlowInput("In", (f) => { Begin(Invoke(p1.value, p2.value, p3.value, p4.value), f); });
        }
    }

    abstract public class LatentActionNode<T1, T2, T3, T4, T5> : LatentActionNodeBase
    {
        abstract public IEnumerator Invoke(T1 a, T2 b, T3 c, T4 d, T5 e);
        sealed protected override void OnRegisterDerivedPorts(FlowNode node) {
            var p1 = node.AddValueInput<T1>(parameters[0].Name);
            var p2 = node.AddValueInput<T2>(parameters[1].Name);
            var p3 = node.AddValueInput<T3>(parameters[2].Name);
            var p4 = node.AddValueInput<T4>(parameters[3].Name);
            var p5 = node.AddValueInput<T5>(parameters[4].Name);
            node.AddFlowInput("In", (f) => { Begin(Invoke(p1.value, p2.value, p3.value, p4.value, p5.value), f); });
        }
    }

    abstract public class LatentActionNode<T1, T2, T3, T4, T5, T6> : LatentActionNodeBase
    {
        abstract public IEnumerator Invoke(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f);
        sealed protected override void OnRegisterDerivedPorts(FlowNode node) {
            var p1 = node.AddValueInput<T1>(parameters[0].Name);
            var p2 = node.AddValueInput<T2>(parameters[1].Name);
            var p3 = node.AddValueInput<T3>(parameters[2].Name);
            var p4 = node.AddValueInput<T4>(parameters[3].Name);
            var p5 = node.AddValueInput<T5>(parameters[4].Name);
            var p6 = node.AddValueInput<T6>(parameters[5].Name);
            node.AddFlowInput("In", (f) => { Begin(Invoke(p1.value, p2.value, p3.value, p4.value, p5.value, p6.value), f); });
        }
    }

    abstract public class LatentActionNode<T1, T2, T3, T4, T5, T6, T7> : LatentActionNodeBase
    {
        abstract public IEnumerator Invoke(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g);
        sealed protected override void OnRegisterDerivedPorts(FlowNode node) {
            var p1 = node.AddValueInput<T1>(parameters[0].Name);
            var p2 = node.AddValueInput<T2>(parameters[1].Name);
            var p3 = node.AddValueInput<T3>(parameters[2].Name);
            var p4 = node.AddValueInput<T4>(parameters[3].Name);
            var p5 = node.AddValueInput<T5>(parameters[4].Name);
            var p6 = node.AddValueInput<T6>(parameters[5].Name);
            var p7 = node.AddValueInput<T7>(parameters[6].Name);
            node.AddFlowInput("In", (f) => { Begin(Invoke(p1.value, p2.value, p3.value, p4.value, p5.value, p6.value, p7.value), f); });
        }
    }

    abstract public class LatentActionNode<T1, T2, T3, T4, T5, T6, T7, T8> : LatentActionNodeBase
    {
        abstract public IEnumerator Invoke(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g, T8 h);
        sealed protected override void OnRegisterDerivedPorts(FlowNode node) {
            var p1 = node.AddValueInput<T1>(parameters[0].Name);
            var p2 = node.AddValueInput<T2>(parameters[1].Name);
            var p3 = node.AddValueInput<T3>(parameters[2].Name);
            var p4 = node.AddValueInput<T4>(parameters[3].Name);
            var p5 = node.AddValueInput<T5>(parameters[4].Name);
            var p6 = node.AddValueInput<T6>(parameters[5].Name);
            var p7 = node.AddValueInput<T7>(parameters[6].Name);
            var p8 = node.AddValueInput<T8>(parameters[7].Name);
            node.AddFlowInput("In", (f) => { Begin(Invoke(p1.value, p2.value, p3.value, p4.value, p5.value, p6.value, p7.value, p8.value), f); });
        }
    }

    abstract public class LatentActionNode<T1, T2, T3, T4, T5, T6, T7, T8, T9> : LatentActionNodeBase
    {
        abstract public IEnumerator Invoke(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g, T8 h, T9 i);
        sealed protected override void OnRegisterDerivedPorts(FlowNode node) {
            var p1 = node.AddValueInput<T1>(parameters[0].Name);
            var p2 = node.AddValueInput<T2>(parameters[1].Name);
            var p3 = node.AddValueInput<T3>(parameters[2].Name);
            var p4 = node.AddValueInput<T4>(parameters[3].Name);
            var p5 = node.AddValueInput<T5>(parameters[4].Name);
            var p6 = node.AddValueInput<T6>(parameters[5].Name);
            var p7 = node.AddValueInput<T7>(parameters[6].Name);
            var p8 = node.AddValueInput<T8>(parameters[7].Name);
            var p9 = node.AddValueInput<T9>(parameters[8].Name);
            node.AddFlowInput("In", (f) => { Begin(Invoke(p1.value, p2.value, p3.value, p4.value, p5.value, p6.value, p7.value, p8.value, p9.value), f); });
        }
    }

    abstract public class LatentActionNode<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : LatentActionNodeBase
    {
        abstract public IEnumerator Invoke(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g, T8 h, T9 i, T10 j);
        sealed protected override void OnRegisterDerivedPorts(FlowNode node) {
            var p1 = node.AddValueInput<T1>(parameters[0].Name);
            var p2 = node.AddValueInput<T2>(parameters[1].Name);
            var p3 = node.AddValueInput<T3>(parameters[2].Name);
            var p4 = node.AddValueInput<T4>(parameters[3].Name);
            var p5 = node.AddValueInput<T5>(parameters[4].Name);
            var p6 = node.AddValueInput<T6>(parameters[5].Name);
            var p7 = node.AddValueInput<T7>(parameters[6].Name);
            var p8 = node.AddValueInput<T8>(parameters[7].Name);
            var p9 = node.AddValueInput<T9>(parameters[8].Name);
            var p10 = node.AddValueInput<T10>(parameters[9].Name);
            node.AddFlowInput("In", (f) => { Begin(Invoke(p1.value, p2.value, p3.value, p4.value, p5.value, p6.value, p7.value, p8.value, p9.value, p10.value), f); });
        }
    }

}