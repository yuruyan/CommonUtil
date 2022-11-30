using CommonUITools.Utils;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;
using ZXing.Common;
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

public class QRCodeInfo {
    public ECCLevel ECCLevel { get; set; } = ECCLevel.Q;
    public byte PixelPerModule { get; set; } = 20;
    public Color Foreground { get; set; } = Color.Black;
    public Bitmap? Image { get; set; }

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
    /// 解析二维码
    /// </summary>
    /// <param name="filepath"></param>
    /// <returns>失败返回 null</returns>
    public static string? DecodeQRCode(string filepath) {
        var reader = new ZXing.ZKWeb.BarcodeReader {
            Options = new DecodingOptions {
                CharacterSet = "UTF-8"
            }
        };
        // 加载图像
        using var bitmap = new System.DrawingCore.Bitmap(filepath);
        return reader.Decode(bitmap)?.Text;
    }

    /// <summary>
    /// 解析 QRCode
    /// </summary>
    /// <param name="stream"></param>
    /// <returns>失败返回 null</returns>
    public static string? DecodeQRCode(Stream stream) {
        var reader = new ZXing.ZKWeb.BarcodeReader {
            Options = new DecodingOptions {
                CharacterSet = "UTF-8"
            }
        };
        // 解析图像
        return reader.Decode(new System.DrawingCore.Bitmap(stream))?.Text;
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
    /// <param name="phoneNumber">收件人</param>
    /// <param name="qRCodeInfo"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static byte[] GenerateQRCodeForPhonenumber(
        string phoneNumber,
        QRCodeInfo qRCodeInfo,
        QRCodeFormat format = QRCodeFormat.PNG
    ) {
        var generator = new PhoneNumber(phoneNumber);
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
            icon: qRCodeInfo.Image,
            iconBackgroundColor: Color.White,
            iconBorderWidth: 1,
            drawQuietZones: true
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
            logo: new(qRCodeInfo.Image),
            drawQuietZones: true
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
            $"#{UIUtils.DrawingColorToColor(qRCodeInfo.Foreground).ToString()[3..]}",
            "#FFFFFF"
        );
    }
}
