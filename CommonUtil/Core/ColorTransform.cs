using ColorMine.ColorSpaces;
using System.Windows.Media;

namespace CommonUtil.Core {
    public class ColorTransform {
        public static string ColorToHex(Color color) {
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

        /// <summary>
        /// Color 转 RGB
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private static Rgb GetRgb(Color color) {
            return new Rgb() { R = color.R, G = color.G, B = color.B };
        }

        public static Color? HexToColor(string hex) {
            try {
                return (Color)ColorConverter.ConvertFromString(hex);
            } catch {
                return null;
            }
        }

    }
}
