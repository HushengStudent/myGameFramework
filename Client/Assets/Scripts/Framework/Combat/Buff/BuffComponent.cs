/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/06 19:48:41
** desc:  Buff组件;
*********************************************************************************/

using Framework.ECSModule;
using System.Collections.Generic;

namespace Framework
{
    public class BuffComponent : AbsComponent
    {
        private Dictionary<int, Buff> _buffDict;

        protected override void InitializeEx()
        {
            base.InitializeEx();
            _buffDict = new Dictionary<int, Buff>();
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
