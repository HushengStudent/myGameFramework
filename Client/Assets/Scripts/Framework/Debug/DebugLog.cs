/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/21 23:13:05
** desc:  日志;
*********************************************************************************/

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Framework
{
    public partial class DebugMgr
    {
        public class DebugLog
        {
            private LinkedList<LogNode> _logList = new LinkedList<LogNode>();
            private LinkedListNode<LogNode> _selectedNode = null;
            private string _dateTimeFormat = "[HH:mm:ss.fff] ";
            private int _maxCount = 100;
            public bool _automaticPopUp = true;
            private Vector2 _logScrollPosition = Vector2.zero;
            private Vector2 _stackScrollPosition = Vector2.zero;

            public void DrawLog(int windowId)
            {
                GUI.DragWindow(DebugMgr.Instance._dragRect);

                if (GUILayout.Button("返回上级", GUILayout.Width(100f), GUILayout.Height(35f)))
                {
                    //click button
                    DebugMgr.Instance.SetShowType(ShowType.ShowSelect);
                }
                GUILayout.Space(10);
                _automaticPopUp = GUILayout.Toggle(_automaticPopUp, "[error]弹出", GUILayout.Width(150f), GUILayout.Height(35f));
                GUILayout.Space(5);
                if (GUILayout.Button("清除日志", GUILayout.Width(100f), GUILayout.Height(35f)))
                {
                    ClearList();
                }
                GUILayout.Space(10);
                if (GUILayout.Button("点击复制", GUILayout.Width(100f), GUILayout.Height(35f)))
                {

                }
                GUILayout.BeginVertical("box");
                {
                    _logScrollPosition = GUILayout.BeginScrollView(_logScrollPosition, GUILayout.Height(350f));
                    {
                        bool selected = false;
                        for (LinkedListNode<LogNode> i = _logList.First; i != null; i = i.Next)
                        {
                            if (GUILayout.Toggle(_selectedNode == i, GetLogString(i.Value)))
                            {
                                selected = true;
                                if (_selectedNode != i)
                                {
                                    _selectedNode = i;
                                    _stackScrollPosition = Vector2.zero;
                                }
                            }
                        }
                        if (!selected)
                        {
                            _selectedNode = null;
                        }
                    }
                    GUILayout.EndScrollView();
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical("box");
                {
                    _stackScrollPosition = GUILayout.BeginScrollView(_stackScrollPosition, GUILayout.Height(350f));
                    {
                        if (_selectedNode != null)
                        {
                            Color32 color = GetLogStringColor(_selectedNode.Value.LogType);
                            GUILayout.Label(string.Format("<color=#{0}{1}{2}{3}><b>{4}</b></color>", color.r.ToString("x2"),
                                color.g.ToString("x2"), color.b.ToString("x2"), color.a.ToString("x2"), _selectedNode.Value.LogMessage));
                            GUILayout.Label(string.Format("<color=#{0}{1}{2}{3}><b>{4}</b></color>", color.r.ToString("x2"),
                                color.g.ToString("x2"), color.b.ToString("x2"), color.a.ToString("x2"), _selectedNode.Value.StackTrack));
                        }
                        GUILayout.EndScrollView();
                    }
                }
                GUILayout.EndVertical();
            }

            public void OnLogMessageReceived(string logMessage, string stackTrace, LogType logType)
            {
                _logList.AddLast(new LogNode(logType, logMessage, stackTrace));
                while (_logList.Count > _maxCount)
                {
                    _logList.RemoveFirst();
                }
                if (logType == LogType.Error && _automaticPopUp)
                {
                    DebugMgr.Instance._showType = ShowType.ShowLog;
                }
            }

            private string GetLogString(LogNode logNode)
            {
                Color32 color = GetLogStringColor(logNode.LogType);
                return string.Format("<color=#{0}{1}{2}{3}>{4}{5}</color>",
                    color.r.ToString("x2"), color.g.ToString("x2"), color.b.ToString("x2"), color.a.ToString("x2"),
                    logNode.LogTime.ToString(_dateTimeFormat), logNode.LogMessage);
            }

            internal Color32 GetLogStringColor(LogType logType)
            {
                Color32 color = Color.white;
                switch (logType)
                {
                    case LogType.Log:
                        color = Color.white;
                        break;
                    case LogType.Warning:
                        color = Color.blue;
                        break;
                    case LogType.Error:
                        color = Color.red;
                        break;
                    case LogType.Exception:
                        color = Color.yellow;
                        break;
                }
                return color;
            }

            private void ClearList()
            {
                _logList.Clear();
            }

            public int GetCount()
            {
                return _logList.Count;
            }

            private class LogNode
            {
                private DateTime logTime;
                private LogType logType;
                private string logMessage;
                private string stackTrack;

                public LogNode(LogType logType, string logMessage, string stackTrack)
                {
                    this.logTime = DateTime.Now;
                    this.logType = logType;
                    this.logMessage = logMessage;
                    this.stackTrack = stackTrack;
                }

                public DateTime LogTime
                {
                    get
                    {
                        return logTime;
                    }
                }

                public LogType LogType
                {
                    get
                    {
                        return logType;
                    }
                }

                public string LogMessage
                {
                    get
                    {
                        return logMessage;
                    }
                }

                public string StackTrack
                {
                    get
                    {
                        return stackTrack;
                    }
                }
            }
        }
    }
}