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

        [SetUp]
        protected void SetUp()
        {
            _sourceRootFolder = @"C:\work\TRUNK_Post19\Source";
            _destinationRootFolder = @"C:\Users\JohnH\Dropbox\shelf\DS-242";
        }

        [Test]
        public void CopyManager_TypicalScenario_ShouldSucceed()
        {
            DoTypicalScenario("Copying", false);
        }

        [Test]
        public void CopyManager_PreviewOnly_ShouldSucceed()
        {
            DoTypicalScenario("Copy Preview", true);
        }

        private void DoTypicalScenario(string copyActionText, bool isPreview)
        {
            // arrange
            var helper = new CopyManagerTestHelper();
            helper.ArrangeTypicalScenario(copyActionText, _sourceRootFolder, _destinationRootFolder, "debug");

            var copier = new CopyManager(_sourceRootFolder, _destinationRootFolder, helper.MockHelper.Object, isPreview);

            // act
            var isCopySuccess = copier.CopyFiles();

            // assert
            Assert.That(isCopySuccess, Is.True, "Not Successful");
            Assert.That(copier.FileCopyLog, Is.EqualTo(helper.ExpectedFileLog.ToString()), "Wrong File List");
        }

        [Test]
        public void CopyManager_ExceptionThrown_ShouldSucceed()
        {
            // arrange
            var fileInUseText = "File in use";
            var sourceFile = Path.Combine(_sourceRootFolder, "File1.txt");
            var destinationFile = Path.Combine(_destinationRootFolder, "File1.txt");
            var expectedErrorInfo = string.Format("ERROR: Unable to copy {0}\r\nMessage: {1}\r\nDetails: System.IO.IOException: {1}\r\n", sourceFile, fileInUseText);
            var helper = new CopyManagerTestHelper();
            helper.ArrangeTypicalScenario("Copying", _sourceRootFolder, _destinationRootFolder, string.Empty);
            helper.MockHelper.Setup(x => x.FileCopy(sourceFile, destinationFile, true)).Throws(new IOException(fileInUseText));

            var copier = new CopyManager(_sourceRootFolder, _destinationRootFolder, helper.MockHelper.Object, false);

            // act
            var isCopySuccess = copier.CopyFiles();

            // assert
            Assert.That(isCopySuccess, Is.True, "Not Successful");
            Assert.That(copier.FileCopyLog.Contains(expectedErrorInfo), Is.True, "File Copy Log missing error info\r\nCopy Log:" + copier.FileCopyLog);
        }

        [Test]
        public void CopyManager_UseIgnoreList_ShouldOnlyCopyUnignoredFiles()
        {
            // arrange
            var helper = new CopyManagerTestHelper();
            helper.ArrangeTypicalScenario("Copying", _sourceRootFolder, _destinationRootFolder, "File2.txt");

            var copier = new CopyManager(_sourceRootFolder, _destinationRootFolder, helper.MockHelper.Object, false);

            // act
            var isCopySuccess = copier.CopyFiles();

            // assert
            Assert.That(isCopySuccess, Is.True, "Not Successful");
            Assert.That(copier.FileCopyLog, Is.EqualTo(helper.ExpectedFileLog.ToString()), "Wrong File List");
        }

        [Test]
        public void CopyManager_UseEmptyIgnoreList_ShouldCopyAllFiles()
        {
            // arrange
            var helper = new CopyManagerTestHelper();
            helper.ArrangeTypicalScenario("Copying", _sourceRootFolder, _destinationRootFolder, string.Empty);

            var copier = new CopyManager(_sourceRootFolder, _destinationRootFolder, helper.MockHelper.Object, false);

            // act
            var isCopySuccess = copier.CopyFiles();

            // assert
            Assert.That(isCopySuccess, Is.True, "Not Successful");
            Assert.That(copier.FileCopyLog, Is.EqualTo(helper.ExpectedFileLog.ToString()), "Wrong File List");
        }

        private class CopyManagerTestHelper
        {
            internal StringBuilder ExpectedFileLog { get; set; }
            internal Mock<IFileHelper> MockHelper { get; set; }

            internal void ArrangeTypicalScenario(string copyActionText, string sourceRootFolder, string destinationRootFolder, string ignoreList)
            {
                var ignoreFileNameList = ignoreList.Split(",".ToCharArray()).ToList();
                var testFileNameList = new string[] {"File1.txt", "File2.txt", "File3.txt",}.AsEnumerable();
                ExpectedFileLog = new StringBuilder();
                testFileNameList.ToList().
                    ForEach( x =>
                            {
                                if (!ignoreFileNameList.Exists(y => y.Equals(x, StringComparison.CurrentCultureIgnoreCase)))
                                    ExpectedFileLog.AppendFormat("{2} {0} to {1}\r\n", Path.Combine(sourceRootFolder, x), Path.Combine(destinationRootFolder, x), copyActionText);
                            });
                var testFileList = testFileNameList.Select(x => Path.Combine(sourceRootFolder, x)).ToArray();
                MockHelper = new Mock<IFileHelper>();
                MockHelper.Setup(x => x.DirectoryGetFiles(sourceRootFolder)).Returns(testFileList);
                MockHelper.Setup(x => x.IsFileReadOnly(It.IsAny<FileInfo>())).Returns(false);
                MockHelper.Setup(x => x.DirectoryGetDirectories(sourceRootFolder)).Returns((new string[] { string.Empty, }).AsEnumerable());
                var ignoreFileName = ".shelfignore.txt";
                MockHelper.Setup(x => x.FileExists(ignoreFileName)).Returns(true);
                MockHelper.Setup(x => x.FileReadAllText(ignoreFileName)).Returns(string.Join("\r\n", ignoreList));
                foreach (var file in testFileNameList)
                    MockHelper.Setup(x => x.FileCopy(Path.Combine(sourceRootFolder, file), Path.Combine(destinationRootFolder, file), true)).Returns(true);
            }
        }
    }
}
