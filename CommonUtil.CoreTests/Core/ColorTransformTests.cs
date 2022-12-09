using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Media;

namespace CommonUtil.Core.Tests;

[TestClass()]
public class ColorTransformTests {
    private static readonly IReadOnlyList<Color> ColorSource = new Color[] {
        new Color{A = 174, R = 159, G = 163, B = 250},
        new Color{A = 175, R = 58, G = 117, B = 12},
        new Color{A = 1, R = 112, G = 10, B = 221},
        new Color{A = 15, R = 22, G = 186, B = 24},
        new Color{A = 49, R = 81, G = 65, B = 66},
        new Color{A = 59, R = 79, G = 39, B = 81},
        new Color{A = 3, R = 136, G = 7, B = 93},
    };

    [TestMethod()]
    public void ColorToHEX() {
        var expected = new string[] {
            "#AE9FA3FA", "#AF3A750C", "#01700ADD", "#0F16BA18", "#31514142", "#3B4F2751", "#0388075D"
        };
        var actual = ColorSource.Select(ColorTransform.ColorToHEX);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void ColorToRGB() {
        var expected = new string[] {
            "rgb(159,163,250)",
            "rgb(58,117,12)",
            "rgb(112,10,221)",
            "rgb(22,186,24)",
            "rgb(81,65,66)",
            "rgb(79,39,81)",
            "rgb(136,7,93)",
        };
        var actual = ColorSource.Select(ColorTransform.ColorToRGB);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void ColorToRGBA1() {
        var expected = new string[] {
            "rgba(159,163,250,174)",
            "rgba(58,117,12,175)",
            "rgba(112,10,221,1)",
            "rgba(22,186,24,15)",
            "rgba(81,65,66,49)",
            "rgba(79,39,81,59)",
            "rgba(136,7,93,3)",
        };
        var actual = ColorSource.Select(ColorTransform.ColorToRGBA1);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void ColorToRGBA2() {
        var expected = new string[] {
            "rgba(159,163,250,0.68)",
            "rgba(58,117,12,0.69)",
            "rgba(112,10,221,0.00)",
            "rgba(22,186,24,0.06)",
            "rgba(81,65,66,0.19)",
            "rgba(79,39,81,0.23)",
            "rgba(136,7,93,0.01)",
        };
        var actual = ColorSource.Select(ColorTransform.ColorToRGBA2);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void ColorToHSL() {
        var expected = new string[] {
            "hsl(237.36,90.10%,80.20%)",
            "hsl(93.71,81.40%,25.29%)",
            "hsl(269.00,91.34%,45.29%)",
            "hsl(120.73,78.85%,40.78%)",
            "hsl(356.25,10.96%,28.63%)",
            "hsl(297.14,35.00%,23.53%)",
            "hsl(320.00,90.21%,28.04%)",
        };
        var actual = ColorSource.Select(ColorTransform.ColorToHSL);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void ColorToHSV() {
        var expected = new string[] {
            "(237.36,0.36,0.98)",
            "(93.71,0.90,0.46)",
            "(269.00,0.95,0.87)",
            "(120.73,0.88,0.73)",
            "(356.25,0.20,0.32)",
            "(297.14,0.52,0.32)",
            "(320.00,0.95,0.53)",
        };
        var actual = ColorSource.Select(ColorTransform.ColorToHSV);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void ColorToLAB() {
        var expected = new string[] {
            "(69.80,18.85,-43.78)",
            "(43.72,-36.75,45.74)",
            "(35.76,73.52,-82.61)",
            "(66.02,-66.13,62.32)",
            "(29.22,7.09,1.99)",
            "(22.68,25.91,-17.82)",
            "(29.85,54.61,-13.82)",
        };
        var actual = ColorSource.Select(ColorTransform.ColorToLAB);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void ColorToXYZ() {
        var expected = new string[] {
            "(44.65,40.47,95.90)",
            "(8.17,13.65,2.55)",
            "(19.84,8.88,69.08)",
            "(18.05,35.35,6.74)",
            "(6.27,5.92,5.97)",
            "(5.44,3.71,8.21)",
            "(12.21,6.18,10.90)",
        };
        var actual = ColorSource.Select(ColorTransform.ColorToXYZ);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void ColorToLCH() {
        var expected = new string[] {
            "(69.80,47.67,293.30)",
            "(43.72,58.67,128.78)",
            "(35.76,110.59,311.67)",
            "(66.02,90.86,136.70)",
            "(29.22,7.37,15.70)",
            "(22.68,31.45,325.48)",
            "(29.85,56.33,345.80)",
        };
        var actual = ColorSource.Select(ColorTransform.ColorToLCH);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void ColorToCMYK() {
        var expected = new string[] {
            "(0.36,0.35,0.00,0.02)",
            "(0.50,0.00,0.90,0.54)",
            "(0.49,0.95,0.00,0.13)",
            "(0.88,0.00,0.87,0.27)",
            "(0.00,0.20,0.19,0.68)",
            "(0.02,0.52,0.00,0.68)",
            "(0.00,0.95,0.32,0.47)",
        };
        var actual = ColorSource.Select(ColorTransform.ColorToCMYK);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void ColorToLUV() {
        var expected = new string[] {
            "(69.80,-6.99,-73.16)",
            "(43.72,-28.21,50.37)",
            "(35.76,10.43,-114.57)",
            "(66.02,-60.79,78.35)",
            "(29.22,9.10,1.27)",
            "(22.68,16.48,-23.27)",
            "(29.85,60.95,-24.94)",
        };
        var actual = ColorSource.Select(ColorTransform.ColorToLUV);
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void ColorToHEXValues() {
        var expected = new string[] {
            "(174.00, 159.00, 163.00, 250.00)",
            "(175.00, 58.00, 117.00, 12.00)",
            "(1.00, 112.00, 10.00, 221.00)",
            "(15.00, 22.00, 186.00, 24.00)",
            "(49.00, 81.00, 65.00, 66.00)",
            "(59.00, 79.00, 39.00, 81.00)",
            "(3.00, 136.00, 7.00, 93.00)",
        };
        var actual = ColorSource.Select(c => {
            var value = ColorTransform.ColorToHEXValues(c);
            return (value.Item1.ToString("f2"), value.Item2.ToString("f2"), value.Item3.ToString("f2"), value.Item4.ToString("f2")).ToString();
        });
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void ColorToRGBValues() {
        var expected = new string[] {
            "(159.00, 163.00, 250.00)",
            "(58.00, 117.00, 12.00)",
            "(112.00, 10.00, 221.00)",
            "(22.00, 186.00, 24.00)",
            "(81.00, 65.00, 66.00)",
            "(79.00, 39.00, 81.00)",
            "(136.00, 7.00, 93.00)",
        };
        var actual = ColorSource.Select(c => {
            var value = ColorTransform.ColorToRGBValues(c);
            return (value.Item1.ToString("f2"), value.Item2.ToString("f2"), value.Item3.ToString("f2")).ToString();
        });
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void ColorToRGBA1Values() {
        var expected = new string[] {
            "(159.00, 163.00, 250.00, 174.00)",
            "(58.00, 117.00, 12.00, 175.00)",
            "(112.00, 10.00, 221.00, 1.00)",
            "(22.00, 186.00, 24.00, 15.00)",
            "(81.00, 65.00, 66.00, 49.00)",
            "(79.00, 39.00, 81.00, 59.00)",
            "(136.00, 7.00, 93.00, 3.00)",
        };
        var actual = ColorSource.Select(c => {
            var value = ColorTransform.ColorToRGBA1Values(c);
            return (value.Item1.ToString("f2"), value.Item2.ToString("f2"), value.Item3.ToString("f2"), value.Item4.ToString("f2")).ToString();
        });
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void ColorToRGBA2Values() {
        var expected = new string[] {
            "(159.00, 163.00, 250.00, 0.68)",
            "(58.00, 117.00, 12.00, 0.69)",
            "(112.00, 10.00, 221.00, 0.00)",
            "(22.00, 186.00, 24.00, 0.06)",
            "(81.00, 65.00, 66.00, 0.19)",
            "(79.00, 39.00, 81.00, 0.23)",
            "(136.00, 7.00, 93.00, 0.01)",
        };
        var actual = ColorSource.Select(c => {
            var value = ColorTransform.ColorToRGBA2Values(c);
            return (value.Item1.ToString("f2"), value.Item2.ToString("f2"), value.Item3.ToString("f2"), value.Item4.ToString("f2")).ToString();
        });
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void ColorToHSLValues() {
        var expected = new string[] {
            "(237.36, 90.10, 80.20)",
            "(93.71, 81.40, 25.29)",
            "(269.00, 91.34, 45.29)",
            "(120.73, 78.85, 40.78)",
            "(356.25, 10.96, 28.63)",
            "(297.14, 35.00, 23.53)",
            "(320.00, 90.21, 28.04)",
        };
        var actual = ColorSource.Select(c => {
            var value = ColorTransform.ColorToHSLValues(c);
            return (value.Item1.ToString("f2"), value.Item2.ToString("f2"), value.Item3.ToString("f2")).ToString();
        });
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void ColorToHSVValues() {
        var expected = new string[] {
            "(237.36, 36.40, 98.04)",
            "(93.71, 89.74, 45.88)",
            "(269.00, 95.48, 86.67)",
            "(120.73, 88.17, 72.94)",
            "(356.25, 19.75, 31.76)",
            "(297.14, 51.85, 31.76)",
            "(320.00, 94.85, 53.33)",
        };
        var actual = ColorSource.Select(c => {
            var value = ColorTransform.ColorToHSVValues(c);
            return (value.Item1.ToString("f2"), value.Item2.ToString("f2"), value.Item3.ToString("f2")).ToString();
        });
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void ColorToLABValues() {
        var expected = new string[] {
            "(69.80, 18.85, -43.78)",
            "(43.72, -36.75, 45.74)",
            "(35.76, 73.52, -82.61)",
            "(66.02, -66.13, 62.32)",
            "(29.22, 7.09, 1.99)",
            "(22.68, 25.91, -17.82)",
            "(29.85, 54.61, -13.82)",
        };
        var actual = ColorSource.Select(c => {
            var value = ColorTransform.ColorToLABValues(c);
            return (value.Item1.ToString("f2"), value.Item2.ToString("f2"), value.Item3.ToString("f2")).ToString();
        });
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void ColorToXYZValues() {
        var expected = new string[] {
            "(44.65, 40.47, 95.90)",
            "(8.17, 13.65, 2.55)",
            "(19.84, 8.88, 69.08)",
            "(18.05, 35.35, 6.74)",
            "(6.27, 5.92, 5.97)",
            "(5.44, 3.71, 8.21)",
            "(12.21, 6.18, 10.90)",
        };
        var actual = ColorSource.Select(c => {
            var value = ColorTransform.ColorToXYZValues(c);
            return (value.Item1.ToString("f2"), value.Item2.ToString("f2"), value.Item3.ToString("f2")).ToString();
        });
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void ColorToLCHValues() {
        var expected = new string[] {
            "(69.80, 47.67, 293.30)",
            "(43.72, 58.67, 128.78)",
            "(35.76, 110.59, 311.67)",
            "(66.02, 90.86, 136.70)",
            "(29.22, 7.37, 15.70)",
            "(22.68, 31.45, 325.48)",
            "(29.85, 56.33, 345.80)",
        };
        var actual = ColorSource.Select(c => {
            var value = ColorTransform.ColorToLCHValues(c);
            return (value.Item1.ToString("f2"), value.Item2.ToString("f2"), value.Item3.ToString("f2")).ToString();
        });
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void ColorToCMYKValues() {
        var expected = new string[] {
            "(0.36, 0.35, 0.00, 0.02)",
            "(0.50, 0.00, 0.90, 0.54)",
            "(0.49, 0.95, 0.00, 0.13)",
            "(0.88, 0.00, 0.87, 0.27)",
            "(0.00, 0.20, 0.19, 0.68)",
            "(0.02, 0.52, 0.00, 0.68)",
            "(0.00, 0.95, 0.32, 0.47)",
        };
        var actual = ColorSource.Select(c => {
            var value = ColorTransform.ColorToCMYKValues(c);
            return (value.Item1.ToString("f2"), value.Item2.ToString("f2"), value.Item3.ToString("f2"), value.Item4.ToString("f2")).ToString();
        });
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void ColorToLUVValues() {
        var expected = new string[] {
            "(69.80, -6.99, -73.16)",
            "(43.72, -28.21, 50.37)",
            "(35.76, 10.43, -114.57)",
            "(66.02, -60.79, 78.35)",
            "(29.22, 9.10, 1.27)",
            "(22.68, 16.48, -23.27)",
            "(29.85, 60.95, -24.94)",
        };
        var actual = ColorSource.Select(c => {
            var value = ColorTransform.ColorToLUVValues(c);
            return (value.Item1.ToString("f2"), value.Item2.ToString("f2"), value.Item3.ToString("f2")).ToString();
        });
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void HEXToColor_string() {
        var source = new string[] {
            "#AE9FA3FA", "#AF3A750C", "#01700ADD", "#0F16BA18", "#31514142", "#3B4F2751", "#0388075D"
        };
        var expected = source;
        var actual = source.Select(item => ColorTransform.HEXToColor(item)!.Value.ToString());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void RGBToColor_string() {
        var source = new string[] {
            "rgb(159,163,250)",
            "rgb(58,117,12)",
            "rgb(112,10,221)",
            "rgb(22,186,24)",
            "rgb(81,65,66)",
            "rgb(79,39,81)",
            "rgb(136,7,93)",
        };
        var expected = new string[] { "#FF9FA3FA", "#FF3A750C", "#FF700ADD", "#FF16BA18", "#FF514142", "#FF4F2751", "#FF88075D", };
        var actual = source.Select(item => ColorTransform.RGBToColor(item)!.Value.ToString());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void RGBA1ToColor_string() {
        var source = new string[] {
            "rgba(159,163,250,174)",
            "rgba(58,117,12,175)",
            "rgba(112,10,221,1)",
            "rgba(22,186,24,15)",
            "rgba(81,65,66,49)",
            "rgba(79,39,81,59)",
            "rgba(136,7,93,3)",
        };
        var expected = new string[] { "#AE9FA3FA", "#AF3A750C", "#01700ADD", "#0F16BA18", "#31514142", "#3B4F2751", "#0388075D", };
        var actual = source.Select(item => ColorTransform.RGBA1ToColor(item)!.Value.ToString());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void RGBA2ToColor_string() {
        var source = new string[] {
            "rgba(159,163,250,0.68)",
            "rgba(58,117,12,0.69)",
            "rgba(112,10,221,0.00)",
            "rgba(22,186,24,0.06)",
            "rgba(81,65,66,0.19)",
            "rgba(79,39,81,0.23)",
            "rgba(136,7,93,0.01)",
        };
        var expected = new string[] { "#AD9FA3FA", "#AF3A750C", "#00700ADD", "#0F16BA18", "#30514142", "#3A4F2751", "#0288075D", };
        var actual = source.Select(item => ColorTransform.RGBA2ToColor(item)!.Value.ToString());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void HSLToColor_string() {
        var source = new string[] {
            "hsl(237.36,90.10%,80.20%)",
            "hsl(93.71,81.40%,25.29%)",
            "hsl(269.00,91.34%,45.29%)",
            "hsl(120.73,78.85%,40.78%)",
            "hsl(356.25,10.96%,28.63%)",
            "hsl(297.14,35.00%,23.53%)",
            "hsl(320.00,90.21%,28.04%)",
        };
        var expected = new string[] { "#FF9FA3FA", "#FF39740B", "#FF6F0ADC", "#FF15B917", "#FF514142", "#FF4E2751", "#FF88075D", };
        var actual = source.Select(item => ColorTransform.HSLToColor(item)!.Value.ToString());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void HSVToColor_string() {
        var source = new string[] {
            "(237.36,0.36,0.98)",
            "(93.71,0.90,0.46)",
            "(269.00,0.95,0.87)",
            "(120.73,0.88,0.73)",
            "(356.25,0.20,0.32)",
            "(297.14,0.52,0.32)",
            "(320.00,0.95,0.53)",
        };
        var expected = new string[] { "#FF9FA3F9", "#FF39750B", "#FF700BDD", "#FF16BA18", "#FF514142", "#FF4F2751", "#FF87065C", };
        var actual = source.Select(item => ColorTransform.HSVToColor(item)!.Value.ToString());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void LABToColor_string() {
        var source = new string[] {
            "(69.80,18.85,-43.78)",
            "(43.72,-36.75,45.74)",
            "(35.76,73.52,-82.61)",
            "(66.02,-66.13,62.32)",
            "(29.22,7.09,1.99)",
            "(22.68,25.91,-17.82)",
            "(29.85,54.61,-13.82)",
        };
        var expected = new string[] { "#FF9EA3F9", "#FF39740B", "#FF700ADD", "#FF15B917", "#FF514142", "#FF4F2751", "#FF87065C", };
        var actual = source.Select(item => ColorTransform.LABToColor(item)!.Value.ToString());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void XYZToColor_string() {
        var source = new string[] {
            "(44.65,40.47,95.90)",
            "(8.17,13.65,2.55)",
            "(19.84,8.88,69.08)",
            "(18.05,35.35,6.74)",
            "(6.27,5.92,5.97)",
            "(5.44,3.71,8.21)",
            "(12.21,6.18,10.90)",
        };
        var expected = new string[] { "#FF9EA3FA", "#FF39750B", "#FF6F09DD", "#FF15B918", "#FF514042", "#FF4F2750", "#FF88075C", };
        var actual = source.Select(item => ColorTransform.XYZToColor(item)!.Value.ToString());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void LCHToColor_string() {
        var source = new string[] {
            "(69.80,47.67,293.30)",
            "(43.72,58.67,128.78)",
            "(35.76,110.59,311.67)",
            "(66.02,90.86,136.70)",
            "(29.22,7.37,15.70)",
            "(22.68,31.45,325.48)",
            "(29.85,56.33,345.80)",
        };
        var expected = new string[] { "#FF9EA2FA", "#FF39740C", "#FF7009DD", "#FF15B918", "#FF514142", "#FF4F2751", "#FF87065C", };
        var actual = source.Select(item => ColorTransform.LCHToColor(item)!.Value.ToString());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void CMYKToColor_string() {
        var source = new string[] {
            "(0.36,0.35,0.00,0.02)",
            "(0.50,0.00,0.90,0.54)",
            "(0.49,0.95,0.00,0.13)",
            "(0.88,0.00,0.87,0.27)",
            "(0.00,0.20,0.19,0.68)",
            "(0.02,0.52,0.00,0.68)",
            "(0.00,0.95,0.32,0.47)",
        };
        var expected = new string[] { "#FF9FA2F9", "#FF3A750B", "#FF710BDD", "#FF16BA18", "#FF514142", "#FF4F2751", "#FF87065B", };
        var actual = source.Select(item => ColorTransform.CMYKToColor(item)!.Value.ToString());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void LUVToColor_string() {
        var source = new string[] {
            "(69.80,-6.99,-73.16)",
            "(43.72,-28.21,50.37)",
            "(35.76,10.43,-114.57)",
            "(66.02,-60.79,78.35)",
            "(29.22,9.10,1.27)",
            "(22.68,16.48,-23.27)",
            "(29.85,60.95,-24.94)",
        };
        var expected = new string[] { "#FF9FA2FA", "#FF39740B", "#FF700ADD", "#FF15B917", "#FF514142", "#FF4F2750", "#FF87065C", };
        var actual = source.Select(item => ColorTransform.LUVToColor(item)!.Value.ToString());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void HEXToColor_double() {
        var source = new (double, double, double, double)[] {
            (159,163,250,174),
            (58,117,12,175),
            (112,10,221,1),
            (22,186,24,15),
            (81,65,66,49),
            (79,39,81,59),
            (136,7,93,3),
        };
        var expected = new string[] { "#AE9FA3FA", "#AF3A750C", "#01700ADD", "#0F16BA18", "#31514142", "#3B4F2751", "#0388075D", };
        var actual = source.Select(item => ColorTransform.HEXToColor(item.Item4, item.Item1, item.Item2, item.Item3)!.Value.ToString());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void RGBToColor_double() {
        var source = new (double, double, double)[] {
            (159,163,250),
            (58,117,12),
            (112,10,221),
            (22,186,24),
            (81,65,66),
            (79,39,81),
            (136,7,93),
        };
        var expected = new string[] { "#FF9FA3FA", "#FF3A750C", "#FF700ADD", "#FF16BA18", "#FF514142", "#FF4F2751", "#FF88075D", };
        var actual = source.Select(item => ColorTransform.RGBToColor(item.Item1, item.Item2, item.Item3)!.Value.ToString());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void RGBA1ToColor_double() {
        var source = new (double, double, double, double)[] {
            (159,163,250,174),
            (58,117,12,175),
            (112,10,221,1),
            (22,186,24,15),
            (81,65,66,49),
            (79,39,81,59),
            (136,7,93,3),
        };
        var expected = new string[] { "#FAAE9FA3", "#0CAF3A75", "#DD01700A", "#180F16BA", "#42315141", "#513B4F27", "#5D038807", };
        var actual = source.Select(item => ColorTransform.RGBA1ToColor(item.Item4, item.Item1, item.Item2, item.Item3)!.Value.ToString());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void RGBA2ToColor_double() {
        var source = new (double, double, double, double)[] {
            (159,163,250,0.68),
            (58,117,12,0.69),
            (112,10,221,0.00),
            (22,186,24,0.06),
            (81,65,66,0.19),
            (79,39,81,0.23),
            (136,7,93,0.01),
        };
        var expected = new string[] { "#06009FA3", "#F4003A75", "#2300700A", "#E80016BA", "#BE005141", "#AF004F27", "#A3008807", };
        var actual = source.Select(item => ColorTransform.RGBA2ToColor(item.Item4, item.Item1, item.Item2, item.Item3)!.Value.ToString());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void HSLToColor_double() {
        var source = new (double, double, double)[] {
            (237.36,0.90,0.80),
            (93.71,0.81,0.25),
            (269.00,0.91,0.45),
            (120.73,0.78,0.40),
            (356.25,0.10,0.28),
            (297.14,0.35,0.23),
            (320.00,0.90,0.28),
        };
        var expected = new string[] { "#FF020202", "#FF000000", "#FF010101", "#FF010101", "#FF000000", "#FF000000", "#FF000000", };
        var actual = source.Select(item => ColorTransform.HSLToColor(item.Item1, item.Item2, item.Item3)!.Value.ToString());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void HSVToColor_double() {
        var source = new (double, double, double)[] {
            (237.36,0.36,0.98),
            (93.71,0.90,0.46),
            (269.00,0.95,0.87),
            (120.73,0.88,0.73),
            (356.25,0.20,0.32),
            (297.14,0.52,0.32),
            (320.00,0.95,0.53),
        };
        var expected = new string[] { "#FF020202", "#FF010101", "#FF020202", "#FF010101", "#FF000000", "#FF000000", "#FF010101", };
        var actual = source.Select(item => ColorTransform.HSVToColor(item.Item1, item.Item2, item.Item3)!.Value.ToString());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void LABToColor_double() {
        var source = new (double, double, double)[] {
            (69.80,18.85,-43.78),
            (43.72,-36.75,45.74),
            (35.76,73.52,-82.61),
            (66.02,-66.13,62.32),
            (29.22,7.09,1.99),
            (22.68,25.91,-17.82),
            (29.85,54.61,-13.82),
        };
        var expected = new string[] { "#FF9EA3F9", "#FF39740B", "#FF700ADD", "#FF15B917", "#FF514142", "#FF4F2751", "#FF87065C", };
        var actual = source.Select(item => ColorTransform.LABToColor(item.Item1, item.Item2, item.Item3)!.Value.ToString());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void XYZToColor_double() {
        var source = new (double, double, double)[] {
            (44.65,40.47,95.90),
            (8.17,13.65,2.55),
            (19.84,8.88,69.08),
            (18.05,35.35,6.74),
            (6.27,5.92,5.97),
            (5.44,3.71,8.21),
            (12.21,6.18,10.90),
        };
        var expected = new string[] { "#FF9EA3FA", "#FF39750B", "#FF6F09DD", "#FF15B918", "#FF514042", "#FF4F2750", "#FF88075C", };
        var actual = source.Select(item => ColorTransform.XYZToColor(item.Item1, item.Item2, item.Item3)!.Value.ToString());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void LCHToColor_double() {
        var source = new (double, double, double)[] {
            (69.80,47.67,293.30),
            (43.72,58.67,128.78),
            (35.76,110.59,311.67),
            (66.02,90.86,136.70),
            (29.22,7.37,15.70),
            (22.68,31.45,325.48),
            (29.85,56.33,345.80),
        };
        var expected = new string[] { "#FF9EA2FA", "#FF39740C", "#FF7009DD", "#FF15B918", "#FF514142", "#FF4F2751", "#FF87065C", };
        var actual = source.Select(item => ColorTransform.LCHToColor(item.Item1, item.Item2, item.Item3)!.Value.ToString());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void CMYKToColor_double() {
        var source = new (double, double, double, double)[] {
            (0.36,0.35,0.00,0.02),
            (0.50,0.00,0.90,0.54),
            (0.49,0.95,0.00,0.13),
            (0.88,0.00,0.87,0.27),
            (0.00,0.20,0.19,0.68),
            (0.02,0.52,0.00,0.68),
            (0.00,0.95,0.32,0.47),
        };
        var expected = new string[] { "#FF9FA2F9", "#FF3A750B", "#FF710BDD", "#FF16BA18", "#FF514142", "#FF4F2751", "#FF87065B", };
        var actual = source.Select(item => ColorTransform.CMYKToColor(item.Item1, item.Item2, item.Item3, item.Item4)!.Value.ToString());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }

    [TestMethod()]
    public void LUVToColor_double() {
        var source = new (double, double, double)[] {
            (69.80,-6.99,-73.16),
            (43.72,-28.21,50.37),
            (35.76,10.43,-114.57),
            (66.02,-60.79,78.35),
            (29.22,9.10,1.27),
            (22.68,16.48,-23.27),
            (29.85,60.95,-24.94),
        };
        var expected = new string[] { "#FF9FA2FA", "#FF39740B", "#FF700ADD", "#FF15B917", "#FF514142", "#FF4F2750", "#FF87065C", };
        var actual = source.Select(item => ColorTransform.LUVToColor(item.Item1, item.Item2, item.Item3)!.Value.ToString());
        Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
    }
}