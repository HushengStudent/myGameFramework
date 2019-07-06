/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/23 01:31:00
** desc:  重复执行节点;
*********************************************************************************/

using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.BehaviourTrees
{
    [Name("Repeater", 10)]
    [Category("MyGameFramework/Decorator/Repeater")]
    [Description("重复执行节点.")]
    [Icon("Repeat")]
    [Color("bf7fff")]
    public class BTRepeater : BTDecorator
    {
        public int RepeaterTimes;
    }
}