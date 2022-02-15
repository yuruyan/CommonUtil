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
        public static readonly DependencyProperty HexColorProperty = DependencyProperty.Register("HexColor", typeof(string), typeof(ColorTransformView), new PropertyMetadata(""));
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
        public string HexColor {
            get { return (string)GetValue(HexColorProperty); }
            set { SetValue(HexColorProperty, value); }
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
            //DependencyPropertyDescriptor.FromProperty(HexColorProperty, typeof(ColorTransformView)).AddValueChanged(this, HexColorChangedHandler);
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
                    var hexColor = ColorTransform.ColorToHex(color);
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
                        HexColor = hexColor;
                        RGBColor = rGBColor;
                        RGBA1Color = rGBA1Color;
                        RGBA2Color = rGBA2Color;
                        HSLColor = hSLColor;
                        HSVColor = hSVColor;
                        LABColor = lABColor;
                        XYZColor = xYZColor;
                        LCHColor = lCHColor;
                        CMYKColor = cMYKColor;
                        LUVColor = lUVColor;
                    });
                }
            });
        }

        private void HexColorChangedHandler(object? sender, EventArgs e) {
            if (ColorTransform.HexToColor(HexColor) is Color color) {
                // 不能和 SelectedColor 比较
                if (CompareColor != color) {
                    SelectedColor = color;
                }
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
    }
}
