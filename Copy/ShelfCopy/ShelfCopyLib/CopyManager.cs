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

        public bool CopyFiles()
        {
            _fileHelper.SetCurrentDirectory(_sourceRootFolder);
            foreach (var file in _fileHelper.CopyFiles(_sourceRootFolder, _destinationRootFolder, string.Empty))
            {
                var sourceFile = file;
                var destinationFile = file.Replace(_sourceRootFolder, _destinationRootFolder);
                _fileCopyLog.AppendFormat("Copying {0} to {1}\r\n", file, destinationFile);
            }

            return true;
        }
    }
}
