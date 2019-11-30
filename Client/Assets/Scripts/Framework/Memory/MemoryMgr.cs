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
        private readonly int _monitTime = 10;
        private float _lastUpdateTimer;
        private const int MBSize = 1024 * 1024;

        public int MaxMemoryUse { get; set; } = 1024;

        protected override void OnInitialize()
        {
            float allMenory = GetAllMemory();
            LogHelper.Print($"Used Heap Size: {allMenory.ToString("F3")} MB");
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
            if (allMenory > MaxMemoryUse)
            {
                LogHelper.PrintError($"Used Heap Size: {allMenory.ToString("F3")} MB");
                FreeMemory();
            }
            else
            {
                LogHelper.Print($"Used Heap Size: {allMenory.ToString("F3")} MB");
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
