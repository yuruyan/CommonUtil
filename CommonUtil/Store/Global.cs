﻿using CommonUtil.View;
using System;
using System.Collections.Generic;
using System.IO;

namespace CommonUtil.Store {
    public class Global {
        public static readonly string AppTitle = "工具集";
        public static readonly string ImagePath = "/Resource/image/";
        /// <summary>
        /// 当前可执行文件目录别名
        /// </summary>
        public static readonly string ApplicationPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        private static readonly string _CacheDirectory = Path.Combine(Global.ApplicationPath, "cache");
        /// <summary>
        /// 缓存文件目录
        /// </summary>
        public static string CacheDirectory {
            get {
                // 检查缓存文件目录是否存在，不存在则创建
                if (!Directory.Exists(Global._CacheDirectory)) {
                    Directory.CreateDirectory(Global._CacheDirectory);
                }
                return _CacheDirectory;
            }
        }
        /// <summary>
        /// 菜单项目
        /// </summary>
        public static readonly List<ToolMenuItem> MenuItems = new() {
            new() { Name = "Base64 编码/解码", ImagePath = ImagePath + "base64.svg", ClassType = typeof(Base64ToolView) },
            new() { Name = "随机数/字符串生成器", ImagePath = ImagePath + "random.svg", ClassType = typeof(RandomGeneratorView) },
            new() { Name = "简体繁体转换", ImagePath = ImagePath + "ChineseTransform.svg", ClassType = typeof(ChineseTransformView) },
            new() { Name = "时间戳转换", ImagePath = ImagePath + "DateTime.svg", ClassType = typeof(TimeStampView) },
            new() { Name = "文件分割/合并", ImagePath = ImagePath + "Merge.svg", ClassType = typeof(FileMergeSplitView) },
            new() { Name = "数据散列工具", ImagePath = ImagePath + "Encryption.svg", ClassType = typeof(DataDigestView) },
            new() { Name = "ASCII 表格", ImagePath = ImagePath + "ascii.svg", ClassType = typeof(AsciiTableView) },
            new() { Name = "BMI 计算", ImagePath = ImagePath + "bmi.svg", ClassType = typeof(BMICalculatorView) },
            new() { Name = "进制转换", ImagePath = ImagePath + "BaseConversion.svg", ClassType = typeof(BaseConversionView) },
            new() { Name = "顺序文本生成", ImagePath = ImagePath + "order.svg", ClassType = typeof(OrdinalTextGeneratorView) },
            new() { Name = "文件内容搜索", ImagePath = ImagePath + "FileSearch.svg", ClassType = typeof(KeywordFinderView) },
            new() { Name = "文本工具", ImagePath = ImagePath + "TextTool.svg", ClassType = typeof(TextToolView) },
            new() { Name = "二维码工具", ImagePath = ImagePath + "qrcode.svg", ClassType = typeof(QRCodeToolView) },
            new() { Name = "颜色转换", ImagePath = ImagePath + "ColorPicker.svg", ClassType = typeof(ColorTransformView) },
            new() { Name = "代码格式化", ImagePath = ImagePath + "format.svg", ClassType = typeof(CodeFormatingView) },
        };
    }

    public class ToolMenuItem {
        public string Name { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public Type ClassType { get; set; } = typeof(MainContentView);
    }
}
