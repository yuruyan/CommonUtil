using CommonUtil.Model;
using CommonUITools.Utils;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;

namespace CommonUtil.Core {
    public class AsciiTable {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly List<AsciiInfo> AsciiInfoList = new();
        private static readonly List<AsciiInfo> AsciiInfoControlList = new(); // 控制字符
        private static readonly List<AsciiInfo> AsciiInfoNormalList = new(); // 非控制字符
        private static readonly List<AsciiInfo> AsciiInfoExtendedList = new(); // 扩展字符

        private static readonly string ResourcePath = "./resource/ascii.json";

        /// <summary>
        /// 加载数据
        /// </summary>
        static AsciiTable() {
            if (!File.Exists(ResourcePath)) {
                CommonUITools.Widget.MessageBox.Error("找不到文件 " + ResourcePath);
                Logger.Error("cannot find the file " + ResourcePath);
                return;
            }
            try {
                List<AsciiInfo>? list = JsonConvert.DeserializeObject<List<AsciiInfo>>(File.ReadAllText(ResourcePath));
                if (list == null) {
                    throw new Exception("List<AsciiInfo>? list is null");
                }
                AsciiInfoList.AddRange(list);
                #region 控制字符
                for (int i = 0; i < 32; i++) {
                    AsciiInfoControlList.Add(AsciiInfoList[i]);
                }
                AsciiInfoControlList.Add(AsciiInfoList[127]);
                #endregion
                #region 非控制字符
                for (int i = 32; i < 127; i++) {
                    AsciiInfoNormalList.Add(AsciiInfoList[i]);
                }
                #endregion
                #region 其他字符
                for (int i = 128; i < 256; i++) {
                    AsciiInfoNormalList.Add(AsciiInfoList[i]);
                }
                #endregion
            } catch (Exception e) {
                Logger.Error(e);
            }
        }

        /// <summary>
        /// 返回全部 ASCII 列表
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyList<AsciiInfo> GetAsciiInfoList() {
            return AsciiInfoList;
        }

        /// <summary>
        /// 返回控制 ASCII 列表
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyList<AsciiInfo> GetAsciiInfoControlList() {
            return AsciiInfoControlList;
        }

        /// <summary>
        /// 返回非控制 ASCII 列表
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyList<AsciiInfo> GetAsciiInfoNormalList() {
            return AsciiInfoNormalList;
        }

        /// <summary>
        /// 返回扩展 ASCII 列表
        /// </summary>
        /// <returns></returns>
        public static IReadOnlyList<AsciiInfo> GetAsciiInfoExtendedList() {
            return AsciiInfoExtendedList;
        }
    }
}
