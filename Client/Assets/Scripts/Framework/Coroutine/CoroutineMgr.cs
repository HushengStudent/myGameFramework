/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/02/09 00:37:15
** desc:  协程管理;
*********************************************************************************/

using MEC;
using System.Collections.Generic;

namespace Framework
{
    public class CoroutineMgr : MonoSingleton<CoroutineMgr>
    {
        public CoroutineHandle RunCoroutine(IEnumerator<float> coroutine)
        {
            return Timing.RunCoroutine(coroutine);
        }

        public CoroutineHandle RunCoroutine(IEnumerator<float> coroutine, string tag)
        {
            return Timing.RunCoroutine(coroutine, tag);
        }

        public CoroutineHandle RunCoroutine(IEnumerator<float> coroutine, Segment timing)
        {
            return Timing.RunCoroutine(coroutine, timing);
        }

        public CoroutineHandle RunCoroutine(IEnumerator<float> coroutine, Segment timing, string tag)
        {
            return Timing.RunCoroutine(coroutine, timing, tag);
        }

        public int KillAllCoroutines(string methodName)
        {
            return Timing.KillCoroutines();
        }

        public int KillCoroutines(CoroutineHandle handle)
        {
            return Timing.KillCoroutines(handle);
        }

        public int KillCoroutines(string tag)
        {
            return Timing.KillCoroutines(tag);
        }
    }
}
