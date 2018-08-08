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
    public class FsmMgr : MonoSingleton<FsmMgr>, IMgr
    {
        private Dictionary<BaseEntity, FsmMachine> _fsmDict = new Dictionary<BaseEntity, FsmMachine>();
        private List<FsmMachine> _fsmList = new List<FsmMachine>();

        public void InitMgr()
        {
            _fsmDict.Clear();
            _fsmList.Clear();
        }

        protected override void UpdateEx(float interval)
        {
            base.UpdateEx(interval);
        }
    }
}
