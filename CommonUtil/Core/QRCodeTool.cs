using QRCoder;
using System.Collections.Generic;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace CommonUtil.Core {
    public enum QRCodeFormat {
        BMP,
        JPG,
        PNG,
        SVG,
        PDF,
    }

    public class QRCodeTool {
        private static readonly Dictionary<QRCodeFormat, Func<QRCodeData, byte[]>> QRCodeGeneratorDict = new();

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
        public static byte[] GenerateQRCode(string input, QRCodeFormat format = QRCodeFormat.PNG) {
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(input, QRCodeGenerator.ECCLevel.Q);
            return QRCodeGeneratorDict[format](qrCodeData);
        }

        /// <summary>
        /// 生成 PNG 格式
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static byte[] PNGQRCode(QRCodeData data) {
            var bitmap = new QRCode(data).GetGraphic(15);
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
        private static byte[] SVGQRCode(QRCodeData data) {
            return Encoding.UTF8.GetBytes(new SvgQRCode(data).GetGraphic(8));
        }

        /// <summary>
        /// 生成 PDF 格式
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static byte[] PDFQRCode(QRCodeData data) {
            return new PdfByteQRCode(data).GetGraphic(8);
        }
    }
}
