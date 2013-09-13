using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parser;

namespace pe
{
    // NOTE: this should be replaced with a script - the library already does the heavy lifting, it would be appropriate to have a script do the orchestration ...
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting parser engine ...");

            var parser = new ElmahXmlParser(new FileHelper());
            var scriptNameList = parser.GetScriptNameList("errors.xml");
            var stackTraceList = parser.GetStackTraceList("errors.xml");
            var userAgentList = parser.GetUserAgentList("errors.xml");
            var outputLines = new StringBuilder("ID\tPage\tStackTrace\tUserAgent\r\n");
            var count = scriptNameList.Count;
            Console.WriteLine("Page Count: {0}\r\nStack Trace Count: {1}\r\nUser Agent Count: {2}", count, stackTraceList.Count, userAgentList.Count);
            foreach (var item in scriptNameList)
            {
                if (stackTraceList.ContainsKey(item.Key) && userAgentList.ContainsKey(item.Key))
                    outputLines.AppendFormat("{0}\t{1}\t{2}\t{3}\r\n", item.Key, item.Value, stackTraceList[item.Key], userAgentList[item.Key]);
                else
                    Console.WriteLine("Missing key: " + item.Key);
            }
            File.WriteAllText("formattedErrors.txt", outputLines.ToString());

            Console.WriteLine("Parsing Complete.");
        }
    }
}
