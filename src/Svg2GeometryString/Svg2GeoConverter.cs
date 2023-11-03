using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Svg2GeometryString
{
    /// <summary>
    /// Svg格式文件数据转换成WPF支持的图标字符串转换器
    /// </summary>
    public static class Svg2GeoConverter
    {
        /// <summary>
        /// 将文件夹中所有svg文件转成C#格式的类文件，类名为XxxxIconSet
        /// Xxxx由输入的文件夹限定
        /// 类文件中根据SVG的文件生成对应的一个只读的String类型属性
        /// </summary>
        /// <param name="directory">输入的文件夹（包含svg图标）</param>
        /// <param name="outputFile">输出的类文件</param>
        /// <param name="getPropertyName">类中属性名与svg文件名的转换关系,默认为null</param>
        /// <returns></returns>
        public static bool ParseToClassFile(DirectoryInfo directory, FileInfo outputFile, Func<string, string> getPropertyName = null)
        {
            var svgFiles = Parse(directory);

            Console.WriteLine($"一共包含{svgFiles.Count}个图标。");

            if (outputFile.Exists)
            {
                outputFile.Delete();
            }

            Console.WriteLine($"开始转换...");
            using (FileStream fs = new FileStream(outputFile.FullName, FileMode.OpenOrCreate))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine("\t" + $"public class {directory.Name}IconSet");
                    sw.WriteLine("\t" + "{");

                    foreach (var item in svgFiles)
                    {
                        string propertyName = item.Name;

                        if (getPropertyName != null)
                        {
                            propertyName = getPropertyName.Invoke(propertyName);
                        }

                        sw.WriteLine("\t\t" + $"public String {propertyName} => \"{item.PathData}\";");
                        sw.WriteLine();
                        Console.WriteLine($"{propertyName}\t{item.Name}");
                    }

                    sw.WriteLine("\t" + "}");
                    sw.Close();
                }
            }
            Console.WriteLine($"转换完成");
            Console.WriteLine($"存储到 {outputFile.FullName}");
            return true;
        }

        /// <summary>
        /// 将一个svg文件的图标转换成 Wpf Path Data 支持的字符串格式（图形微语言）
        /// </summary>
        /// <param name="svgFile">SVG文件信息</param>
        /// <returns>图标对应的数据内容（图形微语言）</returns>
        public static String Parse(FileInfo svgFile)
        {
            // 读取svg文件
            XElement xElement = XElement.Load(svgFile.FullName);
            var eles = xElement.Elements();
            string pathData = "";
            foreach (var ele in eles)
            {
                pathData += ele.Attribute("d").Value.ToString();
            }
            return pathData;
        }

        /// <summary>
        /// 将文件夹当前路径下的所有svg文件的图标转换成 Wpf Path Data 支持的字符串格式（图形微语言）
        /// </summary>
        /// <param name="directory">文件夹</param>
        /// <returns>所有图标对应的名称和数据内容（图形微语言）</returns>
        public static List<SvgFileModel> Parse(DirectoryInfo directory)
        {
            List<SvgFileModel> svgDataFiles = new List<SvgFileModel>();

            string fileExtension = ".svg";
            var files = directory.GetFiles();
            var svgFiles = files.Where(t => System.IO.Path.GetExtension(t.FullName).Equals(fileExtension, StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var item in svgFiles)
            {
                string propertyName = item.Name.Substring(0, item.Name.Length - 4);

                SvgFileModel svgFile = new SvgFileModel();
                svgFile.Name = propertyName;
                svgFile.PathData = Parse(item);
                svgDataFiles.Add(svgFile);
            }

            return svgDataFiles;
        }
    }
}
