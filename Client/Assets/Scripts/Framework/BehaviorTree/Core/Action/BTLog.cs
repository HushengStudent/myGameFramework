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
    public class BTLog : AbsDecorator
    {
        private string _log = string.Empty;

        public BTLog(Hashtable table) : base(table)
        {

        }

        protected override void AwakeEx()
        {
        }

        protected override void Reset()
        {
        }

        protected override void UpdateExx()
        {
            throw new System.NotImplementedException();
        }
    }
}