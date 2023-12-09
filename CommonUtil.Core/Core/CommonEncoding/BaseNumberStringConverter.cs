namespace CommonUtil.Core;

/// <summary>
/// 进制数转字符串
/// </summary>
public static partial class BaseNumberStringConverter {
    private const int MaxOctalNumber = 511;
    private const int MaxHexidecimalASCIINumber = 255;

#if NET7_0_OR_GREATER
    private static readonly Regex OctalNumberRegex = GetOctalNumberRegex();
    private static readonly Regex HexidecimalASCIINumberRegex = GetHexidecimalASCIINumberRegex();
    private static readonly Regex HexidecimalUnicodeNumberRegex = GetHexidecimalUnicodeNumberRegex();
    private static readonly Regex HexidecimalFullUnicodeNumberRegex = GetHexidecimalFullUnicodeNumberRegex();
#else
    private static readonly Regex OctalNumberRegex = new(@"\\(?<number>\d{1,3})");
    private static readonly Regex HexidecimalASCIINumberRegex = new(@"\\x(?<number>[a-f0-9]{1,2})");
    private static readonly Regex HexidecimalUnicodeNumberRegex = new(@"\\u(?<number>[a-f0-9]{1,4})");
    private static readonly Regex HexidecimalFullUnicodeNumberRegex = new(@"\\U(?<number>[a-f0-9]{1,8})");
#endif

