/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/06 19:48:41
** desc:  Buff组件;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class BuffComponent : AbsComponent
    {
        private Dictionary<int, Buff> _buffDict = new Dictionary<int, Buff>();

        protected override void OnReset()
        {
            base.OnReset();
            _buffDict.Clear();
        }

        public override void UpdateEx(float interval)
        {
            base.UpdateEx(interval);
        }

        protected override void EventSubscribe()
        {
            base.EventSubscribe();
        }

        protected override void EventUnsubscribe()
        {
            base.EventUnsubscribe();
        }

        public void AddBuff(Buff buff)
        {

        }

        public void RemoveBuff(Buff buff)
        {

        }
    }
}
