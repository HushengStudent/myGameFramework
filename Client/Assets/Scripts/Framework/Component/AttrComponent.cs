/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/12/07 17:01:23
** desc:  �������;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class AttrComponent : AbsComponent
    {
        public override string UID
        {
            get
            {
                return HashHelper.GetMD5(typeof(AttrComponent).ToString());
            }
        }
    }
}