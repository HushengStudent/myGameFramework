/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/18 18:08:49
** desc:  事件执行节点;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class BTEvent : AbsDecorator
    {
        public BTEvent(Hashtable table) : base(table) { }

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