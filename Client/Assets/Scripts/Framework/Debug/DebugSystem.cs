/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/21 23:13:04
** desc:  #####
*********************************************************************************/

using UnityEngine;
using System.Collections;

namespace Framework
{
    public partial class DebugMgr
    {
        private class DebugSystem
        {
            public void DrawSystem(int windowId)
            {
                GUI.DragWindow(DebugMgr.Instance._dragRect);
                if (GUILayout.Button("返回上级", GUILayout.Width(100f), GUILayout.Height(35f)))
                {
                    DebugMgr.Instance.SetShowType(ShowType.ShowSelect);
                }
                GUILayout.Space(10);
                GUILayout.Label("<b>PersistentDataPath</b>");
                GUILayout.Label("<b>设备信息</b>");
                GUILayout.BeginVertical("box");
                {
                    DebugMgr.Instance.DrawItem("Device Unique ID:", SystemInfo.deviceUniqueIdentifier);
                    DebugMgr.Instance.DrawItem("Device Name:", SystemInfo.deviceName);
                    DebugMgr.Instance.DrawItem("Device Type:", SystemInfo.deviceType.ToString());
                    DebugMgr.Instance.DrawItem("Processor Type:", SystemInfo.processorType);
                    DebugMgr.Instance.DrawItem("Processor Count:", SystemInfo.processorCount.ToString());
                    DebugMgr.Instance.DrawItem("Memory Size:", string.Format("{0} MB", SystemInfo.systemMemorySize.ToString()));
                }
                GUILayout.EndVertical();
                GUILayout.Label("<b>显卡信息</b>");
                GUILayout.BeginVertical("box");
                {
                    DebugMgr.Instance.DrawItem("Device ID:", SystemInfo.graphicsDeviceID.ToString());
                    DebugMgr.Instance.DrawItem("Device Name:", SystemInfo.graphicsDeviceName);
                    DebugMgr.Instance.DrawItem("Memory Size:", string.Format("{0} MB", SystemInfo.graphicsMemorySize.ToString()));
                    DebugMgr.Instance.DrawItem("Shader Level:", GetShaderLevelString(SystemInfo.graphicsShaderLevel));
                    DebugMgr.Instance.DrawItem("Supports Shadows:", SystemInfo.supportsShadows.ToString());
                    DebugMgr.Instance.DrawItem("Supports Image Effects:", SystemInfo.supportsImageEffects.ToString());
                }
                GUILayout.EndVertical();
                GUILayout.Label("<b>其他信息</b>");
                GUILayout.BeginVertical("box");
                {
                    DebugMgr.Instance.DrawItem("Master Texture Limit:", QualitySettings.masterTextureLimit.ToString());
                    DebugMgr.Instance.DrawItem("Blend Weights:", QualitySettings.blendWeights.ToString());
                    DebugMgr.Instance.DrawItem("VSync Count:", QualitySettings.vSyncCount.ToString());
                    DebugMgr.Instance.DrawItem("LOD Bias:", QualitySettings.lodBias.ToString());
                    DebugMgr.Instance.DrawItem("Maximum LOD Level:", QualitySettings.maximumLODLevel.ToString());
                }
                GUILayout.EndVertical();
            }

            private string GetShaderLevelString(int shaderLevel)
            {
                return string.Format("Shader Model {0}.{1}", (shaderLevel / 10).ToString(), (shaderLevel % 10).ToString());
            }
        }
    }
}