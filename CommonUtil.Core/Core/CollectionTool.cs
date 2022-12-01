using System.Collections.Generic;
using System.Linq;

namespace CommonUtil.Core;

public static class CollectionTool {
    /// <summary>
    /// 交集
    /// </summary>
    /// <param name="list1"></param>
    /// <param name="list2"></param>
    /// <returns></returns>
    public static IEnumerable<string> Intersect(IEnumerable<string> list1, IEnumerable<string> list2) {
        return list1.Intersect(list2);
    }

    /// <summary>
    /// 差集
    /// </summary>
    /// <param name="list1"></param>
    /// <param name="list2"></param>
    /// <returns></returns>
    public static IEnumerable<string> Except(IEnumerable<string> list1, IEnumerable<string> list2) {
        return list1.Except(list2);
    }

    /// <summary>
    /// 并集
    /// </summary>
    /// <param name="list1"></param>
    /// <param name="list2"></param>
    /// <returns></returns>
    public static IEnumerable<string> Union(IEnumerable<string> list1, IEnumerable<string> list2) {
        return list1.Union(list2);
    }
}
