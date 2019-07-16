/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/21 23:13:05
** desc:  调试工具;
*********************************************************************************/

using UnityEngine;
using System.Collections;

namespace Framework
{
    public partial class DebugMgr : MonoSingleton<DebugMgr>
    {
        private DebugFps _fpsInfo = null;
        private DebugSystem _sysInfo = new DebugSystem();
        private DebugSelect _selectInfo = new DebugSelect();
        private DebugProfiler _profilerInfo = new DebugProfiler();
        private DebugLog _logInfo = new DebugLog();

        private static float _myScreenWidth = 512f;
        private static float _myScreenHeight = 910f;

        private static float _myScaleWidth = (Screen.width / _myScreenWidth);
        private static float _myScaleHeight = (Screen.height / _myScreenHeight);

        private static float _myScale = _myScaleWidth <= _myScaleHeight ? _myScaleWidth : _myScaleHeight;

        internal static readonly Rect _defaultIconRect = new Rect(10f, 10f, 60f, 60f);
        internal static readonly Rect _defaultWindowRect = new Rect(10f, 10f, 492f, 870f);
        internal static readonly Rect _dDefaultSwitchRect = new Rect(10f, 10f, 120f, 120f);

        private Rect _iconRect = _defaultIconRect;
        private Rect _windowRect = _defaultWindowRect;

        private Rect _dragRect = new Rect(0f, 0f, float.MaxValue, 25f);

        private ShowType _showType = ShowType.ShowFps;

        private enum ShowType : int
        {
            ShowNon = 0,
            ShowFps,
            ShowSelect,
            ShowLog,
            ShowSystem,
            ShowProfiler,
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _fpsInfo = new DebugFps(0.5f);
            Application.logMessageReceived += _logInfo.OnLogMessageReceived;
            SetShowType(ShowType.ShowNon);
        }

        protected override void UpdateEx(float interval)
        {
            base.UpdateEx(interval);
            if (_fpsInfo == null)
                return;
            _fpsInfo.UpdateFps(Time.deltaTime, Time.unscaledDeltaTime);
            if (Input.GetKeyDown(KeyCode.D) && _showType == ShowType.ShowNon)
            {
                SetShowType(ShowType.ShowFps);
            }
            if (Input.GetKeyDown(KeyCode.F4) && _showType != ShowType.ShowNon)
            {
                SetShowType(ShowType.ShowNon);
            }
        }

        private void SetShowType(ShowType type)
        {
            _showType = type;
        }

        private void OnGUI()
        {
            if (_showType == ShowType.ShowNon) return;
            GUISkin cachedGuiSkin = GUI.skin;
            Matrix4x4 cachedMatrix = GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(_myScale, _myScale, 1));
            switch (_showType)
            {
                case ShowType.ShowFps:
                    _iconRect = GUILayout.Window((int)_showType, _iconRect, DrawFps, "<b>FPS</b>");
                    break;
                case ShowType.ShowLog:
                    _windowRect = GUILayout.Window((int)_showType, _windowRect, _logInfo.DrawLog, "<b>日志信息</b>");
                    break;
                case ShowType.ShowSelect:
                    _windowRect = GUILayout.Window((int)_showType, _windowRect, _selectInfo.DrawSelect, "<b>调试选项</b>");
                    break;
                case ShowType.ShowSystem:
                    _windowRect = GUILayout.Window((int)_showType, _windowRect, _sysInfo.DrawSystem, "<b>系统信息</b>");
                    break;
                case ShowType.ShowProfiler:
                    _windowRect = GUILayout.Window((int)_showType, _windowRect, _profilerInfo.DrawProfiler, "<b>内存信息</b>");
                    break;
                case ShowType.ShowNon:
                    break;
            }
            GUI.matrix = cachedMatrix;
            GUI.skin = cachedGuiSkin;
        }

        protected void DrawItem(string title, string content)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(title, GUILayout.Width(240f));
                GUILayout.Label(content);
            }
            GUILayout.EndHorizontal();
        }

        private void DrawFps(int windowId)
        {
            GUI.DragWindow(_dragRect);
            Color32 color = Color.white;
            Color32 colorLog = Color.red;
            string title = string.Format("<color=#{0}{1}{2}{3}><b>FPS:{4}</b></color>", color.r.ToString("x2"),
                color.g.ToString("x2"), color.b.ToString("x2"), color.a.ToString("x2"), _fpsInfo.CurrentFps.ToString("F2"));

            string error = string.Format("<color=#{0}{1}{2}{3}><b>Log:{4}</b></color>", colorLog.r.ToString("x2"),
                colorLog.g.ToString("x2"), colorLog.b.ToString("x2"), colorLog.a.ToString("x2"), _logInfo.GetCount() + "");
            if (GUILayout.Button(title + " " + error, GUILayout.Width(150f), GUILayout.Height(35f)))
            {
                SetShowType(ShowType.ShowSelect);
            }
        }
    }
}