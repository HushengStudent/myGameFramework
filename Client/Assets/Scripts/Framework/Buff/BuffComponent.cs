/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/06 19:48:41
** desc:  Buff组件
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class BuffComponent : BaseComponent
    {
        private Dictionary<int, Buff> _buffDict = new Dictionary<int, Buff>();

        public override void OnResetComponent()
        {
            base.OnResetComponent();
            _buffDict.Clear();
        }

        public override void UpdateEx()
        {
            base.UpdateEx();
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
