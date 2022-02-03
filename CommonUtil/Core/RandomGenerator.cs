using System;
using System.Text;

namespace CommonUtil.Core {

    [Flags]
    public enum RandomStringChoice {
        None = 0,
        Number = 1,
        UpperCase = 2,
        LowerCase = 4,
        SpacialCharacter = 8
    }

    public class RandomGenerator {
        private static readonly string NumberCharacter = "0123456789";
        private static readonly string UpperCaseCharacter = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly string LowerCaseCharacter = "abcdefghijklmnopqrstuvwxyz";
        private static readonly string SpacialCharacter = @"$%&'()*+,-./\:;<=>?@[]^_`{|}~";
        //private static readonly string SpacialCharacterCompiled = "\\$|%|&|'|\\(|\\)|\\*|\\+|\\,|\\-|\\.|/|\\|:|;|<|=|>|\\?|@|\\[|\\]|\\^|_|`|\\{|\\||\\}|~";
        private static Random Random = new();

        /// <summary>
        /// 生成随机数
        /// </summary>
        /// <param name="minValue">最小值，包括</param>
        /// <param name="maxValue">最大值，不包括</param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static int[] GenerateRandomNumber(int minValue, int maxValue, int count) {
            int[] results = new int[count];
            for (int i = 0; i < count; i++) {
                results[i] = Random.Next(minValue, maxValue);
            }
            return results;
        }

        /// <summary>
        /// 生成随机字符串
        /// </summary>
        /// <param name="choice"></param>
        /// <param name="length">字符串长度</param>
        /// <param name="count">随机字符串个数</param>
        /// <returns></returns>
        public static string[] GenerateRandomString(RandomStringChoice choice, int length, int count) {
            if (choice == RandomStringChoice.None) { 
                return Array.Empty<string>();
            }
            string dataSource = "";
            if (choice.HasFlag(RandomStringChoice.Number)) {
                dataSource += NumberCharacter;
            }
            if (choice.HasFlag(RandomStringChoice.UpperCase)) {
                dataSource += UpperCaseCharacter;
            }
            if (choice.HasFlag(RandomStringChoice.LowerCase)) {
                dataSource += LowerCaseCharacter;
            }
            if (choice.HasFlag(RandomStringChoice.SpacialCharacter)) {
                dataSource += SpacialCharacter;
            }
            string[] results = new string[count];
            if (string.IsNullOrEmpty(dataSource)) {
                return results;
            }
            // 随机选择字符
            var sb = new StringBuilder(length);
            for (int i = 0; i < count; i++) {
                sb.Clear();
                for (int j = 0; j < length; j++) {
                    sb.Append(dataSource[Random.Next(dataSource.Length)]);
                }
                results[i] = sb.ToString();
            }
            return results;
        }
    }
}
