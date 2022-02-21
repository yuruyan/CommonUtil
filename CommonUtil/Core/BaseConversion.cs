using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonUtil.Core {
    public class BaseConversion {
        private static readonly string BaseChar = "0123456789ABCDEFGHJKLMNPQRSTUVWXYZ";

        /// <summary>
        /// 将十进制转换为指定的进制
        /// </summary>
        /// <param name="val">十进制值</param>
        /// <param name="targetRadix">目标进制</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">基数错误</exception>
        public static string ConvertFromDecimal(ulong val, int targetRadix = 10) {
            if (targetRadix <= 1 || targetRadix > 36) {
                throw new ArgumentException("基数错误");
            }
            var numList = new List<char>();
            do {
                ulong y = val % (ulong)targetRadix;
                numList.Add(BaseChar[Convert.ToInt32(y)]);
                val = Convert.ToUInt64(Math.Floor(val / (double)targetRadix));
            } while (val > 0);
            numList.Reverse();
            return string.Join("", numList);
        }

        /// <summary>
        /// 将任意进制转化为十进制
        /// </summary>
        /// <param name="value">任意进制的字任串</param>
        /// <param name="sourceRadix">源字符串的进制</param>
        /// <returns></returns>
        /// <exception cref="FormatException">格式有误</exception>
        /// <exception cref="ArgumentException">基数错误</exception>
        public static ulong ConvertToDecimal(string value, int sourceRadix = 10) {
            ulong r = 0;
            if (sourceRadix <= 1 || sourceRadix > 36) {
                throw new ArgumentException("基数错误");
            }
            List<char> charList = value.Trim().ToUpper().ToCharArray().ToList();
            charList.Reverse();
            // 检查输入合法性
            foreach (var c in charList) {
                int index = BaseChar.IndexOf(c);
                if (index >= sourceRadix || index < 0) {
                    throw new FormatException("格式有误");
                }
            }
            for (int i = 0; i < charList.Count; i++) {
                int index = BaseChar.IndexOf(charList[i]);
                if (index > -1) {
                    r += Convert.ToUInt64(index * Math.Pow(sourceRadix, i));
                }
            }
            return r;
        }
    }
}
