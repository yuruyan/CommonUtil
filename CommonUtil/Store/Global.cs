using CommonUtil.Route;
using CommonUtil.View;
using System;
using System.Collections.Generic;

namespace CommonUtil.Store {
    public class Global {
        public static readonly string AppTitle = "工具集";
        public static readonly string ImagePath = "/Resource/image/";

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
        };
    }

    public class ToolMenuItem {
        public string Name { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public Type ClassType { get; set; } = typeof(MainContentView);
    }
}
