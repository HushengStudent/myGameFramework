/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2020/03/22 15:44:03
** desc:  Lua UI±à¼­Æ÷;
*********************************************************************************/

using UnityEngine;
using UnityEngine.UI;

public class LuaUIEditorHelper
{
    public static string GetLuaUIComName(GameObject go)
    {
        if (!go)
        {
            return string.Empty;
        }

        var name = go.name;
        if (!name.StartsWith("Com_"))
        {
            return string.Empty;
        }
        name = name.Replace("Com_", "");
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
}