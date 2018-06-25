/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/18 18:07:31
** desc:  日志执行节点;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class BTLog : AbsBehavior
    {
        private string _log = string.Empty;

        public BTLog(object[] args) : base(args)
        {
            _log = (string)args[0];
        }

        protected override void AwakeEx()
        {
        }

        protected override void Reset()
        {
        }

        protected override void UpdateEx()
        {
            LogUtil.LogUtility.Print(string.Format("[BTLog]:{0}", _log));
            Reslut = BehaviorState.Success;
        }
    }
}