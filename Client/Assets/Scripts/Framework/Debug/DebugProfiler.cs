/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/21 23:13:05
** desc:  #####
*********************************************************************************/

using UnityEngine;
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
                GUI.DragWindow(singleton._dragRect);
                if (GUILayout.Button("返回上级", GUILayout.Width(100f), GUILayout.Height(35f)))
                {
                    singleton.SetShowType(ShowType.ShowSelect);
                }
                GUILayout.Space(10);
                GUILayout.Label("<b>内存信息</b>");
                GUILayout.BeginVertical("box");
                {
                    singleton.DrawItem("Supported:", Profiler.supported.ToString());
                    singleton.DrawItem("Enable Binary Log:", Profiler.enableBinaryLog ? string.Format("True, {0}", Profiler.logFile) : "False");
                    singleton.DrawItem("Mono Used Size:", string.Format("{0} MB", (Profiler.GetMonoUsedSizeLong() / (float)MBSize).ToString("F3")));
                    singleton.DrawItem("Mono Heap Size:", string.Format("{0} MB", (Profiler.GetMonoHeapSizeLong() / (float)MBSize).ToString("F3")));
                    singleton.DrawItem("Used Heap Size:", string.Format("{0} MB", (Profiler.usedHeapSizeLong / (float)MBSize).ToString("F3")));
                    singleton.DrawItem("Total Allocated Memory:", string.Format("{0} MB", (Profiler.GetTotalAllocatedMemoryLong() / (float)MBSize).ToString("F3")));
                    singleton.DrawItem("Total Reserved Memory:", string.Format("{0} MB", (Profiler.GetTotalReservedMemoryLong() / (float)MBSize).ToString("F3")));
                    singleton.DrawItem("Total Unused Reserved Memory:", string.Format("{0} MB", (Profiler.GetTotalUnusedReservedMemoryLong() / (float)MBSize).ToString("F3")));
                }
                GUILayout.EndVertical();
            }
        }
    }
}
