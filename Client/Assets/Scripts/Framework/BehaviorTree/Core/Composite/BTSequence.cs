/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/18 14:04:14
** desc:  顺序执行节点;
*********************************************************************************/

using System.Collections;

namespace Framework
{
    public class BTSequence : AbsComposite
    {
        public BTSequence(Hashtable table) : base(table)
        {
        }

        protected override void AwakeEx()
        {
        }

        protected override void Reset()
        {
            //处理子节点;
            for (var i = 0; i < _list.Count; i++)
            {
                _list[i].ResetBehavior();
            }
            //处理自己;
        }

        protected override void UpdateEx(float interval)
        {
            if (Reslut == BehaviorState.Running)
            {
                for (var i = 0; i < _list.Count; i++)
                {
                    switch (_list[i].Behave(Entity, interval))
                    {
                        case BehaviorState.Running:
                        case BehaviorState.Finish:
                            Reslut = BehaviorState.Running;
                            return;
                        case BehaviorState.Success:
                            break;
                        case BehaviorState.Failure:
                        case BehaviorState.Reset:
                            Reslut = BehaviorState.Failure;
                            LogHelper.PrintError("[BTSequence]BTSequence execute failure!");
                            return;
                        default:
                            Reslut = BehaviorState.Failure;
                            LogHelper.PrintError("[BTSequence]BTSequence execute failure!");
                            return;
                    }
                }
                Reslut = BehaviorState.Finish;
            }
        }
    }
}