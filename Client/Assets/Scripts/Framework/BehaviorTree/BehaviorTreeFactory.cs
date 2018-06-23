/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/24 01:43:40
** desc:  行为树工厂;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public static class BehaviorTreeFactory
    {
        public static BehaviorTree CreateBehaviorTree()
        {
            BehaviorTree tree = new BehaviorTree(null);
            return tree;
        }
    }
}