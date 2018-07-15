/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/18 18:11:57
** desc:  重复执行节点;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class BTRepeater : AbsDecorator
    {
        public BTRepeater(Hashtable table) : base(table) { }

        protected override void AwakeEx()
        {
        }

        protected override void Reset()
        {
        }

        protected override void UpdateEx()
        {
        }
    }
}