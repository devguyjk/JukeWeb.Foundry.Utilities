using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if !CLR_V35
using System.Xml.Linq;
using System.Text.RegularExpressions;
#endif

namespace JukeWeb.Foundry.Utilities
{
    public static partial class ExtensionMethods
    {
        #region String Related Extensions
#if !CLR_V35
        public static string ToElements(this string[] o)
        {
            XDocument xDoc = XDocument.Parse("<Elements></Elements>", LoadOptions.PreserveWhitespace);
            for (int i = 0; i < o.Length; i++)
            {
                XElement elem = new XElement("Element", o[i]);
                if (i == o.Length - 1)
                    elem.Add(new XAttribute("Last", true));
                xDoc.Root.Add(elem);
            }

            if (xDoc.Document.Descendants("Element").Count() == 0)
            {
                XElement elem = new XElement("Element", "");
                xDoc.Root.Add(elem);
            }

            return xDoc.ToString();
        }
#endif

        public static bool IsUnicode(this string str)
        {
            return str.Any(x => x > 255);
        }

        public static int ConvertASCIILengthToUnicode(this string str, int maxAsciiLen)
        {
            if (str.IsUnicode())
            {

                return maxAsciiLen * (127/255);
            }
            else
                return maxAsciiLen;
            
        }

        public static bool IsInt(this string str)
        {
            foreach (char ch in str)
                if (!char.IsDigit(ch))
                    return false;
            return true;
        }

        public static StringBuilder AppendWithPositionAndNewLine(this StringBuilder o, string position, string value)
        {
            o.AppendFormat("{0}{1}.) {2} ", Environment.NewLine, position, value);
            return o;
        }

        public static StringBuilder AppendWithNewLine(this StringBuilder o, string value)
        {
            o.AppendFormat("{0} {1} ", Environment.NewLine, value);
            return o;
        }

        public static int[] ConvertIntoIntArray(this string[] o)
        {
            int[] values = new int[o.Length];

            for (int x = 0; x < o.Length; x++)
            {
                values[x] = Convert.ToInt32(o[x].ToString());
            }
            return values;
        }

        public static string RemoveSpecialCharacters(this string s)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in s)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string StripHTML(this string s)
        {
            return Regex.Replace
              (s, "<.*?>", string.Empty);
        }

        public static string ParseFileExtension(this string s)
        {
            return s.Substring(s.LastIndexOf(".") + 1);
        }

        public static string GetEmailPrefix(this string s)
        {
            int indexOfAt = s.IndexOf("@");
            string emailPrefix = (indexOfAt == -1) ? s : s.Substring(0, indexOfAt);
            return emailPrefix;
        }

        public static Dictionary<string, string> ParseQueryStringIntoDictionary(this string s)
        {
            return ParseQueryStringIntoDictionary(s, ',', null);
        }

        public static Dictionary<string, string> ParseQueryStringIntoDictionary(this string s, char delimiter)
        {
            return ParseQueryStringIntoDictionary(s, delimiter, null);
        }

        // This method is expecting a string in the following formate Name=Value{delimiter}Name=Value{delimiter}....
        public static Dictionary<string, string> ParseQueryStringIntoDictionary(this string s, char delimiter, string qualifier)
        {
            string delimiterStr = (delimiter == null) ? "" : delimiter.ToString();

            qualifier = ((string.IsNullOrEmpty(qualifier)) ? "" : qualifier);
            string[] valuePairs = Split(s, delimiterStr, qualifier, true);

            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (string valuePair in valuePairs)
            {
                if (!string.IsNullOrEmpty(valuePair))
                {
                    string[] valuePairAry = valuePair.Split('=');
                    string name = valuePairAry[0];
                    string value = valuePairAry[1];
                    dictionary.Add(name, value);
                }
            }
            return dictionary;
        }


        public static string ToMD5Hash(this string s)
        {
            return EncryptionUtils.CalculateMD5Hash(s);
        }

        public static Guid ToGuid(this string s)
        {
            return new Guid(s);
        }

        public static int ToInt(this string s)
        {
            return int.Parse(s);
        }

        public static string Trim(this string s, int maxLen)
        {
            if (s.Length > maxLen)
                return s.Substring(0, maxLen);
            return s;
        }

        public static string TrimWithTail(this string s, int len)
        {
            return TrimWithTail(s, len, "...");
        }

        public static string TrimWithTail(this string s, int len, string tail)
        {
            if (s.Length > len)
                return s.Substring(0, len) + tail;
            return s;
        }

        public static string[] Split(this string s, string delimiter, string qualifier, bool ignoreCase)
        {
            bool _QualifierState = false;
            int _StartIndex = 0;
            System.Collections.ArrayList _Values = new System.Collections.ArrayList();

            for (int _CharIndex = 0; _CharIndex < s.Length - 1; _CharIndex++)
            {
                if ((!string.IsNullOrEmpty(qualifier))
                 & (string.Compare(s.Substring
                (_CharIndex, qualifier.Length), qualifier, ignoreCase) == 0))
                {
                    _QualifierState = !(_QualifierState);
                }
                else if (!(_QualifierState) & (!string.IsNullOrEmpty(delimiter))
                      & (string.Compare(s.Substring
                (_CharIndex, delimiter.Length), delimiter, ignoreCase) == 0))
                {
                    _Values.Add(s.Substring
                (_StartIndex, _CharIndex - _StartIndex));
                    _StartIndex = _CharIndex + 1;
                }
            }

            if (_StartIndex < s.Length)
                _Values.Add(s.Substring
                (_StartIndex, s.Length - _StartIndex));

            string[] _returnValues = new string[_Values.Count];
            _Values.CopyTo(_returnValues);
            return _returnValues;
        }

        public static string PadLeftWithCharacter(this string s, string charVal, int length)
        {
            if (s.Length >= length)
                return s;

            while (s.Length < length)
                s = charVal + s;
            return s;
        }

        public static string PadRightWithCharacter(this string s, string charVal, int length)
        {
            if (s.Length >= length)
                return s;

            while (s.Length < length)
                s = s + charVal;
            return s;
        }

        #endregion
    }
}
