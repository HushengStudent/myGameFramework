/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/04/15 21:46:30
** desc:  图文混排工具;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    public class EmojiBuilder
    {
        private const string OutputPath = "Assets/ArtRes/Emoji/Output/";
        private const string InputPath = "Assets/ArtRes/Emoji/Input/";
        private static readonly Vector2[] AtlasSize = new Vector2[]{
        new Vector2(32,32),
        new Vector2(64,64),
        new Vector2(128,128),
        new Vector2(256,256),
        new Vector2(512,512),
        new Vector2(1024,1024),
        new Vector2(2048,2048)
    };

        struct EmojiInfo
        {
            public string key;
            public string x;
            public string y;
            public string size;
        }

        private const int EmojiSize = 32;//the size of emoji;

        [MenuItem("MGame/EmojiText/Build Emoji", false, 4)]
        public static void BuildEmoji()
        {
            List<char> keylist = new List<char>();
            for (int i = 48; i <= 57; i++)
            {
                keylist.Add(System.Convert.ToChar(i));//0-9
            }
            for (int i = 65; i <= 90; i++)
            {
                keylist.Add(System.Convert.ToChar(i));//A-Z
            }
            for (int i = 97; i <= 122; i++)
            {
                keylist.Add(System.Convert.ToChar(i));//a-z
            }
            //search all emojis and compute they frames;
            Dictionary<string, int> sourceDic = new Dictionary<string, int>();
            string[] files = Directory.GetFiles(InputPath, "*.png");
            for (int i = 0; i < files.Length; i++)
            {
                string[] strs = files[i].Split('/');
                string[] strs2 = strs[strs.Length - 1].Split('.');
                string filename = strs2[0];
                string[] t = filename.Split('_');
                string id = t[0];
                if (sourceDic.ContainsKey(id))
                {
                    sourceDic[id]++;
                }
                else
                {
                    sourceDic.Add(id, 1);
                }
            }
            //create the directory if it is not exist;
            if (!Directory.Exists(OutputPath))
            {
                Directory.CreateDirectory(OutputPath);
            }
            Dictionary<string, EmojiInfo> emojiDic = new Dictionary<string, EmojiInfo>();
            int totalFrames = 0;
            foreach (int value in sourceDic.Values)
            {
                totalFrames += value;
            }
            Vector2 texSize = ComputeAtlasSize(totalFrames);
            Texture2D newTex = new Texture2D((int)texSize.x, (int)texSize.y, TextureFormat.ARGB32, false);
            Texture2D dataTex = new Texture2D((int)texSize.x / EmojiSize, (int)texSize.y / EmojiSize, TextureFormat.ARGB32, false);
            int x = 0;
            int y = 0;
            int keyindex = 0;
            foreach (string key in sourceDic.Keys)
            {
                for (int index = 0; index < sourceDic[key]; index++)
                {
                    string path = InputPath + key;
                    if (sourceDic[key] == 1)
                    {
                        path += ".png";
                    }
                    else
                    {
                        path += "_" + (index + 1).ToString() + ".png";
                    }
                    Texture2D asset = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    Color[] colors = asset.GetPixels(0);
                    for (int i = 0; i < EmojiSize; i++)
                    {
                        for (int j = 0; j < EmojiSize; j++)
                        {
                            newTex.SetPixel(x + i, y + j, colors[i + j * EmojiSize]);
                        }
                    }
                    string t = System.Convert.ToString(sourceDic[key] - 1, 2);
                    float r = 0, g = 0, b = 0;
                    if (t.Length >= 3)
                    {
                        r = t[2] == '1' ? 0.5f : 0;
                        g = t[1] == '1' ? 0.5f : 0;
                        b = t[0] == '1' ? 0.5f : 0;
                    }
                    else if (t.Length >= 2)
                    {
                        r = t[1] == '1' ? 0.5f : 0;
                        g = t[0] == '1' ? 0.5f : 0;
                    }
                    else
                    {
                        r = t[0] == '1' ? 0.5f : 0;
                    }
                    dataTex.SetPixel(x / EmojiSize, y / EmojiSize, new Color(r, g, b, 1));
                    if (!emojiDic.ContainsKey(key))
                    {
                        EmojiInfo info;
                        if (keyindex < keylist.Count)
                        {
                            info.key = "[" + char.ToString(keylist[keyindex]) + "]";
                        }
                        else
                        {
                            info.key = "[" + char.ToString(keylist[keyindex / keylist.Count]) + char.ToString(keylist[keyindex % keylist.Count]) + "]";
                        }
                        info.x = (x * 1.0f / texSize.x).ToString();
                        info.y = (y * 1.0f / texSize.y).ToString();
                        info.size = (EmojiSize * 1.0f / texSize.x).ToString();
                        emojiDic.Add(key, info);
                        keyindex++;
                    }
                    x += EmojiSize;
                    if (x >= texSize.x)
                    {
                        x = 0;
                        y += EmojiSize;
                    }
                }
            }

            byte[] bytes1 = newTex.EncodeToPNG();
            string outputfile1 = OutputPath + "emoji_tex.png";
            File.WriteAllBytes(outputfile1, bytes1);
            byte[] bytes2 = dataTex.EncodeToPNG();
            string outputfile2 = OutputPath + "emoji_data.png";
            File.WriteAllBytes(outputfile2, bytes2);
            using (StreamWriter sw = new StreamWriter(OutputPath + "emoji.txt", false))
            {
                sw.WriteLine("Name\tKey\tFrames\tX\tY\tSize");
                foreach (string key in emojiDic.Keys)
                {
                    sw.WriteLine("{" + key + "}\t" + emojiDic[key].key + "\t" + sourceDic[key] + "\t" + emojiDic[key].x + "\t" + emojiDic[key].y + "\t" + emojiDic[key].size);
                }
                sw.Close();
            }
            File.Copy(OutputPath + "emoji.txt", "Assets/Resources/Bytes/emoji.txt", true);
            File.Copy(OutputPath + "emoji.txt", "Assets/Bundles/Single/Bytes/emoji.txt", true);
            AssetDatabase.Refresh();
            FormatTexture();
            EditorUtility.DisplayDialog("提示", "生成成功！", "OK");
        }

        private static Vector2 ComputeAtlasSize(int count)
        {
            long total = count * EmojiSize * EmojiSize;
            for (int i = 0; i < AtlasSize.Length; i++)
            {
                if (total <= AtlasSize[i].x * AtlasSize[i].y)
                {
                    return AtlasSize[i];
                }
            }
            return Vector2.zero;
        }

        private static void FormatTexture()
        {
            TextureImporter emojiTex = AssetImporter.GetAtPath(OutputPath + "emoji_tex.png") as TextureImporter;
            emojiTex.filterMode = FilterMode.Point;
            emojiTex.mipmapEnabled = false;
            emojiTex.sRGBTexture = true;
            emojiTex.alphaSource = TextureImporterAlphaSource.FromInput;
            emojiTex.textureCompression = TextureImporterCompression.Uncompressed;
            emojiTex.SaveAndReimport();

            TextureImporter emojiData = AssetImporter.GetAtPath(OutputPath + "emoji_data.png") as TextureImporter;
            emojiData.filterMode = FilterMode.Point;
            emojiData.mipmapEnabled = false;
            emojiData.sRGBTexture = false;
            emojiData.alphaSource = TextureImporterAlphaSource.None;
            emojiData.textureCompression = TextureImporterCompression.Uncompressed;
            emojiData.SaveAndReimport();
        }
    }
}