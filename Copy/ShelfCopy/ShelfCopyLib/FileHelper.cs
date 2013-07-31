using System;
using System.IO;
using System.Collections.Generic;

namespace ShelfCopyLib
{
    public class FileHelper : IFileHelper
    {
        private IFileWrapper _fileWrapper;

        public FileHelper()
            : this(new FileWrapper())
        {

        }

        public FileHelper(IFileWrapper fileWrapper)
        {
            _fileWrapper = fileWrapper;
        }

        public void SetCurrentDirectory(string sourceRootFolder)
        {
            Directory.SetCurrentDirectory(sourceRootFolder);
        }

        public IEnumerable<string> CopyFiles(string sourceRootFolder, string destinationRootFolder, string projectName)
        {
            foreach (var sourceFile in _fileWrapper.DirectoryGetFiles(sourceRootFolder))
            {
                var fileInfo = new FileInfo(sourceFile);

                //if (fileInfo.Extension.Equals(".sln", StringComparison.CurrentCultureIgnoreCase) || fileInfo.Extension.Equals(".csproj", StringComparison.CurrentCultureIgnoreCase))
                //    projectName = sourceFile;

                if (fileInfo.Extension.Equals(".csproj", StringComparison.CurrentCultureIgnoreCase))
                    projectName = sourceFile;

                if (!(_fileWrapper.IsFileReadOnly(fileInfo)) && IsFileInProject(fileInfo.Name, fileInfo.Extension, projectName))
                {
                    yield return sourceFile;
                    var destinationFile = sourceFile.Replace(sourceRootFolder, destinationRootFolder);
                    if (!Directory.Exists(destinationRootFolder))
                        Directory.CreateDirectory(destinationRootFolder);
                    var errorOccurred = false;
                    IOException exceptionInfo = null;
                    try
                    {
                        _fileWrapper.FileCopy(sourceFile, destinationFile, true);
                    }
                    catch (IOException e)
                    {
                        errorOccurred = true;
                        exceptionInfo = e;
                    }
                    if (errorOccurred)
                        yield return string.Format("\r\nERROR: Unable to copy {0}\r\nMessage: {1}\r\nDetails: {2}", sourceFile, exceptionInfo.Message, exceptionInfo.ToString());
                }
            }

            foreach (var directory in _fileWrapper.DirectoryGetDirectories(sourceRootFolder))
            {
                var directoryName = directory.Replace(sourceRootFolder + "\\", string.Empty);
                foreach (var file in CopyFiles(directory, Path.Combine(destinationRootFolder, directoryName), projectName))
                {
                    yield return file;
                }
            }
        }

        private bool IsFileInProject(string fileName, string extension, string projectName)
        {
            if (string.IsNullOrEmpty(projectName))
                return false;

            if (extension.Equals(".dll", StringComparison.OrdinalIgnoreCase))
                return false;

            if (extension.Equals(".sln", StringComparison.OrdinalIgnoreCase) || extension.Equals(".csproj", StringComparison.OrdinalIgnoreCase))
                return true;

            string projectText = _fileWrapper.FileReadAllText(projectName).ToLower();
            return projectText.Contains(fileName.ToLower());
        }
    }
}