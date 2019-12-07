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
        public override EntityType EntityType
        {
            get
            {
                return EntityType.Role;
            }
        }

        public BuffComponent BuffComp { get; private set; }

        protected override void RegisterComponent()
        {
            base.RegisterComponent();
            BuffComp = ComponentMgr.Instance.CreateComponent<BuffComponent>(this);
        }

        protected override void UnRegisterComponent()
        {
            base.UnRegisterComponent();
            ComponentMgr.Instance.ReleaseComponent<BuffComponent>(BuffComp);
        }
    }
}