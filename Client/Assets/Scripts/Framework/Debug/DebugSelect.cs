/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/21 23:13:04
** desc:  #####
*********************************************************************************/

using UnityEngine;

namespace Framework
{
    public partial class DebugMgr
    {
        public class DebugSelect
        {
            public void DrawSelect(int windowId)
            {
                GUI.DragWindow(singleton._dragRect);
                GUILayout.Label("<b>D开启调试,F4关闭调试!</b>");
                if (GUILayout.Button("返回上级", GUILayout.Width(100f), GUILayout.Height(35f)))
                {
                    singleton.SetShowType(ShowType.ShowFps);
                }
                GUILayout.Space(10);
                if (GUILayout.Button("日志信息", GUILayout.Width(100f), GUILayout.Height(35f)))
                {
                    singleton.SetShowType(ShowType.ShowLog);
                }
                GUILayout.Space(5);
                if (GUILayout.Button("系统信息", GUILayout.Width(100f), GUILayout.Height(35f)))
                {
                    singleton.SetShowType(ShowType.ShowSystem);
                }
                GUILayout.Space(5);
                if (GUILayout.Button("内存信息", GUILayout.Width(100f), GUILayout.Height(35f)))
                {
                    singleton.SetShowType(ShowType.ShowProfiler);
                }
                GUILayout.Space(5);
                if (GUILayout.Button("显示Fps", GUILayout.Width(100f), GUILayout.Height(35f)))
                {
                    singleton.SetShowType(ShowType.ShowFps);
                }
                GUILayout.Space(5);
                if (GUILayout.Button("关闭调试", GUILayout.Width(100f), GUILayout.Height(35f)))
                {
                    singleton.SetShowType(ShowType.ShowNon);
                }
            }
        }
    }
}