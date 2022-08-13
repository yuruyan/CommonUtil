using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimpleFileSystemServer.Service.Tests {
    [TestClass()]
    public class FileServiceTests {
        private FileService FileService = new();

        [TestMethod()]
        public void ListFilesTest() {
            foreach (var file in FileService.ListFiles("/")) {
                Console.WriteLine(file);
            }
        }
    }
}