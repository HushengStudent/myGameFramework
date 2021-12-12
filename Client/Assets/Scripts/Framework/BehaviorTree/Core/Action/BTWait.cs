/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/18 18:06:24
** desc:  延时执行节点;
*********************************************************************************/

using System.Collections;

namespace Framework.BehaviorTreeModule
{
    public class BTWait : AbsDecorator
    {
        public BTWait(Hashtable table) : base(table) { }

        protected override void AwakeEx()
        {
        }

        protected override void Reset()
        {
        }

        protected override void UpdateEx(float interval)
        {
        }
    }
}