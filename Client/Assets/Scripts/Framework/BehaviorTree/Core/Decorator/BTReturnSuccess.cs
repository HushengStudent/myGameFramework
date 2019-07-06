/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/18 18:11:19
** desc:  返回成功执行节点;
*********************************************************************************/

using System.Collections;

namespace Framework
{
    public class BTReturnSuccess : AbsDecorator
    {
        public BTReturnSuccess(Hashtable table) : base(table) { }

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