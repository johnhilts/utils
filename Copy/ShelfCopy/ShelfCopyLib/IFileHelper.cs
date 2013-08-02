using System.Collections.Generic;
using System.IO;

namespace ShelfCopyLib
{
    public interface IFileHelper
    {
        bool FileCopy(string sourceFile, string destinationFile, bool overwriteIfExists);

        string FileReadAllText(string fileName);

        IEnumerable<string> DirectoryGetFiles(string path);

        IEnumerable<string> DirectoryGetDirectories(string path);

        bool IsFileReadOnly(FileInfo fileInfo);
    }
}