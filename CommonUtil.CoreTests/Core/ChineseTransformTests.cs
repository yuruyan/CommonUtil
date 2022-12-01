using CommonUtilTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonUtil.Core.Tests;

[TestClass()]
public class ChineseTransformTests {
    private const string SimplifiedFile = "resource/ChineseTransformTests/Simplified.txt";
    private const string TraditionalFile = "resource/ChineseTransformTests/Traditional.txt";
    private const string OutputFile1 = "temp/8WW94tSTmmdVjkNR.png";

    [TestMethod()]
    public void InitializeExplicitly() {
        ChineseTransform.InitializeExplicitly();
    }

    [TestMethod()]
    public void ToTraditional_string() {
        Assert.AreEqual(
            ChineseTransform.ToTraditional("噗，拟声词，水、气挤出的声音。"),
            "噗，擬聲詞，水、気擠齣的聲音。"
        );
        Assert.AreEqual(
            ChineseTransform.ToTraditional("【噗通】 pūtōng 同“扑通”重物落地或落水声，心跳的声音。"),
            "【噗通】 pūtōng 衕“撲通”重物落地或落水聲，心跳的聲音。"
        );
        Assert.AreEqual(
            ChineseTransform.ToTraditional("2020年7月29日 1 简体转繁体：选中需要转换的内容"),
            "2020年7月29日 1 簡躰轉緐躰：選中需要轉換的內容"
        );
    }

    [TestMethod()]
    public void ToTraditional_CharArray() {
        Assert.AreEqual(
            new string(ChineseTransform.ToTraditional("噗，拟声词，水、气挤出的声音。".ToCharArray())),
            "噗，擬聲詞，水、気擠齣的聲音。"
        );
        Assert.AreEqual(
            new string(ChineseTransform.ToTraditional("【噗通】 pūtōng 同“扑通”重物落地或落水声，心跳的声音。".ToCharArray())),
            "【噗通】 pūtōng 衕“撲通”重物落地或落水聲，心跳的聲音。"
        );
        Assert.AreEqual(
            new string(ChineseTransform.ToTraditional("2020年7月29日 1 简体转繁体：选中需要转换的内容".ToCharArray())),
            "2020年7月29日 1 簡躰轉緐躰：選中需要轉換的內容"
        );
    }

    [TestMethod()]
    public void ToTraditional_CharArray_int_int() {
        Assert.AreEqual(
            new string(ChineseTransform.ToTraditional("噗，拟声词，水、气挤出的声音。".ToCharArray(), 0, 7)),
            "噗，擬聲詞，水、气挤出的声音。"
        );
        Assert.AreEqual(
            new string(ChineseTransform.ToTraditional("【噗通】 pūtōng 同“扑通”重物落地或落水声，心跳的声音。".ToCharArray(), 7, 7)),
            "【噗通】 pūtōng 衕“扑通”重物落地或落水声，心跳的声音。"
        );
        Assert.AreEqual(
            new string(ChineseTransform.ToTraditional("2020年7月29日 1 简体转繁体：选中需要转换的内容".ToCharArray(), 10, 0)),
            "2020年7月29日 1 简体转繁体：选中需要转换的内容"
        );
    }

    [TestMethod()]
    public void ToSimplified_string() {
        Assert.AreEqual(
            ChineseTransform.ToSimplified("噗，擬聲詞，水、気擠齣的聲音。"),
            "噗，拟声词，水、气挤出的声音。"
        );
        Assert.AreEqual(
            ChineseTransform.ToSimplified("【噗通】 pūtōng 衕“撲通”重物落地或落水聲，心跳的聲音。"),
            "【噗通】 pūtōng 同“扑通”重物落地或落水声，心跳的声音。"
        );
        Assert.AreEqual(
            ChineseTransform.ToSimplified("2020年7月29日 1 簡躰轉緐躰：選中需要轉換的內容"),
            "2020年7月29日 1 简体转繁体：选中需要转换的内容"
        );
    }

    [TestMethod()]
    public void ToSimplified_CharArray() {
        Assert.AreEqual(
            new string(ChineseTransform.ToSimplified("噗，擬聲詞，水、気擠齣的聲音。".ToCharArray())),
            "噗，拟声词，水、气挤出的声音。"
        );
        Assert.AreEqual(
            new string(ChineseTransform.ToSimplified("【噗通】 pūtōng 衕“撲通”重物落地或落水聲，心跳的聲音。".ToCharArray())),
            "【噗通】 pūtōng 同“扑通”重物落地或落水声，心跳的声音。"
        );
        Assert.AreEqual(
            new string(ChineseTransform.ToSimplified("2020年7月29日 1 簡躰轉緐躰：選中需要轉換的內容".ToCharArray())),
            "2020年7月29日 1 简体转繁体：选中需要转换的内容"
        );
    }

    [TestMethod()]
    public void ToSimplified_CharArray_int_int() {
        Assert.AreEqual(
            new string(ChineseTransform.ToSimplified("噗，擬聲詞，水、気擠齣的聲音。".ToCharArray(), 0, 7)),
            "噗，拟声词，水、気擠齣的聲音。"
        );
        Assert.AreEqual(
            new string(ChineseTransform.ToSimplified("【噗通】 pūtōng 衕“扑通”重物落地或落水声，気擠齣的聲音。".ToCharArray(), 7, 7)),
            "【噗通】 pūtōng 同“扑通”重物落地或落水声，気擠齣的聲音。"
        );
        Assert.AreEqual(
            new string(ChineseTransform.ToSimplified("2020年7月29日 1 簡躰轉緐躰：选中需要转换的内容".ToCharArray(), 10, 0)),
            "2020年7月29日 1 簡躰轉緐躰：选中需要转换的内容"
        );
    }

    [TestMethod()]
    public void FileToTraditional() {
        ChineseTransform.FileToTraditional(SimplifiedFile, OutputFile1);
        TestUtils.AreFilesEqual(TraditionalFile, OutputFile1);
    }

    [TestMethod()]
    public void FileToSimplified() {
        ChineseTransform.FileToSimplified(TraditionalFile, OutputFile1);
        TestUtils.AreFilesEqual(SimplifiedFile, OutputFile1);
    }
}