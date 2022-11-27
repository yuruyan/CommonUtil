using ColorMine.ColorSpaces;
using CommonUITools.Utils;
using System;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace CommonUtil.Core;

public static partial class ColorTransform {
    private static readonly Regex RGBRegex = CompileThreeParametersRegex("rgb");
    private static readonly Regex RGBARegex = CompileFourParametersRegex("rgba");
    private static readonly Regex HSLRegex = GetHSLRegex();
    private static readonly Regex ThreeColorsRegex = CompileThreeParametersRegex("");
    private static readonly Regex FourColorsRegex = CompileFourParametersRegex("");

    [GeneratedRegex("hsl *\\( *([\\d\\.\\-]+) *, *([\\d\\.\\-]+) *% *, *([\\d\\.\\-]+) *% *\\)", RegexOptions.IgnoreCase, "zh-CN")]
    private static partial Regex GetHSLRegex();

    /// <summary>
    /// 解析 3 个参数
    /// </summary>
    /// <param name="input"></param>
    /// <param name="regex"></param>
    /// <returns></returns>
    /// <exception cref="Exception">未匹配</exception>
    /// <exception cref="FormatException">解析为数字失败</exception>
    /// <exception cref="OverflowException"></exception>
    private static ValueTuple<double, double, double> ParseThreeParameters(string input, Regex regex) {
        var match = regex.Match(input);
        if (!match.Success) {
            throw new Exception("input doesn't match " + regex);
        }
        return new(
            double.Parse(match.Groups[1].Value),
            double.Parse(match.Groups[2].Value),
            double.Parse(match.Groups[3].Value)
        );
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
    private static ValueTuple<double, double, double, double> ParseFourParameters(string input, Regex regex) {
        var match = regex.Match(input);
        if (!match.Success) {
            throw new Exception("input doesn't match " + regex);
        }
        return new(
            double.Parse(match.Groups[1].Value),
            double.Parse(match.Groups[2].Value),
            double.Parse(match.Groups[3].Value),
            double.Parse(match.Groups[4].Value)
        );
    }
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
    private static Color GetColor(ValueTuple<double, double, double> args) {
        return new Color() {
            R = (byte)args.Item1,
            G = (byte)args.Item2,
            B = (byte)args.Item3,
            A = 255
        };
    }

    /// <summary>
    /// 转 Color
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    private static Color GetColor(ValueTuple<double, double, double, double> args) {
        return new Color() {
            R = (byte)args.Item1,
            G = (byte)args.Item2,
            B = (byte)args.Item3,
            A = (byte)args.Item4,
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

    public static ValueTuple<double, double, double, double> ColorToHEXValues(Color color) {
        return new(color.A, color.R, color.G, color.B);
    }

    public static ValueTuple<double, double, double> ColorToRGBValues(Color color) {
        return new(color.R, color.G, color.B);
    }

    public static ValueTuple<double, double, double, double> ColorToRGBA1Values(Color color) {
        return new(color.R, color.G, color.B, color.A);
    }

    public static ValueTuple<double, double, double, double> ColorToRGBA2Values(Color color) {
        return new(color.R, color.G, color.B, color.A / (double)255);
    }

    public static ValueTuple<double, double, double> ColorToHSLValues(Color color) {
        var hsl = GetRgb(color).To<Hsl>();
        return new(hsl.H, hsl.S * 100, hsl.L * 100);
    }

    public static ValueTuple<double, double, double> ColorToHSVValues(Color color) {
        var hsv = GetRgb(color).To<Hsv>();
        return new(hsv.H, hsv.S * 100, hsv.V * 100);
    }

    public static ValueTuple<double, double, double> ColorToLABValues(Color color) {
        var lab = GetRgb(color).To<Lab>();
        return new(lab.L, lab.A, lab.B);
    }

    public static ValueTuple<double, double, double> ColorToXYZValues(Color color) {
        var xyz = GetRgb(color).To<Xyz>();
        return new(xyz.X, xyz.Y, xyz.Z);
    }

    public static ValueTuple<double, double, double> ColorToLCHValues(Color color) {
        var lch = GetRgb(color).To<Lch>();
        return new(lch.L, lch.C, lch.H);
    }

    public static ValueTuple<double, double, double, double> ColorToCMYKValues(Color color) {
        var cmyk = GetRgb(color).To<Cmyk>();
        return new(cmyk.C, cmyk.M, cmyk.Y, cmyk.K);
    }

    public static ValueTuple<double, double, double> ColorToLUVValues(Color color) {
        var luv = GetRgb(color).To<Luv>();
        return new(luv.L, luv.U, luv.V);
    }

    public static Color? HEXToColor(string hex)
        => TaskUtils.Try(() => (Color)ColorConverter.ConvertFromString(hex));

    public static Color? RGBToColor(string input)
        => TaskUtils.Try(() => GetColor(ParseThreeParameters(input, RGBRegex)));

    public static Color? RGBA1ToColor(string input)
        => TaskUtils.Try(() => GetColor(ParseFourParameters(input, RGBARegex)));

    public static Color? RGBA2ToColor(string input)
        => TaskUtils.Try(() => {
            var args = ParseFourParameters(input, RGBARegex);
            args.Item4 *= 255;
            return GetColor(args);
        });

    public static Color? HSLToColor(string input)
        => TaskUtils.Try(() => {
            var args = ParseThreeParameters(input, HSLRegex);
            args.Item2 /= 100;
            args.Item3 /= 100;
            return GetColor(new Hsl() { H = args.Item1, S = args.Item2, L = args.Item3 }.To<Rgb>());
        });

    public static Color? HSVToColor(string input)
        => TaskUtils.Try(() => {
            var args = ParseThreeParameters(input, ThreeColorsRegex);
            return GetColor(new Hsv() { H = args.Item1, S = args.Item2, V = args.Item3 }.To<Rgb>());
        });

    public static Color? LABToColor(string input)
        => TaskUtils.Try(() => {
            var args = ParseThreeParameters(input, ThreeColorsRegex);
            return GetColor(new Lab() { L = args.Item1, A = args.Item2, B = args.Item3 }.To<Rgb>());
        });

    public static Color? XYZToColor(string input)
        => TaskUtils.Try(() => {
            var args = ParseThreeParameters(input, ThreeColorsRegex);
            return GetColor(new Xyz() { X = args.Item1, Y = args.Item2, Z = args.Item3 }.To<Rgb>());
        });

    public static Color? LCHToColor(string input) =>
        TaskUtils.Try(() => {
            var args = ParseThreeParameters(input, ThreeColorsRegex);
            return GetColor(new Lch() { L = args.Item1, C = args.Item2, H = args.Item3 }.To<Rgb>());
        });

    public static Color? CMYKToColor(string input)
        => TaskUtils.Try(() => {
            var args = ParseFourParameters(input, FourColorsRegex);
            return GetColor(new Cmyk() { C = args.Item1, M = args.Item2, Y = args.Item3, K = args.Item4 }.To<Rgb>());
        });

    public static Color? LUVToColor(string input)
        => TaskUtils.Try(() => {
            var args = ParseThreeParameters(input, ThreeColorsRegex);
            return GetColor(new Luv() { L = args.Item1, U = args.Item2, V = args.Item3 }.To<Rgb>());
        });

    public static Color? HEXToColor(double a, double r, double g, double b)
        => TaskUtils.Try(() => new Color() { R = (byte)r, G = (byte)g, B = (byte)b, A = (byte)a });

    public static Color? RGBToColor(double r, double g, double b)
        => TaskUtils.Try(() => new Color() { R = (byte)r, G = (byte)g, B = (byte)b, A = 255 });

    public static Color? RGBA1ToColor(double r, double g, double b, double a)
        => TaskUtils.Try(() => new Color() { R = (byte)r, G = (byte)g, B = (byte)b, A = (byte)a });

    public static Color? RGBA2ToColor(double r, double g, double b, double a)
        => TaskUtils.Try(() => new Color() { R = (byte)r, G = (byte)g, B = (byte)b, A = (byte)(a * 255) });

    public static Color? HSLToColor(double h, double s, double l)
        => TaskUtils.Try(() => GetColor(new Hsl() { H = h, S = s / 100, L = l / 100 }.To<Rgb>()));

    public static Color? HSVToColor(double h, double s, double v)
        => TaskUtils.Try(() => GetColor(new Hsv() { H = h, S = s / 100, V = v / 100 }.To<Rgb>()));

    public static Color? LABToColor(double l, double a, double b)
        => TaskUtils.Try(() => GetColor(new Lab() { L = l, A = a, B = b }.To<Rgb>()));

    public static Color? XYZToColor(double x, double y, double z)
        => TaskUtils.Try(() => GetColor(new Xyz() { X = x, Y = y, Z = z }.To<Rgb>()));

    public static Color? LCHToColor(double l, double c, double h)
        => TaskUtils.Try(() => GetColor(new Lch() { L = l, C = c, H = h }.To<Rgb>()));

    public static Color? CMYKToColor(double c, double m, double y, double k)
        => TaskUtils.Try(() => GetColor(new Cmyk() { C = c, M = m, Y = y, K = k }.To<Rgb>()));

    public static Color? LUVToColor(double l, double u, double v)
        => TaskUtils.Try(() => GetColor(new Luv() { L = l, U = u, V = v }.To<Rgb>()));
}
