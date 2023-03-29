using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System.Text.RegularExpressions;
using Org.BouncyCastle.Math;

namespace CommonUtil.Core;

public static partial class RSACrypto {
#if NET7_0_OR_GREATER
    private static readonly Regex NewLineRege = GetNewLineRegex();
    [GeneratedRegex("[\\r\\n]")]
    private static partial Regex GetNewLineRegex();
#elif NET6_0_OR_GREATER
    private static readonly Regex NewLineRege = new(@"[\r\n]");
#endif

    private static RSAKey FromKeyPair(AsymmetricCipherKeyPair keyPair) {
        var publicData = SubjectPublicKeyInfoFactory
            .CreateSubjectPublicKeyInfo(keyPair.Public)
            .ToAsn1Object()
            .GetEncoded();
        var privateData = PrivateKeyInfoFactory
            .CreatePrivateKeyInfo(keyPair.Private)
            .ToAsn1Object()
            .GetEncoded();
        return new(Convert.ToBase64String(publicData), Convert.ToBase64String(privateData));
    }

    private static AsymmetricKeyParameter GetPublicKey(string publicKey) {
        return PublicKeyFactory.CreateKey(Convert.FromBase64String(
            NewLineRege.Replace(publicKey, "").Trim()
        ));
    }

    private static AsymmetricKeyParameter GetPrivateKey(string privateKey) {
        return PrivateKeyFactory.CreateKey(Convert.FromBase64String(
            NewLineRege.Replace(privateKey, "").Trim()
        ));
    }

    private static AsymmetricCipherKeyPair ToKeyPair(RSAKey key) {
        return new(
            GetPublicKey(key.PublicKey),
            GetPrivateKey(key.PrivateKey)
        );
    }

    /// <summary>
    /// 生成 Key
    /// </summary>
    /// <param name="strength">It's better to be greater than 1024</param>
    /// <returns></returns>
    public static RSAKey GenerateKey(uint strength) {
        var generator = new RsaKeyPairGenerator();
        generator.Init(new RsaKeyGenerationParameters(
            BigInteger.ValueOf(65537),
            new SecureRandom(),
            (int)strength,
            128
        ));
        return FromKeyPair(generator.GenerateKeyPair());
    }

    /// <summary>
    /// 加密
    /// </summary>
    /// <param name="publicKey">公钥</param>
    /// <param name="data">要加密的数据</param>
    /// <param name="algorithm"><see cref="RSAAlgorithm"/></param>
    /// <returns></returns>
    public static byte[] Encrypt(string publicKey, byte[] data, string algorithm) {
        IBufferedCipher c = CipherUtilities.GetCipher(algorithm);
        c.Init(true, GetPublicKey(publicKey));
        return c.DoFinal(data);
    }

    /// <summary>
    /// 解密
    /// </summary>
    /// <param name="privateKey">私钥</param>
    /// <param name="data">要解密的数据</param>
    /// <param name="algorithm"><see cref="RSAAlgorithm"/></param>
    /// <returns></returns>
    public static byte[] Decrypt(string privateKey, byte[] data, string algorithm) {
        IBufferedCipher c = CipherUtilities.GetCipher(algorithm);
        c.Init(false, GetPrivateKey(privateKey));
        return c.DoFinal(data);
    }
}
