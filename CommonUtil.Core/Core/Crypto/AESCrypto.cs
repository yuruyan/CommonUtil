using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace CommonUtil.Core;

public static class AESCryptoUtils {
    /// <summary>
    /// 默认读取缓冲区大小
    /// </summary>
    private const int ReadBuffer = 8192;

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
    /// 文件加密
    /// </summary>
    /// <inheritdoc cref="Encrypt(AESCryptoMode, AESPaddingMode, byte[], byte[], byte[]?)"/>
    /// <param name="inputPath">要加密的文件</param>
    /// <param name="outputPath">加密后的文件</param>
    /// <param name="cancellationToken"></param>
    /// <param name="callback">进度回调，参数为进度百分比</param>
    public static void EncryptFile(
        AESCryptoMode cryptoMode,
        AESPaddingMode paddingMode,
        string inputPath,
        string outputPath,
        byte[] key,
        byte[]? iv = null,
        CancellationToken? cancellationToken = null,
        Action<double>? callback = null
    ) {
        using var readStream = File.OpenRead(inputPath);
        using var writeStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
        DoCrypto(
            cryptoMode,
            paddingMode,
            true,
            readStream,
            writeStream,
            key,
            iv,
            cancellationToken,
            callback
        );
    }

    /// <summary>
    /// 文件解密
    /// </summary>
    /// <inheritdoc cref="Decrypt(AESCryptoMode, AESPaddingMode, byte[], byte[], byte[]?)"/>
    /// <param name="inputPath">要解密的文件</param>
    /// <param name="outputPath">解密后的文件</param>
    /// <param name="cancellationToken"></param>
    /// <param name="callback">进度回调，参数为进度百分比</param>
    public static void DecryptFile(
        AESCryptoMode cryptoMode,
        AESPaddingMode paddingMode,
        string inputPath,
        string outputPath,
        byte[] key,
        byte[]? iv = null,
        CancellationToken? cancellationToken = null,
        Action<double>? callback = null
    ) {
        using var readStream = File.OpenRead(inputPath);
        using var writeStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
        DoCrypto(
            cryptoMode,
            paddingMode,
            false,
            readStream,
            writeStream,
            key,
            iv,
            cancellationToken,
            callback
        );
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
    private static byte[] DoCrypto(
        AESCryptoMode cryptoMode,
        AESPaddingMode paddingMode,
        bool isEncryption,
        byte[] data,
        byte[] key,
        byte[]? iv = null
    ) {
        var keyParameter = ParameterUtilities.CreateKeyParameter("AES", key);
        var cipher = CipherUtilities.GetCipher($"AES/{cryptoMode}/{paddingMode}");
        if (iv is null) {
            cipher.Init(isEncryption, keyParameter);
        } else {
            cipher.Init(isEncryption, new ParametersWithIV(keyParameter, iv));
        }
        return cipher.DoFinal(data);
    }

    /// <inheritdoc cref="DoCrypto(AESCryptoMode, AESPaddingMode, bool, byte[], byte[], byte[]?)"/>
    /// <param name="inputStream">输入流</param>
    /// <param name="outputStream">输出流</param>
    /// <param name="cancellationToken"></param>
    /// <param name="callback">进度回调，参数为进度百分比</param>
    private static void DoCrypto(
        AESCryptoMode cryptoMode,
        AESPaddingMode paddingMode,
        bool isEncryption,
        Stream inputStream,
        Stream outputStream,
        byte[] key,
        byte[]? iv = null,
        CancellationToken? cancellationToken = null,
        Action<double>? callback = null
    ) {
        var keyParameter = ParameterUtilities.CreateKeyParameter("AES", key);
        var cipher = CipherUtilities.GetCipher($"AES/{cryptoMode}/{paddingMode}");
        if (iv is null) {
            cipher.Init(isEncryption, keyParameter);
        } else {
            cipher.Init(isEncryption, new ParametersWithIV(keyParameter, iv));
        }
        var readBuffer = new byte[ReadBuffer];
        var writeBuffer = new byte[ReadBuffer];
        int readCount;
        long totalRead = 0, streamLength = inputStream.Length;
        inputStream.Position = 0;
        // Processing
        while ((readCount = inputStream.Read(readBuffer)) > 0) {
            // Terminate
            if (cancellationToken?.IsCancellationRequested is true) {
                return;
            }
            var writeCount = cipher.ProcessBytes(readBuffer, 0, readCount, writeBuffer, 0);
            outputStream.Write(writeBuffer, 0, writeCount);
            totalRead += readCount;
            callback?.Invoke((double)totalRead / streamLength);
        }
        outputStream.Write(writeBuffer, 0, cipher.DoFinal(writeBuffer, 0));
        callback?.Invoke(1);
    }
}
