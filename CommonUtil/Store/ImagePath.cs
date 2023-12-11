namespace CommonUtil.Store;

internal static class ImagePath {
    public static readonly Uri JapaneseSymbolImageUri = GetUri("JapaneseSymbol.png");
    public static readonly Uri UTF8ImageUri = GetUri("UTF8Encoding.png");
    public static readonly Uri Base64ImageUri = GetUri("base64.png");
    public static readonly Uri WIFIImageUri = GetUri("WIFI.png");
    public static readonly Uri LocationImageUri = GetUri("Location.png");
    public static readonly Uri MessageImageUri = GetUri("Message.png");
    public static readonly Uri GMailImageUri = GetUri("GMail.png");
    public static readonly Uri PhoneCallImageUri = GetUri("PhoneCall.png");
    public static readonly Uri HexidecimalImageUri = GetUri("Hexidecimal.png");

    private static Uri GetUri(string imageName) => new($"{Global.ImageSource}{imageName}", UriKind.Relative);
}