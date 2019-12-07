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

        public override string UID
        {
            get
            {
                return HashHelper.GetMD5(typeof(BuffComponent).ToString());
            }
        }

        protected override void UnInitializeEx()
        {
            base.UnInitializeEx();
            _buffDict.Clear();
        }

        public void AddBuff(Buff buff)
        {

        }

        public void RemoveBuff(Buff buff)
        {

        }
    }
}
