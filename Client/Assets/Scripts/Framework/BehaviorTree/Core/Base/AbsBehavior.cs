/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/18 13:12:11
** desc:  行为抽象基类;
*********************************************************************************/

using System.Collections;

namespace Framework
{
    public abstract class AbsBehavior
    {
        private bool _awake = false;
        public int Id;

        public AbsEntity Entity { get; private set; }
        public BehaviorState Reslut { get; set; } = BehaviorState.Reset;

        public virtual bool IsComposite { get { return false; } }

        public BehaviorState Behave(AbsEntity entity, float interval)
        {
            if (!_awake)
            {
                _awake = true;
                Entity = entity;
                Reslut = BehaviorState.Running;
                AwakeEx();
            }
            Update(interval);
            return Reslut;
        }

        public bool ResetBehavior()
        {
            if (Reslut == BehaviorState.Running)
                return false;
            Reset();
            _awake = false;
            Entity = null;
            Reslut = BehaviorState.Reset;
            return true;
        }

        public AbsBehavior(Hashtable table) { }

        protected abstract void AwakeEx();

        protected abstract void Update(float interval);

        protected abstract void UpdateEx(float interval);

        protected abstract void Reset();

    }
}
