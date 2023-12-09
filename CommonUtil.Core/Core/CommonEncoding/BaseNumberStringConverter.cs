namespace CommonUtil.Core;

/// <summary>
/// 进制数转字符串
/// </summary>
static class BaseNumberStringConverter {
    private const int MaxOctalNumber = 511;
    private const int MaxHexidecimalASCIINumber = 255;

    /// <summary>
    /// 字符串转 8 进制数字
    /// </summary>
    /// <param name="value">字符串</param>
    /// <param name="padding">是否补全左侧为 0</param>
    /// <returns></returns>
    /// <exception cref="FormatException">字符串中含有超过 <see cref="MaxOctalNumber"/> 的字符</exception>
    public static string ConvertToOctalNumber(string value, bool padding = false) {
        if (!CheckCharacterInRange(value, MaxOctalNumber)) {
            throw new FormatException("Invalid string conversion");
        }

        var sb = new StringBuilder(value.Length << 2);
        if (padding) {
            foreach (char c in value) {
                sb.Append('\\').Append(
                    Convert.ToString(c, 8).PadLeft(3, '0')
                );
            }
        } else {
            foreach (char c in value) {
                sb.Append('\\').Append(Convert.ToString(c, 8));
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// ASCII 字符串转 16 进制数字
    /// </summary>
    /// <param name="value">ASCII 字符串</param>
    /// <param name="padding">是否补全左侧为 0</param>
    /// <returns></returns>
    /// <exception cref="FormatException">字符串中含有超过 <see cref="MaxHexidecimalASCIINumber"/> 的字符</exception>
    public static string ConvertToHexASCIINumber(string value, bool padding = false) {
        if (!CheckCharacterInRange(value, MaxHexidecimalASCIINumber)) {
            throw new FormatException("Invalid string conversion");
        }

        var sb = new StringBuilder(value.Length << 2);
        if (padding) {
            foreach (char c in value) {
                sb.Append("\\x").Append(
                    Convert.ToString(c, 16).PadLeft(2, '0')
                );
            }
        } else {
            foreach (char c in value) {
                sb.Append("\\x").Append(Convert.ToString(c, 16));
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// Unicode 字符串转 16 进制数字
    /// </summary>
    /// <param name="value">Unicode 字符串</param>
    /// <param name="padding">是否补全左侧为 0</param>
    /// <returns></returns>
    public static string ConvertToHexUnicodeNumber(string value, bool padding = false) {
        var sb = new StringBuilder(value.Length * 6);
        if (padding) {
            foreach (char c in value) {
                sb.Append("\\u").Append(
                    Convert.ToString(c, 16).PadLeft(4, '0')
                );
            }
        } else {
            foreach (char c in value) {
                sb.Append("\\u").Append(Convert.ToString(c, 16));
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// FullUnicode 字符串转 16 进制数字
    /// </summary>
    /// <param name="value">FullUnicode 字符串</param>
    /// <param name="padding">是否补全左侧为 0</param>
    /// <returns></returns>
    public static string ConvertToHexFullUnicodeNumber(string value, bool padding = false) {
        var sb = new StringBuilder(value.Length * 10);
        var data = Encoding.UTF32.GetBytes(value);
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
}