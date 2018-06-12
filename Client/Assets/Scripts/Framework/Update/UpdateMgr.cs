/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/23 00:53:17
** desc:  热更管理;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class UpdateMgr : Singleton<UpdateMgr>
    {
        private string _localVersion = string.Empty;
        private string _curVersion = string.Empty;

        public string LoaclVersion { get { return _localVersion; } }
        public string CurVersion { get { return _curVersion; } set { _curVersion = value; } }
    }
}