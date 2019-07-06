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
                GUI.DragWindow(Instance._dragRect);
                if (GUILayout.Button("返回上级", GUILayout.Width(100f), GUILayout.Height(35f)))
                {
                    Instance.SetShowType(ShowType.ShowSelect);
                }
                GUILayout.Space(10);
                GUILayout.Label("<b>PersistentDataPath</b>");
                GUILayout.Label("<b>设备信息</b>");
                GUILayout.BeginVertical("box");
                {
                    Instance.DrawItem("Device Unique ID:", SystemInfo.deviceUniqueIdentifier);
                    Instance.DrawItem("Device Name:", SystemInfo.deviceName);
                    Instance.DrawItem("Device Type:", SystemInfo.deviceType.ToString());
                    Instance.DrawItem("Processor Type:", SystemInfo.processorType);
                    Instance.DrawItem("Processor Count:", SystemInfo.processorCount.ToString());
                    Instance.DrawItem("Memory Size:", string.Format("{0} MB", SystemInfo.systemMemorySize.ToString()));
                }
                GUILayout.EndVertical();
                GUILayout.Label("<b>显卡信息</b>");
                GUILayout.BeginVertical("box");
                {
                    Instance.DrawItem("Device ID:", SystemInfo.graphicsDeviceID.ToString());
                    Instance.DrawItem("Device Name:", SystemInfo.graphicsDeviceName);
                    Instance.DrawItem("Memory Size:", string.Format("{0} MB", SystemInfo.graphicsMemorySize.ToString()));
                    Instance.DrawItem("Shader Level:", GetShaderLevelString(SystemInfo.graphicsShaderLevel));
                    Instance.DrawItem("Supports Shadows:", SystemInfo.supportsShadows.ToString());
                    Instance.DrawItem("Supports Image Effects:", SystemInfo.supportsImageEffects.ToString());
                }
                GUILayout.EndVertical();
                GUILayout.Label("<b>其他信息</b>");
                GUILayout.BeginVertical("box");
                {
                    Instance.DrawItem("Master Texture Limit:", QualitySettings.masterTextureLimit.ToString());
                    Instance.DrawItem("Blend Weights:", QualitySettings.blendWeights.ToString());
                    Instance.DrawItem("VSync Count:", QualitySettings.vSyncCount.ToString());
                    Instance.DrawItem("LOD Bias:", QualitySettings.lodBias.ToString());
                    Instance.DrawItem("Maximum LOD Level:", QualitySettings.maximumLODLevel.ToString());
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