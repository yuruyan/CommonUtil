namespace CommonUtil.Store;

internal static class ImagePath {
    public static readonly Uri JapaneseSymbolImageUri = new($"{Global.ImageSource}JapaneseSymbol.png", UriKind.Relative);
    public static readonly Uri UTF8ImageUri = new($"{Global.ImageSource}UTF8Encoding.png", UriKind.Relative);
    public static readonly Uri Base64ImageUri = new($"{Global.ImageSource}base64.png", UriKind.Relative);
    public static readonly Uri WIFIImageUri = new($"{Global.ImageSource}WIFI.png", UriKind.Relative);
    public static readonly Uri LocationImageUri = new($"{Global.ImageSource}Location.png", UriKind.Relative);
    public static readonly Uri MessageImageUri = new($"{Global.ImageSource}Message.png", UriKind.Relative);
    public static readonly Uri GMailImageUri = new($"{Global.ImageSource}GMail.png", UriKind.Relative);
    public static readonly Uri PhoneCallImageUri = new($"{Global.ImageSource}PhoneCall.png", UriKind.Relative);
}