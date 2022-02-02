using System.IO;

public class ChangeScriptTemplates : UnityEditor.AssetModificationProcessor
{
    private static string str =
    "/********************************************************************************\r\n"
    + "** auth:  https://github.com/HushengStudent\r\n"
    + "** date:  #CreateTime#\r\n"
    + "** desc:  #####\r\n"
    + "*********************************************************************************/\r\n"
    + "\r\n";

    public static void OnWillCreateAsset(string path)
    {
        if (path.Contains("Assets/Scripts")
            && !path.Contains("Assets/Scripts/Common/Protocol")
            && !path.Contains("Assets/Scripts/Common/Table"))
        {
            path = path.Replace(".meta", "");
            if (path.EndsWith(".cs"))
            {
                string allText = str;
                allText += File.ReadAllText(path);
                allText = allText.Replace("#CreateTime#", System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                File.WriteAllText(path, allText);
            }
        }
    }
}
