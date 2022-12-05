namespace CommonUtil.View;

public partial class ColorTransformView : Page {
    public class SliderInfo : DependencyObject {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(SliderInfo), new PropertyMetadata(0.0));

        public string Header { get; }
        public double MinValue { get; }
        public double MaxValue { get; }
        public double SmallChange { get; }
        /// <summary>
        /// 当前值
        /// </summary>
        public double Value {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public int ToolTipPrecision { get; set; } = 0;

        public SliderInfo(string header, double minValue, double maxValue, double smallChange) {
            Header = header;
            MinValue = minValue;
            MaxValue = maxValue;
            SmallChange = smallChange;
        }
    }

    public class ColorItem : DependencyObject {
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(string), typeof(ColorItem), new PropertyMetadata(""));

        public string Color {
            get { return (string)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        public string Tag { get; set; } = string.Empty;
        public Func<Color, string> ColorToString { get; set; }
        public Func<string, Color?> StringToColor { get; set; }
        public Delegate ValuesToColorConverter { get; }
        public Delegate ColorToValuesConverter { get; }
        public IList<SliderInfo> Sliders { get; }

        public ColorItem(string tag, Func<Color, string> colorToString, Func<string, Color?> stringToColor, Delegate valuesToColorConverter, Delegate colorToValuesConverter, IList<SliderInfo> sliders) {
            Tag = tag;
            ColorToString = colorToString;
            StringToColor = stringToColor;
            ValuesToColorConverter = valuesToColorConverter;
            ColorToValuesConverter = colorToValuesConverter;
            Sliders = sliders;
        }

    }

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorTransformView), new PropertyMetadata());
    private static readonly DependencyProperty ColorItemsProperty = DependencyProperty.Register("ColorItems", typeof(IList<ColorItem>), typeof(ColorTransformView), new PropertyMetadata());

    public Color SelectedColor {
        get { return (Color)GetValue(SelectedColorProperty); }
        set { SetValue(SelectedColorProperty, value); }
    }
    private IList<ColorItem> ColorItems {
        get { return (IList<ColorItem>)GetValue(ColorItemsProperty); }
        set { SetValue(ColorItemsProperty, value); }
    }
    /// <summary>
    /// 更新时间间隔，ms
    /// </summary>
    private const int UpdateInterval = 100;
    private Color CompareColor;
    /// <summary>
    /// UpdateColorResults，提示为成员变量，节省内存
    /// </summary>
    private readonly List<string> UpdateColorResults;

    public ColorTransformView() {
        DependencyPropertyDescriptor.FromProperty(SelectedColorProperty, typeof(ColorTransformView)).AddValueChanged(this, ColorChangedHandler);
        ColorItems = new List<ColorItem>() {
             new (
                 "HEX",
                 ColorTransform.ColorToHEX,
                 ColorTransform.HEXToColor,
                 (Func<double,double,double,double,Color?>)ColorTransform.HEXToColor,
                 ColorTransform.ColorToHEXValues,
                 new List<SliderInfo>() {
                     new ("A",0, 255, 1),
                     new ("R",0, 255, 1),
                     new ("G",0, 255, 1),
                     new ("B",0, 255, 1),
                 }
             ),
             new (
                 "RGB",
                 ColorTransform.ColorToRGB,
                 ColorTransform.RGBToColor,
                 (Func<double,double,double,Color?>)ColorTransform.RGBToColor,
                 ColorTransform.ColorToRGBValues,
                 new List<SliderInfo>() {
                     new ("R",0, 255, 1),
                     new ("G",0, 255, 1),
                     new ("B",0, 255, 1),
                 }
             ),
             new (
                 "RGBA1",
                 ColorTransform.ColorToRGBA1,
                 ColorTransform.RGBA1ToColor,
                 (Func<double,double,double,double,Color?>)ColorTransform.RGBA1ToColor,
                 ColorTransform.ColorToRGBA1Values,
                 new List<SliderInfo>() {
                     new ("R",0, 255, 1),
                     new ("G",0, 255, 1),
                     new ("B",0, 255, 1),
                     new ("A",0, 255, 1),
                 }
             ),
             new (
                 "RGBA2",
                 ColorTransform.ColorToRGBA2,
                 ColorTransform.RGBA2ToColor,
                 (Func<double,double,double,double,Color?>)ColorTransform.RGBA2ToColor,
                 ColorTransform.ColorToRGBA2Values,
                 new List<SliderInfo>() {
                     new ("R",0, 255, 1),
                     new ("G",0, 255, 1),
                     new ("B",0, 255, 1),
                     new ("A",0, 1, 0.01) { ToolTipPrecision = 2},
                 }
             ),
             new (
                 "HSL",
                 ColorTransform.ColorToHSL,
                 ColorTransform.HSLToColor,
                 (Func<double, double, double, Color?>)ColorTransform.HSLToColor,
                 ColorTransform.ColorToHSLValues,
                 new List<SliderInfo>() {
                     new ("H",0, 360, 1),
                     new ("S",0, 100, 1),
                     new ("L",0, 100, 1),
                 }
             ),
             new (
                 "HSV",
                 ColorTransform.ColorToHSV,
                 ColorTransform.HSVToColor,
                 (Func<double, double, double, Color?>)ColorTransform.HSVToColor,
                 ColorTransform.ColorToHSVValues,
                 new List<SliderInfo>() {
                     new ("H",0, 360, 1),
                     new ("S",0, 100, 1),
                     new ("V",0, 100, 1),
                 }
             ),
             new (
                 "LAB",
                 ColorTransform.ColorToLAB,
                 ColorTransform.LABToColor,
                 (Func<double, double, double, Color?>)ColorTransform.LABToColor,
                 ColorTransform.ColorToLABValues,
                 new List<SliderInfo>() {
                     new ("L",0, 100, 1),
                     new ("A",-128, 127, 1),
                     new ("B",-128, 127, 1),
                 }
             ),
             new (
                 "XYZ",
                 ColorTransform.ColorToXYZ,
                 ColorTransform.XYZToColor,
                 (Func<double, double, double, Color?>)ColorTransform.XYZToColor,
                 ColorTransform.ColorToXYZValues,
                 new List<SliderInfo>() {
                     new ("X",0, 95.05, 1),
                     new ("Y",0, 100, 1),
                     new ("Z",0, 108.88, 1),
                 }
             ),
             new (
                 "LCH",
                 ColorTransform.ColorToLCH,
                 ColorTransform.LCHToColor,
                 (Func<double, double, double, Color?>)ColorTransform.LCHToColor,
                 ColorTransform.ColorToLCHValues,
                 new List<SliderInfo>() {
                     new ("L",0, 100, 1),
                     new ("C",0, 230, 1),
                     new ("H",0, 270, 1),
                 }
             ),
             new (
                 "CMYK",
                 ColorTransform.ColorToCMYK,
                 ColorTransform.CMYKToColor,
                 (Func<double,double,double,double,Color?>)ColorTransform.CMYKToColor,
                 ColorTransform.ColorToCMYKValues,
                 new List<SliderInfo>() {
                     new ("C",0, 1, 0.01) { ToolTipPrecision = 2},
                     new ("M",0, 1, 0.01) { ToolTipPrecision = 2},
                     new ("Y",0, 1, 0.01) { ToolTipPrecision = 2},
                     new ("K",0, 1, 0.01) { ToolTipPrecision = 2},
                 }
             ),
             new (
                 "LUV",
                 ColorTransform.ColorToLUV,
                 ColorTransform.LUVToColor,
                 (Func<double, double, double, Color?>)ColorTransform.LUVToColor,
                 ColorTransform.ColorToLUVValues,
                 new List<SliderInfo>() {
                     new ("L",0, 100, 1),
                     new ("U",-134, 220, 1),
                     new ("V",-140, 122, 1),
                 }
             ),
        };
        UpdateColorResults = new(ColorItems.Count);
        InitializeComponent();
        SelectedColor = Colors.White;
    }

    /// <summary>
    /// 更新颜色
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void UpdateColor(Color color) {
        DebounceUtils.Debounce(UpdateColor, () => {
            UpdateColorResults.Clear();
            foreach (var item in Dispatcher.Invoke(() => ColorItems)) {
                UpdateColorResults.Add(item.ColorToString(color));
            }
            CompareColor = color;
            // 更新修改
            Dispatcher.Invoke(() => {
                for (int i = 0; i < UpdateColorResults.Count; i++) {
                    ColorItems[i].Color = UpdateColorResults[i];
                }
            });
        }, true, UpdateInterval);
    }

    /// <summary>
    /// 颜色变化
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ColorChangedHandler(object? sender, EventArgs e) {
        ColorPanel.Fill = new SolidColorBrush(SelectedColor);
        UpdateColor(SelectedColor);
    }

    /// <summary>
    /// 更新输入颜色
    /// </summary>
    /// <param name="color"></param>
    private void UpdateInputColor(Color? color) {
        if (color is Color c) {
            // 不能和 SelectedColor 比较
            if (CompareColor != c) {
                SelectedColor = c;
            }
        }
    }

    /// <summary>
    /// 处理键盘事件，进行更新
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TextBoxKeyUpHandler(object sender, KeyEventArgs e) {
        e.Handled = true;
        if (e.Key != Key.Enter) {
            return;
        }
        if (sender is TextBox box && box.DataContext is ColorItem colorItem) {
            UpdateInputColor(colorItem.StringToColor(colorItem.Color));
        }
    }

    /// <summary>
    /// 复制到剪贴板
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CopyColorClick(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (sender is FrameworkElement element && element.DataContext is ColorItem colorItem) {
            Clipboard.SetDataObject(colorItem.ColorToString(SelectedColor));
            MessageBox.Success("已复制");
        }
    }

    /// <summary>
    /// 通过 SliderInfo 获取 ColorItem
    /// </summary>
    private readonly IDictionary<SliderInfo, ColorItem> SliderInfoColorItemDict = new Dictionary<SliderInfo, ColorItem>();

    /// <summary>
    /// SliderValueChanged
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SliderValueChangedHandler(object? sender, EventArgs e) {
        DebounceUtils.Debounce(SliderValueChangedHandler, () => {
            Dispatcher.Invoke(() => {
                if (sender is SliderInfo info) {
                    var colorItem = SliderInfoColorItemDict[info];
                    var color = colorItem.ValuesToColorConverter.DynamicInvoke(
                        colorItem.Sliders.Select(s => s.Value).Cast<object>().ToArray()
                    ) as Color?;
                    UpdateInputColor(color);
                }
            });
        }, true, UpdateInterval);
    }

    /// <summary>
    /// ColorItemContextMenuLoaded
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ColorItemContextMenuLoadedHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (sender is not FrameworkElement element) {
            return;
        }
        element.UpdateDefaultStyle();
        var colorItem = (ColorItem)element.DataContext;
        #region 移除 Hook
        foreach (var item in colorItem.Sliders) {
            SliderInfoColorItemDict[item] = colorItem;
            var descriptor = DependencyPropertyDescriptor.FromProperty(SliderInfo.ValueProperty, item.GetType());
            descriptor.RemoveValueChanged(item, SliderValueChangedHandler);
        }
        #endregion
        #region 设置 Slider Values
        var values = colorItem.ColorToValuesConverter.DynamicInvoke(SelectedColor);
        var sliders = colorItem.Sliders;
        if (values is ValueTuple<double, double, double> threeValues) {
            (sliders[0].Value, sliders[1].Value, sliders[2].Value) = threeValues;
        } else if (values is ValueTuple<double, double, double, double> fourValues) {
            (sliders[0].Value, sliders[1].Value, sliders[2].Value, sliders[3].Value) = fourValues;
        }
        #endregion
        #region 添加 Hook
        foreach (var item in colorItem.Sliders) {
            SliderInfoColorItemDict[item] = colorItem;
            var descriptor = DependencyPropertyDescriptor.FromProperty(SliderInfo.ValueProperty, item.GetType());
            descriptor.AddValueChanged(item, SliderValueChangedHandler);
        }
        #endregion
    }

    /// <summary>
    /// 左键单击显示 ContextMenu
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ColorItemHeaderMouseUpHandler(object sender, MouseButtonEventArgs e) {
        if (sender is FrameworkElement element) {
            element.ContextMenu.DataContext = element.DataContext;
            element.ContextMenu.IsOpen = true;
        }
    }
}
