using System;
using System.IO;
using ShelfCopyLib;

namespace ShelfCopy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var previewArgument = "-p";
                if (args.Length < 2 || args.Length > 3)
                {
                    Console.WriteLine("*** Usage ***");
                    Console.WriteLine(string.Format("ShelfCopy SourceFolder DestinationFolder [{0}]", previewArgument));
                    Console.WriteLine("Note optional -p parameter for Preview");
                    return;
                }

                var sourceFolder = args[0];
                var destinationFolder = args[1];
                bool isPreview = args.Length == 3 && args[2].Equals(previewArgument, StringComparison.CurrentCultureIgnoreCase); // TODO: this will need to be improved if more options added

                CopyFiles(sourceFolder, destinationFolder, isPreview);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR:\r\n" + e.ToString());
            }
        }

        private static void CopyFiles(string sourceFolder, string destinationFolder, bool isPreview)
        {
            if (!Directory.Exists(sourceFolder))
            {
                Console.WriteLine("Source folder: {0} does not exist.", sourceFolder);
                return;
            }

            Console.WriteLine("Copying files from {0} to {1}", sourceFolder, destinationFolder);

            var copier = new CopyManager(sourceFolder, destinationFolder, new FileHelper(), isPreview);
            var success = copier.CopyFiles();
            Console.WriteLine("File Copy was {0}", success ? "Successful!" : "unsuccessful");
            Console.WriteLine("Copy log is as follows:");
            Console.WriteLine(copier.FileCopyLog);
        }
    }
}
