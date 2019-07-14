/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/23 23:54:19
** desc:  ugui源码;
*********************************************************************************/

using System.Collections.Generic;

namespace Framework.ObjectPool
{
    internal class CsharpListPool<T>
    {
        // Object pool to avoid allocations.
        private CsharpObjectPool<List<T>> s_ListPool = new CsharpObjectPool<List<T>>(null, l => l.Clear());

        public List<T> Get()
        {
            return s_ListPool.Get();
        }

        public void Release(List<T> toRelease)
        {
            s_ListPool.Release(toRelease);
        }
    }
}
