using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JukeWeb.Foundry.Utilities.Utilities
{
    public class HtmlUtilities
    {
        public static string MinifyHtml(string html)
        {
            html = Regex.Replace(html, @"\s+", " ");
            html = Regex.Replace(html, @"\s*\n\s*", "\n");
            html = Regex.Replace(html, @"\s*\>\s*\<\s*", "><");

            // single-line doctype must be preserved 
            var firstEndBracketPosition = html.IndexOf(">");
            if (firstEndBracketPosition >= 0)
            {
                html = html.Remove(firstEndBracketPosition, 1);
                html = html.Insert(firstEndBracketPosition, ">\r\n");
            }

            return html;
        }
    }
}
