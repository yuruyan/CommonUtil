namespace CommonUtil.Core;

public static class OrdinalTextGenerator {
    public enum OrdinalType {
        Number,
        Alphabet,
        ChineseNumber,
        ChineseUpperNumber,
    }

    private const string LeftBracketReplacement = "\0\u0001\u0020\u0300\u1234\uffff\u1234\u0300\u0020\u0001\0";
    private const string RightBracketReplacement = "\uffff\u0001\u0020\u0300\u1234\0\u1234\u0300\u0020\u0001\uffff";
    private static readonly char[] Alphabet = {
        'Z','A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y'
    };
    /// <summary>
    /// 中文数字字符
    /// </summary>
    private static readonly string[] CNNumberString = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
    /// <summary>
    /// 大单位
    /// </summary>
    private static readonly string[] CNBigUnit = { "", "万", "亿", "万亿" };
    /// <summary>
    /// 小单位
    /// </summary>
    private static readonly string[] CNSmallUnit = { "", "十", "百", "千" };
    /// <summary>
    /// 汉字大小写
    /// </summary>
    private static readonly IDictionary<char, char> ChineseUpperCharacterDict = new Dictionary<char, char>() {
        {'零', '零'},
        {'一', '壹'},
        {'二', '贰'},
        {'三', '叁'},
        {'四', '肆'},
        {'五', '伍'},
        {'六', '陆'},
        {'七', '柒'},
        {'八', '捌'},
        {'九', '玖'},
        {'十', '拾'},
        {'百', '佰'},
        {'千', '仟'},
        {'万', '万'},
        {'亿', '亿'},
    };

    /// <summary>
    /// 小单位计算
    /// </summary>
    /// <param name="section">小单位数字</param>
    /// <param name="cnString">拼接的目标字符串</param>
    private static void SectionToChines(int section, ref string cnString) {
        string strIns = string.Empty;
        int unitPos = 0;
        bool zero = true;
        while (section > 0) {
            int v = section % 10;
            if (v == 0) {
                if (!zero) {
                    zero = true;
                    cnString = cnString.Insert(0, CNNumberString[v]);
                }
            } else {
                zero = false;
                strIns = $"{CNNumberString[v]}{CNSmallUnit[unitPos]}";
                cnString = cnString.Insert(0, strIns);
            }
            unitPos++;
            section /= 10;
        }
    }

    /// <summary>
    /// 数字转中文
    /// </summary>
    /// <param name="number">数字</param>
    /// <returns>返回转传成功的字符串</returns>
    private static string ConvertToChinese(int number) {
        int tempNumber = Math.Abs(number);
        if (tempNumber == 0) {
            return "零";
        }

        string result = string.Empty;
        int unitPos = 0;
        bool needZero = false;
        while (tempNumber > 0) {
            string strIns = string.Empty;
            int section = tempNumber % 10000;
            if (needZero) {
                result = result.Insert(0, CNNumberString[0]);
            }
            SectionToChines(section, ref strIns);
            strIns += (section != 0) ? CNBigUnit[unitPos] : CNBigUnit[0];
            result = result.Insert(0, strIns);
            needZero = (section < 1000) && (section > 0);
            tempNumber /= 10000;
            unitPos++;
        }
        if (number < 0) {
            result = $"负{result}";
        }
        return result;
    }

    /// <summary>
    /// 数字转中文大写
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    private static string ConvertToChineseUpperCase(int number) {
        char[] chars = ConvertToChinese(number).ToCharArray();
        for (int i = 0; i < chars.Length; i++) {
            chars[i] = ChineseUpperCharacterDict[chars[i]];
        }
        return string.Join("", chars);
    }

    /// <summary>
    /// 转换为字母
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    private static string ConvertToAlphabet(int num) {
        var sb = new StringBuilder();
        while (num > 0) {
            int mod = num % 26;
            sb.Append(Alphabet[mod]);
            num /= 26;
            if (mod == 0) {
                num--;
            }
        }
        return string.Join("", sb.ToString().Reverse());
    }

    /// <summary>
    /// 生成顺序文本
    /// </summary>
    /// <param name="format"></param>
    /// <param name="startIndex"></param>
    /// <param name="type">数字类型</param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static string[] Generate(string format, int startIndex, OrdinalType type, uint count) {
        format = format.Replace("{{", LeftBracketReplacement)
            .Replace("}}", RightBracketReplacement)
            .Replace("{}", "{0}")
            .Replace(LeftBracketReplacement, "{{")
            .Replace(RightBracketReplacement, "}}");
        string[] data = new string[count];

        if (type == OrdinalType.Number) {
            for (int i = 0; i < data.Length; i++) {
                data[i] = string.Format(format, i + startIndex);
            }
        } else if (type == OrdinalType.Alphabet) {
            for (int i = 0; i < data.Length; i++) {
                data[i] = string.Format(format, ConvertToAlphabet(i + startIndex));
            }
        } else if (type == OrdinalType.ChineseNumber) {
            for (int i = 0; i < data.Length; i++) {
                data[i] = string.Format(format, ConvertToChinese(i + startIndex));
            }
        } else if (type == OrdinalType.ChineseUpperNumber) {
            for (int i = 0; i < data.Length; i++) {
                data[i] = string.Format(format, ConvertToChineseUpperCase(i + startIndex));
            }
        }
        return data;
    }
}
