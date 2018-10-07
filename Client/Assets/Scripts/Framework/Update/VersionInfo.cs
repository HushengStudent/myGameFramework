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
    public struct Version
    {
        public Version(int num)
        {
            _apkVersion = 0;
            _hotVersion = 0;
            _versionTime = 0;
            _apkPermissions = 0;
        }

        public int _apkVersion;
        public int _hotVersion;
        public int _versionTime;
        public int _apkPermissions;
    }

    public class VersionInfo
    {
        public string _updateUrl = "";

        //版本号:xxx.xxx.xxx.xxx:大版本.热更版本.时间.内/外版本;
        //1.3.181008.0:第一个大版本,第三个热更版本,18年10月8号出的版本,偶数为内部版本;
        public Version _version;
    }
}