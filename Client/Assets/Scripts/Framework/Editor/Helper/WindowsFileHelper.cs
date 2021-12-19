/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2020/06/14 19:26:39
** desc:  文件操作;
*********************************************************************************/

namespace FrameworkEditor
{
    public class WindowsFileHelper
    {
        public static void OpenExplorer(string path)
        {
            System.Diagnostics.Process.Start("Explorer.exe", $"/select,{path}");
        }

        public static void OpenFile(string path)
        {
            System.Diagnostics.Process.Start($"{path}");
        }
    }
}