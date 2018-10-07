/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/10/07 15:50:22
** desc:  版本控制信息;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class VersionInfo
    {
        public string _updateUrl = "";
        public int _versionNumber;
        public Dictionary<int, string> _md5Dict = new Dictionary<int, string>();
    }
}