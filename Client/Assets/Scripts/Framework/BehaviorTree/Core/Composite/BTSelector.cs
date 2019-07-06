/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/18 17:44:22
** desc:  选择执行节点;
*********************************************************************************/

using System.Collections;

namespace Framework
{
    public class BTSelector : AbsComposite
    {
        public BTSelector(Hashtable table) : base(table)
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

        protected override void UpdateEx(float interval)
        {
            if (Reslut == BehaviorState.Running)
            {
                for (int i = 0; i < _list.Count; i++)
                {
                    switch (_list[i].Behave(Entity, interval))
                    {
                        case BehaviorState.Running:
                        case BehaviorState.Finish:
                            Reslut = BehaviorState.Running;
                            return;
                        case BehaviorState.Success:
                            Reslut = BehaviorState.Finish;
                            return;
                        case BehaviorState.Failure:
                        case BehaviorState.Reset:
                            LogHelper.PrintError("[BTSelector]BTSelector execute failure!");
                            break;
                        default:
                            LogHelper.PrintError("[BTSelector]BTSelector execute failure!");
                            break;
                    }
                }
                Reslut = BehaviorState.Failure;
            }
        }
    }
}