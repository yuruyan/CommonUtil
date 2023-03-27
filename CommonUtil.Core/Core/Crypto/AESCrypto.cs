using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace CommonUtil.Core;

public static class AESCryptoUtils {
    /// <summary>
    /// AES 加密
    /// </summary>    
    /// <inheritdoc cref="DoCrypto(AESCryptoMode, AESPaddingMode, bool, byte[], byte[], byte[]?)"/>
    public static byte[] Encrypt(
        AESCryptoMode cryptoMode,
        AESPaddingMode paddingMode,
        byte[] data,
        byte[] key,
        byte[]? iv = null
    ) {
        return DoCrypto(cryptoMode, paddingMode, true, data, key, iv);
    }

    /// <summary>
    /// AES 解密
    /// </summary>
    /// <inheritdoc cref="DoCrypto(AESCryptoMode, AESPaddingMode, bool, byte[], byte[], byte[]?)"/>
    public static byte[] Decrypt(
        AESCryptoMode cryptoMode,
        AESPaddingMode paddingMode,
        byte[] data,
        byte[] key,
        byte[]? iv = null
    ) {
        return DoCrypto(cryptoMode, paddingMode, false, data, key, iv);
    }

    /// <summary>
    /// AES 加密 / 解密
    /// </summary>
    /// <param name="cryptoMode">加密模式</param>
    /// <param name="paddingMode">填充模式</param>
    /// <param name="isEncryption">加密 / 解密</param>
    /// <param name="data">数据</param>
    /// <param name="key">key, must be 16, 24 or 32 long</param>
    /// <param name="iv">iv, must be 16 long</param>
    /// <returns></returns>
    public static byte[] DoCrypto(
        AESCryptoMode cryptoMode,
        AESPaddingMode paddingMode,
        bool isEncryption,
        byte[] data,
        byte[] key,
        byte[]? iv = null
    ) {
        var keyParameter = ParameterUtilities.CreateKeyParameter("AES", key);
        var inCipher = CipherUtilities.GetCipher($"AES/{cryptoMode}/{paddingMode}");
        if (iv is null) {
            inCipher.Init(isEncryption, keyParameter);
        } else {
            inCipher.Init(isEncryption, new ParametersWithIV(keyParameter, iv));
        }
        return inCipher.DoFinal(data);
    }
}
