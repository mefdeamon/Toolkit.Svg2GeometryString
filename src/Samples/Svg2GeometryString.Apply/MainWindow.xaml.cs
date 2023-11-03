using Microsoft.Win32;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Path = System.Windows.Shapes.Path;

namespace Svg2GeometryString.Apply
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            SaveCSButton.IsEnabled = false;
        }

        private void LoadSvgFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog() { Title = "请选择SVG文件", Filter = "Scalable Vector Graphics (.svg)|*.svg|All files (*.*)|*.*" };
            if (dialog.ShowDialog() == true)
            {

                FileInfo file = new FileInfo(dialog.FileName);

                var pathData = Svg2GeoConverter.Parse(file);

                SvgFileContentText.Text = pathData;
                SvgFileViewPath.Data = StreamGeometry.Parse(pathData);

            }
        }


        private void LoadSvgFiles_Click(object sender, RoutedEventArgs e)
        {
            SaveCSButton.IsEnabled = false;
            OpenFileDialog dialog = new OpenFileDialog() { Title = "请选择SVG文件所在的文件，处理当前文件夹内所有的SVG文件", Filter = "Scalable Vector Graphics (.svg)|*.svg|All files (*.*)|*.*" };
            if (dialog.ShowDialog() == true)
            {

                FileInfo file = new FileInfo(dialog.FileName);
                LoadDirectory = file.Directory;
                var svgs = Svg2GeoConverter.Parse(LoadDirectory);
                SvgsWarp.Children.Clear();

                foreach (var item in svgs)
                {
                    StackPanel stack = new StackPanel() { Margin = new Thickness(5) };


                    Path path = new Path();
                    path.Fill = Brushes.Purple;
                    path.Data = StreamGeometry.Parse(item.PathData);
                    path.Stretch = Stretch.Uniform;
                    path.Width = 56;
                    path.Height = 56;
                    path.HorizontalAlignment = HorizontalAlignment.Center;
                    path.VerticalAlignment = VerticalAlignment.Center;


                    Border border = new Border() { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1), HorizontalAlignment = HorizontalAlignment.Center };
                    border.Width = 96;
                    border.Height = 96;
                    border.Child = path;
                    stack.Children.Add(border);

                    TextBlock text = new TextBlock();
                    text.Text = item.Name;

                    text.HorizontalAlignment = HorizontalAlignment.Center;
                    text.VerticalAlignment = VerticalAlignment.Bottom;

                    stack.Children.Add(text);

                    SvgsWarp.Children.Add(stack);
                }

                SaveCSButton.IsEnabled = true;
            }
        }

        DirectoryInfo LoadDirectory;




        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (LoadDirectory != null && LoadDirectory.Exists)
            {
                saveFile = new FileInfo(System.IO.Path.Combine(LoadDirectory.FullName, LoadDirectory.Name + "IconSet.cs"));

                // 文件名过滤 ( 只包含字母数组和下划线）
                Func<string, string> getPropertyNameByEnglishName = new Func<string, string>(propertyName =>
                {
                    propertyName = propertyName.Trim();

                    // 去掉非法字符
                    // 使用正则表达式替换非字母、数字和下划线的字符为下划线
                    propertyName = Regex.Replace(propertyName, @"[^A-Za-z0-9_]", "_");

                    // 检查是否以数字开头，如果是，在前面添加下划线
                    if (char.IsDigit(propertyName[0]))
                    {
                        propertyName = "_" + propertyName;
                    }
                    else
                    {
                        // 首字母大写
                        propertyName = char.ToUpper(propertyName[0]) + propertyName.Substring(1);
                    }
                    return propertyName;
                });

                if (saveFile.Exists)
                {
                    saveFile.Delete();
                }

                if (Svg2GeoConverter.ParseToClassFile(LoadDirectory, saveFile, KeepAllTextCheckBox.IsChecked == true ? null : getPropertyNameByEnglishName))
                {
                    MessageBox.Show("导出成功，数据存储在" + saveFile.FullName);
                }
                else
                {
                    MessageBox.Show("导出失败！");
                }

            }
        }

        FileInfo saveFile;


        private void Goto_Click(object sender, RoutedEventArgs e)
        {
            if (saveFile != null && saveFile.Exists)
            {
                System.Diagnostics.Process.Start("explorer.exe", string.Format("/select, \"{0}\"", saveFile.FullName));
            }
            else
            {
                System.Diagnostics.Process.Start("explorer.exe", string.Format("/select, \"{0}\"", LoadDirectory.FullName));
            }
        }

        private void OpenClass_Click(object sender, RoutedEventArgs e)
        {
            var win = new IconSetWindow();
            win.Owner = this;
            win.Show();
        }
    }
}
