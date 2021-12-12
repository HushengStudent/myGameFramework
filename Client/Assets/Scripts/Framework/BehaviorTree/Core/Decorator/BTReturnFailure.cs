/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/18 18:11:39
** desc:  返回失败执行节点;
*********************************************************************************/

using System.Collections;

namespace Framework.BehaviorTreeModule
{
    public class BTReturnFailure : AbsDecorator
    {
        public BTReturnFailure(Hashtable table) : base(table) { }

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