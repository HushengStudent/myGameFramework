/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/18 13:13:25
** desc:  #####
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class BehaviorTree
    {
        private bool _enable;

        public bool Enable { get { return _enable; } set { _enable = value; } }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}