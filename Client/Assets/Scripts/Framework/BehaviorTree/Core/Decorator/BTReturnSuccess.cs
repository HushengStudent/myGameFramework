/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/18 18:11:19
** desc:  返回成功执行节点;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class BTReturnSuccess : AbsDecorator
    {
        public BTReturnSuccess(Hashtable table) : base(table) { }

        protected override void AwakeEx()
        {
            throw new System.NotImplementedException();
        }

        protected override void Reset()
        {
            throw new System.NotImplementedException();
        }

        protected override void UpdateExx()
        {
            throw new System.NotImplementedException();
        }
    }
}