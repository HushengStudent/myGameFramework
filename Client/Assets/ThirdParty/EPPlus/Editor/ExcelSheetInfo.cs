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
    }
}
