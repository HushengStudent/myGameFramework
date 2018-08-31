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

        private BuffComponent _buffComponent;

        public BuffComponent BuffComp { get { return _buffComponent; } }

        protected override void InitEx()
        {
            base.InitEx();
            _buffComponent = ComponentMgr.Instance.CreateComponent<BuffComponent>(this);
        }

        protected override void UninitEx()
        {
            base.UninitEx();
            ComponentMgr.Instance.ReleaseComponent<BuffComponent>(_buffComponent);
        }
    }
}