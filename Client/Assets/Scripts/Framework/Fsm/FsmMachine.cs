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
    public class FsmMachine
    {
        private BaseEntity _entity = null;
        private AbsFsmState _currentState;

        public BaseEntity Entity { get { return _entity; } set { _entity = value; } }
        public AbsFsmState CurrentState { get { return _currentState; } set { _currentState = value; } }

    }
}