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
    public class BTParallel : AbsComposite
    {
        public BTParallel(Hashtable table) : base(table)
        {
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

        protected override void UpdateExx()
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