/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/27 15:37:48
** desc:  内存管理;
*********************************************************************************/

using UnityEngine;
using UnityEngine.Profiling;

namespace Framework
{
    public class MemoryMgr : MonoSingleton<MemoryMgr>
    {
        private int _maxMemoryUse = 1024;
        private int _monitTime = 10;
        private float _lastUpdateTimer;
        private const int MBSize = 1024 * 1024;

        public int MaxMemoryUse { get { return _maxMemoryUse; } set { _maxMemoryUse = value; } }

        protected override void OnInitialize()
        {
            float allMenory = GetAllMemory();
            LogHelper.Print(string.Format("Used Heap Size: {0} MB", allMenory.ToString("F3")));
        }

        protected override void AwakeEx()
        {
            base.AwakeEx();
            _lastUpdateTimer = Time.realtimeSinceStartup;
        }

        protected override void UpdateEx(float interval)
        {
            base.UpdateEx(interval);
            if (Time.realtimeSinceStartup - _lastUpdateTimer > _monitTime)
            {
                _lastUpdateTimer = Time.realtimeSinceStartup;
                MonitorMemorySize();
            }
        }

        private void MonitorMemorySize()
        {
            float allMenory = GetAllMemory();
            if (allMenory > _maxMemoryUse)
            {
                LogHelper.PrintError(string.Format("Used Heap Size: {0} MB", allMenory.ToString("F3")));
                FreeMemory();
            }
            else
            {
                LogHelper.Print(string.Format("Used Heap Size: {0} MB", allMenory.ToString("F3")));
            }
        }

        private float GetAllMemory()
        {
            float allMenory = Profiler.usedHeapSize / (float)MBSize;
            return allMenory;
        }

        private void FreeMemory()
        {

        }
    }
}
