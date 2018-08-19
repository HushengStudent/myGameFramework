/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/06 17:15:17
** desc:  状态机管理;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class FsmMgr : MonoSingleton<FsmMgr>
    {
        private Dictionary<BaseEntity, FsmMachine> _fsmDict = new Dictionary<BaseEntity, FsmMachine>();

        protected override void InitEx()
        {
            base.InitEx();
            _fsmDict.Clear();
        }

        protected override void UpdateEx(float interval)
        {
            base.UpdateEx(interval);
            foreach(var target in _fsmDict)
            {
                target.Value.Update(interval);
            }
        }

        public void CreateFsmMachine(BaseEntity entity, string name,List<AbsFsmState> stateList)
        {

        }
    }
}
