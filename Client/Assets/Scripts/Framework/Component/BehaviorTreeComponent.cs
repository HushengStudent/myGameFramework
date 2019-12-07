/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/12/07 17:04:54
** desc:  ×´Ì¬»ú×é¼þ;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class BehaviorTreeComponent : AbsComponent
    {
        public override string UID
        {
            get
            {
                return HashHelper.GetMD5(typeof(BehaviorTreeComponent).ToString());
            }
        }

        protected override void InitializeEx()
        {
            base.InitializeEx();
        }

        protected override void UnInitializeEx()
        {
            base.UnInitializeEx();
        }
    }
}