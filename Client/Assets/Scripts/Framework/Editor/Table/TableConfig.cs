/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/17 23:02:39
** desc:  数据表配置;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public enum TableFiledType : int
    {
        STRING = 0, //字符串;
        FLOAT,      //浮点数;
        INT,        //整数;
        BOOL,       //Boolean;
    }

    public static class TableConfig
    {
        private static string _tablePath = Application.dataPath.ToLower() + "/../../Table/";

        public static string TablePath { get { return _tablePath; } }
    }
}