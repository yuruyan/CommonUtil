using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommonUtil.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtilTests.Utils;

namespace CommonUtil.Core.Tests;

[TestClass()]
public class Base64ToolTests {
    private const string SourceFile = "resource/ok.png";
    private const string OutputFile1 = "temp/UFM2VJHID55OHRZ6.png";
    private const string OutputFile2 = "temp/QIHJD1OKGHZY8U26.png";

    [TestMethod()]
    public void Base64DecodeString_string_Encoding() {
        Assert.AreEqual(
            Base64Encoding.Base64DecodeString("aGVsbG8gd29ybGQ=", Encoding.UTF8),
            "hello world"
        );
        Assert.AreEqual(
            Base64Encoding.Base64DecodeString("aGVsbG8gd29ybGQ=", Encoding.ASCII),
            "hello world"
        );
        Assert.AreEqual(
            Base64Encoding.Base64DecodeString("5L2g5aW977yMSGVsbG8=", Encoding.UTF8),
            "你好，Hello"
        );
    }

    [TestMethod()]
    public void Base64DecodeString_string() {
        Assert.AreEqual(
            Base64Encoding.Base64DecodeString("5L2g5aW977yMSGVsbG8="),
            "你好，Hello"
        );
        Assert.AreEqual(
            Base64Encoding.Base64DecodeString("QmFzZTY05Yqg5a+G5ZKMQmFzZTY06Kej5a+G"),
            "Base64加密和Base64解密"
        );
    }

    [TestMethod()]
    public void Base64EncodeString_string_Encoding() {
        Assert.AreEqual(
            Base64Encoding.Base64EncodeString("Base64 在线编码解码", Encoding.UTF8),
            "QmFzZTY0IOWcqOe6v+e8lueggeino+eggQ=="
        );
        Assert.AreEqual(
            Base64Encoding.Base64EncodeString("QmFzZTY0IOWcqOe6v+e8lueggeino+eggQ==", Encoding.ASCII),
            "UW1GelpUWTBJT1djcU9lNnYrZThsdWVnZ2Vpbm8rZWdnUT09"
        );
    }

    [TestMethod()]
    public void Base64EncodeString_string() {
        Assert.AreEqual(
            Base64Encoding.Base64EncodeString("请输入要进行 Base64 编码或解码的字符"),
            "6K+36L6T5YWl6KaB6L+b6KGMIEJhc2U2NCDnvJbnoIHmiJbop6PnoIHnmoTlrZfnrKY="
        );
        Assert.AreEqual(
            Base64Encoding.Base64EncodeString("编码完毕，原文本字节数：50，编码后字节数：68"),
            "57yW56CB5a6M5q+V77yM5Y6f5paH5pys5a2X6IqC5pWw77yaNTDvvIznvJbnoIHlkI7lrZfoioLmlbDvvJo2OA=="
        );
    }

    [TestMethod()]
    public void Base64EncodeFile_string() {
        File.WriteAllText(OutputFile1, Base64Encoding.Base64EncodeFile(SourceFile));
        File.WriteAllBytes(OutputFile1, Base64Encoding.Base64DecodeFile(OutputFile1));
        Assert.IsTrue(TestUtils.AreFilesEqual(SourceFile, OutputFile1));
    }

    [TestMethod()]
    public void Base64EncodeFile_string_string_CancellationToken() {
        Assert.IsTrue(Base64Encoding.Base64EncodeFile(SourceFile, OutputFile1) > 0);
        Assert.IsTrue(Base64Encoding.Base64DecodeFile(OutputFile1, OutputFile2) > 0);
        Assert.IsTrue(TestUtils.AreFilesEqual(SourceFile, OutputFile2));
    }

    [TestMethod()]
    public void Base64DecodeFile_string() {
        File.WriteAllText(OutputFile1, Base64Encoding.Base64EncodeFile(SourceFile));
        File.WriteAllBytes(OutputFile1, Base64Encoding.Base64DecodeFile(OutputFile1));
        Assert.IsTrue(TestUtils.AreFilesEqual(SourceFile, OutputFile1));
    }

    [TestMethod()]
    public void Base64DecodeFile_string_string_CancellationToken() {
        Assert.IsTrue(Base64Encoding.Base64EncodeFile(SourceFile, OutputFile1) > 0);
        Assert.IsTrue(Base64Encoding.Base64DecodeFile(OutputFile1, OutputFile2) > 0);
        Assert.IsTrue(TestUtils.AreFilesEqual(SourceFile, OutputFile2));
    }

    [TestMethod()]
    public void TryDecode() {
        Assert.IsNotNull(Base64Encoding.TryDecode("VGltZVN0YW1w"));
        Assert.IsNotNull(Base64Encoding.TryDecode("5ZCM5LiW55WM55u45LqS5Lqk6J6N55u45LqS5oiQ5bCx"));
        Assert.IsNotNull(Base64Encoding.TryDecode("5Zyo57q/YmFzZTY06Kej56CBL+e8lueggeW3peWFt+aYr+S4gOS4quWPr+S7peWwhuWtl+espuS4sui/m+ihjGJhc2U2NOino+eggS/nvJbnoIHnmoTlt6Xlhbc="));
    }
}