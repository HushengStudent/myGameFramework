/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/8/17 21:13:39
** desc:  Excel Element Info;
*********************************************************************************/


using System.Drawing;

namespace OfficeOpenXml
{
    public class ExcelElementInfo
    {
        private readonly int _defaultFontSize = 12;

        public string Value { get; private set; }
        public Color Color { get; private set; }
        public ExcelComment ExcelComment { get; private set; }
        public int FontSize { get; private set; }


        public ExcelElementInfo(string value, Color color, ExcelComment excelComment
            , int fontSize) : this(value, color, excelComment)
        {
            FontSize = fontSize;
        }

        public ExcelElementInfo(string value, Color color
            , ExcelComment excelComment) : this(value, excelComment)
        {
            Color = color;
        }

        public ExcelElementInfo(string value, ExcelComment excelComment) : this(value)
        {
            ExcelComment = excelComment;
        }

        public ExcelElementInfo(string value, Color color) : this(value)
        {
            Color = color;
        }

        public ExcelElementInfo(string value)
        {
            Value = value;
            Color = Color.White;
            ExcelComment = null;
            FontSize = _defaultFontSize;
        }
    }
}
