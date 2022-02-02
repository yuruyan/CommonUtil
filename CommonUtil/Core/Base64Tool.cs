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
        /// <param name="encoded"></param>
        /// <param name="encodeType">字符串编码方式</param>
        /// <returns></returns>
        public static string Base64StringDecode(string encoded, Encoding encodeType) {
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
        /// base64 解码
        /// </summary>
        /// <param name="encoded">以 UTF8 编码的字符串</param>
        /// <returns></returns>
        public static string Base64StringDecode(string encoded) {
            return Base64StringDecode(encoded, Encoding.UTF8);
        }

        /// <summary>
        /// base64 编码
        /// </summary>
        /// <param name="source"></param>
        /// <param name="encodeType">编码方式</param>
        /// <returns></returns>
        public static string Base64StringEncode(string source, Encoding encodeType) {
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
        /// base64 编码，以 UTF8 进行编码
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Base64StringEncode(string source) {
            return Base64StringEncode(source, Encoding.UTF8);
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
        /// base64 转图片
        /// </summary>
        /// <param name="encoded"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool Base64ImageDecode(string encoded, string path) {
            try {
                File.WriteAllBytes(path, Convert.FromBase64String(encoded));
                return true;
            } catch (Exception e) {
                Logger.Info(e);
            }
            return false;
        }
    }
}
