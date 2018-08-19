/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/06/09 16:52:58
** desc:  导出AssetBundle更新包;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    public class ExportABPackage
    {
        private string _lastAbPath;
        private string _curAbPath;

        [MenuItem("myGameFramework/AssetBundleTools/Export AssetBundle Package", false, 21)]
        public static void ExportAssetBundlePackage()
        {
            //VerifierUtility.GetMD5();
        }
    }
}