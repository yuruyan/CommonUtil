namespace CommonUtil.Core;

public static class BaseConversion {
    private const string BaseCharacters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private static readonly List<string> PrefixCharacterList = new();

    static BaseConversion() {
        for (var i = 2; i <= 36; i++) {
            PrefixCharacterList.Add(BaseCharacters[..i]);
        }
    }

    /// <summary>
    /// 将十进制转换为指定的进制
    /// </summary>
    /// <param name="val">十进制值</param>
    /// <param name="targetRadix">目标进制</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">基数错误</exception>
    public static string ConvertFromDecimal(ulong val, int targetRadix = 10) {
        if (targetRadix <= 1 || targetRadix > 36) {
            throw new ArgumentException("基数错误");
        }
        var numList = new List<char>();
        do {
            ulong y = val % (ulong)targetRadix;
            numList.Add(BaseCharacters[Convert.ToInt32(y)]);
            val = Convert.ToUInt64(Math.Floor(val / (double)targetRadix));
        } while (val > 0);
        numList.Reverse();
        return string.Join("", numList);
    }

    /// <summary>
    /// 将任意进制转化为十进制
    /// </summary>
    /// <param name="value">任意进制的字任串</param>
    /// <param name="sourceRadix">源字符串的进制</param>
    /// <returns></returns>
    /// <exception cref="FormatException">格式有误</exception>
    /// <exception cref="ArgumentException">基数错误</exception>
    public static ulong ConvertToDecimal(string value, int sourceRadix = 10) {
        ulong r = 0;
        if (sourceRadix is <= 1 or > 36) {
            throw new ArgumentException("基数错误");
        }
        var charArray = value.Trim().ToUpperInvariant().ToCharArray();
        // 检查是否是有效进制数字
        if (charArray.Except(PrefixCharacterList[sourceRadix - 2]).Any()) {
            throw new FormatException("格式有误");
        }
        Array.Reverse(charArray);
        for (int i = 0; i < charArray.Length; i++) {
            var ch = charArray[i];
            int index = ch switch {
                >= '0' and <= '9' => ch - 48,
                >= 'A' and <= 'Z' => ch - 55,
                _ => throw new FormatException("格式有误")
            };
            r += Convert.ToUInt64(index * Math.Pow(sourceRadix, i));
        }
        return r;
    }
}
