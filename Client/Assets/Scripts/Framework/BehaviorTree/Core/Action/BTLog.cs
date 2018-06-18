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

        public override BehavioResult Behave(BaseEntity entity)
        {
            LogUtil.LogUtility.Print(_log);
            return BehavioResult.Success;
        }

        protected override void Reset()
        {
            throw new System.NotImplementedException();
        }
    }
}