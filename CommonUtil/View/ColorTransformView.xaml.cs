using CommonUtil.Core;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CommonUtil.View;

public partial class ColorTransformView : Page {
    public class SliderInfo : DependencyObject {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(SliderInfo), new PropertyMetadata(0.0));

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

        public SliderInfo(double minValue, double maxValue, double smallChange) {
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
        public Delegate SliderConverter { get; }
        public IList<SliderInfo> Sliders { get; }

        public ColorItem(string tag, Func<Color, string> colorToString, Func<string, Color?> stringToColor, Delegate sliderConverter, IList<SliderInfo> sliders) {
            Tag = tag;
            ColorToString = colorToString;
            StringToColor = stringToColor;
            SliderConverter = sliderConverter;
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

    private Color CompareColor;
    private bool IsColorChanged = false;

    public ColorTransformView() {
        DependencyPropertyDescriptor.FromProperty(SelectedColorProperty, typeof(ColorTransformView)).AddValueChanged(this, ColorChangedHandler);
        ColorItems = new List<ColorItem>() {
             new (
                 "HEX",
                 ColorTransform.ColorToHEX,
                 ColorTransform.HEXToColor,
                 (Func<double,double,double,double,Color?>)ColorTransform.HEXToColor,
                 new List<SliderInfo>(){
                     new (0, 255, 1),
                     new (0, 255, 1),
                     new (0, 255, 1),
                     new (0, 255, 1),
                 }
             ),
             new (
                 "RGB",
                 ColorTransform.ColorToRGB,
                 ColorTransform.RGBToColor,
                 (Func<double,double,double,Color?>)ColorTransform.RGBToColor,
                 new List<SliderInfo>(){
                     new (0, 255, 1),
                     new (0, 255, 1),
                     new (0, 255, 1),
                 }
             ),
             new (
                 "RGBA1",
                 ColorTransform.ColorToRGBA1,
                 ColorTransform.RGBA1ToColor,
                 (Func<double,double,double,double,Color?>)ColorTransform.RGBA1ToColor,
                 new List<SliderInfo>(){
                     new (0, 255, 1),
                     new (0, 255, 1),
                     new (0, 255, 1),
                     new (0, 255, 1),
                 }
             ),
             new (
                 "RGBA2",
                 ColorTransform.ColorToRGBA2,
                 ColorTransform.RGBA2ToColor,
                 (Func<double,double,double,double,Color?>)ColorTransform.RGBA2ToColor,
                 new List<SliderInfo>(){
                     new (0, 255, 1),
                     new (0, 255, 1),
                     new (0, 255, 1),
                     new (0, 1, 0.01),
                 }
             ),
             new (
                 "HSL",
                 ColorTransform.ColorToHSL,
                 ColorTransform.HSLToColor,
                 (Func<double, double, double, Color?>)ColorTransform.HSLToColor,
                 new List<SliderInfo>(){
                     new (0, 360, 1),
                     new (0, 100, 1),
                     new (0, 100, 1),
                 }
             ),
             new (
                 "HSV",
                 ColorTransform.ColorToHSV,
                 ColorTransform.HSVToColor,
                 (Func<double, double, double, Color?>)ColorTransform.HSVToColor,
                 new List<SliderInfo>(){
                     new (0, 360, 1),
                     new (0, 100, 1),
                     new (0, 100, 1),
                 }
             ),
             new (
                 "LAB",
                 ColorTransform.ColorToLAB,
                 ColorTransform.LABToColor,
                 (Func<double, double, double, Color?>)ColorTransform.LABToColor,
                 new List<SliderInfo>(){
                     new (0, 100, 1),
                     new (-128, 127, 1),
                     new (-128, 127, 1),
                 }
             ),
             new (
                 "XYZ",
                 ColorTransform.ColorToXYZ,
                 ColorTransform.XYZToColor,
                 (Func<double, double, double, Color?>)ColorTransform.XYZToColor,
                 new List<SliderInfo>(){
                     new (0, 95.05, 1),
                     new (0, 100, 1),
                     new (0, 108.88, 1),
                 }
             ),
             new (
                 "LCH",
                 ColorTransform.ColorToLCH,
                 ColorTransform.LCHToColor,
                 (Func<double, double, double, Color?>)ColorTransform.LCHToColor,
                 new List<SliderInfo>(){
                     new (0, 100, 1),
                     new (0, 230, 1),
                     new (0, 270, 1),
                 }
             ),
             new (
                 "CMYK",
                 ColorTransform.ColorToCMYK,
                 ColorTransform.CMYKToColor,
                 (Func<double,double,double,double,Color?>)ColorTransform.CMYKToColor,
                 new List<SliderInfo>(){
                     new (0, 1, 0.01),
                     new (0, 1, 0.01),
                     new (0, 1, 0.01),
                     new (0, 1, 0.01),
                 }
             ),
             new (
                 "LUV",
                 ColorTransform.ColorToLUV,
                 ColorTransform.LUVToColor,
                 (Func<double, double, double, Color?>)ColorTransform.LUVToColor,
                 new List<SliderInfo>(){
                     new (0, 100, 1),
                     new (-134, 220, 1),
                     new (-140, 122, 1),
                 }
             ),
        };
        InitializeComponent();
        SelectedColor = Colors.White;

        // 定时更新而不是立即更新防止界面卡顿
        var timer = new System.Timers.Timer {
            Interval = 100
        };
        timer.Elapsed += UpdateColor;
        timer.Start();
    }

    /// <summary>
    /// 更新颜色
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void UpdateColor(object? sender, ElapsedEventArgs e) {
        if (!IsColorChanged) {
            return;
        }
        IsColorChanged = false;
        Color color = Dispatcher.Invoke(() => SelectedColor);
        // 最好加锁
        lock (this) {
            var results = new List<string>();
            foreach (var item in Dispatcher.Invoke(() => ColorItems)) {
                results.Add(item.ColorToString(color));
            }
            CompareColor = color;
            // 更新修改
            Dispatcher.Invoke(() => {
                for (int i = 0; i < results.Count; i++) {
                    ColorItems[i].Color = results[i];
                }
            });
        }
    }

    /// <summary>
    /// 颜色变化
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ColorChangedHandler(object? sender, EventArgs e) {
        ColorPanel.Fill = new SolidColorBrush(SelectedColor);
        IsColorChanged = true;
    }

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
    private void TextBoxKeyUpHandler(object sender, System.Windows.Input.KeyEventArgs e) {
        e.Handled = true;
        if (e.Key != System.Windows.Input.Key.Enter) {
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
            CommonUITools.Widget.MessageBox.Success("已复制");
        }
    }
}
