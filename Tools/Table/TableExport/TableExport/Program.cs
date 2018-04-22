using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableExport
{
    class Program
    {
        private static bool _noWaring = true;

        /// <summary>
        /// log类型;
        /// </summary>
        private enum LogType : int
        {
            normal = 0,
            waring,
            error,
        }

        /// <summary>
        /// 配置类型;
        /// </summary>
        public enum TableValueType : int
        {
            _string = 0,
            _int = 1,
            _float = 2,
            _bool = 3,
        }

        static void Main(string[] args)
        {
            CreateCsharp("c:/test.cs", "c:/test.xls");
            string _xlsxpath = args.Length < 1 ? string.Empty : args[0];
            bool _csargs = args.Length < 2 ? false : string.IsNullOrEmpty(args[1]);
            string _cspath = args.Length < 3 ? string.Empty : args[2];
            bool _byteargs = args.Length < 4 ? false : string.IsNullOrEmpty(args[3]);
            string _bytespath = args.Length < 5 ? string.Empty : args[4];

            if (string.IsNullOrEmpty(_xlsxpath))
            {
                PrintLog("#####未指定数据表文件!#####", LogType.error);
                return;
            }
            List<string> paths = SpliteStr(_xlsxpath, "#");
            if (paths == null || paths.Count == 0)
            {
                PrintLog("#####数据表文件路径错误!#####", LogType.error);
                return;
            }
            for (int i = 0; i < paths.Count; i++)
            {
                if (_csargs || !string.IsNullOrEmpty(_cspath))
                {
                    CreateCsharp(_cspath, paths[i]);
                }
                if (_byteargs || !string.IsNullOrEmpty(_bytespath))
                {

                }
            }
            if (!_noWaring)
                Console.ReadKey();
        }

        /// <summary>
        /// 字符串分割;
        /// </summary>
        /// <param name="str"></param>
        /// <param name="splite"></param>
        /// <returns></returns>
        private static List<string> SpliteStr(string str, string splite)
        {
            string[] paths = str.Split(splite.ToArray());
            return paths.ToList();
        }

        /// <summary>
        /// 获取类型;
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string GetTypeByType(TableValueType type)
        {
            string target = "string";
            switch (type)
            {
                case TableValueType._int:
                    target = "int";
                    break;
                case TableValueType._float:
                    target = "float";
                    break;
                case TableValueType._bool:
                    target = "bool";
                    break;
                default:
                    target = "string";
                    break;
            }
            return target;
        }

        /// <summary>
        /// log输出;
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        private static void PrintLog(string str, LogType type = LogType.normal)
        {
            ConsoleColor color = ConsoleColor.Green;
            switch (type)
            {
                case LogType.error:
                    color = ConsoleColor.Red;
                    break;
                case LogType.waring:
                    color = ConsoleColor.Yellow;
                    _noWaring = false;
                    break;
                default:
                    color = ConsoleColor.Green;
                    _noWaring = false;
                    break;
            }
            Console.ForegroundColor = color;
            Console.Write("[TableTools]" + str);
            if (type == LogType.error)
            {
                Console.ReadKey();
            }
        }

        /// <summary>
        /// 写文件;
        /// </summary>
        /// <param name="path"></param>
        /// <param name="code"></param>
        private static void CreateFile(string path, string code)
        {
            string allText = code;
            File.WriteAllText(path, allText);
        }

        /// <summary>
        /// 读表;
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        private static DataRowCollection ReadXlsx(string filepath)
        {
            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filepath + ";" + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
            //创建连接到数据源的对象;
            OleDbConnection connection = new OleDbConnection(connectionString);
            //打开连接;
            connection.Open();
            string sql = "select * from [Sheet1$]";//这个是一个查询命令; 
            OleDbDataAdapter adapter = new OleDbDataAdapter(sql, connection);
            DataSet dataSet = new DataSet();//用来存放数据用来存放DataTable;
            adapter.Fill(dataSet);//表示把查询的结果(datatable)放到(填充)dataset里面; 
            connection.Close();//释放连接资源;
            //取得数据;
            DataTableCollection tableCollection = dataSet.Tables;//获取当前集合中所有的表格;
            DataTable table = tableCollection[0];//因为我们只往dataset里面放置了一张表格,所以这里取得索引为0的表格就是我们刚刚查询到的表格;
            //取得表格中的数据;
            //取得table中所有的行;
            DataRowCollection rowCollection = table.Rows;//返回了一个行的集合;
            return rowCollection;
            //遍历行的集合,取得每一个行的datarow对象;
            //foreach (DataRow row in rowCollection)
            //{
            //    //取得row中前8列的数据 索引0-7  
            //    for (int i = 0; i < 8; i++)
            //    {
            //        Console.Write(row[i] + " ");
            //    }
            //    Console.WriteLine();
            //}
            //Console.ReadKey();
        }

        /// <summary>
        /// 创建cs文件;
        /// </summary>
        /// <param name="cspath"></param>
        /// <param name="filepath"></param>
        public static void CreateCsharp(string cspath, string filepath)
        {
            DataRowCollection collection = ReadXlsx(filepath);
            string className = "";
            string code = "";
            string filed = "public {0} {1};" +
                "";
            string fileds = "";
            if (collection.Count < 2)
                return;
            className = Path.GetFileNameWithoutExtension(filepath);
            className = className.Substring(0, 1).ToUpper() + className.Substring(1);
            object[] values = collection[0].ItemArray;
            for (int i = 0; i < values.Length; i++)
            {
                List<string> str = SpliteStr(values[i].ToString(), ":");
                int type;
                int.TryParse(str[1], out type);
                string temp = string.Format(filed, GetTypeByType((TableValueType)type), str[0]) + "\r\n";
                fileds = fileds + temp;
            }
            code = _csharpTemplate.Replace("{arg0}", (className + "Data"));
            code = code.Replace("{arg1}", className);
            code = code.Replace("{arg2}", className);
            code = code.Replace("{arg3}", className);
            code = code.Replace("{arg4}", fileds);
            CreateFile(cspath + "/" + className + ".cs", code);
        }

        /// <summary>
        /// cs模板文件;
        /// </summary>
        private static string _csharpTemplate =
            "//\r\n" +
            "// this source code was auto-generated by tools, do not modify it;\r\n" +
            "//\r\n" +
            "// generated time:" + System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss").ToString() + "\r\n" +
            "//\r\n" +
            " \r\n" +
            "using System.Collections;\r\n" +
            "using System.Collections.Generic;\r\n" +
            " \r\n" +
            "public class {arg0}{\r\n" +//{0}数据表类;
            "\r\n" +
            "public List<{arg1}> Data = new List<{arg2}>();\r\n" +//{1}{2}数据类集合;
            "\r\n" +
            "}\r\n" +
            " \r\n" +
            "public class {arg3}{\r\n" +//{3}数据类;
            "\r\n" +
            "{arg4}\r\n" +//{4}字段;
            "}\r\n";
    }
}
