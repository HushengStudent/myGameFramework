/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/17 23:02:39
** desc:  数据表配置;
*********************************************************************************/

using UnityEngine;

namespace Framework.EditorModule.Window
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
        public static string TablePath { get; } = Application.dataPath.ToLower() + "/../../Table/";
    }
}