/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/18 18:11:39
** desc:  返回失败执行节点;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class BTReturnFailure : AbsBehavior
    {
        public BTReturnFailure(object[] args) : base(args) { }

        protected override void AwakeEx()
        {
            throw new System.NotImplementedException();
        }

        protected override void Reset()
        {
            throw new System.NotImplementedException();
        }

        protected override void UpdateEx()
        {
            throw new System.NotImplementedException();
        }
    }
}