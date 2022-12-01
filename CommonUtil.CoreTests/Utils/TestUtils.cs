using CommonUtil.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtilTests.Utils;

internal static class TestUtils {
    public static bool AreFilesEqual(string file1, string file2) {
        using var stream1 = File.OpenRead(file1);
        using var stream2 = File.OpenRead(file2);
        return DataDigest.MD5Digest(stream1) == DataDigest.MD5Digest(stream2);
    }
}
