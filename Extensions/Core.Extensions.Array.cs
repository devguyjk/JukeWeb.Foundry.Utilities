using System;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using JukeWeb.Foundry.Utilities.Common.Attributes;

using JukeWeb.Foundry.Utilities.Common;

namespace JukeWeb.Foundry.Utilities
{
    public static partial class ExtensionMethods
    {
        #region Array Extensions
        public static string ToElements(this Array o)
        {
            XDocument xDoc = XDocument.Parse("<Elements></Elements>", LoadOptions.PreserveWhitespace);
            for (int i = 0; i < o.Length; i++)
            {
                XElement elem = new XElement("Element", o.GetValue(i).ToString());
                if (i == o.Length - 1)
                    elem.Add(new XAttribute("Last", true));
                xDoc.Add(elem);
            }

            return xDoc.ToString();
        }

        public static List<object> ToList(this Array o)
        {
            List<object> results = new List<object>();
            for (int i = 0; i < o.Length; i++)
            {
                results.Add(o.GetValue(i));
            }
            return results;
        }
        #endregion

    }
}
