using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LogUtil
{
    public class LogUtility
    {
        private static bool _logEnable = true;
        private static bool _warningEnable = true;
        private static bool _errorEnable = true;

        public static bool LogEnable { get { return _logEnable; } set { _logEnable = value; } }
        public static bool WarningEnable { get { return _warningEnable; } set { _warningEnable = value; } }
        public static bool ErrorEnable { get { return _errorEnable; } set { _errorEnable = value; } }

        public static void Print(string str)
        {
            if (_logEnable)
            {
                Debug.Log(str);
            }
        }

        public static void Print(string str, LogColor color)
        {
            switch (color)
            {
                case LogColor.Non:
                    Print(str);
                    break;
                case LogColor.Red:
                    PrintError(str);
                    break;
                case LogColor.Green:
                    if (_logEnable)
                    {
                        Debug.Log("<color=#7FFF00> " + str + " </color>");
                    }
                    break;
                case LogColor.Yellow:
                    PrintWarning(str);
                    break;
                default:
                    Print(str);
                    break;
            }
        }

        public static void PrintWarning(string str)
        {
            if (_warningEnable)
            {
                Debug.LogWarning("<color=#EEEE00> " + str + " </color>");
            }
        }

        public static void PrintError(string str)
        {
            if (_errorEnable)
            {
                Debug.LogError("<color=#FF0000> " + str + " </color>");
            }
        }
    }

    public enum LogColor : byte
    {
        Non = 0,
        Red = 1,
        Green = 2,
        Yellow = 3,
    }
}
