using CommonUtil.Core.Model;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Utilities.Encoders;
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
    /// WhirlpoolDigest
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string WhirlpoolDigest(string s) {
        return GeneralDigest(s, new WhirlpoolDigest());
    }

    /// <summary>
    /// TigerDigest
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string TigerDigest(string s) {
        return GeneralDigest(s, new TigerDigest());
    }

    /// <summary>
    /// SM3Digest
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string SM3Digest(string s) {
        return GeneralDigest(s, new SM3Digest());
    }

    /// <summary>
    /// Sha512tDigest
    /// </summary>
    /// <param name="s"></param>
    /// <param name="multipleOfEight">8 的倍数</param>
    /// <returns></returns>
    public static string Sha512tDigest(string s, int multipleOfEight) {
        return GeneralDigest(s, new Sha512tDigest(multipleOfEight));
    }

    /// <summary>
    /// SkeinDigest
    /// </summary>
    /// <param name="s"></param>
    /// <param name="blockSize"></param>
    /// <param name="multipleOfEight">8 的倍数</param>
    /// <returns></returns>
    public static string SkeinDigest(string s, SkeinDigestBlockSize blockSize, int multipleOfEight) {
        return GeneralDigest(s, new SkeinDigest((int)blockSize, multipleOfEight));
    }

    /// <summary>
    /// ShakeDigest
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string ShakeDigest(string s) {
        return GeneralDigest(s, new ShakeDigest());
    }

    /// <summary>
    /// RipeMD128Digest
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string RipeMD128Digest(string s) {
        return GeneralDigest(s, new RipeMD128Digest());
    }

    /// <summary>
    /// RipeMD160Digest
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string RipeMD160Digest(string s) {
        return GeneralDigest(s, new RipeMD160Digest());
    }

    /// <summary>
    /// RipeMD256Digest
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string RipeMD256Digest(string s) {
        return GeneralDigest(s, new RipeMD256Digest());
    }

    /// <summary>
    /// RipeMD320Digest
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string RipeMD320Digest(string s) {
        return GeneralDigest(s, new RipeMD320Digest());
    }

    /// <summary>
    /// KeccakDigest
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string KeccakDigest(string s) {
        return GeneralDigest(s, new KeccakDigest());
    }

    /// <summary>
    /// Gost3411Digest
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string Gost3411Digest(string s) {
        return GeneralDigest(s, new Gost3411Digest());
    }

    /// <summary>
    /// Gost3411_2012_256Digest
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string Gost3411_2012_256Digest(string s) {
        return GeneralDigest(s, new Gost3411_2012_256Digest());
    }

    /// <summary>
    /// Gost3411_2012_512Digest
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string Gost3411_2012_512Digest(string s) {
        return GeneralDigest(s, new Gost3411_2012_512Digest());
    }

    /// <summary>
    /// Blake2bDigest
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string Blake2bDigest(string s) {
        return GeneralDigest(s, new Blake2bDigest());
    }

    /// <summary>
    /// Blake2sDigest
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string Blake2sDigest(string s) {
        return GeneralDigest(s, new Blake2sDigest());
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

    /// <summary>
    /// WhirlpoolDigest
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="callback">进度回调，参数为进度百分比</param>
    /// <returns>任务取消返回 null</returns>
    public static string? WhirlpoolDigest(FileStream stream, CancellationToken? cancellationToken = null, Action<double>? callback = null) {
        return GeneralDigest(stream, new WhirlpoolDigest(), cancellationToken, callback);
    }

    /// <summary>
    /// TigerDigest
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="callback">进度回调，参数为进度百分比</param>
    /// <returns>任务取消返回 null</returns>
    public static string? TigerDigest(FileStream stream, CancellationToken? cancellationToken = null, Action<double>? callback = null) {
        return GeneralDigest(stream, new TigerDigest(), cancellationToken, callback);
    }

    /// <summary>
    /// SM3Digest
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="callback">进度回调，参数为进度百分比</param>
    /// <returns>任务取消返回 null</returns>
    public static string? SM3Digest(FileStream stream, CancellationToken? cancellationToken = null, Action<double>? callback = null) {
        return GeneralDigest(stream, new SM3Digest(), cancellationToken, callback);
    }

    /// <summary>
    /// SkeinDigest
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="blockSize"></param>
    /// <param name="multipleOfEight">8 的倍数</param>
    /// <param name="cancellationToken"></param>
    /// <param name="callback">进度回调，参数为进度百分比</param>
    /// <returns>任务取消返回 null</returns>
    public static string? SkeinDigest(FileStream stream, SkeinDigestBlockSize blockSize, int multipleOfEight, CancellationToken? cancellationToken = null, Action<double>? callback = null) {
        return GeneralDigest(stream, new SkeinDigest((int)blockSize, multipleOfEight), cancellationToken, callback);
    }

    /// <summary>
    /// ShakeDigest
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="callback">进度回调，参数为进度百分比</param>
    /// <returns>任务取消返回 null</returns>
    public static string? ShakeDigest(FileStream stream, CancellationToken? cancellationToken = null, Action<double>? callback = null) {
        return GeneralDigest(stream, new ShakeDigest(), cancellationToken, callback);
    }

    /// <summary>
    /// Sha512tDigest
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="multipleOfEight">8 的倍数</param>
    /// <param name="cancellationToken"></param>
    /// <param name="callback">进度回调，参数为进度百分比</param>
    /// <returns>任务取消返回 null</returns>
    public static string? Sha512tDigest(FileStream stream, int multipleOfEight, CancellationToken? cancellationToken = null, Action<double>? callback = null) {
        return GeneralDigest(stream, new Sha512tDigest(multipleOfEight), cancellationToken, callback);
    }

    /// <summary>
    /// RipeMD128Digest
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="callback">进度回调，参数为进度百分比</param>
    /// <returns>任务取消返回 null</returns>
    public static string? RipeMD128Digest(FileStream stream, CancellationToken? cancellationToken = null, Action<double>? callback = null) {
        return GeneralDigest(stream, new RipeMD128Digest(), cancellationToken, callback);
    }

    /// <summary>
    /// RipeMD160Digest
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="callback">进度回调，参数为进度百分比</param>
    /// <returns>任务取消返回 null</returns>
    public static string? RipeMD160Digest(FileStream stream, CancellationToken? cancellationToken = null, Action<double>? callback = null) {
        return GeneralDigest(stream, new RipeMD160Digest(), cancellationToken, callback);
    }

    /// <summary>
    /// RipeMD256Digest
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="callback">进度回调，参数为进度百分比</param>
    /// <returns>任务取消返回 null</returns>
    public static string? RipeMD256Digest(FileStream stream, CancellationToken? cancellationToken = null, Action<double>? callback = null) {
        return GeneralDigest(stream, new RipeMD256Digest(), cancellationToken, callback);
    }

    /// <summary>
    /// RipeMD320Digest
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="callback">进度回调，参数为进度百分比</param>
    /// <returns>任务取消返回 null</returns>
    public static string? RipeMD320Digest(FileStream stream, CancellationToken? cancellationToken = null, Action<double>? callback = null) {
        return GeneralDigest(stream, new RipeMD320Digest(), cancellationToken, callback);
    }

    /// <summary>
    /// KeccakDigest
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="callback">进度回调，参数为进度百分比</param>
    /// <returns>任务取消返回 null</returns>
    public static string? KeccakDigest(FileStream stream, CancellationToken? cancellationToken = null, Action<double>? callback = null) {
        return GeneralDigest(stream, new KeccakDigest(), cancellationToken, callback);
    }

    /// <summary>
    /// Gost3411Digest
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="callback">进度回调，参数为进度百分比</param>
    /// <returns>任务取消返回 null</returns>
    public static string? Gost3411Digest(FileStream stream, CancellationToken? cancellationToken = null, Action<double>? callback = null) {
        return GeneralDigest(stream, new Gost3411Digest(), cancellationToken, callback);
    }

    /// <summary>
    /// Gost3411_2012_256Digest
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="callback">进度回调，参数为进度百分比</param>
    /// <returns>任务取消返回 null</returns>
    public static string? Gost3411_2012_256Digest(FileStream stream, CancellationToken? cancellationToken = null, Action<double>? callback = null) {
        return GeneralDigest(stream, new Gost3411_2012_256Digest(), cancellationToken, callback);
    }

    /// <summary>
    /// Gost3411_2012_512Digest
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="callback">进度回调，参数为进度百分比</param>
    /// <returns>任务取消返回 null</returns>
    public static string? Gost3411_2012_512Digest(FileStream stream, CancellationToken? cancellationToken = null, Action<double>? callback = null) {
        return GeneralDigest(stream, new Gost3411_2012_512Digest(), cancellationToken, callback);
    }

    /// <summary>
    /// Blake2bDigest
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="callback">进度回调，参数为进度百分比</param>
    /// <returns>任务取消返回 null</returns>
    public static string? Blake2bDigest(FileStream stream, CancellationToken? cancellationToken = null, Action<double>? callback = null) {
        return GeneralDigest(stream, new Blake2bDigest(), cancellationToken, callback);
    }

    /// <summary>
    /// Blake2sDigest
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="callback">进度回调，参数为进度百分比</param>
    /// <returns>任务取消返回 null</returns>
    public static string? Blake2sDigest(FileStream stream, CancellationToken? cancellationToken = null, Action<double>? callback = null) {
        return GeneralDigest(stream, new Blake2sDigest(), cancellationToken, callback);
    }
}
