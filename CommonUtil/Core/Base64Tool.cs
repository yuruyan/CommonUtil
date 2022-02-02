using NLog;
using System;
using System.IO;
using System.Text;

namespace CommonUtil.Core {
    class Base64Tool {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// base64 解码
        /// </summary>
        /// <param name="encodeType">字符串编码方式</param>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public static string Base64StringDecode(Encoding encodeType, string encoded) {
            string decode = string.Empty;
            byte[] bytes = Convert.FromBase64String(encoded);
            try {
                return encodeType.GetString(bytes);
            } catch (Exception e) {
                Logger.Info(e);
            }
            return decode;
        }

        /// <summary>
        /// base64 编码
        /// </summary>
        /// <param name="encodeType">编码方式</param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Base64StringEncode(Encoding encodeType, string source) {
            string encode = string.Empty;
            byte[] bytes = encodeType.GetBytes(source);
            try {
                return Convert.ToBase64String(bytes);
            } catch (Exception e) {
                Logger.Info(e);
            }
            return encode;
        }

        /// <summary>
        /// 图片转base64
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string Base64ImageEncode(string path) {
            try {
                return Convert.ToBase64String(File.ReadAllBytes(path));
            } catch (Exception e) {
                Logger.Info(e);
            }
            return string.Empty;
        }

        /// <summary>
        /// base64转 图片
        /// </summary>
        /// <param name="encoded"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static byte[] Base64ImageDecode(string encoded, string path) {
            try {
                return Convert.FromBase64String(encoded);
            } catch (Exception e) {
                Logger.Info(e);
            }
            return null;
        }
    }
}
