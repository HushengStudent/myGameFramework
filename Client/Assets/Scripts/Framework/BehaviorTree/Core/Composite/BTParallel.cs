/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/18 17:46:55
** desc:  并行执行节点;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class BTParallel : AbsBehavior
    {
        private List<AbsBehavior> _list = new List<AbsBehavior>();

        public BTParallel(object[] args) : base(args)
        {
            _list.Clear();
            for (int i = 0; i < args.Length; i++)
            {
                _list.Add((AbsBehavior)args[i]);
            }
        }

        protected override void AwakeEx()
        {
        }

        protected override void Reset()
        {
            //处理子节点;
            for (int i = 0; i < _list.Count; i++)
            {
                _list[i].ResetBehavior();
            }
            //处理自己;
        }

        protected override void UpdateEx()
        {
            if (Reslut == BehaviorState.Running)
            {
                bool success = true;
                for (int i = 0; i < _list.Count; i++)
                {
                    switch (_list[i].Behave(Entity))
                    {
                        case BehaviorState.Running:
                            success = false;
                            break;
                        case BehaviorState.Success:
                            break;
                        case BehaviorState.Failure:
                        case BehaviorState.Reset:
                            Reslut = BehaviorState.Failure;
                            LogUtil.LogUtility.PrintError("[BTParallel]BTParallel execute failure!");
                            return;
                        default:
                            Reslut = BehaviorState.Failure;
                            LogUtil.LogUtility.PrintError("[BTParallel]BTParallel execute failure!");
                            return;
                    }
                }
                if (success)
                {
                    Reslut = BehaviorState.Success;
                }
            }
        }
    }
}