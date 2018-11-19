/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/11/11 22:20:33
** desc:  模型管理;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class ModelMgr : Singleton<ModelMgr>
    {
        private static string _model = "ch_pc_hou";
        private static List<string> _modeList = new List<string>() { "ch_pc_hou_004", "ch_pc_hou_006", "ch_pc_hou_008" };
        private static List<string> _headList = new List<string>() { "ch_pc_hou_004_tou", "ch_pc_hou_006_tou", "ch_pc_hou_008_tou" };
        private static List<string> _bodyList = new List<string>() { "ch_pc_hou_004_shen", "ch_pc_hou_006_shen", "ch_pc_hou_008_shen" };
        private static List<string> _handList = new List<string>() { "ch_pc_hou_004_shou", "ch_pc_hou_006_shou", "ch_pc_hou_008_shou" };
        private static List<string> _feetList = new List<string>() { "ch_pc_hou_004_jiao", "ch_pc_hou_006_jiao", "ch_pc_hou_008_jiao" };
        private static List<string> _weaponList = new List<string>() { "ch_we_one_hou_004", "ch_we_one_hou_006", "ch_we_one_hou_008" };
    }
}