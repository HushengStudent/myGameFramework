/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/21 23:13:05
** desc:  #####
*********************************************************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.Profiling;

namespace Framework
{
    public partial class DebugMgr
    {
        public class DebugProfiler
        {
            private const int MBSize = 1024 * 1024;
            public void DrawProfiler(int windowId)
            {
                GUI.DragWindow(DebugMgr.Instance._dragRect);
                if (GUILayout.Button("返回上级", GUILayout.Width(100f), GUILayout.Height(35f)))
                {
                    DebugMgr.Instance.SetShowType(ShowType.ShowSelect);
                }
                GUILayout.Space(10);
                GUILayout.Label("<b>内存信息</b>");
                GUILayout.BeginVertical("box");
                {
                    DebugMgr.Instance.DrawItem("Supported:", Profiler.supported.ToString());
                    DebugMgr.Instance.DrawItem("Enable Binary Log:", Profiler.enableBinaryLog ? string.Format("True, {0}", Profiler.logFile) : "False");
                    DebugMgr.Instance.DrawItem("Mono Used Size:", string.Format("{0} MB", (Profiler.GetMonoUsedSize() / (float)MBSize).ToString("F3")));
                    DebugMgr.Instance.DrawItem("Mono Heap Size:", string.Format("{0} MB", (Profiler.GetMonoHeapSize() / (float)MBSize).ToString("F3")));
                    DebugMgr.Instance.DrawItem("Used Heap Size:", string.Format("{0} MB", (Profiler.usedHeapSize / (float)MBSize).ToString("F3")));
                    DebugMgr.Instance.DrawItem("Total Allocated Memory:", string.Format("{0} MB", (Profiler.GetTotalAllocatedMemory() / (float)MBSize).ToString("F3")));
                    DebugMgr.Instance.DrawItem("Total Reserved Memory:", string.Format("{0} MB", (Profiler.GetTotalReservedMemory() / (float)MBSize).ToString("F3")));
                    DebugMgr.Instance.DrawItem("Total Unused Reserved Memory:", string.Format("{0} MB", (Profiler.GetTotalUnusedReservedMemory() / (float)MBSize).ToString("F3")));
                }
                GUILayout.EndVertical();
            }
        }
    }
}