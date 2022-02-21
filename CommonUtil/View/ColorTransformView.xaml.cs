using CommonUtil.Core;
using NLog;
using System;
using System.ComponentModel;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CommonUtil.View {
    public partial class ColorTransformView : Page {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorTransformView), new PropertyMetadata());
        public static readonly DependencyProperty HEXColorProperty = DependencyProperty.Register("HEXColor", typeof(string), typeof(ColorTransformView), new PropertyMetadata(""));
        public static readonly DependencyProperty RGBColorProperty = DependencyProperty.Register("RGBColor", typeof(string), typeof(ColorTransformView), new PropertyMetadata(""));
        public static readonly DependencyProperty RGBA1ColorProperty = DependencyProperty.Register("RGBA1Color", typeof(string), typeof(ColorTransformView), new PropertyMetadata(""));
        public static readonly DependencyProperty RGBA2ColorProperty = DependencyProperty.Register("RGBA2Color", typeof(string), typeof(ColorTransformView), new PropertyMetadata(""));
        public static readonly DependencyProperty HSLColorProperty = DependencyProperty.Register("HSLColor", typeof(string), typeof(ColorTransformView), new PropertyMetadata(""));
        public static readonly DependencyProperty HSVColorProperty = DependencyProperty.Register("HSVColor", typeof(string), typeof(ColorTransformView), new PropertyMetadata(""));
        public static readonly DependencyProperty LABColorProperty = DependencyProperty.Register("LABColor", typeof(string), typeof(ColorTransformView), new PropertyMetadata(""));
        public static readonly DependencyProperty XYZColorProperty = DependencyProperty.Register("XYZColor", typeof(string), typeof(ColorTransformView), new PropertyMetadata(""));
        public static readonly DependencyProperty LCHColorProperty = DependencyProperty.Register("LCHColor", typeof(string), typeof(ColorTransformView), new PropertyMetadata(""));
        public static readonly DependencyProperty CMYKColorProperty = DependencyProperty.Register("CMYKColor", typeof(string), typeof(ColorTransformView), new PropertyMetadata(""));
        public static readonly DependencyProperty LUVColorProperty = DependencyProperty.Register("LUVColor", typeof(string), typeof(ColorTransformView), new PropertyMetadata(""));

        public Color SelectedColor {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }
        /// <summary>
        /// 十六进制 Color
        /// </summary>
        public string HEXColor {
            get { return (string)GetValue(HEXColorProperty); }
            set { SetValue(HEXColorProperty, value); }
        }
        /// <summary>
        /// RGB
        /// </summary>
        public string RGBColor {
            get { return (string)GetValue(RGBColorProperty); }
            set { SetValue(RGBColorProperty, value); }
        }
        /// <summary>
        /// RGBA
        /// </summary>
        public string RGBA1Color {
            get { return (string)GetValue(RGBA1ColorProperty); }
            set { SetValue(RGBA1ColorProperty, value); }
        }
        /// <summary>
        /// RGBA2
        /// </summary>
        public string RGBA2Color {
            get { return (string)GetValue(RGBA2ColorProperty); }
            set { SetValue(RGBA2ColorProperty, value); }
        }
        /// <summary>
        /// HSL
        /// </summary>
        public string HSLColor {
            get { return (string)GetValue(HSLColorProperty); }
            set { SetValue(HSLColorProperty, value); }
        }
        /// <summary>
        /// HSV
        /// </summary>
        public string HSVColor {
            get { return (string)GetValue(HSVColorProperty); }
            set { SetValue(HSVColorProperty, value); }
        }
        /// <summary>
        /// LAB
        /// </summary>
        public string LABColor {
            get { return (string)GetValue(LABColorProperty); }
            set { SetValue(LABColorProperty, value); }
        }
        /// <summary>
        /// XYZ
        /// </summary>
        public string XYZColor {
            get { return (string)GetValue(XYZColorProperty); }
            set { SetValue(XYZColorProperty, value); }
        }
        /// <summary>
        /// LCH
        /// </summary>
        public string LCHColor {
            get { return (string)GetValue(LCHColorProperty); }
            set { SetValue(LCHColorProperty, value); }
        }
        /// <summary>
        /// CMYK
        /// </summary>
        public string CMYKColor {
            get { return (string)GetValue(CMYKColorProperty); }
            set { SetValue(CMYKColorProperty, value); }
        }
        /// <summary>
        /// LUV
        /// </summary>
        public string LUVColor {
            get { return (string)GetValue(LUVColorProperty); }
            set { SetValue(LUVColorProperty, value); }
        }

        private Color CompareColor;
        private bool IsColorChanged = false;

        public ColorTransformView() {
            DependencyPropertyDescriptor.FromProperty(SelectedColorProperty, typeof(ColorTransformView)).AddValueChanged(this, ColorChangedHandler);
            DependencyPropertyDescriptor.FromProperty(HEXColorProperty, typeof(ColorTransformView)).AddValueChanged(this, HEXColorChangedHandler);
            DependencyPropertyDescriptor.FromProperty(RGBColorProperty, typeof(ColorTransformView)).AddValueChanged(this, RGBColorChangedHandler);
            DependencyPropertyDescriptor.FromProperty(RGBA1ColorProperty, typeof(ColorTransformView)).AddValueChanged(this, RGBA1ColorChangedHandler);
            DependencyPropertyDescriptor.FromProperty(RGBA2ColorProperty, typeof(ColorTransformView)).AddValueChanged(this, RGBA2ColorChangedHandler);
            DependencyPropertyDescriptor.FromProperty(HSLColorProperty, typeof(ColorTransformView)).AddValueChanged(this, HSLColorChangedHandler);
            DependencyPropertyDescriptor.FromProperty(HSVColorProperty, typeof(ColorTransformView)).AddValueChanged(this, HSVColorChangedHandler);
            DependencyPropertyDescriptor.FromProperty(LABColorProperty, typeof(ColorTransformView)).AddValueChanged(this, LABColorChangedHandler);
            DependencyPropertyDescriptor.FromProperty(XYZColorProperty, typeof(ColorTransformView)).AddValueChanged(this, XYZColorChangedHandler);
            DependencyPropertyDescriptor.FromProperty(LCHColorProperty, typeof(ColorTransformView)).AddValueChanged(this, LCHColorChangedHandler);
            DependencyPropertyDescriptor.FromProperty(CMYKColorProperty, typeof(ColorTransformView)).AddValueChanged(this, CMYKColorChangedHandler);
            DependencyPropertyDescriptor.FromProperty(LUVColorProperty, typeof(ColorTransformView)).AddValueChanged(this, LUVColorChangedHandler);

            InitializeComponent();
            SelectedColor = Colors.Bisque;

            // 定时更新而不是立即更新防止界面卡顿
            var timer = new System.Timers.Timer {
                Interval = 75
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
            Color color = Colors.Transparent;
            Dispatcher.Invoke(() => color = SelectedColor);
            ThreadPool.QueueUserWorkItem(o => {
                // 最好加锁
                lock (this) {
                    var hEXColor = ColorTransform.ColorToHEX(color);
                    var rGBColor = ColorTransform.ColorToRGB(color);
                    var rGBA1Color = ColorTransform.ColorToRGBA1(color);
                    var rGBA2Color = ColorTransform.ColorToRGBA2(color);
                    var hSLColor = ColorTransform.ColorToHSL(color);
                    var hSVColor = ColorTransform.ColorToHSV(color);
                    var lABColor = ColorTransform.ColorToLAB(color);
                    var xYZColor = ColorTransform.ColorToXYZ(color);
                    var lCHColor = ColorTransform.ColorToLCH(color);
                    var cMYKColor = ColorTransform.ColorToCMYK(color);
                    var lUVColor = ColorTransform.ColorToLUV(color);
                    CompareColor = color;
                    Dispatcher.Invoke(() => {
                        // 不改变正在修改的值
                        if (!HEXColorBox.IsFocused) HEXColor = hEXColor;
                        if (!RGBColorBox.IsFocused) RGBColor = rGBColor;
                        if (!RGBA1ColorBox.IsFocused) RGBA1Color = rGBA1Color;
                        if (!RGBA2ColorBox.IsFocused) RGBA2Color = rGBA2Color;
                        if (!HSLColorBox.IsFocused) HSLColor = hSLColor;
                        if (!HSVColorBox.IsFocused) HSVColor = hSVColor;
                        if (!LABColorBox.IsFocused) LABColor = lABColor;
                        if (!XYZColorBox.IsFocused) XYZColor = xYZColor;
                        if (!LCHColorBox.IsFocused) LCHColor = lCHColor;
                        if (!CMYKColorBox.IsFocused) CMYKColor = cMYKColor;
                        if (!LUVColorBox.IsFocused) LUVColor = lUVColor;
                    });
                }
            });
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

        private void HEXColorChangedHandler(object? sender, EventArgs e) {
            if (HEXColorBox.IsFocused) {
                UpdateInputColor(ColorTransform.HEXToColor(HEXColor));
            }
        }

        private void RGBColorChangedHandler(object? sender, EventArgs e) {
            if (RGBColorBox.IsFocused) {
                UpdateInputColor(ColorTransform.RGBToColor(RGBColor));
            }
        }

        private void RGBA1ColorChangedHandler(object? sender, EventArgs e) {
            if (RGBA1ColorBox.IsFocused) {
                UpdateInputColor(ColorTransform.RGBA1ToColor(RGBA1Color));
            }
        }

        private void RGBA2ColorChangedHandler(object? sender, EventArgs e) {
            if (RGBA2ColorBox.IsFocused) {
                UpdateInputColor(ColorTransform.RGBA2ToColor(RGBA2Color));
            }
        }

        private void HSLColorChangedHandler(object? sender, EventArgs e) {
            if (HSLColorBox.IsFocused) {
                UpdateInputColor(ColorTransform.HSLToColor(HSLColor));
            }
        }

        private void HSVColorChangedHandler(object? sender, EventArgs e) {
            if (HSVColorBox.IsFocused) {
                UpdateInputColor(ColorTransform.HSVToColor(HSVColor));
            }
        }

        private void LABColorChangedHandler(object? sender, EventArgs e) {
            if (LABColorBox.IsFocused) {
                UpdateInputColor(ColorTransform.LABToColor(LABColor));
            }
        }

        private void XYZColorChangedHandler(object? sender, EventArgs e) {
            if (XYZColorBox.IsFocused) {
                UpdateInputColor(ColorTransform.XYZToColor(XYZColor));
            }
        }

        private void LCHColorChangedHandler(object? sender, EventArgs e) {
            if (LCHColorBox.IsFocused) {
                UpdateInputColor(ColorTransform.LCHToColor(LCHColor));
            }
        }

        private void CMYKColorChangedHandler(object? sender, EventArgs e) {
            if (CMYKColorBox.IsFocused) {
                UpdateInputColor(ColorTransform.CMYKToColor(CMYKColor));
            }
        }

        private void LUVColorChangedHandler(object? sender, EventArgs e) {
            if (LUVColorBox.IsFocused) {
                UpdateInputColor(ColorTransform.LUVToColor(LUVColor));
            }
        }

        private void CopyHexClick(object sender, RoutedEventArgs e) {
            e.Handled = true;
            Clipboard.SetDataObject(ColorTransform.ColorToHEX(SelectedColor));
            Widget.MessageBox.Success("已复制");
        }

        private void CopyRGBClick(object sender, RoutedEventArgs e) {
            e.Handled = true;
            Clipboard.SetDataObject(ColorTransform.ColorToRGB(SelectedColor));
            Widget.MessageBox.Success("已复制");
        }

        private void CopyRGBA1Click(object sender, RoutedEventArgs e) {
            e.Handled = true;
            Clipboard.SetDataObject(ColorTransform.ColorToRGB(SelectedColor));
            Widget.MessageBox.Success("已复制");
        }

        private void CopyRGBA2Click(object sender, RoutedEventArgs e) {
            e.Handled = true;
            Clipboard.SetDataObject(ColorTransform.ColorToRGB(SelectedColor));
            Widget.MessageBox.Success("已复制");
        }

        private void CopyHSLClick(object sender, RoutedEventArgs e) {
            e.Handled = true;
            Clipboard.SetDataObject(ColorTransform.ColorToHSL(SelectedColor));
            Widget.MessageBox.Success("已复制");
        }

        private void CopyHSVClick(object sender, RoutedEventArgs e) {
            e.Handled = true;
            Clipboard.SetDataObject(ColorTransform.ColorToHSV(SelectedColor));
            Widget.MessageBox.Success("已复制");
        }

        private void CopyLABClick(object sender, RoutedEventArgs e) {
            e.Handled = true;
            Clipboard.SetDataObject(ColorTransform.ColorToLAB(SelectedColor));
            Widget.MessageBox.Success("已复制");
        }

        private void CopyXYZClick(object sender, RoutedEventArgs e) {
            e.Handled = true;
            Clipboard.SetDataObject(ColorTransform.ColorToXYZ(SelectedColor));
            Widget.MessageBox.Success("已复制");
        }

        private void CopyLCHClick(object sender, RoutedEventArgs e) {
            e.Handled = true;
            Clipboard.SetDataObject(ColorTransform.ColorToLCH(SelectedColor));
            Widget.MessageBox.Success("已复制");
        }

        private void CopyCMYKClick(object sender, RoutedEventArgs e) {
            e.Handled = true;
            Clipboard.SetDataObject(ColorTransform.ColorToCMYK(SelectedColor));
            Widget.MessageBox.Success("已复制");
        }

        private void CopyLUVClick(object sender, RoutedEventArgs e) {
            e.Handled = true;
            Clipboard.SetDataObject(ColorTransform.ColorToLUV(SelectedColor));
            Widget.MessageBox.Success("已复制");
        }

    }
}
