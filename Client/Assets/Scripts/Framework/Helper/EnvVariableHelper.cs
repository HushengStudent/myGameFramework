/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/04/20 21:57:59
** desc:  环境变量;
*********************************************************************************/

using System;

namespace Framework
{
    public static class EnvVariableHelper
    {
        public static string ProtocPath
        {
            get
            {
                string protocPath = Environment.GetEnvironmentVariable("myGameFramework_protoc");
                if (string.IsNullOrEmpty(protocPath))
                {
                    LogHelper.PrintError("not find Environment variable:myGameFramework_protoc");
                }
                return protocPath;
            }
        }

        public static string GameFrameworkPath
        {
            get
            {
                string gameFrameworkPath = Environment.GetEnvironmentVariable("myGameFramework");
                if (string.IsNullOrEmpty(gameFrameworkPath))
                {
                    LogHelper.PrintError("not find Environment variable:myGameFramework");
                }
                return gameFrameworkPath;
            }
        }
    }
}