using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JukeWeb.Foundry.Utilities
{
    public class FileUtilities
    {
        public static void WriteToFile(string path, string content)
        {
            WriteToFile(path, new List<string>() { content }, true);
        }

        public static void WriteToFile(string path, string content, bool append)
        {
            WriteToFile(path, new List<string>() { content }, append);
        }

        public static void WriteToFile(string path, List<string> content)
        {
            WriteToFile(path, content, true);
        }

        public static void WriteToFile(string path, List<string> content, bool append)
        {
            using (StreamWriter sw = new StreamWriter(path,append))
            {
                foreach (string line in content)
                    sw.WriteLine(line);
            }
        }

        public static string GetFileContents(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw (new FileNotFoundException(
                  "logfile cannot be read since it does not exist.", filePath));
            }
            string contents = "";

            using (FileStream fileStream = new FileStream(filePath,
                        FileMode.Open,
                        FileAccess.Read,
                     FileShare.Read))
            {
                using (StreamReader streamReader = new StreamReader(filePath))
                {
                    contents = streamReader.ReadToEnd();
                }

            }

            return contents;
        }

        public static List<string> GetFileContentLines(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw (new FileNotFoundException(string.Format("file {0} was not found.", filePath)));
            }

            List<string> contents = new List<string>();
            string line = "";
            using (StreamReader streamReader = new StreamReader(filePath))
            {
                while ((line = streamReader.ReadLine()) != null)
                {
                    contents.Add(line);
                }
            }

            return contents;
        }
    }

}
