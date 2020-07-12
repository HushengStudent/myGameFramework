/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/01/28 15:48:00
** desc:  ini文件读取工具;
*********************************************************************************/

using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Framework
{
    public static class IniHelper
    {
        //去掉一行信息的开始和末尾不需要的信息;
        private static readonly char[] TrimStart = new char[] { ' ', '\t' };
        private static readonly char[] TrimEnd = new char[] { ' ', '\t', '\r', '\n' };
        //key和value的分隔符;
        private static string DELEMITER = "=";
        //路径;
        private static string strFilePath = null;
        //是否区分大小写;
        private static bool IsCaseSensitive = false;
        private static Dictionary<string, Dictionary<string, string>> IniConfigDic = new Dictionary<string, Dictionary<string, string>>();

        //初始化;
        public static void IniFile(string path, bool isCaseSensitive = false)
        {
            strFilePath = path;
            IsCaseSensitive = isCaseSensitive;
        }

        //解析ini;
        public static void ParseIni()
        {
            if (!File.Exists(strFilePath))
            {
                Debug.LogWarning($"the ini file's path is error：{strFilePath}");
                return;
            }
            using (var reader = new StreamReader(strFilePath))
            {
                string section = null;
                string key = null;
                string val = null;
                Dictionary<string, string> config = null;

                string strLine = null;
                while ((strLine = reader.ReadLine()) != null)
                {
                    strLine = strLine.TrimStart(TrimStart);
                    strLine = strLine.TrimEnd(TrimEnd);
                    //'#'开始代表注释;
                    if (strLine.StartsWith("#"))
                    {
                        continue;
                    }
                    if (TryParseSection(strLine, out section))
                    {
                        if (!IniConfigDic.ContainsKey(section))
                        {
                            IniConfigDic.Add(section, new Dictionary<string, string>());
                        }
                        config = IniConfigDic[section];
                    }
                    else
                    {
                        if (config != null)
                        {
                            if (TryParseConfig(strLine, out key, out val))
                            {
                                if (config.ContainsKey(key))
                                {
                                    config[key] = val;
                                    Debug.LogWarning($"the Key[{key}] is appear repeat");
                                }
                                else
                                {
                                    config.Add(key, val);
                                }
                            }
                        }
                        else
                        {
                            Debug.LogWarning("the ini file's format is error,lost [Section]'s information");
                        }
                    }
                }
            }
        }

        //写入ini;
        public static void SaveIni()
        {
            if (string.IsNullOrEmpty(strFilePath))
            {
                Debug.LogWarning("Empty file name for SaveIni.");
                return;
            }

            var dirName = Path.GetDirectoryName(strFilePath);
            if (string.IsNullOrEmpty(dirName))
            {
                Debug.LogWarning($"Empty directory for SaveIni:{strFilePath}.");
                return;
            }
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
            using (var sw = new StreamWriter(strFilePath))
            {
                foreach (KeyValuePair<string, Dictionary<string, string>> pair in IniConfigDic)
                {
                    sw.WriteLine($"[{pair.Key}]");
                    foreach (KeyValuePair<string, string> cfg in pair.Value)
                    {
                        sw.WriteLine(cfg.Key + DELEMITER + cfg.Value);
                    }
                }
            }
        }

        public static string GetString(string section, string key, string defaultVal)
        {
            if (!IsCaseSensitive)
            {
                section = section.ToUpper();
                key = key.ToUpper();
            }
            if (IniConfigDic.TryGetValue(section, out var config))
            {
                if (config.TryGetValue(key, out var ret))
                {
                    return ret;
                }
            }
            return defaultVal;
        }

        public static int GetInt(string section, string key, int defaultVal)
        {
            var val = GetString(section, key, null);
            if (val != null)
            {
                return int.Parse(val);
            }
            return defaultVal;
        }

        public static void SetString(string section, string key, string val)
        {
            if (!string.IsNullOrEmpty(section) && !string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(val))
            {
                if (!IsCaseSensitive)
                {
                    section = section.ToUpper();
                    key = key.ToUpper();
                }
                if (!IniConfigDic.TryGetValue(section, out var config))
                {
                    config = new Dictionary<string, string>();
                    IniConfigDic[section] = config;
                }
                config[key] = val;
            }
        }

        public static void SetInt(string section, string key, int val)
        {
            SetString(section, key, val.ToString());
        }

        public static void AddString(string section, string key, string val)
        {
            if (!string.IsNullOrEmpty(section) && !string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(val))
            {
                if (!IsCaseSensitive)
                {
                    section = section.ToUpper();
                    key = key.ToUpper();
                }
                if (!IniConfigDic.TryGetValue(section, out var config))
                {
                    config = new Dictionary<string, string>();
                    IniConfigDic[section] = config;
                }
                if (!config.ContainsKey(key))
                {
                    config.Add(key, val);
                }
            }
        }

        public static void AddInt(string section, string key, int val)
        {
            AddString(section, key, val.ToString());
        }

        public static bool RemoveSection(string section)
        {
            if (IniConfigDic.ContainsKey(section))
            {
                IniConfigDic.Remove(section);
                return true;
            }
            return false;
        }

        public static bool RemoveConfig(string section, string key)
        {
            if (!IsCaseSensitive)
            {
                section = section.ToUpper();
                key = key.ToUpper();
            }
            if (IniConfigDic.TryGetValue(section, out var config))
            {
                if (config.ContainsKey(key))
                {
                    config.Remove(key);
                    return true;
                }
            }
            return false;
        }

        public static Dictionary<string, string> GetSectionInfo(string section)
        {
            if (!IsCaseSensitive)
            {
                section = section.ToUpper();
            }
            IniConfigDic.TryGetValue(section, out var res);
            return res;
        }

        private static bool TryParseSection(string strLine, out string section)
        {
            section = null;
            if (!string.IsNullOrEmpty(strLine))
            {
                var len = strLine.Length;
                if (strLine[0] == '[' && strLine[len - 1] == ']')
                {
                    section = strLine.Substring(1, len - 2);
                    if (!IsCaseSensitive)
                    {
                        section = section.ToUpper();
                    }
                    return true;
                }
            }
            return false;
        }

        private static bool TryParseConfig(string strLine, out string key, out string val)
        {
            if (strLine != null && strLine.Length >= 3)
            {
                var contents = strLine.Split(DELEMITER.ToCharArray());
                if (contents.Length == 2)
                {
                    key = contents[0].TrimStart(TrimStart);
                    key = key.TrimEnd(TrimEnd);
                    val = contents[1].TrimStart(TrimStart);
                    val = val.TrimEnd(TrimEnd);
                    if (key.Length > 0 && val.Length > 0)
                    {
                        if (!IsCaseSensitive)
                        {
                            key = key.ToUpper();
                        }

                        return true;
                    }
                }
            }
            key = null;
            val = null;
            return false;
        }
    }
}
