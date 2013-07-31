using System.Collections.Generic;
using System.IO;

namespace ShelfCopyLib
{
    public class FileWrapper : IFileWrapper
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
