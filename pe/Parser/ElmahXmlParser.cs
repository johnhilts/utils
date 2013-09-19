using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Parser
{
    public class ElmahXmlParser 
    {
        private readonly IFileHelper _fileHelper;

        public ElmahXmlParser(IFileHelper fileHelper)
        {
            _fileHelper = fileHelper;
        }

        public Dictionary<Guid, string> GetScriptNameList(string elmahXmlFileName)
        {
            return GetListWithGuidFromFileUsingRegex(elmahXmlFileName, "(?<=HTTP_X_ORIGINAL_URL:).+?(?=&#xD;&#xA;)", null);
        }

        public Dictionary<Guid, string> GetStackTraceList(string elmahXmlFileName)
        {
            return GetListWithGuidFromFileUsingRegex(elmahXmlFileName, @"(?<=detail\="").+?(?="")", null);
        }

        public Dictionary<Guid, string> GetUserAgentList(string elmahXmlFileName)
        {
            return GetListWithGuidFromFileUsingRegex(elmahXmlFileName, "(?<=HTTP_USER_AGENT:).+?(?=&#xD;&#xA;)", null);
        }

        private Dictionary<Guid, string> GetListWithGuidFromFileUsingRegex(string fileName, string regexPattern, int? groupIndex)
        {
            var re = new Regex(regexPattern, RegexOptions.IgnoreCase);
            var list = new Dictionary<Guid, string>();
            var guidRe = new Regex("^({.+?}),", RegexOptions.IgnoreCase);
            Guid index = Guid.NewGuid();
            foreach (var line in _fileHelper.ReadAllLines(fileName))
            {
                if (guidRe.IsMatch(line))
                    Guid.TryParse(guidRe.Match(line).Groups[1].Value, out index);
                if (re.IsMatch(line))
                {
                    if (list.ContainsKey(index))
                    {
                        Console.WriteLine("Duplicate GUID: " + index.ToString() + " using pattern: " + regexPattern);
                        continue;
                    }
                    if (groupIndex.HasValue)
                        list.Add(index, re.Match(line).Groups[groupIndex.Value].Value);
                    else
                        list.Add(index, re.Match(line).Value);
                    
                }
            }
            return list;
        }

        public void FlattenFileContents(string fileName)
        {
            var lines = new StringBuilder();
            var input = File.OpenRead(fileName);
            var output = File.CreateText("flatErrors.xml");
            foreach (var line in _fileHelper.ReadAllLines(fileName))
            {
                if (line.Contains("</error>"))
                {
                    lines.AppendLine(line);
                    //File.WriteAllText("flatErrors.xml", lines.ToString());
                    output.Write(lines.ToString());
                    lines = new StringBuilder();
                }
                else
                {
                    lines.Append(line.Replace("\r\n", " "));
                }
            }

            //File.WriteAllText("flatErrors.xml", lines.ToString());
            output.Write(lines.ToString());
            output.Flush();
            output.Close();
            output.Dispose();
        }
    }
}
