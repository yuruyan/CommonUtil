using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleFileSystemServer.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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