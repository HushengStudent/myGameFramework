/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/27 15:37:48
** desc:  内存管理;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Framework
{
    public class MemoryMgr : MonoSingleton<MemoryMgr>, IMgr
    {
        private int _maxMemoryUse = 1024;
        private int _monitTime = 10;
        private float _lastUpdateTimer;
        private const int MBSize = 1024 * 1024;

        public int MaxMemoryUse { get { return _maxMemoryUse; } set { _maxMemoryUse = value; } }

        public void InitMgr()
        {
            float allMenory = GetAllMemory();
            LogUtil.LogUtility.Print(string.Format("Used Heap Size: {0} MB", allMenory.ToString("F3")));
        }

        public override void AwakeEx()
        {
            base.AwakeEx();
            _lastUpdateTimer = Time.realtimeSinceStartup;
        }

        public override void UpdateEx()
        {
            base.UpdateEx();
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
                LogUtil.LogUtility.PrintError(string.Format("Used Heap Size: {0} MB", allMenory.ToString("F3")));
                FreeMemory();
            }
            else
            {
                LogUtil.LogUtility.Print(string.Format("Used Heap Size: {0} MB", allMenory.ToString("F3")));
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
