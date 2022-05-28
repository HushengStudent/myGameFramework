/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/02/09 00:37:15
** desc:  协程管理;
*********************************************************************************/

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Framework
{
    public class CoroutineMgr : MonoSingleton<CoroutineMgr>
    {
        private static WaitForEndOfFrame _waitForEndOfFrame;
        public static WaitForEndOfFrame WaitForEndOfFrame
        {
            get
            {
                if (_waitForEndOfFrame == null)
                {
                    _waitForEndOfFrame = new WaitForEndOfFrame();
                }
                return _waitForEndOfFrame;
            }
        }

        public Coroutine RunCoroutine(IEnumerator coroutine)
        {
            return StartCoroutine(CoroutineWrap(coroutine));
        }

        private IEnumerator CoroutineWrap(IEnumerator coroutine)
        {
            var stacktrace = new StackTrace();

            var itor = coroutine;
            while (true)
            {
                bool state;
                try
                {
                    state = itor.MoveNext();
                }
                catch (Exception e)
                {
                    if (e is GameException)
                    {
                        (e as GameException).PrintException(stacktrace);
                    }
                    else
                    {
                        LogHelper.PrintError(e.ToString());
                    }
                    break;
                }
                if (!state)
                {
                    break;
                }
                yield return WaitForEndOfFrame;
            }
        }
    }
}