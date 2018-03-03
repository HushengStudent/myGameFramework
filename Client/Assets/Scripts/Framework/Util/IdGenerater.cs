/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/02/05 01:32:16
** desc:  ID生成
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public static class IdGenerater
    {
        public static long AppId { private get; set; }

        private static ushort value;

        public static long GenerateId()
        {
            long time = TimeHelper.ClientNowSeconds();

            return (AppId << 48) + (time << 16) + ++value;
        }
    }
}