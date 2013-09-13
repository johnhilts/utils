using System.Collections.Generic;

namespace Parser
{
    public interface IFileHelper
    {
        IEnumerable<string> ReadAllLines(string fileName);
    }
}