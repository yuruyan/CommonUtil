namespace CommonUtil.Core.Model;

public enum AESPaddingMode {
    NoPadding,
    PKCS7Padding,
    ISO10126Padding,
}

public enum AESCryptoMode {
    /// <summary>
    /// Must not with iv
    /// </summary>
    ECB,
    CBC,
    CFB,
    OFB,
    /// <summary>
    /// Must with iv
    /// </summary>
    CTR,
}