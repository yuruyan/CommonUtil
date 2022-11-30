namespace CommonUtil.Core.Model;

public class AsciiInfo {
    public string Binary { get; set; } = string.Empty;
    public string Octal { get; set; } = string.Empty;
    public string Decimal { get; set; } = string.Empty;
    public string HexaDecimal { get; set; } = string.Empty;
    public string Character { get; set; } = string.Empty;
    public string HtmlEntity { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public override string ToString() {
        return $"{{{nameof(Binary)}={Binary}, {nameof(Octal)}={Octal}, {nameof(Decimal)}={Decimal}, {nameof(HexaDecimal)}={HexaDecimal}, {nameof(Character)}={Character}, {nameof(HtmlEntity)}={HtmlEntity}, {nameof(Description)}={Description}}}";
    }
}
