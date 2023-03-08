using CommonTools.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleFileSystemServerTests.Utils;

namespace SimpleFileSystemServer.Service.Tests;

[TestClass()]
public class FileServiceTests {

    [TestInitialize()]
    public void Before() {
        TestUtils.Initialize();
    }

    [TestMethod()]
    public void ListFilesTest() {
        var ls = FileService.ListFiles("/");
        ls.Print();
        Assert.IsTrue(ls.Count > 0);
    }
}