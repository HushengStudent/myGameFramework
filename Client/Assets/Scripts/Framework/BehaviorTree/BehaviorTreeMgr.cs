/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/15 18:31:48
** desc:  行为树管理;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class BehaviorTreeMgr : MonoSingleton<BehaviorTreeMgr>,IMgr
    {
        private Dictionary<BaseEntity, BehaviorTree> _tree = new Dictionary<BaseEntity, BehaviorTree>();

        public void InitMgr()
        {
            _tree.Clear();
        }

        public override void UpdateEx(float interval)
        {
            base.UpdateEx(interval);
            List<BehaviorTree> treeList = new List<BehaviorTree>(_tree.Values);
            for (int i = 0; i < treeList.Count; i++)
            {
                treeList[i].Update();
            }
        }
    }
}
