using System;
using System.IO;
using System.Collections.Generic;

namespace ShelfCopyLib
{
    public class FileHelper : IFileHelper
    {
        public void SetCurrentDirectory(string sourceRootFolder)
        {
            Directory.SetCurrentDirectory(sourceRootFolder);
        }

        public IEnumerable<string> CopyFiles(string sourceRootFolder, string destinationRootFolder, string projectName)
        {
            foreach (var sourceFile in Directory.GetFiles(sourceRootFolder))
            {
                var fileInfo = new FileInfo(sourceFile);

                //if (fileInfo.Extension.Equals(".sln", StringComparison.CurrentCultureIgnoreCase) || fileInfo.Extension.Equals(".csproj", StringComparison.CurrentCultureIgnoreCase))
                //    projectName = sourceFile;

                if (fileInfo.Extension.Equals(".csproj", StringComparison.CurrentCultureIgnoreCase))
                    projectName = sourceFile;

                if (!(fileInfo.Attributes.HasFlag(FileAttributes.ReadOnly)) && IsFileInProject(fileInfo.Name, fileInfo.Extension, projectName))
                {
                    yield return sourceFile;
                    var destinationFile = sourceFile.Replace(sourceRootFolder, destinationRootFolder);
                    if (!Directory.Exists(destinationRootFolder))
                        Directory.CreateDirectory(destinationRootFolder);
                    File.Copy(sourceFile, destinationFile, true);
                }
            }

            foreach (var directory in Directory.GetDirectories(sourceRootFolder))
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

            string projectText = File.ReadAllText(projectName).ToLower();
            return projectText.Contains(fileName.ToLower());
        }
    }
}