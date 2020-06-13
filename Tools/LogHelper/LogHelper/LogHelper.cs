using UnityEngine;

public class LogHelper
{
    public static bool LogEnable { get; set; } = true;
    public static bool WarningEnable { get; set; } = true;
    public static bool ErrorEnable { get; set; } = true;

    public static void Print(string str, string stack = "")
    {
        if (LogEnable)
        {
            Debug.Log($"{str}\n{stack}");
        }
    }

    public static void PrintGreen(string str, string stack = "")
    {
        if (LogEnable)
        {
            Print($"<color=#008000>{str}\n{stack}</color>");
        }
    }

    public static void PrintRed(string str, string stack = "")
    {
        if (LogEnable)
        {
            Print($"<color=#FF0000>{str}\n{stack}</color>");
        }
    }

    public static void PrintYellow(string str, string stack = "")
    {
        if (LogEnable)
        {
            Print($"<color=#FFFF00>{str}\n{stack}</color>");
        }
    }

    public static void PrintWarning(string str, string stack = "")
    {
        if (WarningEnable)
        {
            Debug.LogWarning($"<color=#FFFF00>{str}\n{stack}</color>");
        }
    }

    public static void PrintError(string str, string stack = "")
    {
        if (ErrorEnable)
        {
            Debug.LogError($"<color=#FF0000>{str}\n{stack}</color>");
        }
    }
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
*/
