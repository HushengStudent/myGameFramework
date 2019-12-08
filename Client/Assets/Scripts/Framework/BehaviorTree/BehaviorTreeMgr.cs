/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/15 18:31:48
** desc:  行为树管理;
*********************************************************************************/

using System.Collections.Generic;

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

    public class BehaviorTreeMgr : MonoSingleton<BehaviorTreeMgr>
    {
        private Dictionary<AbsEntity, BehaviorTree> _behaviorTreeDict
            = new Dictionary<AbsEntity, BehaviorTree>();

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _behaviorTreeDict.Clear();
        }

        protected override void UpdateEx(float interval)
        {
            base.UpdateEx(interval);
            foreach (var tree in _behaviorTreeDict)
            {
                if (tree.Value.Enable)
                {
                    tree.Value.Update(interval);
                }
            }
        }

        public void CreateBehaviorTree(AbsEntity entity, string path, bool enable = false)
        {
            BehaviorTree tree = BehaviorTreeFactory.CreateBehaviorTree(entity, path);
            if (entity == null)
            {
                LogHelper.PrintError("[BehaviorTreeMgr]Create BehaviorTree error,entity is null!");
                return;
            }
            if (_behaviorTreeDict.ContainsKey(entity))
            {
                LogHelper.PrintWarning($"[BehaviorTreeMgr]repeat add BehaviorTree at EntityName: {entity.EntityName}.");
            }
            tree.Enable = enable;
            _behaviorTreeDict[entity] = tree;
        }

        public void RemoveBehaviorTree(AbsEntity entity)
        {
            if (entity == null)
            {
                LogHelper.PrintError("[BehaviorTreeMgr]Remove BehaviorTree error,entity is null!");
                return;
            }
            if (!_behaviorTreeDict.ContainsKey(entity))
            {
                LogHelper.PrintWarning($"[BehaviorTreeMgr]can not find a BehaviorTree at EntityName: {entity.EntityName}.");
            }
            else
            {
                _behaviorTreeDict.Remove(entity);
            }
        }
    }
}
