/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/8/17 21:12:29
** desc:  Excel Sheet Info;
*********************************************************************************/


using System.Collections.Generic;

namespace OfficeOpenXml
{
    public class ExcelSheetInfo
    {
        public string ExcelPath { get; private set; }
        public string SheetName { get; private set; }
        public List<List<ExcelElementInfo>> Info { get; private set; }

        public ExcelSheetInfo(string excelPath, string sheetName, List<List<ExcelElementInfo>> info)
        {
            ExcelPath = excelPath;
            SheetName = sheetName;
            Info = new List<List<ExcelElementInfo>>(info);
        }

        public List<ExcelElementInfo> GetRow(int row)
        {
            if (Info == null || Info.Count < row)
            {
                return null;
            }
            return Info[row - 1];
        }

        public List<ExcelElementInfo> GetColumn(int column)
        {
            if (Info == null || Info.Count < column)
            {
                return null;
            }
            var list = new List<ExcelElementInfo>();
            for (int i = 0; i < Info.Count; i++)
            {
                if (Info[i] != null)
                {
                    for (int j = 0; j < Info[i].Count; i++)
                    {
                        var element = Info[i][j];
                        if (element != null)
                        {
                            list.Add(element);
                        }
                    }
                }
            }
            return list;
        }
    }
}
