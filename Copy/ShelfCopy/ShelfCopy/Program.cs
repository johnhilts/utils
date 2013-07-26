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
                if (args.Length != 2)
                {
                    Console.WriteLine("*** Usage ***");
                    Console.WriteLine("ShelfCopy SourceFolder DestinationFolder");
                    return;
                }

                var sourceFolder = args[0];
                var destinationFolder = args[1];

                CopyFiles(sourceFolder, destinationFolder);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR:\r\n" + e.ToString());
            }
        }

        private static void CopyFiles(string sourceFolder, string destinationFolder)
        {
            if (!Directory.Exists(sourceFolder))
            {
                Console.WriteLine("Source folder: {0} does not exist.", sourceFolder);
                return;
            }

            Console.WriteLine("Copying files from {0} to {1}", sourceFolder, destinationFolder);

            var copier = new CopyManager(sourceFolder, destinationFolder, new FileHelper());
            var success = copier.CopyFiles();
            Console.WriteLine("File Copy was {0}", success ? "Successful!" : "unsuccessful");
            Console.WriteLine("Copy log is as follows:");
            Console.WriteLine(copier.FileCopyLog);
        }
    }
}