    /// <summary>
    /// 字符串转 8 进制数字
    /// </summary>
    /// <param name="text">字符串</param>
    /// <param name="padding">是否补全左侧为 0</param>
    /// <returns></returns>
    /// <exception cref="FormatException">字符串中含有超过 <see cref="MaxOctalNumber"/> 的字符</exception>
    public static string ConvertToOctalNumber(string text, bool padding = false) {
        if (!CheckCharacterInRange(text, MaxOctalNumber)) {
            throw new FormatException("Invalid string conversion");
        }

        var sb = new StringBuilder(text.Length << 2);
        if (padding) {
            foreach (char c in text) {
                sb.Append('\\').Append(
                    Convert.ToString(c, 8).PadLeft(3, '0')
                );
            }
        } else {
            foreach (char c in text) {
                sb.Append('\\').Append(Convert.ToString(c, 8));
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// 8 进制数字转字符串
    /// </summary>
    /// <param name="number">8 进制数字，如 \123\456</param>
    /// <returns></returns>
    public static string ConvertFromOctalNumber(string number) {
        var values = OctalNumberRegex
            .Matches(number)
            .Select(m => (char)Convert.ToInt32(
                m.Groups["number"].Value, 8
            ));
        return string.Join("", values);
    }

    /// <summary>
    /// ASCII 字符串转 16 进制数字
    /// </summary>
    /// <param name="text">ASCII 字符串</param>
    /// <param name="padding">是否补全左侧为 0</param>
    /// <returns></returns>
    /// <exception cref="FormatException">字符串中含有超过 <see cref="MaxHexidecimalASCIINumber"/> 的字符</exception>
    public static string ConvertToHexASCIINumber(string text, bool padding = false) {
        if (!CheckCharacterInRange(text, MaxHexidecimalASCIINumber)) {
            throw new FormatException("Invalid string conversion");
        }

        var sb = new StringBuilder(text.Length << 2);
        if (padding) {
            foreach (char c in text) {
                sb.Append("\\x").Append(
                    Convert.ToString(c, 16).PadLeft(2, '0')
                );
            }
        } else {
            foreach (char c in text) {
                sb.Append("\\x").Append(Convert.ToString(c, 16));
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// 16 进制数字转字符串
    /// </summary>
    /// <param name="number">16 进制数字，如 \x12\xab</param>
    /// <returns></returns>
    public static string ConvertFromHexASCIINumber(string number) {
        var values = HexidecimalASCIINumberRegex
            .Matches(number)
            .Select(m => (char)Convert.ToInt32(
                m.Groups["number"].Value, 16
            ));
        return string.Join("", values);
    }

    /// <summary>
    /// Unicode 字符串转 16 进制数字
    /// </summary>
    /// <param name="text">Unicode 字符串</param>
    /// <param name="padding">是否补全左侧为 0</param>
    /// <returns></returns>
    public static string ConvertToHexUnicodeNumber(string text, bool padding = false) {
        var sb = new StringBuilder(text.Length * 6);
        if (padding) {
            foreach (char c in text) {
                sb.Append("\\u").Append(
                    Convert.ToString(c, 16).PadLeft(4, '0')
                );
            }
        } else {
            foreach (char c in text) {
                sb.Append("\\u").Append(Convert.ToString(c, 16));
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// 16 进制 Unicode 数字转字符串
    /// </summary>
    /// <param name="number">16 进制 Unicode 数字，如 \u1234\uabcd</param>
    /// <returns></returns>
    public static string ConvertFromHexUnicodeNumber(string number) {
        var values = HexidecimalUnicodeNumberRegex
            .Matches(number)
            .Select(m => (char)Convert.ToInt32(
                m.Groups["number"].Value, 16
            ));
        return string.Join("", values);
    }

    /// <summary>
    /// FullUnicode 字符串转 16 进制数字
    /// </summary>
    /// <param name="text">FullUnicode 字符串</param>
    /// <param name="padding">是否补全左侧为 0</param>
    /// <returns></returns>
    public static string ConvertToHexFullUnicodeNumber(string text, bool padding = false) {
        var sb = new StringBuilder(text.Length * 10);
        var data = Encoding.UTF32.GetBytes(text);
        if (padding) {
            for (int i = 0; i < data.Length; i += 4) {
                sb.Append("\\U").Append(
                    ConvertUTF32ToString(data, i).PadLeft(8, '0')
                );
            }
        } else {
            for (int i = 0; i < data.Length; i += 4) {
                sb.Append("\\U").Append(
                    ConvertUTF32ToString(data, i).TrimStart('0')
                );
            }
        }
        return sb.ToString();

        // 转换 UTF32 为 16 进制字符串
        static string ConvertUTF32ToString(byte[] utf32, int start) {
            var s = new StringBuilder(8);
            for (var i = 3 + start; i >= start; i--) {
                s.Append(Convert.ToString(utf32[i], 16).PadLeft(2, '0'));
            }
            return s.ToString();
        }
    }

    /// <summary>
    /// 16 进制 FullUnicode 数字转字符串
    /// </summary>
    /// <param name="number">16 进制 FullUnicode 数字，如 \U12345\Uabcdef</param>
    /// <returns></returns>
    public static string ConvertFromHexFullUnicodeNumber(string number) {
        var numbers = HexidecimalFullUnicodeNumberRegex
            .Matches(number)
            .Select(m => m.Groups["number"].Value.PadLeft(8, '0'))
            .ToArray();
        var data = new byte[numbers.Length << 2];
        int i = 0;
        foreach (var item in numbers) {
            data[i++] = Convert.ToByte(item[6..8], 16);
            data[i++] = Convert.ToByte(item[4..6], 16);
            data[i++] = Convert.ToByte(item[2..4], 16);
            data[i++] = Convert.ToByte(item[0..2], 16);
        }
        return Encoding.UTF32.GetString(data);
    }

    /// <summary>
    /// 检查字符是否在有效范围内
    /// </summary>
    /// <param name="chars"></param>
    /// <param name="max">字符最大数字</param>
    /// <returns></returns>
    private static bool CheckCharacterInRange(IEnumerable<char> chars, int max) {
        foreach (char c in chars) {
            if (c > max) {
                return false;
            }
        }
        return true;
    }

#if NET7_0_OR_GREATER
    [GeneratedRegex(@"\\(?<number>\d{1,3})")]
    private static partial Regex GetOctalNumberRegex();
    [GeneratedRegex(@"\\x(?<number>[a-f0-9]{1,2})")]
    private static partial Regex GetHexidecimalASCIINumberRegex();
    [GeneratedRegex(@"\\u(?<number>[a-f0-9]{1,4})")]
    private static partial Regex GetHexidecimalUnicodeNumberRegex();
    [GeneratedRegex(@"\\U(?<number>[a-f0-9]{1,8})")]
    private static partial Regex GetHexidecimalFullUnicodeNumberRegex();
#endif
}