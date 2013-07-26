using System.Collections.Generic;

namespace ShelfCopyLib
{
    public interface IFileHelper
    {
        void SetCurrentDirectory(string sourceRootFolder);
        IEnumerable<string> CopyFiles(string sourceRootFolder, string destinationRootFolder, string projectName);
    }
}