using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;


namespace JukeWeb.Foundry.Utilities.Extensions
{
    public static partial class ExtensionMethods
    {

        #region XDocument Related Extensions
        public static string InnerXml(this XElement x)
        {
            return x.Nodes().Aggregate(string.Empty, (element, node) => element += node.ToString());
        }
        #endregion
	}
}
