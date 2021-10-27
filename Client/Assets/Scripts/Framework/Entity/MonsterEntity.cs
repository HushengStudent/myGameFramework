/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/08/24 23:35:20
** desc:  MonsterEntity;
*********************************************************************************/

namespace Framework
{
    public class MonsterEntity : AbsEntity
    {
        public override EntityType EntityType
        {
            get
            {
                return EntityType.Monster;
            }
        }
    }
}