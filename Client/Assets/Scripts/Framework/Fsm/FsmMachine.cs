/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/08/07 00:45:00
** desc:  ×´Ì¬»ú;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public sealed class FsmMachine
    {
        private string _name;
        private BaseEntity _entity = null;
        private AbsFsmState _currentState = null;
        private List<AbsFsmState> _stateList;

        public string Name { get { return _name; } set { _name = value; } }
        public BaseEntity Entity { get { return _entity; } set { _entity = value; } }
        public AbsFsmState CurrentState { get { return _currentState; } set { _currentState = value; } }
        public List<AbsFsmState> StateList { get { return _stateList; } set { _stateList = value; } }

        public FsmMachine(BaseEntity entity, string name, List<AbsFsmState> stateList)
        {
            _entity = entity;
            _name = name;
            _stateList = stateList;
        }

        public void Update(float interval)
        {
            if (CurrentState != null)
            {
                CurrentState.Update(interval);
            }
        }
    }
}