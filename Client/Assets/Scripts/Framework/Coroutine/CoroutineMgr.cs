/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/02/09 00:37:15
** desc:  协程管理;
*********************************************************************************/

using System.Collections;
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
            return StartCoroutine(coroutine);
        }
    }
}
