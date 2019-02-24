using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;

namespace FormatTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private FolderBrowserDialog _folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
        private string[] _paths;
        private readonly string _textBoxName = "请选择路径";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void FromatTarget(object sender, RoutedEventArgs e)
        {
            if (_paths != null && _paths.Length > 0)
            {
                string message = string.Empty;
                string str = "\r\n";
                foreach (string path in _paths)
                {
                    string[] strs = File.ReadAllLines(path, Encoding.Default);
                    File.WriteAllLines(path, strs, Encoding.UTF8);
                    message = message + path + str;
                }
                ShowDialog(message);
            }
            else
            {
                ShowDialog("未找到文件！");
            }
        }

        private void SelectFilePath(object sender, RoutedEventArgs e)
        {
            _folderBrowserDialog.Description = _textBoxName;
            _folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
            if (_folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _paths = Directory.GetFiles(_folderBrowserDialog.SelectedPath, "*.cs*", SearchOption.AllDirectories);
                Grid grid = FindName("TopGrid") as Grid;
                if (grid != null)
                {
                    DependencyObject obj = grid as DependencyObject;
                    if (obj != null)
                    {
                        DependencyObject parent = VisualTreeHelper.GetParent(obj);
                        if (parent != null)
                        {
                            System.Windows.Controls.TextBox textBox = FindChild<System.Windows.Controls.TextBox>(parent, "textBox");
                            if (textBox != null)
                            {
                                RegisterName("textBox", textBox);
                                textBox.Text = _folderBrowserDialog.SelectedPath;
                            }
                        }
                    }
                }
            }
            else
            {
                _paths = null;
            }

        }

        private void ShowDialog(string message)
        {
            MessageBoxResult _result = System.Windows.MessageBox.Show(message, "提示", MessageBoxButton.OK);
            System.Windows.Controls.TextBox _textBox = FindName("textBox") as System.Windows.Controls.TextBox;
            if (_textBox != null)
            {
                if (_result == MessageBoxResult.OK)
                {

                }
                if (_result == MessageBoxResult.None || _result == MessageBoxResult.Cancel)
                {
                    _textBox.Text = _textBoxName;
                }
            }
        }

        public T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null) return null;
            T _foundChild = null;
            int _childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < _childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                T childType = child as T;
                if (childType == null)
                {
                    _foundChild = FindChild<T>(child, childName);
                    if (_foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        _foundChild = (T)child;
                        break;
                    }
                    _foundChild = FindChild<T>(child, childName);
                    if (_foundChild != null) break;
                }
                else
                {
                    _foundChild = (T)child;
                    break;
                }
            }
            return _foundChild;
        }
    }
}
