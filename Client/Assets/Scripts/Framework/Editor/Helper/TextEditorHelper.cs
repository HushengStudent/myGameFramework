/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/9/1 16:20:33
** desc:  复制剪切板;
*********************************************************************************/

using UnityEngine;

namespace Framework
{
    public class TextEditorHelper
    {
        public static void Copy(string str)
        {
            TextEditor textEditor = new TextEditor
            {
                text = str
            };
            textEditor.SelectAll();
            textEditor.Copy();
        }
    }
}
