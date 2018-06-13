using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LogUtil
{
    public class LogUtility
    {
        public static void Print(string str)
        {
            Debug.Log(str);
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
                    Debug.Log("<color=#7FFF00> " + str + " </color>");
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
            Debug.LogWarning("<color=#EEEE00> " + str + " </color>");
        }

        public static void PrintError(string str)
        {
            Debug.LogError("<color=#FF0000> " + str + " </color>");
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
