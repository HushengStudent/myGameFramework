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
                GUI.DragWindow(Singleton._dragRect);
                if (GUILayout.Button("返回上级", GUILayout.Width(100f), GUILayout.Height(35f)))
                {
                    Singleton.SetShowType(ShowType.ShowSelect);
                }
                GUILayout.Space(10);
                GUILayout.Label("<b>PersistentDataPath</b>");
                GUILayout.Label("<b>设备信息</b>");
                GUILayout.BeginVertical("box");
                {
                    Singleton.DrawItem("Device Unique ID:", SystemInfo.deviceUniqueIdentifier);
                    Singleton.DrawItem("Device Name:", SystemInfo.deviceName);
                    Singleton.DrawItem("Device Type:", SystemInfo.deviceType.ToString());
                    Singleton.DrawItem("Processor Type:", SystemInfo.processorType);
                    Singleton.DrawItem("Processor Count:", SystemInfo.processorCount.ToString());
                    Singleton.DrawItem("Memory Size:", string.Format("{0} MB", SystemInfo.systemMemorySize.ToString()));
                }
                GUILayout.EndVertical();
                GUILayout.Label("<b>显卡信息</b>");
                GUILayout.BeginVertical("box");
                {
                    Singleton.DrawItem("Device ID:", SystemInfo.graphicsDeviceID.ToString());
                    Singleton.DrawItem("Device Name:", SystemInfo.graphicsDeviceName);
                    Singleton.DrawItem("Memory Size:", string.Format("{0} MB", SystemInfo.graphicsMemorySize.ToString()));
                    Singleton.DrawItem("Shader Level:", GetShaderLevelString(SystemInfo.graphicsShaderLevel));
                    Singleton.DrawItem("Supports Shadows:", SystemInfo.supportsShadows.ToString());
                    Singleton.DrawItem("Supports Image Effects:", SystemInfo.supportsImageEffects.ToString());
                }
                GUILayout.EndVertical();
                GUILayout.Label("<b>其他信息</b>");
                GUILayout.BeginVertical("box");
                {
                    Singleton.DrawItem("Master Texture Limit:", QualitySettings.masterTextureLimit.ToString());
                    Singleton.DrawItem("Blend Weights:", QualitySettings.blendWeights.ToString());
                    Singleton.DrawItem("VSync Count:", QualitySettings.vSyncCount.ToString());
                    Singleton.DrawItem("LOD Bias:", QualitySettings.lodBias.ToString());
                    Singleton.DrawItem("Maximum LOD Level:", QualitySettings.maximumLODLevel.ToString());
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