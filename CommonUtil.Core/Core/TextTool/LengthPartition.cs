namespace CommonUtil.Core;

/// <summary>
/// 根据长度分割字符串
/// </summary>
public static class LengthPartition {
    public static List<string> Split(string text, int length) {
        if (length <= 0) {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        var textLength = text.Length;
        var results = new List<string>(textLength / length);
        int i = 0;
        for (i = 0; i + length < textLength; i += length) {
            results.Add(text[i..(i + length)]);
        }
        if (i < textLength) {
            results.Add(text[i..]);
        }
        return results;
    }
}
