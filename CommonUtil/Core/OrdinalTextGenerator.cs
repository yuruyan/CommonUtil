using System.Collections.Generic;
using System.Linq;

namespace CommonUtil.Core;

public class OrdinalTextGenerator {
    /// <summary>
    /// 生成顺序文本
    /// </summary>
    /// <param name="raw"></param>
    /// <param name="startIndex"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static List<string> Generate(string raw, int startIndex, int count) {
        string[] vs = raw.Split("{{}}");
        var list = new List<string>(count);
        // 去掉最后的空元素
        if (!vs[^1].Any()) {
            vs = vs[..^1];
        }
        for (int i = 0; i < vs.Length; i++) {
            vs[i] = vs[i].Replace("{{", "{").Replace("}}", "}");
        }
        var replaceIndexes = new List<int>();
        // 查找需替换的段
        for (int i = 0; i < vs.Length; i++) {
            if (vs[i].Contains("{}")) {
                replaceIndexes.Add(i);
            }
        }
        // 生成
        for (int i = 0; i < count; i++) {
            var template = new List<string>(vs);
            foreach (var index in replaceIndexes) {
                template[index] = template[index].Replace("{}", (startIndex + i).ToString());
            }
            list.Add(string.Join("{}", template));
        }
        return list;
    }
}

