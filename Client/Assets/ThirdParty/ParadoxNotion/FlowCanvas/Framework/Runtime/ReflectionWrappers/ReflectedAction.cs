using System;
using ParadoxNotion;


namespace NodeCanvas.Framework.Internal {

    /// <summary>
    /// Wraps a reflected method call of return type void
    /// </summary>
    [Serializable]
    public class ReflectedAction : ReflectedActionWrapper
    {
        private ActionCall call;
        public override BBParameter[] GetVariables() { return new BBParameter[0]; }
        public override void Init(object instance){
            call = GetMethod().RTCreateDelegate<ActionCall>(instance);
        }
        public override void Call() { call(); }
    }

    [Serializable] [ParadoxNotion.Design.SpoofAOT]
    public class ReflectedAction<T1> : ReflectedActionWrapper
    {
        private ActionCall<T1> call;
        public BBParameter<T1> p1 = new BBParameter<T1>();
        public override BBParameter[] GetVariables(){
            return new BBParameter[] { p1 };
        }
        public override void Init(object instance){
            call = GetMethod().RTCreateDelegate<ActionCall<T1>>(instance);
        }
        public override void Call() { call(p1.value); }
    }

    [Serializable]
    public class ReflectedAction<T1, T2> : ReflectedActionWrapper
    {
        private ActionCall<T1, T2> call;
        public BBParameter<T1> p1 = new BBParameter<T1>();
        public BBParameter<T2> p2 = new BBParameter<T2>();
        public override BBParameter[] GetVariables(){
            return new BBParameter[] { p1, p2 };
        }
        public override void Init(object instance){
            call = GetMethod().RTCreateDelegate<ActionCall<T1, T2>>(instance);
        }
        public override void Call() { call(p1.value, p2.value); }
    }

    [Serializable]
    public class ReflectedAction<T1, T2, T3> : ReflectedActionWrapper
    {
        private ActionCall<T1, T2, T3> call;
        public BBParameter<T1> p1 = new BBParameter<T1>();
        public BBParameter<T2> p2 = new BBParameter<T2>();
        public BBParameter<T3> p3 = new BBParameter<T3>();
        public override BBParameter[] GetVariables(){
            return new BBParameter[] { p1, p2, p3 };
        }
        public override void Init(object instance){
            call = GetMethod().RTCreateDelegate<ActionCall<T1, T2, T3>>(instance);
        }
        public override void Call() { call(p1.value, p2.value, p3.value); }
    }

    [Serializable]
    public class ReflectedAction<T1, T2, T3, T4> : ReflectedActionWrapper
    {
        private ActionCall<T1, T2, T3, T4> call;
        public BBParameter<T1> p1 = new BBParameter<T1>();
        public BBParameter<T2> p2 = new BBParameter<T2>();
        public BBParameter<T3> p3 = new BBParameter<T3>();
        public BBParameter<T4> p4 = new BBParameter<T4>();
        public override BBParameter[] GetVariables(){
            return new BBParameter[] { p1, p2, p3, p4 };
        }
        public override void Init(object instance){
            call = GetMethod().RTCreateDelegate<ActionCall<T1, T2, T3, T4>>(instance);
        }
        public override void Call() { call(p1.value, p2.value, p3.value, p4.value); }
    }

    [Serializable]
    public class ReflectedAction<T1, T2, T3, T4, T5> : ReflectedActionWrapper
    {
        private ActionCall<T1, T2, T3, T4, T5> call;
        public BBParameter<T1> p1 = new BBParameter<T1>();
        public BBParameter<T2> p2 = new BBParameter<T2>();
        public BBParameter<T3> p3 = new BBParameter<T3>();
        public BBParameter<T4> p4 = new BBParameter<T4>();
        public BBParameter<T5> p5 = new BBParameter<T5>();
        public override BBParameter[] GetVariables(){
            return new BBParameter[] { p1, p2, p3, p4, p5 };
        }
        public override void Init(object instance){
            call = GetMethod().RTCreateDelegate<ActionCall<T1, T2, T3, T4, T5>>(instance);
        }
        public override void Call() { call(p1.value, p2.value, p3.value, p4.value, p5.value); }
    }

    [Serializable]
    public class ReflectedAction<T1, T2, T3, T4, T5, T6> : ReflectedActionWrapper
    {
        private ActionCall<T1, T2, T3, T4, T5, T6> call;
        public BBParameter<T1> p1 = new BBParameter<T1>();
        public BBParameter<T2> p2 = new BBParameter<T2>();
        public BBParameter<T3> p3 = new BBParameter<T3>();
        public BBParameter<T4> p4 = new BBParameter<T4>();
        public BBParameter<T5> p5 = new BBParameter<T5>();
        public BBParameter<T6> p6 = new BBParameter<T6>();
        public override BBParameter[] GetVariables(){
            return new BBParameter[] { p1, p2, p3, p4, p5, p6 };
        }
        public override void Init(object instance){
            call = GetMethod().RTCreateDelegate<ActionCall<T1, T2, T3, T4, T5, T6>>(instance);
        }
        public override void Call() { call(p1.value, p2.value, p3.value, p4.value, p5.value, p6.value); }
    }

}