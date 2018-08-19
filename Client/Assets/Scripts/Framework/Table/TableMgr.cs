/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/17 23:35:26
** desc:  数据表管理;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Framework
{
    public class TableMgr : Singleton<TableMgr>, IMgr
    {
        private string _path = Application.dataPath.ToLower() + "/Bundles/Single/Table/";

        private Dictionary<string, Table> _dbDict = new Dictionary<string, Table>();

        public void InitMgr()
        {
            _dbDict.Clear();
            PreLoad();
        }

        public void LoadTable(string tableName, Table table)
        {
            if (_dbDict.ContainsKey(tableName))
                return;
            byte[] bytes = FileUtility.ReadFromBytes(_path + tableName + ".byte");
            table.LoadData(bytes);
            _dbDict[tableName] = table;
        }

        private void PreLoad()
        {
            LoadTable("ItemTable", ItemTableDB.instance);
        }
    }
}
