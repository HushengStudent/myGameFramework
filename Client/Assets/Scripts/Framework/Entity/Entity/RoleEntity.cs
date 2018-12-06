/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/08/24 23:24:46
** desc:  RoleEntity;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class RoleEntity : AbsEntity
    {
        public override EntityTypeEnum EntityType
        {
            get
            {
                return EntityTypeEnum.Role;
            }
        }

        public BuffComponent BuffComp { get; private set; }

        protected override void InitEx()
        {
            base.InitEx();
            BuffComp = ComponentMgr.Instance.CreateComponent<BuffComponent>(this);
        }

        protected override void UninitEx()
        {
            base.UninitEx();
            ComponentMgr.Instance.ReleaseComponent<BuffComponent>(BuffComp);
        }
    }
}