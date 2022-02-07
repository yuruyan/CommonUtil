using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Utilities.Encoders;
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
    }
}
