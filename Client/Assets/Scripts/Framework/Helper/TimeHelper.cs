/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/02/05 01:29:56
** desc:  时间工具;
*********************************************************************************/

using System;

namespace Framework
{
    public static class TimeHelper
    {
        private static readonly long epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;

        /// <summary>
        /// 客户端时间;
        /// </summary>
        /// <returns></returns>
        public static long ClientNow()
        {
            return (DateTime.UtcNow.Ticks - epoch) / 10000;
        }

        public static long ClientNowSeconds()
        {
            return (DateTime.UtcNow.Ticks - epoch) / 10000000;
        }

        /// <summary>
        /// 登陆前是客户端时间,登陆后是同步过的服务器时间;
        /// </summary>
        /// <returns></returns>
        public static long Now()
        {
            return ClientNow();
        }

        /// <summary>
        /// 时间戳比较;
        /// </summary>
        /// <param name="utcSecconds">时间戳</param>
        /// <param name="targetUtcSecconds">时间戳</param>
        /// <returns></returns>
        public static double CompareTime(long utcSecconds, long targetUtcSecconds)
        {
            var _baseTimeTicks = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).Ticks;
            var dateTime = new DateTime(_baseTimeTicks + utcSecconds * 1000 * 10000L);
            var targetDateTime = new DateTime(_baseTimeTicks + targetUtcSecconds * 1000 * 10000L);
            var span = dateTime - targetDateTime;
            return span.TotalSeconds;
        }
    }
}