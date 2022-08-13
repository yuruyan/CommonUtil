using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using static QRCoder.PayloadGenerator;
using static QRCoder.QRCodeGenerator;

namespace CommonUtil.Core;

public enum QRCodeFormat {
    BMP,
    JPG,
    PNG,
    SVG,
    PDF,
}

public struct QRCodeInfo {
    public ECCLevel ECCLevel { get; set; } = ECCLevel.Q;
    public byte PixelPerModule { get; set; } = 20;
    public Color Foreground { get; set; } = Color.Black;

    public QRCodeInfo() {
    }

    public QRCodeInfo(ECCLevel eCCLevel, byte pixelPerModule, Color foreground) {
        ECCLevel = eCCLevel;
        PixelPerModule = pixelPerModule;
        Foreground = foreground;
    }
}

public class QRCodeTool {
    private static readonly Dictionary<QRCodeFormat, Func<string, QRCodeInfo, byte[]>> QRCodeGeneratorDict = new();

    static QRCodeTool() {
        QRCodeGeneratorDict[QRCodeFormat.BMP] = PNGQRCode;
        QRCodeGeneratorDict[QRCodeFormat.JPG] = PNGQRCode;
        QRCodeGeneratorDict[QRCodeFormat.PNG] = PNGQRCode;
        QRCodeGeneratorDict[QRCodeFormat.SVG] = SVGQRCode;
        QRCodeGeneratorDict[QRCodeFormat.PDF] = PDFQRCode;
    }

    /// <summary>
    /// 生成文本 QRCode
    /// </summary>
    /// <param name="input"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static byte[] GenerateQRCodeForText(
        string input,
        QRCodeInfo qRCodeInfo,
        QRCodeFormat format = QRCodeFormat.PNG
    ) {
        return QRCodeGeneratorDict[format](input, qRCodeInfo);
    }

    /// <summary>
    /// 生成 sms QRCode
    /// </summary>
    /// <param name="receiver">收件人</param>
    /// <param name="message">信息</param>
    /// <param name="qRCodeInfo"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static byte[] GenerateQRCodeForSMS(
        string receiver,
        string message,
        QRCodeInfo qRCodeInfo,
        QRCodeFormat format = QRCodeFormat.PNG
    ) {
        var generator = new SMS(receiver, message);
        return QRCodeGeneratorDict[format](generator.ToString(), qRCodeInfo);
    }

    /// <summary>
    /// 生成 Mail QRCode
    /// </summary>
    /// <param name="receiver">收件人</param>
    /// <param name="subject">主题</param>
    /// <param name="message">信息</param>
    /// <param name="qRCodeInfo"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static byte[] GenerateQRCodeForMail(
        string receiver,
        string subject,
        string message,
        QRCodeInfo qRCodeInfo,
        QRCodeFormat format = QRCodeFormat.PNG
    ) {
        var generator = new Mail(receiver, subject, message);
        return QRCodeGeneratorDict[format](generator.ToString(), qRCodeInfo);
    }

    /// <summary>
    /// 生成 Phonenumber QRCode
    /// </summary>
    /// <param name="receiver">收件人</param>
    /// <param name="qRCodeInfo"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static byte[] GenerateQRCodeForPhonenumber(
        string receiver,
        QRCodeInfo qRCodeInfo,
        QRCodeFormat format = QRCodeFormat.PNG
    ) {
        var generator = new PhoneNumber(receiver);
        return QRCodeGeneratorDict[format](generator.ToString(), qRCodeInfo);
    }

    /// <summary>
    /// 生成 Geolocation QRCode
    /// </summary>
    /// <param name="longitude">经度</param>
    /// <param name="latitude">纬度</param>
    /// <param name="qRCodeInfo"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static byte[] GenerateQRCodeForGeolocation(
        double longitude,
        double latitude,
        QRCodeInfo qRCodeInfo,
        QRCodeFormat format = QRCodeFormat.PNG
    ) {
        var generator = new Geolocation(latitude.ToString(), longitude.ToString());
        return QRCodeGeneratorDict[format](generator.ToString(), qRCodeInfo);
    }

    /// <summary>
    /// 生成 wifi QRCode
    /// </summary>
    /// <param name="name">wifi 名称</param>
    /// <param name="password">wifi 密码</param>
    /// <param name="qRCodeInfo"></param>
    /// <param name="authentication">加密方式</param>
    /// <param name="isWifiHidden">是否是隐藏 wifi</param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static byte[] GenerateQRCodeForWiFi(
        string name,
        string password,
        QRCodeInfo qRCodeInfo,
        WiFi.Authentication authentication = WiFi.Authentication.WPA,
        bool isWifiHidden = false,
        QRCodeFormat format = QRCodeFormat.PNG
    ) {
        var generator = new WiFi(
            name,
            password,
            authentication,
            isWifiHidden,
            escapeHexStrings: false
        );
        return QRCodeGeneratorDict[format](generator.ToString(), qRCodeInfo);
    }

    /// <summary>
    /// 生成 PNG 格式
    /// </summary>
    /// <param name="payload"></param>
    /// <returns></returns>
    private static byte[] PNGQRCode(string payload, QRCodeInfo qRCodeInfo) {
        var generator = new QRCodeGenerator();
        var data = generator.CreateQrCode(payload, qRCodeInfo.ECCLevel);
        var bitmap = new QRCode(data).GetGraphic(
            qRCodeInfo.PixelPerModule,
            qRCodeInfo.Foreground,
            Color.Transparent,
            true
        );
        var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
        bitmap.Dispose();
        byte[] buffer = new byte[stream.Length];
        stream.Seek(0, SeekOrigin.Begin);
        stream.Read(buffer, 0, buffer.Length);
        stream.Close();
        return buffer;
    }

    /// <summary>
    /// 生成 SVG 格式
    /// </summary>
    /// <param name="payload"></param>
    /// <returns></returns>
    private static byte[] SVGQRCode(string payload, QRCodeInfo qRCodeInfo) {
        var generator = new QRCodeGenerator();
        var data = generator.CreateQrCode(payload, qRCodeInfo.ECCLevel);
        return Encoding.UTF8.GetBytes(new SvgQRCode(data).GetGraphic(
            qRCodeInfo.PixelPerModule,
            qRCodeInfo.Foreground,
            Color.Transparent,
            true
        ));
    }

    /// <summary>
    /// 生成 PDF 格式
    /// </summary>
    /// <param name="payload"></param>
    /// <returns></returns>
    private static byte[] PDFQRCode(string payload, QRCodeInfo qRCodeInfo) {
        var generator = new QRCodeGenerator();
        var data = generator.CreateQrCode(payload, qRCodeInfo.ECCLevel);
        return new PdfByteQRCode(data).GetGraphic(
            qRCodeInfo.PixelPerModule,
            qRCodeInfo.Foreground.ToString(),
            Color.Transparent.ToString()
        );
    }
}
