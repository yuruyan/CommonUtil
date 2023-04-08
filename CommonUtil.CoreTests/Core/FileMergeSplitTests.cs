using CommonTools.Utils;
using CommonUtil.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonUtil.CoreTests.Core;

[TestClass()]
public class FileMergeSplitTests {
    private static readonly string[] Files = {
        @"Resource\DataDigestTests\test1.txt",
        @"Resource\DataDigestTests\test2.lnk",
        @"Resource\DataDigestTests\test3.zip",
        @"Resource\DataDigestTests\test4.txt",
        @"Resource\DataDigestTests\test5.docx",
        @"Resource\DataDigestTests\test6.vsdx",
        @"Resource\DataDigestTests\test7.xlsx",
        @"Resource\DataDigestTests\test8.jpg",
    };
    private readonly string FileMergeSplitDirectory = @$"Resource\FileMergeSplitTests-{Random.Shared.Next()}";
    private readonly string FileSplitDirectory;
    private readonly string FileMergeDirectory;

    public FileMergeSplitTests() {
        FileSplitDirectory = Path.Combine(FileMergeSplitDirectory, "FileSplit");
        FileMergeDirectory = Path.Combine(FileMergeSplitDirectory, "FileMerge");
    }

    [TestMethod()]
    public void Run() {
        var splitFileDirs = SplitFileTest();
        var mergeFiles = MergeFileTest(splitFileDirs);
        var result = Validate(mergeFiles).All(r => r);
        Directory.Delete(FileMergeSplitDirectory, true);
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 验证
    /// </summary>
    /// <param name="mergedFiles"></param>
    /// <returns></returns>
    private bool[] Validate(string[] mergedFiles) {
        var results = new bool[mergedFiles.Length];
        for (int i = 0; i < mergedFiles.Length; i++) {
            results[i] = DigestUtils.MD5Digest(Files[i]) == DigestUtils.MD5Digest(mergedFiles[i]);
        }
        return results;
    }

    /// <summary>
    /// 文件分割
    /// </summary>
    /// <returns>生成的文件夹</returns>
    private string[] SplitFileTest() {
        var outputDirs = new string[Files.Length];
        Files.ForEach((index, file) => {
            var fileName = Path.GetFileNameWithoutExtension(file);
            var outputDir = Path.Combine(FileSplitDirectory, fileName);
            Directory.CreateDirectory(outputDir);
            outputDirs[index] = outputDir;
            FileMergeSplit.SplitFile(
                file,
                outputDir,
                1024
            );
        });
        return outputDirs;
    }

    /// <summary>
    /// 合并文件
    /// </summary>
    /// <param name="inputDirs"></param>
    /// <returns></returns>
    private string[] MergeFileTest(string[] inputDirs) {
        var files = Files.Select(f => {
            return Path.Combine(FileMergeDirectory, Path.GetFileName(f));
        }).ToArray();
        Directory.CreateDirectory(FileMergeDirectory);
        for (int i = 0; i < inputDirs.Length; i++) {
            FileMergeSplit.MergeFile(
                Directory.GetFiles(inputDirs[i]).Order(),
                files[i]
            );
        }
        return files;
    }
}