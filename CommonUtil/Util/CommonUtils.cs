using CommonUtil.Store;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CommonUtil.Utils;

public class CommonUtils {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly Random Random = new();

    /// <summary>
    /// 当前时间戳(ms)
    /// </summary>
    public long CuruentMilliseconds {
        get {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
    }

    /// <summary>
    /// 拷贝对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T Copy<T>(T obj) {
        return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
    }

    /// <summary>
    /// 比较两个对象是否相同
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool Equals<T>(T? x, T? y) {
        if (x != null) {
            return x.Equals(y);
        } else if (y != null) {
            return y.Equals(x);
        }
        return true;
    }

    /// <summary>
    /// DateTime 转 Timestamp
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static long ConvertToTimestamp(DateTime value) {
        TimeSpan elapsedTime = value - Epoch;
        return (long)elapsedTime.TotalMilliseconds;
    }

    /// <summary>
    /// string 日期转 DateTime
    /// </summary>
    /// <param name="value"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public static DateTime ConvertToDateTime(string value, string pattern = "yyyy-MM-dd HH:mm:ss") {
        return Convert.ToDateTime(
            value,
            new DateTimeFormatInfo() { ShortDatePattern = pattern }
        );
    }

    /// <summary>
    /// 时间戳转 DateTime
    /// </summary>
    /// <param name="timestamp"></param>
    /// <param name="milliseconds"></param>
    /// <returns></returns>
    public static DateTime ConvertToDateTime(long timestamp, bool milliseconds = true) {
        if (milliseconds) {
            timestamp /= 1000;
        }
        long lTime = long.Parse(timestamp + "0000000");
        var toNow = new TimeSpan(lTime);
        return TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0), TimeZoneInfo.Local).Add(toNow);
    }

    /// <summary>
    /// 获取集合相同前缀
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public static string GetSamePrefix(IEnumerable<string> list) {
        if (!list.Any()) {
            return "";
        }
        var sb = new StringBuilder();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        foreach (var s in list.FirstOrDefault()) {
            foreach (var item in list) {
                if (item[sb.Length] != s) {
                    return sb.ToString();
                }
            }
            sb.Append(s);
        }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        return sb.ToString();
    }

    /// <summary>
    /// 简化 try 代码块
    /// </summary>
    /// <param name="action"></param>
    public static void Try(Action action) {
        try {
            action();
        } catch { }
    }

    /// <summary>
    /// 获取本机 IP 地址
    /// </summary>
    /// <returns></returns>
    public static string? GetLocalIpAddress() {
        try {
            using Socket socket = new(AddressFamily.InterNetwork, SocketType.Dgram, 0);
            socket.Connect("8.8.8.8", 65530);
            if (socket.LocalEndPoint is IPEndPoint endPoint) {
                return endPoint.Address.ToString();
            }
        } catch (Exception e) {
            Logger.Info(e);
        }
        return null;
    }
}

