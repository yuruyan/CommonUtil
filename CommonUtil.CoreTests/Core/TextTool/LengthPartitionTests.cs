using CommonTools.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonUtil.Core.Tests;

[TestClass()]
public class LengthPartitionTests {
    [TestMethod()]
    public void SplitTest() {
        var list = LengthPartition.Split("hell", 2);
        Console.WriteLine(list.Count);
        list.Print();
    }
}