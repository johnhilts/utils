using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private string _projectName;
        private string _projectFile;

        [SetUp]
        protected void SetUp()
        {
            _sourceRootFolder = @"C:\work\TRUNK_Post19\Source";
            _destinationRootFolder = @"C:\Users\JohnH\Dropbox\shelf\DS-242";
            _projectName = "testProject.csproj";
            _projectFile = Path.Combine(_sourceRootFolder, _projectName);
        }

        [Test]
        public void WhenGivenValidFileListFileCopyShouldSucceed()
        {
            // arrange
            var testFileNameList = new string[] { _projectName, "File1.txt", "File2.txt", "File3.txt", }.AsEnumerable();
            var expectedFileLog = new StringBuilder();
            testFileNameList.ToList().ForEach(x => expectedFileLog.AppendFormat("Copying {0} to {1}\r\n", Path.Combine(_sourceRootFolder, x), Path.Combine(_destinationRootFolder, x)));
            var testFileList = testFileNameList.Select(x => Path.Combine(_sourceRootFolder, x)).ToArray();
            var mockHelper = new Mock<IFileHelper>();
            mockHelper.Setup(x => x.DirectoryGetFiles(_sourceRootFolder)).Returns(testFileList);
            mockHelper.Setup(x => x.IsFileReadOnly(It.IsAny<FileInfo>())).Returns(false);
            mockHelper.Setup(x => x.DirectoryGetDirectories(_sourceRootFolder)).Returns((new string[] { string.Empty, }).AsEnumerable());
            mockHelper.Setup(x => x.FileReadAllText(_projectFile)).Returns(string.Join(",", testFileList));
            foreach (var file in testFileNameList)
                mockHelper.Setup(x => x.FileCopy(Path.Combine(_sourceRootFolder, file), Path.Combine(_destinationRootFolder, file), true)).Returns(true);

            var copier = new CopyManager(_sourceRootFolder, _destinationRootFolder, mockHelper.Object);

            // act
            var isCopySuccess = copier.CopyFiles();

            // assert
            Assert.That(isCopySuccess, Is.True, "Not Successful");
            Assert.That(copier.FileCopyLog, Is.EqualTo(expectedFileLog.ToString()), "Wrong File List");
        }

        [Test]
        public void WhenFileCopyThrowsExceptionShouldStillSucceed()
        {
            // arrange
            var sourceFile = Path.Combine(_sourceRootFolder, "File1.txt");
            var destinationFile = Path.Combine(_destinationRootFolder, "File1.txt");
            var expectedFileList = new string[] { _projectFile, sourceFile };
            var expectedErrorInfo = string.Format("ERROR: Unable to copy {0}\r\nMessage: File in use\r\nDetails: System.IO.IOException: File in use\r\n", expectedFileList[1]);
            var mockHelper = new Mock<IFileHelper>();
            mockHelper.Setup(x => x.DirectoryGetFiles(_sourceRootFolder)).Returns(expectedFileList);
            mockHelper.Setup(x => x.IsFileReadOnly(It.IsAny<FileInfo>())).Returns(false);
            mockHelper.Setup(x => x.DirectoryGetDirectories(_sourceRootFolder)).Returns((new string[] { string.Empty, }).AsEnumerable());
            mockHelper.Setup(x => x.FileReadAllText(_projectFile)).Returns(sourceFile);
            mockHelper.Setup(x => x.FileCopy(sourceFile, destinationFile, true)).Throws(new IOException("File in use"));

            var copier = new CopyManager(_sourceRootFolder, _destinationRootFolder, mockHelper.Object);

            // act
            var isCopySuccess = copier.CopyFiles();

            // assert
            Assert.That(isCopySuccess, Is.True, "Not Successful");
            Assert.That(copier.FileCopyLog.Contains(expectedErrorInfo), Is.True, "File Copy Log missing error info\r\nCopy Log:" + copier.FileCopyLog);
        }
    }
}
