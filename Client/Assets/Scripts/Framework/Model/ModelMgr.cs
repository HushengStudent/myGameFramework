/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/11/11 22:20:33
** desc:  模型管理;
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class ModelMgr : Singleton<ModelMgr>, ISingleton
    {
        public readonly string Model = "ch_pc_hou";
        public readonly string[] HeadArray = new string[] { "ch_pc_hou_004_tou", "ch_pc_hou_006_tou", "ch_pc_hou_008_tou" };
        public readonly string[] BodyArray = new string[] { "ch_pc_hou_004_shen", "ch_pc_hou_006_shen", "ch_pc_hou_008_shen" };
        public readonly string[] HandArray = new string[] { "ch_pc_hou_004_shou", "ch_pc_hou_006_shou", "ch_pc_hou_008_shou" };
        public readonly string[] FeetArray = new string[] { "ch_pc_hou_004_jiao", "ch_pc_hou_006_jiao", "ch_pc_hou_008_jiao" };
        public readonly string[] WeaponArray = new string[] { "ch_we_one_hou_004", "ch_we_one_hou_006", "ch_we_one_hou_008" };

        public void OnInitialize()
        {
        }
    }
}