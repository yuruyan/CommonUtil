namespace CommonUtil.Data.Model;

public record AsciiInfo {
    public string Binary { get; set; } = string.Empty;
    public string Octal { get; set; } = string.Empty;
    public string Decimal { get; set; } = string.Empty;
    public string HexaDecimal { get; set; } = string.Empty;
    public string Character { get; set; } = string.Empty;
    public string HtmlEntity { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
