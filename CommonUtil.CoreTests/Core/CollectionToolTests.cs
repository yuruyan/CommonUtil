using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonUtil.Core.Tests;

[TestClass()]
public class CollectionToolTests {
    private static readonly IReadOnlyList<(IEnumerable<string>, IEnumerable<string>)> SourceData = new (IEnumerable<string>, IEnumerable<string>)[] {
        (new [] {""}, new [] {""}),
        (new [] {"123"}, Array.Empty<string>()),
        (Array.Empty<string>(), Array.Empty<string>()),
        (new [] {"1","2"}, new [] {"1","3","13","2"}),
        (new [] {"abc","123","abcd"}, new [] {"abc ","xyz"," \t "}),
        (new [] {"\0\0\uffff","\0","\uffff","\u0001"}, new [] {"\u0000","\0","\b\n\r"}),
        (new [] {"abcdefg。","_+?*",}, new [] {".?xyz","gbk","utf-8"}),
    };
    private static readonly IReadOnlyList<IEnumerable<string>> IntersectResults = new IEnumerable<string>[] {
        new string[] {""},
        Array.Empty<string>(),
        Array.Empty<string>(),
        new string[] {"1","2"},
        Array.Empty<string>(),
        new string[] {"\0"},
        Array.Empty<string>(),
    };
    private static readonly IReadOnlyList<IEnumerable<string>> ExceptResults = new IEnumerable<string>[] {
        Array.Empty<string>(),
        new string[] {"123"},
        Array.Empty<string>(),
        Array.Empty<string>(),
        new string[]{"abc","123","abcd"},
        new string[] {"\0\0\uffff","\uffff","\u0001"},
        new [] {"abcdefg。","_+?*",},
    };
    private static readonly IReadOnlyList<IEnumerable<string>> UnionResults = new IEnumerable<string>[] {
        new string[] {""},
        new [] {"123"},
        Array.Empty<string>(),
        new string[] {"1","13","2","3"},
        new [] {" \t ","123","abc","abc ","abcd","xyz",},
        new string[] {"\0", "\u0001", "\b\n\r", "\0\0\uffff", "\uffff"},
        new [] {"_+?*",".?xyz","abcdefg。","gbk","utf-8"},
    };

    private void TestResult(IReadOnlyList<IEnumerable<string>> expectedResults, Func<IEnumerable<string>, IEnumerable<string>, IList<string>> func) {
        for (int i = 0; i < SourceData.Count; i++) {
            var expected = expectedResults[i].ToArray();
            var actual = func(SourceData[i].Item1, SourceData[i].Item2).ToArray();
            Array.Sort(expected);
            Array.Sort(actual);
            Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
        }
    }

    [TestMethod()]
    public void Intersect() {
        TestResult(IntersectResults, CollectionTool.Intersect);
    }

    [TestMethod()]
    public void Except() {
        TestResult(ExceptResults, CollectionTool.Except);
    }

    [TestMethod()]
    public void Union() {
        TestResult(UnionResults, CollectionTool.Union);
    }
}