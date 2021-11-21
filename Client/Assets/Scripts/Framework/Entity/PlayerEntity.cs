/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/08/24 23:34:51
** desc:  PlayerEntity;
*********************************************************************************/

using Framework.ECSModule;

namespace Framework
{
    public class PlayerEntity : RoleEntity
    {
        public override EntityType EntityType
        {
            get
            {
                return base.EntityType | EntityType.Player;
            }
        }
    }
}