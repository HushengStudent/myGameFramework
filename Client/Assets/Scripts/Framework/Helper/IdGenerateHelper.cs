/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/02/05 01:32:16
** desc:  ID生成;
*********************************************************************************/

namespace Framework
{
    public static class IdGenerateHelper
    {
        private static long _id = 0;

        public static long GenerateId()
        {
            return _id++;
        }
    }
}