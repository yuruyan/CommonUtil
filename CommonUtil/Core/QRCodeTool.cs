﻿using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
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
    private static readonly Dictionary<QRCodeFormat, Func<QRCodeData, QRCodeInfo, byte[]>> QRCodeGeneratorDict = new();

    static QRCodeTool() {
        QRCodeGeneratorDict[QRCodeFormat.BMP] = PNGQRCode;
        QRCodeGeneratorDict[QRCodeFormat.JPG] = PNGQRCode;
        QRCodeGeneratorDict[QRCodeFormat.PNG] = PNGQRCode;
        QRCodeGeneratorDict[QRCodeFormat.SVG] = SVGQRCode;
        QRCodeGeneratorDict[QRCodeFormat.PDF] = PDFQRCode;
    }

    /// <summary>
    /// 生成QRCode
    /// </summary>
    /// <param name="input"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static byte[] GenerateQRCodeForText(string input, QRCodeInfo qRCodeInfo, QRCodeFormat format = QRCodeFormat.PNG) {
        var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(input, QRCodeGenerator.ECCLevel.Q);
        return QRCodeGeneratorDict[format](qrCodeData, qRCodeInfo);
    }

    /// <summary>
    /// 生成 PNG 格式
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private static byte[] PNGQRCode(QRCodeData data, QRCodeInfo qRCodeInfo) {
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
    /// <param name="data"></param>
    /// <returns></returns>
    private static byte[] SVGQRCode(QRCodeData data, QRCodeInfo qRCodeInfo) {
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
    /// <param name="data"></param>
    /// <returns></returns>
    private static byte[] PDFQRCode(QRCodeData data, QRCodeInfo qRCodeInfo) {
        return new PdfByteQRCode(data).GetGraphic(
            qRCodeInfo.PixelPerModule,
            qRCodeInfo.Foreground.ToString(),
            Color.Transparent.ToString()
        );
    }
}
