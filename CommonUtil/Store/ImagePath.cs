namespace CommonUtil.Store;

internal static class ImagePath {
    public static readonly Uri JapaneseSymbolImageUri = new($"{Global.ImageSource}JapaneseSymbol.png", UriKind.Relative);
    public static readonly Uri UTF8ImageUri = new($"{Global.ImageSource}UTF8Encoding.png", UriKind.Relative);
    public static readonly Uri Base64ImageUri = new($"{Global.ImageSource}base64.png", UriKind.Relative);
}
