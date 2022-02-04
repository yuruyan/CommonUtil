using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CommonUtil.Core {
    /// <summary>
    /// 简繁体转换
    /// </summary>
    public class ChineseTransform {
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        private static Dictionary<string, string> TraditionalSimplifiedMap = new();
        private static Dictionary<string, string> SimplifiedTraditionalMap = new();
        private static readonly string SourcePath = "./resource/wordmap.json";
        private static bool IsLoaded = false;

        /// <summary>
        /// 转繁体字
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToTraditional(string s) {
            if (!IsLoaded) {
                LoadData();
            }
            var sb = new StringBuilder();
            foreach (var item in s) {
                if (SimplifiedTraditionalMap.ContainsKey(item.ToString())) {
                    sb.Append(SimplifiedTraditionalMap[item.ToString()]);
                } else {
                    sb.Append(item);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 转简体
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToSimplified(string s) {
            if (!IsLoaded) {
                LoadData();
            }
            var sb = new StringBuilder();
            foreach (var item in s) {
                if (TraditionalSimplifiedMap.ContainsKey(item.ToString())) {
                    sb.Append(TraditionalSimplifiedMap[item.ToString()]);
                } else {
                    sb.Append(item);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        private static void LoadData() {
            lock (typeof(ChineseTransform)) {
                if (IsLoaded) {
                    return;
                }
                try {
                    var s = File.ReadAllText(SourcePath);
                    Dictionary<string, string>? dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(s);
                    // 填充 TraditionalSimplifiedMap
                    if (dict != null) {
                        TraditionalSimplifiedMap = dict;
                    }
                    // 填充 SimplifiedTraditionalMap
                    foreach (var item in TraditionalSimplifiedMap) {
                        SimplifiedTraditionalMap[item.Value] = item.Key;
                    }
                } catch (Exception e) {
                    Logger.Error(e);
                }
                IsLoaded = true;
            }
        }
    }
}
