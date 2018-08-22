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
    public class FsmMgr : MonoSingleton<FsmMgr>, IManager
    {
        private Dictionary<AbsEntity, FsmMachine> _fsmDict = new Dictionary<AbsEntity, FsmMachine>();

        public void Init()
        {
            _fsmDict.Clear();
        }

        protected override void UpdateEx(float interval)
        {
            base.UpdateEx(interval);
            foreach (var target in _fsmDict)
            {
                target.Value.Update(interval);
            }
        }

        public void CreateFsmMachine(AbsEntity entity, string name, List<AbsFsmState> stateList)
        {

        }
    }
}
