namespace CommonUtil.Core.Model;

public readonly record struct RSAKey(string PublicKey, string PrivateKey);

public sealed class RSAAlgorithm {
    public static readonly IReadOnlyList<string> AllAlgorithms = new List<string> {
        RSA,
        NoPadding,
        OAEPPadding,
        PKCS1Padding,
        NonePKCS1Padding,
        ISO9796_1Padding,
        OAEPWithMD5AndMGF1Padding,
        OAEPWithSHA1AndMGF1Padding,
        OAEPWithSHA224AndMGF1Padding,
        OAEPWithSHA256AndMGF1Padding,
        OAEPWithSHA384AndMGF1Padding,
    };

    public const string RSA = "RSA";
    public const string ISO9796_1Padding = "RSA/NONE/ISO9796-1Padding";
    public const string OAEPPadding = "RSA/NONE/OAEPPadding";
    public const string PKCS1Padding = "RSA//PKCS1Padding";
    public const string NonePKCS1Padding = "RSA/NONE/PKCS1Padding";
    public const string OAEPWithSHA1AndMGF1Padding = "RSA/NONE/OAEPWithSHA1AndMGF1Padding";
    public const string OAEPWithSHA224AndMGF1Padding = "RSA/NONE/OAEPWithSHA224AndMGF1Padding";
    public const string OAEPWithSHA256AndMGF1Padding = "RSA/NONE/OAEPWithSHA256AndMGF1Padding";
    public const string OAEPWithSHA384AndMGF1Padding = "RSA/NONE/OAEPWithSHA384AndMGF1Padding";
    public const string NoPadding = "RSA/NONE/NoPadding";
    public const string OAEPWithMD5AndMGF1Padding = "RSA/NONE/OAEPWithMD5AndMGF1Padding";
}
