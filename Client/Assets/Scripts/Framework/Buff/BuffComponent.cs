/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/06 19:48:41
** desc:  Buff组件
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class BuffComponent : AbsComponent
    {
        private List<Buff> _buffList = new List<Buff>();

        public override void OnResetComponent()
        {
            base.OnResetComponent();
            _buffList.Clear();
        }
    }
}
