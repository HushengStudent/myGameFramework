/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/23 01:16:23
** desc:  日志执行节点;
*********************************************************************************/

using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.BehaviourTrees
{
    [Name("Log", 10)]
    [Category("MyGameFramework/Action/Log")]
    [Description("日志执行节点.")]
    [Icon("Log")]
    [Color("bf7fff")]
    public class BTLog : BTDecorator
    {
        public string Log;
    }
}