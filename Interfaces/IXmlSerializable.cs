using System;
using System.Reflection;

#if !SILVERLIGHT

using System.Xml.Xsl;
using System.Xml.XPath;
using System.Xml.Linq;

#endif

namespace JukeWeb.Foundry.Utilities.Interfaces
{
    interface IXmlSerializable
    {
#if !SILVERLIGHT
        void LoadFromXmlAndBind<T>(string xml, T target) where T : new();
        string GetXml();

        string TransformByXsl(string xslDocumentResourceName);        
        string TransformByXsl(string xslDocumentResourceName, XsltArgumentList args);
        string TransformByXsl(Assembly asm, string xslDocumentResourceName, XsltArgumentList args);
        XDocument XDocument { get; }
        XPathNavigator CreateNavigator();
        XElement GetElementByXpath(string xPath);
#endif

    }
}
