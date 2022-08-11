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
    public class ColorItem : DependencyObject {
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(string), typeof(ColorItem), new PropertyMetadata(""));

        public string Color {
            get { return (string)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        public string Tag { get; set; } = string.Empty;
        public Func<Color, string> ColorToString { get; set; }
        public Func<string, Color?> StringToColor { get; set; }

        public ColorItem(string tag, Func<Color, string> colorToString, Func<string, Color?> stringToColor) {
            Tag = tag;
            ColorToString = colorToString;
            StringToColor = stringToColor;
        }

    }

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorTransformView), new PropertyMetadata());
    public static readonly DependencyProperty ColorItemsProperty = DependencyProperty.Register("ColorItems", typeof(IList<ColorItem>), typeof(ColorTransformView), new PropertyMetadata());

    public Color SelectedColor {
        get { return (Color)GetValue(SelectedColorProperty); }
        set { SetValue(SelectedColorProperty, value); }
    }
    public IList<ColorItem> ColorItems {
        get { return (IList<ColorItem>)GetValue(ColorItemsProperty); }
        set { SetValue(ColorItemsProperty, value); }
    }

    private Color CompareColor;
    private bool IsColorChanged = false;

    public ColorTransformView() {
        DependencyPropertyDescriptor.FromProperty(SelectedColorProperty, typeof(ColorTransformView)).AddValueChanged(this, ColorChangedHandler);
        ColorItems = new List<ColorItem>() {
             new ("HEX", ColorTransform.ColorToHEX, ColorTransform.HEXToColor),
             new ("RGB", ColorTransform.ColorToRGB, ColorTransform.RGBToColor),
             new ("RGBA1", ColorTransform.ColorToRGBA1, ColorTransform.RGBA1ToColor),
             new ("RGBA2", ColorTransform.ColorToRGBA2, ColorTransform.RGBA2ToColor),
             new ("HSL", ColorTransform.ColorToHSL, ColorTransform.HSLToColor),
             new ("HSV", ColorTransform.ColorToHSV, ColorTransform.HSVToColor),
             new ("LAB", ColorTransform.ColorToLAB, ColorTransform.LABToColor),
             new ("XYZ", ColorTransform.ColorToXYZ, ColorTransform.XYZToColor),
             new ("LCH", ColorTransform.ColorToLCH, ColorTransform.LCHToColor),
             new ("CMYK", ColorTransform.ColorToCMYK, ColorTransform.CMYKToColor),
             new ("LUV", ColorTransform.ColorToLUV, ColorTransform.LUVToColor),
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
    /// <exception cref="NotImplementedException"></exception>
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

