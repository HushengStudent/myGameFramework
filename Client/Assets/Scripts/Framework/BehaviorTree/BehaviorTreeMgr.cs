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
    public enum BehaviorState : int
    {
        Reset = 0,
        Running = 1,
        Failure = 2,
        Finish = 3,
        Success = 4,
    }

    public class BehaviorTreeMgr : MonoSingleton<BehaviorTreeMgr>, IMgr
    {
        private Dictionary<BaseEntity, BehaviorTree> _tree = new Dictionary<BaseEntity, BehaviorTree>();
        private List<BehaviorTree> _treeList = new List<BehaviorTree>();

        public void InitMgr()
        {
            _tree.Clear();
        }

        public override void UpdateEx(float interval)
        {
            base.UpdateEx(interval);
            _treeList.Clear();
            foreach (var temp in _tree.Values)
                if (temp.Enable) _treeList.Add(temp);
            for (int i = 0; i < _treeList.Count; i++)
            {
                _treeList[i].Update();
            }
        }

        public void CreateBehaviorTree(BaseEntity entity, string path, bool enable = false)
        {
            BehaviorTree tree = BehaviorTreeFactory.CreateBehaviorTree(entity, path);
            if (entity == null)
            {
                LogUtil.LogUtility.PrintError("[BehaviorTreeMgr]Create BehaviorTree error,entity is null!");
                return;
            }
            if (_tree.ContainsKey(entity))
            {
                LogUtil.LogUtility.PrintWarning(string.Format("[BehaviorTreeMgr]repeat add BehaviorTree at EntityName: {0}.", entity.EntityName));
            }
            tree.Enable = enable;
            _tree[entity] = tree;
        }

        public void RemoveBehaviorTree(BaseEntity entity)
        {
            if (entity == null)
            {
                LogUtil.LogUtility.PrintError("[BehaviorTreeMgr]Remove BehaviorTree error,entity is null!");
                return;
            }
            if (!_tree.ContainsKey(entity))
            {
                LogUtil.LogUtility.PrintWarning(string.Format("[BehaviorTreeMgr]can not find a BehaviorTree at EntityName: {0}.", entity.EntityName));
            }
            else
            {
                _tree.Remove(entity);
            }
        }
    }
}
