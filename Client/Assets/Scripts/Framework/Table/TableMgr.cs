/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/17 23:35:26
** desc:  数据表管理;
*********************************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class TableMgr : Singleton<TableMgr>
    {
        private string _path = $"{Application.dataPath.ToLower()}/Bundles/Single/Table/";

        private Dictionary<string, Table> _dbDict = new Dictionary<string, Table>();

        protected override void OnInitialize()
        {
            _dbDict.Clear();
            PreLoad();
        }

        public void LoadTable(string tableName, Table table)
        {
            if (_dbDict.ContainsKey(tableName))
            {
                return;
            }
            var bytes = FileHelper.ReadFromBytes($"{_path}{tableName}.byte");
            table.LoadData(bytes);
            _dbDict[tableName] = table;
        }

        private void PreLoad()
        {
            LoadTable("ItemTable", ItemTableDB.instance);
        }
    }
}
