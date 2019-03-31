using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LogHelper
{
    private static bool _logEnable = true;
    private static bool _warningEnable = true;
    private static bool _errorEnable = true;

    public static bool LogEnable { get { return _logEnable; } set { _logEnable = value; } }
    public static bool WarningEnable { get { return _warningEnable; } set { _warningEnable = value; } }
    public static bool ErrorEnable { get { return _errorEnable; } set { _errorEnable = value; } }

    public static void Print(string str, string args = "")
    {
        if (_logEnable)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(str);
            builder.Append(args);
            Debug.Log(builder.ToString());
        }
    }

    public static void Print(string str, LogColor color, string args = "")
    {
        switch (color)
        {
            case LogColor.Non:
                Print(str, args);
                break;
            case LogColor.Red:
                PrintError(str, args);
                break;
            case LogColor.Green:
                if (_logEnable)
                {
                    PrintGreen(str, args);
                }
                break;
            case LogColor.Yellow:
                PrintWarning(str, args);
                break;
            default:
                Print(str, args);
                break;
        }
    }

    public static void PrintWarning(string str, string args = "")
    {
        if (_warningEnable)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<color=#EEEE00> ");
            builder.Append(str);
            builder.Append(" </color>\n");
            builder.Append(args);
            Debug.LogWarning(builder.ToString());
        }
    }

    public static void PrintGreen(string str, string args = "")
    {
        if (_warningEnable)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<color=#7FFF00> ");
            builder.Append(str);
            builder.Append(" </color>\n");
            builder.Append(args);
            Debug.Log(builder.ToString());
        }
    }

    public static void PrintError(string str, string args = "")
    {
        if (_errorEnable)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<color=#FF0000> ");
            builder.Append(str);
            builder.Append(" </color>\n");
            builder.Append(args);
            Debug.LogError(builder.ToString());
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