/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/23 01:30:32
** desc:  等级条件执行节点;
*********************************************************************************/

using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.BehaviourTrees
{
    [Name("Level", 10)]
    [Category("MyGameFramework/Conditional/Level")]
    [Description("等级条件执行节点.")]
    [Icon("Condition")]
    [Color("bf7fff")]
    public class BTLevel : BTDecorator
    {
        public int Level;
    }
}