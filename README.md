## Svg2GeometryString

### 开发的缘由：

- WPF中的Path元素可以直接使用图形微语言的字符串来填充其Data属性，并可以显示出对应矢量图形。
- Svg格式文件是可缩放矢量图形文件，他是基于图形微语言和XML文本格式形成的图形格式。
- 常用的图标库（例如：IconFont、FontAwesome）都提供了SVG格式图标的数据复制或下载。
- 但是直接复制或者下载SVG图标不能直接使用，需要删除格式重组，工作量比较多。
- 所以就想加载SVG文件直接输出完整的图形微语言的字符串，可以直接绑定或者赋值给Path的Data，实现图标的显示。



### 好用的图标库推荐

[iconfont-阿里巴巴矢量图标库](https://www.iconfont.cn/)

[ByteDance IconPark (oceanengine.com)-字节跳动图标库](http://iconpark.oceanengine.com/home)

[FontAwesome 字体图标中文Icon](https://fontawesome.com.cn/)



### 使用说明

提供函数库、EXE可执行文件两种方式。



#### 函数库：

用户可以在应用程序中添加svg图片资源，然后通过**Svg2GeometryString**中的函数将SVG文件转换成WPF中Path的Data属性可以识别的矢量图形或者使用StreamGeometry进行转换。库函数共有三个：

```c#
        /// <summary>
        /// 将一个svg文件的图标转换成 Wpf Path Data 支持的字符串格式（图形微语言）
        /// </summary>
        /// <param name="svgFile">SVG文件信息</param>
        /// <returns>图标对应的数据内容（图形微语言）</returns>
        public static String Parse(FileInfo svgFile)
```

用户可以传入一个SVG格式文件，返回文件对应的图形微语言数据字符串。

```c#
        /// <summary>
        /// 将文件夹当前路径下的所有svg文件的图标转换成 Wpf Path Data 支持的字符串格式（图形微语言）
        /// </summary>
        /// <param name="directory">文件夹</param>
        /// <returns>所有图标对应的名称和数据内容（图形微语言）</returns>
        public static List<SvgFileModel> Parse(DirectoryInfo directory)
```

用户可以传入一个包含多个SVG格式文件的文件夹，返回文件夹内所有图标的微语言字符串，按照列表形式反馈

```c#
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
```

用户输入一个文件夹地址，输出C#类结构的文件，每个属性代表一个字符串，防止文件名冲突，用户最好还需要输入文件名检查规则。

以上库函数，仓库中提供了示例程序，位于*\Samples\Svg2GeometryString.Apply*用户可以参考。

#### 可执行文件：

更多的时候，我们（我是这样的）只需要拿到svg的字符串部分，然后直接或者间接的粘贴到我们Path的Data属性上，完成我们图标的显示。因此我们可以直接下载示例程序。导入文件或者文件所在文件夹，生成对应字符串和图标集类，然后集成到我们的项目中。