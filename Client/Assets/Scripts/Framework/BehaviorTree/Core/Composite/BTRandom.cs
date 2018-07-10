/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/18 17:52:44
** desc:  随机执行节点;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class BTRandom : AbsComposite
    {
        private List<AbsBehavior> _list = new List<AbsBehavior>();
        private AbsBehavior _target = null;

        public BTRandom(Hashtable table) : base(table)
        {
            _list.Clear();
            
        }

        public override void Serialize(List<AbsBehavior> behaviorList)
        {
            _list = behaviorList;
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
            _target = null;
        }

        protected override void UpdateEx()
        {
            if (Reslut == BehaviorState.Running)
            {
                int count = _list.Count;
                if (_target == null)
                {
                    int index = Random.Range(0, count);
                    _target = _list[index];
                }
                switch (_target.Behave(Entity))
                {
                    case BehaviorState.Running:
                        Reslut = BehaviorState.Running;
                        break;
                    case BehaviorState.Success:
                        Reslut = BehaviorState.Success;
                        break;
                    case BehaviorState.Failure:
                    case BehaviorState.Reset:
                        Reslut = BehaviorState.Failure;
                        LogUtil.LogUtility.PrintError("[BTRandom]BTRandom execute failure!");
                        break;
                    default:
                        Reslut = BehaviorState.Failure;
                        LogUtil.LogUtility.PrintError("[BTRandom]BTRandom execute failure!");
                        break;
                }
            }
        }
    }
}