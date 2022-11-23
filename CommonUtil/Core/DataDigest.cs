using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace CommonUtil.Core;

public static class DataDigest {
    /// <summary>
    /// 默认读取缓冲区大小
    /// </summary>
    private const int FileReadBuffer = 8192;

    /// <summary>
    /// 摘要算法
    /// </summary>
    /// <param name="s"></param>
    /// <param name="digest"></param>
    /// <returns></returns>
    private static string GeneralDigest(string s, IDigest digest) {
        byte[] sourceBuffer = Encoding.UTF8.GetBytes(s);
        byte[] resultBuffer = new byte[digest.GetDigestSize()];
        digest.BlockUpdate(sourceBuffer, 0, sourceBuffer.Length);
        digest.DoFinal(resultBuffer, 0);
        return Hex.ToHexString(resultBuffer);
    }

    /// <summary>
    /// 摘要算法
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="digest"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="callback">进度回调，参数为进度百分比</param>
    /// <returns></returns>
    private static string? GeneralDigest(
        FileStream stream,
        IDigest digest,
        CancellationToken? cancellationToken = null,
        Action<double>? callback = null
    ) {
        var buffer = new byte[FileReadBuffer];
        var resultBuffer = new byte[digest.GetDigestSize()];
        int readCound;
        long totalRead = 0, streamLength = stream.Length;
        while ((readCound = stream.Read(buffer, 0, buffer.Length)) > 0) {
            // 终止计算
            if (cancellationToken?.IsCancellationRequested == true) {
                return null;
            }
            digest.BlockUpdate(buffer, 0, readCound);
            totalRead += readCound;
            callback?.Invoke((double)totalRead / streamLength);
        }
        digest.DoFinal(resultBuffer, 0);
        callback?.Invoke(1);
        return Hex.ToHexString(resultBuffer);
    }

    /// <summary>
    /// sha1 摘要
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string SHA1Digest(string s) {
        return GeneralDigest(s, new Sha1Digest());
    }

    /// <summary>
    /// sha3 摘要
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string SHA3Digest(string s) {
        return GeneralDigest(s, new Sha3Digest());
    }

    /// <summary>
    /// sha224 摘要
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string SHA224Digest(string s) {
        return GeneralDigest(s, new Sha224Digest());
    }

    /// <summary>
    /// sha256 摘要
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string SHA256Digest(string s) {
        return GeneralDigest(s, new Sha256Digest());
    }

    /// <summary>
    /// sha384 摘要
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string SHA384Digest(string s) {
        return GeneralDigest(s, new Sha384Digest());
    }

    /// <summary>
    /// sha512 摘要
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string SHA512Digest(string s) {
        return GeneralDigest(s, new Sha512Digest());
    }

    /// <summary>
    /// md5 摘要
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string MD5Digest(string s) {
        return GeneralDigest(s, new MD5Digest());
    }

    /// <summary>
    /// md4 摘要
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string MD4Digest(string s) {
        return GeneralDigest(s, new MD4Digest());
    }

    /// <summary>
    /// md2 摘要
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string MD2Digest(string s) {
        return GeneralDigest(s, new MD2Digest());
    }

    /// <summary>
    /// SHA1 摘要
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="callback">进度回调，参数为进度百分比</param>
    /// <returns>任务取消返回 null</returns>
    public static string? SHA1Digest(FileStream stream, CancellationToken? cancellationToken = null, Action<double>? callback = null) {
        return GeneralDigest(stream, new Sha1Digest(), cancellationToken, callback);
    }

    /// <summary>
    /// sha3 摘要
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="callback">进度回调，参数为进度百分比</param>
    /// <returns>任务取消返回 null</returns>
    public static string? SHA3Digest(FileStream stream, CancellationToken? cancellationToken = null, Action<double>? callback = null) {
        return GeneralDigest(stream, new Sha3Digest(), cancellationToken, callback);
    }

    /// <summary>
    /// sha224 摘要
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="callback">进度回调，参数为进度百分比</param>
    /// <returns>任务取消返回 null</returns>
    public static string? SHA224Digest(FileStream stream, CancellationToken? cancellationToken = null, Action<double>? callback = null) {
        return GeneralDigest(stream, new Sha224Digest(), cancellationToken, callback);
    }

    /// <summary>
    /// sha256 摘要
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="callback">进度回调，参数为进度百分比</param>
    /// <returns>任务取消返回 null</returns>
    public static string? SHA256Digest(FileStream stream, CancellationToken? cancellationToken = null, Action<double>? callback = null) {
        return GeneralDigest(stream, new Sha256Digest(), cancellationToken, callback);
    }

    /// <summary>
    /// sha384 摘要
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="callback">进度回调，参数为进度百分比</param>
    /// <returns>任务取消返回 null</returns>
    public static string? SHA384Digest(FileStream stream, CancellationToken? cancellationToken = null, Action<double>? callback = null) {
        return GeneralDigest(stream, new Sha384Digest(), cancellationToken, callback);
    }

    /// <summary>
    /// sha512 摘要
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="callback">进度回调，参数为进度百分比</param>
    /// <returns>任务取消返回 null</returns>
    public static string? SHA512Digest(FileStream stream, CancellationToken? cancellationToken = null, Action<double>? callback = null) {
        return GeneralDigest(stream, new Sha512Digest(), cancellationToken, callback);
    }

    /// <summary>
    /// md5 摘要
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="callback">进度回调，参数为进度百分比</param>
    /// <returns>任务取消返回 null</returns>
    public static string? MD5Digest(FileStream stream, CancellationToken? cancellationToken = null, Action<double>? callback = null) {
        return GeneralDigest(stream, new MD5Digest(), cancellationToken, callback);
    }

    /// <summary>
    /// md4 摘要
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="callback">进度回调，参数为进度百分比</param>
    /// <returns>任务取消返回 null</returns>
    public static string? MD4Digest(FileStream stream, CancellationToken? cancellationToken = null, Action<double>? callback = null) {
        return GeneralDigest(stream, new MD4Digest(), cancellationToken, callback);
    }

    /// <summary>
    /// md2 摘要
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="callback">进度回调，参数为进度百分比</param>
    /// <returns>任务取消返回 null</returns>
    public static string? MD2Digest(FileStream stream, CancellationToken? cancellationToken = null, Action<double>? callback = null) {
        return GeneralDigest(stream, new MD2Digest(), cancellationToken, callback);
    }
}
