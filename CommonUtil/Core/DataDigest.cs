﻿using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Utilities.Encoders;
using System.IO;
using System.Text;

namespace CommonUtil.Core {
    public class DataDigest {
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

        private static readonly int ReadBuffer = 128 * 1024 * 1024;

        /// <summary>
        /// 摘要算法
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="digest"></param>
        /// <returns></returns>
        private static string GeneralDigest(FileStream stream, IDigest digest) {
            byte[] buffer = new byte[ReadBuffer];
            byte[] resultBuffer = new byte[digest.GetDigestSize()];
            int read;
            while ((read = stream.Read(buffer, 0, ReadBuffer)) > 0) {
                digest.BlockUpdate(buffer, 0, read);
            }
            digest.DoFinal(resultBuffer, 0);
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
        /// <returns></returns>
        public static string SHA1Digest(FileStream stream) {
            return GeneralDigest(stream, new Sha1Digest());
        }

        /// <summary>
        /// sha3 摘要
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string SHA3Digest(FileStream stream) {
            return GeneralDigest(stream, new Sha3Digest());
        }

        /// <summary>
        /// sha224 摘要
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string SHA224Digest(FileStream stream) {
            return GeneralDigest(stream, new Sha224Digest());
        }

        /// <summary>
        /// sha256 摘要
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string SHA256Digest(FileStream stream) {
            return GeneralDigest(stream, new Sha256Digest());
        }

        /// <summary>
        /// sha384 摘要
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string SHA384Digest(FileStream stream) {
            return GeneralDigest(stream, new Sha384Digest());
        }

        /// <summary>
        /// sha512 摘要
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string SHA512Digest(FileStream stream) {
            return GeneralDigest(stream, new Sha512Digest());
        }

        /// <summary>
        /// md5 摘要
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string MD5Digest(FileStream stream) {
            return GeneralDigest(stream, new MD5Digest());
        }

        /// <summary>
        /// md4 摘要
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string MD4Digest(FileStream stream) {
            return GeneralDigest(stream, new MD4Digest());
        }

        /// <summary>
        /// md2 摘要
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string MD2Digest(FileStream stream) {
            return GeneralDigest(stream, new MD2Digest());
        }
    }
}
