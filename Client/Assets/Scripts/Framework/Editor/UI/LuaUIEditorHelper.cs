/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2020/03/22 15:44:03
** desc:  Lua UI±à¼­Æ÷;
*********************************************************************************/

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LuaUIEditorHelper
{
    public readonly static string FilePath =
            "Assets/LuaFramework/Lua/UI/Panel/";

    public readonly static string LuaCom =
        "    self.layout.Name = component.LuaUIComArray[index]";

    public readonly static string LuaTemplate =
        "    self.layout.Name = component.LuaUITemplateArray[index]";

    public readonly static string Header = "Com_";

    public static string GetLuaUIComName(GameObject go)
    {
        if (!go)
        {
            return string.Empty;
        }

        var name = go.name;
        if (!name.StartsWith(Header))
        {
            return string.Empty;
        }
        name = name.Replace(Header, "");
        name = CheckName(name);

        if (name.EndsWith("Com"))
        {
            return $"_{name}";
        }
        if (go.GetComponent<Button>())
        {
            return $"_{name}Btn";
        }
        if (go.GetComponent<Image>())
        {
            return $"_{name}Img";
        }
        if (go.GetComponent<RawImage>())
        {
            return $"_{name}RawImg";
        }
        if (go.GetComponent<ScrollRect>())
        {
            return $"_{name}Scroll";
        }
        if (go.GetComponent<InputField>())
        {
            return $"_{name}InputField";
        }
        if (go.GetComponent<Toggle>())
        {
            return $"_{name}Tog";
        }
        if (go.GetComponent<Slider>())
        {
            return $"_{name}Slider";
        }
        if (go.GetComponent<Text>())
        {
            return $"_{name}Text";
        }
        return $"_{name}Com";
    }

    public static string CheckName(string name)
    {
        name = name.Replace(" ", "").Replace(".", "").Replace("¡£", "")
            .Replace("(", "").Replace("£¨", "").Replace(")", "").Replace("£©", "")
            .Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", "")
            .Replace("¡¾", "").Replace("¡¿", "").Replace("_", "");
        return $"{name.Substring(0, 1).ToLower()}{name.Substring(1)}";
    }

    [MenuItem("GameObject/UI/Rename UI Node %#c")]
    public static void MaterialPropertyClean()
    {
        var objects = Selection.gameObjects;
        for (int i = 0; i < objects.Length; i++)
        {
            var go = objects[i];
            if (go.name.StartsWith(Header))
            {
                continue;
            }
            else
            {
                go.name = $"{Header}{go.name}";
            }
            EditorUtility.SetDirty(go);
        }
    }
}