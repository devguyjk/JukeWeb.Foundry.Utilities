using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;

using JukeWeb.Foundry.Utilities.Libraries;

namespace JukeWeb.Foundry.Utilities
{
    public class StringUtils
    {
        private static string ConvertURLsInTextToLink(string val)
        {
            string result = val;
            Regex regex = new Regex(RegexLibrary.URL);
            Match match = regex.Match(val);
            while (match.Success)
            {
                result = result.Replace(match.Groups[0].Value, string.Format("<a href=\"{0}\" target=\"_blank\">{0}</a>", match.Groups[0].Value));
                match = match.NextMatch();
            }

            return result;
        }

        public static string ConvertURLsToLink(string val)
        {
            Regex regex = new Regex(RegexLibrary.HTML);
            Match match = regex.Match(val);
            int lastIndex = 0, anchorTagStackCount = 0;
            StringBuilder result = new StringBuilder();

            while (match.Success)
            {
                string tagText = match.Groups[0].Value;
                string tagName = match.Groups[2].Value;
                bool isClosingTag = !string.IsNullOrWhiteSpace(match.Groups[1].Value);
                string inBetweenTagsText = val.Substring(lastIndex, match.Index - lastIndex);
                lastIndex = match.Index + tagText.Length;

                if (tagName == "a")
                {
                    if (!isClosingTag)
                    {
                        anchorTagStackCount++;
                        result.Append(ConvertURLsInTextToLink(inBetweenTagsText));
                    }
                    else
                    {
                        anchorTagStackCount--;
                        if (anchorTagStackCount == 0)
                            result.Append(inBetweenTagsText);
                    }
                }
                else if(anchorTagStackCount == 0)
                    result.Append(ConvertURLsInTextToLink(inBetweenTagsText));

                result.Append(tagText);
                match = match.NextMatch();
            }
            if (lastIndex < val.Length)
                result.Append(ConvertURLsInTextToLink(val.Substring(lastIndex)));

            return result.ToString();
        }

        public static string StripHTMLTags(string val)
        {
            Regex regex = new Regex(RegexLibrary.HTML_TAGS);
            foreach (Match match in regex.Matches(val))
            {
                val = val.Replace(match.Value, "");
            }
            return val;
        }

        public static List<string> SplitStringIntoChunks(string str, int chunkSize)
        {
            var returnStr = Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize)).ToList<string>();

            if (str.Length % chunkSize > 0)
            {
                var startChar = returnStr.Count * chunkSize;
                returnStr.Add(str.Substring(startChar));
            }

            return returnStr;
        }

        public static string DottedToPascal(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            string returnString = "";
            string[] words = s.Split('-');
            foreach (string word in words)
            {
                returnString += StringUtils.UppercaseFirst(word);
            }
            return returnString;
        }

        public static string RemoveAccent(string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }
        
        #region Get String / Get Bytes


        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static byte[] GetBytesUTF32(string str)
        {
            return Encoding.UTF32.GetBytes(str);
        }


        public static byte[] GetBytesUTF8(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static string GetStringUTF8(byte[] bytes)
        {
           return Encoding.UTF8.GetString(bytes);
        }

        public static string GetStringUTF32(byte[] bytes)
        {
           return Encoding.UTF32.GetString(bytes);
        }
        #endregion


        public static string Slugify(string phrase)
        {
            // string str = StringUtils.RemoveAccent(phrase).ToLower();
            // str = str.Replace("?", "");
            string str = System.Text.RegularExpressions.Regex.Replace(phrase, @"[~!@#%\^\$&\*\(\)_\+=\[\]\{\}\|\\,\.\?\:\;\<\>\/]", ""); // Remove all non valid chars          
            str = System.Text.RegularExpressions.Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space  
            str = System.Text.RegularExpressions.Regex.Replace(str, @"\s", "-"); // //Replace spaces by dashes
            return string.IsNullOrEmpty(str) ? "-" : str; //temp fix to unicode chars empty string
        }

        public static string Coalesce(params string[] args)
        {
            var values = args.ToList();
            for (int s = 0; s < values.Count;s++)
            {
                var val = values[s];
                if (val != null)
                    return val.ToString();
            }

            return null;
        }

        public static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public static string[] FilterListElements(string[] list, string[] paramsFilter)
        {
            List<string> returnList = new List<string>();
            foreach (string item in list)
            {
                foreach(string param in paramsFilter){
                    Regex r = new Regex(param, RegexOptions.IgnoreCase);
                    Match m = r.Match(item);
                    if(m.Length > 0)
                        returnList.Add(Regex.Replace(item,param,""));
                }
            }
            return returnList.ToArray();
        }

        public static int CountStringOccurrences(string text, string pattern)
        {
            // Loop through all instances of the string 'text'.
            int count = 0;
            int i = 0;
            while ((i = text.IndexOf(pattern, i)) != -1)
            {
                i += pattern.Length;
                count++;
            }
            return count;
        }

    }
}
