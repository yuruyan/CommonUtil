using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtil.Core;

public class TextTool {
    /// <summary>
    /// 文本去重
    /// </summary>
    /// <param name="text"></param>
    /// <param name="splitSymbol">分隔符</param>
    /// <param name="mergeSymbol">合并符</param>
    /// <param name="trim">移除元素首尾空白</param>
    /// <returns></returns>
    public static string RemoveDuplicate(string text, string splitSymbol, string mergeSymbol, bool trim = false) {
        if (!trim) {
            return string.Join(mergeSymbol, new HashSet<string>(text.Split(splitSymbol)));
        }
        return string.Join(mergeSymbol, new HashSet<string>(text.Split(splitSymbol, StringSplitOptions.TrimEntries)));
    }
}

