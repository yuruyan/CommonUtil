using System.Linq;
using System.Text;

namespace CommonUtil.Core;

public class OrdinalTextGenerator {
    public enum OrdinalType {
        Number,
        Alphabet
    }

    private const string LeftBracketReplacement = "\0\u0001\u0020\u0300\u1234\uffff\u1234\u0300\u0020\u0001\0";
    private const string RightBracketReplacement = "\uffff\u0001\u0020\u0300\u1234\0\u1234\u0300\u0020\u0001\uffff";
    private static readonly char[] Data = {
        'Z','A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y'
    };

    /// <summary>
    /// 转换为字母
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    private static string ConvertToAlphabet(int num) {
        var sb = new StringBuilder();
        while (num > 0) {
            int mod = num % 26;
            sb.Append(Data[mod]);
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
        }
        return data;
    }
}
