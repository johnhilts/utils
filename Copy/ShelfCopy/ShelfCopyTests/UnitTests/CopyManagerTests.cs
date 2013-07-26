using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using ShelfCopyLib;

namespace ShelfCopyTests.UnitTests
{
    [TestFixture]
    public class CopyManagerTests
    {
        private string _sourceRootFolder;
        private string _destinationRootFolder;

        [SetUp]
        protected void SetUp()
        {
            _sourceRootFolder = @"C:\workspaces\dev\NET45_CartRewrite\Source";
            _destinationRootFolder = @"C:\Users\JohnH\Dropbox\shelf\LSUPPORT-816";
        }

        [Test]
        public void WhenGivenValidFoldersFileCopyShouldSucceed()
        {
            // arrange
            var fileCopyLog = new string[] { "File1.txt", "File2.txt", "File3.txt", }.AsEnumerable();
            var expectedFileList = new StringBuilder();
            fileCopyLog.ToList().ForEach(x => expectedFileList.AppendFormat("Copying {0} to {1}\r\n", Path.Combine(_sourceRootFolder, x), Path.Combine(_destinationRootFolder, x)));
            Mock<IFileHelper> mockHelper = new Mock<IFileHelper>();
            mockHelper.Setup(x => x.CopyFiles(_sourceRootFolder, _destinationRootFolder, string.Empty)).Returns(fileCopyLog.Select(x => Path.Combine(_sourceRootFolder, x)));
            var copier = new CopyManager(_sourceRootFolder, _destinationRootFolder, mockHelper.Object);

            // act
            var isCopySuccess = copier.CopyFiles();

            // assert
            Assert.That(isCopySuccess, Is.True, "Not Successful");
            Assert.That(copier.FileCopyLog, Is.EqualTo(expectedFileList.ToString()), "Wrong File List");
        }
    }
}
