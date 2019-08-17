using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace OfficeOpenXml
{
    public static class EPPlusUtility
    {
        private static readonly string _defaultFont = "微软雅黑";
        private static readonly Color _defaultColor = Color.White;

        public static ExcelSheetInfo ReadExcelSheet(string excelPath, string sheetName)
        {
            if (string.IsNullOrEmpty(excelPath) || string.IsNullOrEmpty(sheetName))
            {
                return null;
            }

            excelPath = excelPath.Replace("\\", "/");
            if (!File.Exists(excelPath))
            {
                return null;
            }

            using (FileStream fs = File.Open(excelPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                ExcelPackage excel = new ExcelPackage(fs);
                ExcelWorksheet sheet = excel.Workbook.Worksheets[sheetName];
                if (sheet == null)
                {
                    LogHelper.PrintError("[EPPlusUtility]ExcelSheet " + sheetName + "is null.");
                    return null;
                }
                var sheetInfo = new List<List<ExcelElementInfo>>();

                var row = sheet.Dimension.End.Row;
                var col = sheet.Dimension.End.Column;

                for (int i = 0; i < row; i++)
                {
                    var elementList = new List<ExcelElementInfo>();
                    for (int j = 0; j < col; j++)
                    {
                        elementList.Add(GetExcelElementInfo(sheet.Cells[i + 1, j + 1]));
                    }
                    sheetInfo.Add(elementList);
                }
                return new ExcelSheetInfo(excelPath, sheetName, sheetInfo);
            }
        }

        public static bool WriteExcelSheet(ExcelSheetInfo sheetInfo)
        {
            if (sheetInfo == null || string.IsNullOrEmpty(sheetInfo.ExcelPath)
                || string.IsNullOrEmpty(sheetInfo.SheetName) || sheetInfo.Info == null)
            {
                return false;
            }
            var name = sheetInfo.SheetName;
            var path = sheetInfo.ExcelPath.Replace("\\", "/");
            if (!File.Exists(path))
            {
                return CreateExcelSheet(sheetInfo);
            }

            var file = new FileInfo(Path.GetFileName(path));

            using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                ExcelPackage oldExcel = new ExcelPackage(fs);
                ExcelWorksheet excelSheet = null;
                ExcelPackage excel = new ExcelPackage(file);
                {

                    foreach (var tempSheet in oldExcel.Workbook.Worksheets)
                    {
                        if (tempSheet.Name == name)
                        {
                            excelSheet = excel.Workbook.Worksheets.Add(tempSheet.Name, tempSheet);
                        }
                        else
                        {
                            excel.Workbook.Worksheets.Add(tempSheet.Name, tempSheet);
                        }
                    }
                    if (excelSheet == null)
                    {
                        excelSheet = excel.Workbook.Worksheets.Add(name);
                    }
                    SetExcelWorksheetInfo(ref excelSheet, sheetInfo);
                    excel.Save();
                }
            }

            var tempPath = file.FullName.Replace("\\", "/");
            if (File.Exists(tempPath))
            {
                try
                {
                    File.Copy(tempPath, path, true);
                    File.Delete(tempPath);
                    return true;
                }
                catch (Exception e)
                {
                    LogHelper.PrintError(e.ToString());
                    File.Delete(tempPath);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private static bool CreateExcelSheet(ExcelSheetInfo sheetInfo)
        {
            if (sheetInfo == null || string.IsNullOrEmpty(sheetInfo.ExcelPath)
                || string.IsNullOrEmpty(sheetInfo.SheetName) || sheetInfo.Info == null)
            {
                return false;
            }
            var name = sheetInfo.SheetName;
            var path = sheetInfo.ExcelPath.Replace("\\", "/");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            try
            {
                using (FileStream fs = File.Open(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    ExcelPackage excel = new ExcelPackage(fs);
                    ExcelWorksheet excelSheet = excel.Workbook.Worksheets.Add(name);
                    SetExcelWorksheetInfo(ref excelSheet, sheetInfo);
                    excel.Save();
                }
                return true;
            }
            catch (Exception e)
            {
                LogHelper.PrintError(e.ToString());
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                return false;
            }
        }

        private static ExcelElementInfo GetExcelElementInfo(ExcelRange cell)
        {
            if (cell == null)
            {
                return new ExcelElementInfo(string.Empty, _defaultColor);
            }
            var value = cell.Value != null ? cell.Value.ToString() : string.Empty;
            var comment = cell.Comment;
            var color = _defaultColor;
            var rgb = cell.Style.Fill.BackgroundColor.Rgb;
            if (!string.IsNullOrEmpty(rgb))
            {
                if (rgb.ToCharArray().Length > 6)
                {
                    rgb = rgb.Substring(2);
                }
                color = ColorTranslator.FromHtml("#" + rgb);
            }
            return new ExcelElementInfo(value, color, comment);
        }

        private static void SetExcelWorksheetInfo(ref ExcelWorksheet excelSheet, ExcelSheetInfo sheetInfo)
        {
            if (excelSheet == null || sheetInfo == null || sheetInfo.Info == null)
            {
                return;
            }

            StandardFormatExcelWorksheet(ref excelSheet);

            var info = sheetInfo.Info;
            for (int i = 0; i < info.Count; i++)
            {
                if (info[i] != null)
                {
                    for (int j = 0; j < info[i].Count; j++)
                    {
                        var element = info[i][j];
                        var cell = excelSheet.Cells[i + 1, j + 1];
                        SetExcelRange(ref cell, element);
                    }
                }
            }
        }

        private static void StandardFormatExcelWorksheet(ref ExcelWorksheet excelSheet)
        {
            if (excelSheet == null)
            {
                return;
            }
            excelSheet.Cells.Clear();

            excelSheet.View.ShowGridLines = true;

            excelSheet.Cells.Style.Font.Name = _defaultFont;
            excelSheet.Cells.Style.Fill.PatternType = Style.ExcelFillStyle.Solid;
            excelSheet.Cells.Style.Fill.PatternColor.SetColor(_defaultColor);
            excelSheet.Cells.Style.Fill.BackgroundColor.SetColor(_defaultColor);
            excelSheet.Cells.Style.HorizontalAlignment = Style.ExcelHorizontalAlignment.Center;
            excelSheet.Cells.Style.VerticalAlignment = Style.ExcelVerticalAlignment.Center;

            var excelBorderStyle = Style.ExcelBorderStyle.Hair;
            excelSheet.Cells.Style.Border.Left.Style = excelBorderStyle;
            excelSheet.Cells.Style.Border.Right.Style = excelBorderStyle;
            excelSheet.Cells.Style.Border.Top.Style = excelBorderStyle;
            excelSheet.Cells.Style.Border.Bottom.Style = excelBorderStyle;
        }

        private static void SetExcelRange(ref ExcelRange cell, ExcelElementInfo elementInfo)
        {
            if (cell == null)
            {
                return;
            }
            if (elementInfo == null)
            {
                cell.Clear();
                return;
            }

            cell.Style.Font.Size = elementInfo.FontSize;
            cell.Style.Fill.BackgroundColor.SetColor(elementInfo.Color);
            if (elementInfo.ExcelComment != null && cell.Comment == null)
            {
                cell.AddComment(elementInfo.ExcelComment.Text, elementInfo.ExcelComment.Author);
            }
            if (cell.Comment != null)
            {
                cell.Comment.Text = elementInfo.ExcelComment.Text;
                cell.Comment.Author = elementInfo.ExcelComment.Author;
                cell.Comment.AutoFit = true;
                cell.Comment.Visible = true;
            }
            cell.Value = elementInfo.ToString();
        }
    }
}