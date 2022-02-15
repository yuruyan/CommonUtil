using ColorMine.ColorSpaces;
using System;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace CommonUtil.Core {
    public class ColorTransform {
        private struct ThreeArgs {
            public double First { get; set; }
            public double Second { get; set; }
            public double Third { get; set; }

            public override string ToString() {
                return $"{{{nameof(First)}={First.ToString()}, {nameof(Second)}={Second.ToString()}, {nameof(Third)}={Third.ToString()}}}";
            }
        }

        private struct FourArgs {
            public double First { get; set; }
            public double Second { get; set; }
            public double Third { get; set; }
            public double Fourth { get; set; }

            public override string ToString() {
                return $"{{{nameof(First)}={First.ToString()}, {nameof(Second)}={Second.ToString()}, {nameof(Third)}={Third.ToString()}, {nameof(Fourth)}={Fourth.ToString()}}}";
            }
        }

        /// <summary>
        /// 解析 3 个参数
        /// </summary>
        /// <param name="input"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        /// <exception cref="Exception">未匹配</exception>
        /// <exception cref="FormatException">解析为数字失败</exception>
        /// <exception cref="OverflowException"></exception>
        private static ThreeArgs ParseThreeParameters(string input, Regex regex) {
            var match = regex.Match(input);
            if (!match.Success) {
                throw new Exception("input doesn't match " + regex);
            }
            var args = new ThreeArgs {
                First = double.Parse(match.Groups[1].Value),
                Second = double.Parse(match.Groups[2].Value),
                Third = double.Parse(match.Groups[3].Value),
            };
            return args;
        }

        /// <summary>
        /// 解析 4 个参数
        /// </summary>
        /// <param name="input"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        /// <exception cref="Exception">未匹配</exception>
        /// <exception cref="FormatException">解析为数字失败</exception>
        /// <exception cref="OverflowException"></exception>
        private static FourArgs ParseFourParameters(string input, Regex regex) {
            var match = regex.Match(input);
            if (!match.Success) {
                throw new Exception("input doesn't match " + regex);
            }
            var args = new FourArgs {
                First = double.Parse(match.Groups[1].Value),
                Second = double.Parse(match.Groups[2].Value),
                Third = double.Parse(match.Groups[3].Value),
                Fourth = double.Parse(match.Groups[4].Value),
            };
            return args;
        }

        private static readonly Regex RGBRegex = CompileThreeParametersRegex("rgb");
        private static readonly Regex RGBARegex = CompileFourParametersRegex("rgba");
        private static readonly Regex HSLRegex = new(@"hsl *\( *([\d\.\-]+) *, *([\d\.\-]+) *% *, *([\d\.\-]+) *% *\)", RegexOptions.IgnoreCase);
        private static readonly Regex ThreeColorsRegex = CompileThreeParametersRegex("");
        private static readonly Regex FourColorsRegex = CompileFourParametersRegex("");

        /// <summary>
        /// 编译 3 个参数正则
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        private static Regex CompileThreeParametersRegex(string prefix) {
            return new Regex(prefix + @" *\( *([\d\.\-]+) *, *([\d\.\-]+) *, *([\d\.\-]+) *\)", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 编译 4 个参数正则
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        private static Regex CompileFourParametersRegex(string prefix) {
            return new Regex(prefix + @" *\( *([\d\.\-]+) *, *([\d\.\-]+) *, *([\d\.\-]+) *, *([\d\.\-]+) *\)", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Color 转 RGB
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private static Rgb GetRgb(Color color) {
            return new Rgb() { R = color.R, G = color.G, B = color.B };
        }

        /// <summary>
        /// 转 Color
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static Color GetColor(ThreeArgs args) {
            return new Color() {
                R = (byte)args.First,
                G = (byte)args.Second,
                B = (byte)args.Third,
                A = 255
            };
        }

        /// <summary>
        /// 转 Color
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static Color GetColor(FourArgs args) {
            return new Color() {
                R = (byte)args.First,
                G = (byte)args.Second,
                B = (byte)args.Third,
                A = (byte)args.Fourth,
            };
        }

        /// <summary>
        /// 转 Color
        /// </summary>
        /// <param name="rgb"></param>
        /// <returns></returns>
        private static Color GetColor(Rgb rgb) {
            return new Color() {
                R = (byte)rgb.R,
                G = (byte)rgb.G,
                B = (byte)rgb.B,
                A = 255,
            };
        }

        public static string ColorToHEX(Color color) {
            return color.ToString();
        }

        public static string ColorToRGB(Color color) {
            return $"rgb({color.R},{color.G},{color.B})";
        }

        public static string ColorToRGBA1(Color color) {
            return $"rgba({color.R},{color.G},{color.B},{color.A})";
        }

        public static string ColorToRGBA2(Color color) {
            return $"rgba({color.R},{color.G},{color.B},{color.A / (double)255:f2})";
        }

        public static string ColorToHSL(Color color) {
            Hsl hsl = GetRgb(color).To<Hsl>();
            return $"hsl({hsl.H:f2},{hsl.S * 100:f2}%,{hsl.L * 100:f2}%)";
        }

        public static string ColorToHSV(Color color) {
            Hsv hsv = GetRgb(color).To<Hsv>();
            return $"({hsv.H:f2},{hsv.S:f2},{hsv.V:f2})";
        }

        public static string ColorToLAB(Color color) {
            Lab lab = GetRgb(color).To<Lab>();
            return $"({lab.L:f2},{lab.A:f2},{lab.B:f2})";
        }

        public static string ColorToXYZ(Color color) {
            Xyz xyz = GetRgb(color).To<Xyz>();
            return $"({xyz.X:f2},{xyz.Y:f2},{xyz.Z:f2})";
        }

        public static string ColorToLCH(Color color) {
            Lch lch = GetRgb(color).To<Lch>();
            return $"({lch.L:f2},{lch.C:f2},{lch.H:f2})";
        }

        public static string ColorToCMYK(Color color) {
            Cmyk cmyk = GetRgb(color).To<Cmyk>();
            return $"({cmyk.C:f2},{cmyk.M:f2},{cmyk.Y:f2},{cmyk.K:f2})";
        }

        public static string ColorToLUV(Color color) {
            Luv luv = GetRgb(color).To<Luv>();
            return $"({luv.L:f2},{luv.U:f2},{luv.V:f2})";
        }

        public static Color? HEXToColor(string hex) {
            try {
                return (Color)ColorConverter.ConvertFromString(hex);
            } catch {
                return null;
            }
        }

        public static Color? RGBToColor(string input) {
            try {
                return GetColor(ParseThreeParameters(input, RGBRegex));
            } catch {
                return null;
            }
        }

        public static Color? RGBA1ToColor(string input) {
            try {
                return GetColor(ParseFourParameters(input, RGBARegex));
            } catch {
                return null;
            }
        }

        public static Color? RGBA2ToColor(string input) {
            try {
                var args = ParseFourParameters(input, RGBARegex);
                args.Fourth *= 255;
                return GetColor(args);
            } catch {
                return null;
            }
        }

        public static Color? HSLToColor(string input) {
            try {
                var args = ParseThreeParameters(input, HSLRegex);
                args.Second /= 100;
                args.Third /= 100;
                return GetColor(new Hsl() { H = args.First, S = args.Second, L = args.Third}.To<Rgb>());
            } catch {
                return null;
            }
        }

        public static Color? HSVToColor(string input) {
            try {
                var args = ParseThreeParameters(input, ThreeColorsRegex);
                return GetColor(new Hsv() { H = args.First, S = args.Second, V = args.Third }.To<Rgb>());
            } catch {
                return null;
            }
        }

        public static Color? LABToColor(string input) {
            try {
                var args = ParseThreeParameters(input, ThreeColorsRegex);
                return GetColor(new Lab() { L = args.First, A = args.Second, B = args.Third }.To<Rgb>());
            } catch {
                return null;
            }
        }

        public static Color? XYZToColor(string input) {
            try {
                var args = ParseThreeParameters(input, ThreeColorsRegex);
                return GetColor(new Xyz() { X = args.First, Y = args.Second, Z = args.Third }.To<Rgb>());
            } catch {
                return null;
            }
        }

        public static Color? LCHToColor(string input) {
            try {
                var args = ParseThreeParameters(input, ThreeColorsRegex);
                return GetColor(new Lch() { L = args.First, C = args.Second, H = args.Third }.To<Rgb>());
            } catch {
                return null;
            }
        }

        public static Color? CMYKToColor(string input) {
            try {
                var args = ParseFourParameters(input, FourColorsRegex);
                return GetColor(new Cmyk() {C  = args.First, M = args.Second, Y = args.Third, K = args.Fourth }.To<Rgb>());
            } catch {
                return null;
            }
        }

        public static Color? LUVToColor(string input) {
            try {
                var args = ParseThreeParameters(input, ThreeColorsRegex);
                return GetColor(new Luv() { L = args.First,U = args.Second, V = args.Third }.To<Rgb>());
            } catch {
                return null;
            }
        }
    }
}
