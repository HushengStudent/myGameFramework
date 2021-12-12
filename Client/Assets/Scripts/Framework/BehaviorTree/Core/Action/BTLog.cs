/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/18 18:07:31
** desc:  日志执行节点;
*********************************************************************************/

using System.Collections;

namespace Framework.BehaviorTreeModule
{
    public class BTLog : AbsDecorator
    {
        private string _log = string.Empty;

        public BTLog(Hashtable table) : base(table)
        {
            _log = table["Log"].ToString();
        }

        protected override void AwakeEx()
        {
        }

        protected override void Reset()
        {
        }

        protected override void UpdateEx(float interval)
        {
            LogHelper.Print(_log);
            Reslut = BehaviorState.Finish;
        }
    }
}