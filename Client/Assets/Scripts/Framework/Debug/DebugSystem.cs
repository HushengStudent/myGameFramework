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
                GUI.DragWindow(singleton._dragRect);
                if (GUILayout.Button("返回上级", GUILayout.Width(100f), GUILayout.Height(35f)))
                {
                    singleton.SetShowType(ShowType.ShowSelect);
                }
                GUILayout.Space(10);
                GUILayout.Label("<b>PersistentDataPath</b>");
                GUILayout.Label("<b>设备信息</b>");
                GUILayout.BeginVertical("box");
                {
                    singleton.DrawItem("Device Unique ID:", SystemInfo.deviceUniqueIdentifier);
                    singleton.DrawItem("Device Name:", SystemInfo.deviceName);
                    singleton.DrawItem("Device Type:", SystemInfo.deviceType.ToString());
                    singleton.DrawItem("Processor Type:", SystemInfo.processorType);
                    singleton.DrawItem("Processor Count:", SystemInfo.processorCount.ToString());
                    singleton.DrawItem("Memory Size:", string.Format("{0} MB", SystemInfo.systemMemorySize.ToString()));
                }
                GUILayout.EndVertical();
                GUILayout.Label("<b>显卡信息</b>");
                GUILayout.BeginVertical("box");
                {
                    singleton.DrawItem("Device ID:", SystemInfo.graphicsDeviceID.ToString());
                    singleton.DrawItem("Device Name:", SystemInfo.graphicsDeviceName);
                    singleton.DrawItem("Memory Size:", string.Format("{0} MB", SystemInfo.graphicsMemorySize.ToString()));
                    singleton.DrawItem("Shader Level:", GetShaderLevelString(SystemInfo.graphicsShaderLevel));
                    singleton.DrawItem("Supports Shadows:", SystemInfo.supportsShadows.ToString());
                    singleton.DrawItem("Supports Image Effects:", SystemInfo.supportsImageEffects.ToString());
                }
                GUILayout.EndVertical();
                GUILayout.Label("<b>其他信息</b>");
                GUILayout.BeginVertical("box");
                {
                    singleton.DrawItem("Master Texture Limit:", QualitySettings.masterTextureLimit.ToString());
                    singleton.DrawItem("Blend Weights:", QualitySettings.blendWeights.ToString());
                    singleton.DrawItem("VSync Count:", QualitySettings.vSyncCount.ToString());
                    singleton.DrawItem("LOD Bias:", QualitySettings.lodBias.ToString());
                    singleton.DrawItem("Maximum LOD Level:", QualitySettings.maximumLODLevel.ToString());
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