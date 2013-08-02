using System;
using System.IO;
using System.Collections.Generic;

namespace ShelfCopyLib
{
    public class FileHelper : IFileHelper
    {
        public bool FileCopy(string sourceFile, string destinationFile, bool overwriteIfExists)
        {
            File.Copy(sourceFile, destinationFile, overwriteIfExists);

            return true;
        }

        public string FileReadAllText(string fileName)
        {
            return File.ReadAllText(fileName);
        }

        public IEnumerable<string>DirectoryGetFiles(string path)
        {
            return Directory.GetFiles(path);
        }

        public IEnumerable<string>DirectoryGetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }

        public bool IsFileReadOnly(FileInfo fileInfo)
        {
            return fileInfo.Attributes.HasFlag(FileAttributes.ReadOnly);
        }
    }
}