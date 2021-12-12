/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/18 17:46:55
** desc:  并行执行节点;
*********************************************************************************/

using System.Collections;

namespace Framework.BehaviorTreeModule
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
                var finish = true;
                for (var i = 0; i < _list.Count; i++)
                {
                    switch (_list[i].Behave(Entity, interval))
                    {
                        case BehaviorState.Running:
                        case BehaviorState.Finish:
                            finish = false;
                            break;
                        case BehaviorState.Success:
                            break;
                        case BehaviorState.Failure:
                        case BehaviorState.Reset:
                            Reslut = BehaviorState.Failure;
                            LogHelper.PrintError("[BTParallel]BTParallel execute failure!");
                            return;
                        default:
                            Reslut = BehaviorState.Failure;
                            LogHelper.PrintError("[BTParallel]BTParallel execute failure!");
                            return;
                    }
                }
                if (finish)
                {
                    Reslut = BehaviorState.Finish;
                }
            }
        }
    }
}