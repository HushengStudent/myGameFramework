/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/08/24 23:34:51
** desc:  PlayerEntity;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class PlayerEntity : RoleEntity
    {
        public override EntityTypeEnum EntityType
        {
            get
            {
                return base.EntityType | EntityTypeEnum.Player;
            }
        }
    }
}