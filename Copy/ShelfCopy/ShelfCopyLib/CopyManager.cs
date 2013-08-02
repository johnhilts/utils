using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShelfCopyLib
{
    // handles file copy operations
    public class CopyManager
    {
        private readonly string _sourceRootFolder;
        private readonly string _destinationRootFolder;
        private readonly StringBuilder _fileCopyLog = new StringBuilder();
        private IFileHelper _fileHelper;

        public string FileCopyLog
        {
            get { return _fileCopyLog.ToString(); }
        }

        public CopyManager(string sourceRootFolder, string destinationRootFolder, IFileHelper fileHelper)
        {
            _sourceRootFolder = sourceRootFolder;
            _destinationRootFolder = destinationRootFolder;
            _fileHelper = fileHelper;
        }

        /// <summary>
        /// Copy Files from source root folder to destination root folder
        /// </summary>
        /// <returns>true if copy succeeded</returns>
        /// <remarks>source and destination folders set in CTOR</remarks>
        public bool CopyFiles()
        {
            SetCurrentDirectory(_sourceRootFolder);
            foreach (var file in CopyFiles(_sourceRootFolder, _destinationRootFolder, string.Empty))
            {
                var sourceFile = file;
                var destinationFile = sourceFile.Replace(_sourceRootFolder, _destinationRootFolder);
                _fileCopyLog.AppendFormat("Copying {0} to {1}\r\n", sourceFile, destinationFile);
            }

            return true;
        }

        private IEnumerable<string> CopyFiles(string sourceRootFolder, string destinationRootFolder, string projectName)
        {
            foreach (var sourceFile in _fileHelper.DirectoryGetFiles(sourceRootFolder))
            {
                var fileInfo = new FileInfo(sourceFile);

                //if (fileInfo.Extension.Equals(".sln", StringComparison.CurrentCultureIgnoreCase) || fileInfo.Extension.Equals(".csproj", StringComparison.CurrentCultureIgnoreCase))
                //    projectName = sourceFile;

                if (fileInfo.Extension.Equals(".csproj", StringComparison.CurrentCultureIgnoreCase))
                    projectName = sourceFile;

                if (!(_fileHelper.IsFileReadOnly(fileInfo)) && IsFileInProject(fileInfo.Name, fileInfo.Extension, projectName))
                {
                    yield return sourceFile;
                    var destinationFile = sourceFile.Replace(sourceRootFolder, destinationRootFolder);
                    if (!Directory.Exists(destinationRootFolder))
                        Directory.CreateDirectory(destinationRootFolder);
                    var errorOccurred = false;
                    IOException exceptionInfo = null;
                    try
                    {
                        _fileHelper.FileCopy(sourceFile, destinationFile, true);
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

            foreach (var directory in _fileHelper.DirectoryGetDirectories(sourceRootFolder))
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

            string projectText = _fileHelper.FileReadAllText(projectName).ToLower();
            return projectText.Contains(fileName.ToLower());
        }

        private void SetCurrentDirectory(string sourceRootFolder)
        {
            Directory.SetCurrentDirectory(sourceRootFolder);
        }

    }
}
