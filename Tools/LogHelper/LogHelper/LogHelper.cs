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

/*
VS预生成事件命令行 和 生成后事件命令行;
宏说明;
$(ConfigurationName) 当前项目配置的名称（例如，“Debug|Any CPU”）;
$(OutDir) 输出文件目录的路径，相对于项目目录。这解析为“输出目录”属性的值。它包括尾部的反斜杠“\”;
$(DevEnvDir) Visual Studio 2005 的安装目录（定义为驱动器 + 路径）；包括尾部的反斜杠“\”;
$(PlatformName) 当前目标平台的名称。例如“AnyCPU”;
$(ProjectDir) 项目的目录（定义为驱动器 + 路径）；包括尾部的反斜杠“\”;
$(ProjectPath) 项目的绝对路径名（定义为驱动器 + 路径 + 基本名称 + 文件扩展名）;
$(ProjectName) 项目的基本名称;
$(ProjectFileName) 项目的文件名（定义为基本名称 + 文件扩展名）;
$(ProjectExt) 项目的文件扩展名。它在文件扩展名的前面包括“.”;
$(SolutionDir) 解决方案的目录（定义为驱动器 + 路径）；包括尾部的反斜杠“\”;
$(SolutionPath) 解决方案的绝对路径名（定义为驱动器 + 路径 + 基本名称 + 文件扩展名）;
$(SolutionName) 解决方案的基本名称;
$(SolutionFileName) 解决方案的文件名（定义为基本名称 + 文件扩展名）;
$(SolutionExt) 解决方案的文件扩展名。它在文件扩展名的前面包括“.”;
$(TargetDir) 生成的主输出文件的目录（定义为驱动器 + 路径）。它包括尾部的反斜杠“\”;
$(TargetPath) 生成的主输出文件的绝对路径名（定义为驱动器 + 路径 + 基本名称 + 文件扩展名）;
$(TargetName) 生成的主输出文件的基本名称;
$(TargetFileName) 生成的主输出文件的文件名（定义为基本名称 + 文件扩展名）;
$(TargetExt) 生成的主输出文件的文件扩展名。它在文件扩展名的前面包括“.”;
* /
