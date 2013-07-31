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
    public class FileHelperTests
    {
        private string _sourceRootFolder;
        private string _destinationRootFolder;

        [SetUp]
        protected void SetUp()
        {
            _sourceRootFolder = @"C:\work\TRUNK_Post19\Source";
            _destinationRootFolder = @"C:\Users\JohnH\Dropbox\shelf\DS-242";
        }

        [Test]
        public void WhenGivenValidFileListFileCopyShouldSucceed()
        {
            // arrange
            var actualFileList = new StringBuilder();
            var sourceFile = Path.Combine(_sourceRootFolder, "File1.txt");
            var destinationFile = Path.Combine(_destinationRootFolder, "File1.txt");
            var expectedFileList = new string[] {sourceFile};
            var projectName = "testProject.csproj";
            var mockWrapper = new Mock<IFileWrapper>();
            mockWrapper.Setup(x => x.DirectoryGetFiles(_sourceRootFolder)).Returns(expectedFileList);
            var fileInfo = new FileInfo(sourceFile);
            mockWrapper.Setup(x => x.IsFileReadOnly(fileInfo)).Returns(false);
            mockWrapper.Setup(x => x.DirectoryGetDirectories(_sourceRootFolder)).Returns((new string[] { string.Empty, }).AsEnumerable());
            mockWrapper.Setup(x => x.FileReadAllText(projectName)).Returns(sourceFile);
            mockWrapper.Setup(x => x.FileCopy(sourceFile, destinationFile, true)).Returns(true);
            var fileHelper = new FileHelper(mockWrapper.Object);

            // act
            foreach (var fileName in fileHelper.CopyFiles(_sourceRootFolder, _destinationRootFolder, projectName))
                actualFileList.Append(fileName);

            // assert
            Assert.That(actualFileList.ToString(), Is.EqualTo(expectedFileList[0]), "File Lists don't match");
        }

        [Test]
        public void WhenFileCopyThrowsExceptionShouldStillSucceed()
        {
            // arrange
            var actualFileList = new StringBuilder();
            var sourceFile = Path.Combine(_sourceRootFolder, "File1.txt");
            var destinationFile = Path.Combine(_destinationRootFolder, "File1.txt");
            var expectedFileList = new string[] {sourceFile};
            var expectedFileInfo = string.Format("{0}\r\nERROR: Unable to copy {0}\r\nMessage: File in use\r\nDetails: System.IO.IOException: File in use\r\n", expectedFileList[0]);
            var projectName = "testProject.csproj";
            var mockWrapper = new Mock<IFileWrapper>();
            mockWrapper.Setup(x => x.DirectoryGetFiles(_sourceRootFolder)).Returns(expectedFileList);
            var fileInfo = new FileInfo(sourceFile);
            mockWrapper.Setup(x => x.IsFileReadOnly(fileInfo)).Returns(false);
            mockWrapper.Setup(x => x.DirectoryGetDirectories(_sourceRootFolder)).Returns((new string[] { string.Empty, }).AsEnumerable());
            mockWrapper.Setup(x => x.FileReadAllText(projectName)).Returns(sourceFile);
            mockWrapper.Setup(x => x.FileCopy(sourceFile, destinationFile, true)).Throws(new IOException("File in use"));
            var fileHelper = new FileHelper(mockWrapper.Object);

            // act
            foreach (var fileName in fileHelper.CopyFiles(_sourceRootFolder, _destinationRootFolder, projectName))
                actualFileList.Append(fileName);

            // assert
            Assert.That(actualFileList.ToString().StartsWith(expectedFileInfo), Is.True, "File Info doesn't match");
        }
    }
}
