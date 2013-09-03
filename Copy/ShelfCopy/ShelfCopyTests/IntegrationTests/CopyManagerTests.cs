using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Moq;
using NUnit.Framework;
using ShelfCopyLib;

namespace ShelfCopyTests.IntegrationTests
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
        public void CopyManager_TypicalScenario_ShouldSucceed()
        {
            // arrange
            var regex = new Regex(@"Copying (.+) to", RegexOptions.IgnoreCase);
            var copier = new CopyManager(_sourceRootFolder, _destinationRootFolder, new FileHelper());

            // act
            var isCopySuccess = copier.CopyFiles();

            // assert
            Assert.That(isCopySuccess, Is.True, "Not Successful");
            Assert.That(copier.FileCopyLog, Is.Not.Null, "NULL File List");
            Assert.That(copier.FileCopyLog.Length, Is.GreaterThan(0), "EMPTY File List");
            Console.WriteLine("File List:\r\n");
            Console.WriteLine(copier.FileCopyLog);
            foreach (var file in copier.FileCopyLog.Split("\r\n".ToCharArray()))
            {
                if (!string.IsNullOrEmpty(file))
                {
                    var sourceFileName = regex.Match(file).Groups[1].Value;
                    Assert.That(File.Exists(sourceFileName), Is.True, string.Format("File: {0} does not exists", sourceFileName));
                    var destinationFileName = sourceFileName.Replace(_sourceRootFolder, _destinationRootFolder);
                    var destinationFileInfo = new FileInfo(destinationFileName);
                    Assert.That(destinationFileInfo.Directory.Exists, Is.True, string.Format("Destination folder {0} does not exist", destinationFileInfo.DirectoryName));
                    Assert.That(File.Exists(destinationFileName), Is.True, string.Format("File: {0} does not exists", destinationFileName));
                    var sourceFileInfo = new FileInfo(sourceFileName);
                    var isReadonly = sourceFileInfo.Attributes.HasFlag(FileAttributes.ReadOnly);
                    Assert.That(isReadonly, Is.False, string.Format("File: {0} is read-only", sourceFileName)); // NOTE: this only works if there are in fact files checked out
                }
            }
        }
    }
}
