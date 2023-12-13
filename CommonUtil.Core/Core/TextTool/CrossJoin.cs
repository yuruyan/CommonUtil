namespace CommonUtil.Core;

/// <summary>
/// 笛卡尔积
/// </summary>
public static class CrossJoin {
    public static List<string> Join(IEnumerable<IEnumerable<string>> data) {
        var results = new List<string>() { string.Empty };
        foreach (var list in data) {
            var copyList = results.ToList();
            results.Clear();
            foreach (var item1 in copyList) {
                foreach (var item2 in list) {
                    results.Add(item1 + item2);
                }
            }
        }
        return results;
    }
}
