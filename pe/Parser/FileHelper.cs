using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    public class FileHelper : IFileHelper
    {
        public IEnumerable<string> ReadAllLines(string fileName)
        {
            foreach (var line in File.ReadAllLines(fileName))
            {
                yield return line;
            }
        }
    }
}
