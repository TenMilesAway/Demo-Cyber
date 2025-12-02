using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class LogData
    {
        public string logString;
        public LogType type;
    }

    public class ConsoleComponent : BaseComponent
    {
        private List<LogData> _logs = new List<LogData>();
        private Vector2 _scrollPosition;

        private bool _showConsole = false;
        private bool _wasConsoleShown = false;

        private const int _maxLogsCount = 300;


        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        private void OnGUI()
        {
            if (!HADebug.DebugMode) return;

            // Console 按钮
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 36;
            buttonStyle.fixedHeight = 80;

            float xPosition = Screen.width - 200;
            float yPosition = 20;
            GUILayout.BeginArea(new Rect(xPosition, yPosition, 180, 100));

            if (GUILayout.Button("Console", buttonStyle))
            {
                _showConsole = !_showConsole;
                if (_showConsole && !_wasConsoleShown)
                {
                    _scrollPosition.y = float.MaxValue;
                }
                _wasConsoleShown = _showConsole;
            }

            GUILayout.EndArea();

            // Console 窗体
            if (_showConsole)
            {
                float margin = 100f;
                float topMargin = 150f;
                float windowWidth = Screen.width - (margin * 2);
                float windowHeight = Screen.height - (topMargin * 2 + margin);
                GUILayout.Window(0, new Rect(margin, topMargin, windowWidth, windowHeight), ConsoleWindow, "Console");
            }
        }

        #region 监听方法
        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (!HADebug.DebugMode) return;

            _logs.Add(new LogData { logString = string.Format("{0}\n{1}", GameManager.Timer.Now.ToString("T"), logString), type = type });
            _logs.Add(new LogData { logString = "--------------------------------------", type = LogType.Log });

            while (_logs.Count >= _maxLogsCount)
            {
                _logs.RemoveAt(0);
            }

            if (_showConsole)
            {
                _scrollPosition.y = float.MaxValue;
            }
        }
        #endregion

        #region 主要方法
        private void ConsoleWindow(int windowID)
        {
            GUILayout.BeginVertical();

            GUIStyle largeButtonStyle = new GUIStyle(GUI.skin.button);
            largeButtonStyle.fontSize = 30;
            largeButtonStyle.fixedHeight = 60;

            if (GUILayout.Button("Clear", largeButtonStyle))
            {
                _logs.Clear();
            }

            GUIStyle verticalScrollbarStyle = new GUIStyle(GUI.skin.verticalScrollbar);
            verticalScrollbarStyle.fixedWidth = 40f;

            GUIStyle verticalScrollbarThumbStyle = new GUIStyle(GUI.skin.verticalScrollbarThumb);
            verticalScrollbarThumbStyle.fixedWidth = 40f;

            GUI.skin.verticalScrollbarThumb = verticalScrollbarThumbStyle;

            _scrollPosition = GUILayout.BeginScrollView(
                _scrollPosition,
                false,
                true,
                GUIStyle.none, // horizontalScrollbarStyle,
                verticalScrollbarStyle
            );

            GUIStyle logStyle = new GUIStyle(GUI.skin.label);
            logStyle.fontSize = 30;

            foreach (LogData log in _logs)
            {
                switch (log.type)
                {
                    case LogType.Error:
                    case LogType.Assert:
                    case LogType.Exception:
                        logStyle.normal.textColor = Color.red;
                        break;
                    case LogType.Warning:
                        logStyle.normal.textColor = Color.yellow;
                        break;
                    case LogType.Log:
                        logStyle.normal.textColor = Color.white;
                        break;
                    default:
                        logStyle.normal.textColor = Color.white;
                        break;
                }
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                GUILayout.Label(log.logString, logStyle, GUILayout.ExpandWidth(false));
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            GUILayout.EndVertical();

            GUI.DragWindow(new Rect(0, 0, 10000, 40));
        }
        #endregion
    }
}
