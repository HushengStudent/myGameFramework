/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/23 01:31:30
** desc:  技能执行节点;
*********************************************************************************/

using ParadoxNotion.Design;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeCanvas.BehaviourTrees
{
    [Name("ExecuteSkill", 10)]
    [Category("MyGameFramework/Task/Skill/ExecuteSkill")]
    [Description("技能执行节点.")]
    [Icon("Sequencer")]
    [Color("bf7fff")]
    public class BTExecuteSkill : BTDecorator
    {
        public int SkillId;
    }
}