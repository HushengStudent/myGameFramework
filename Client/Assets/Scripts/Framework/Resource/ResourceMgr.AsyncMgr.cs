/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/12/11 23:34:41
** desc:  异步加载管理;
*********************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{
    public partial class ResourceMgr
    {
        private class AsyncMgr
        {
            private static float _curStartTime = 0f;

            private static ulong _loadID = ulong.MaxValue;
            public static ulong LoadID
            {
                get
                {
                    if (_loadID < 1)
                        _loadID = ulong.MaxValue;
                    return _loadID--;
                }
            }

            private static ulong _curLoadID = 0;
            public static ulong CurLoadID
            {
                get
                {
                    if (_curLoadID > 0)
                    {
                        return _curLoadID;
                    }
                    if (_curLoadID == 0)
                    {
                        if (AsyncLoadIdLinkedList.Count == 0)
                        {
                            LogHelper.PrintError("[ResourceMgr]Get CurLoadID error!");
                            _curLoadID = 0;
                        }
                        _curLoadID = AsyncLoadIdLinkedList.First.Value;
                        _curStartTime = GetCurTime();
                    }
                    return _curLoadID;
                }
                set
                {
                    if (0 == value)
                    {
                        Remove(_curLoadID);
                        _curLoadID = 0;
                    }
                }
            }

            //维护加载资源ID队列,这样加载完成的回调执行的先后顺序就只与脚本调用接口的顺序有关了;
            private static LinkedList<ulong> AsyncLoadIdLinkedList = new LinkedList<ulong>();

            //同时加载最大数;
            public static readonly int ASYNC_LOAD_MAX_VALUE = 50;
            //加载完成后等待时间;
            public static readonly float ASYNC_WAIT_MAX_DURATION = 500f;

            /// <summary>
            /// 添加;
            /// </summary>
            /// <param name="id"></param>
            public static void Add(ulong id)
            {
                AsyncLoadIdLinkedList.AddLast(id);
            }

            /// <summary>
            /// 删除;
            /// </summary>
            /// <param name="id"></param>
            public static void Remove(ulong id)
            {
                AsyncLoadIdLinkedList.Remove(id);
            }

            /// <summary>
            /// 当前数量;
            /// </summary>
            /// <returns></returns>
            public static int CurCount()
            {
                return AsyncLoadIdLinkedList.Count;
            }

            /// <summary>
            /// 是否在加载队列;
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static bool IsContains(ulong id)
            {
                return AsyncLoadIdLinkedList.Contains(id);
            }

            /// <summary>
            /// 当前时间;
            /// </summary>
            /// <returns></returns>
            public static float GetCurTime()
            {
                return Time.realtimeSinceStartup;
            }

            /// <summary>
            /// 是否超时;
            /// </summary>
            /// <param name="time"></param>
            /// <returns></returns>
            public static bool IsTimeOverflows(float time)
            {
                return (Time.realtimeSinceStartup - time) > ASYNC_WAIT_MAX_DURATION;
            }

            /// <summary>
            /// 当前加载是否超时;
            /// </summary>
            /// <returns></returns>
            public static bool CurLoadTimeOverflows()
            {
                return IsTimeOverflows(_curStartTime);
            }
        }
    }
}
