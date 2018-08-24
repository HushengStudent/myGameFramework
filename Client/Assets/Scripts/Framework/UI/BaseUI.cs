/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/17 03:29:33
** desc:  ui基类;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public abstract class BaseUI
    {
        private string _name = string.Empty;

        public string Name { get { return _name; } }
        public bool Enable { get; set; }

        public abstract void OnInit();
        public abstract void Update();
        public abstract void OnActive();
        public abstract void DeActive();
        public abstract void OnDestroy();
    }
}
