using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JukeWeb.Foundry.Utilities;
using System.Reflection;
using System.Xml;
using System.IO;
using JukeWeb.Foundry.Utilities.Xml;

#if !SILVERLIGHT

using JukeWeb.Foundry.Utilities.Reflection;
using JukeWeb.Foundry.Utilities.Interfaces;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Xml.Linq;

#endif

namespace JukeWeb.Foundry.Utilities.Xml
{
    public class XmlSerializableBase<F>: IXmlSerializable
#if !SILVERLIGHT
        , IXPathNavigable
#endif
    {
        #if !SILVERLIGHT

        public string GetXml()
        {
            return XMLUtilities.XmlSerializeObject<F>(this);
            //return "";
        }


        private XDocument _xDoc;
        public XDocument XDocument
        {
            get
            {
                if (_xDoc == null)
                {
                    string xml = GetXml();
                    if (string.IsNullOrEmpty(xml))
                    {
                        _xDoc = XDocument.Load(new StringReader(xml));
                    }
                }
                return _xDoc;
            }
        }  


        public XPathNavigator CreateNavigator()
        {
            return _xDoc.CreateNavigator();
        }

        public XElement GetElementByXpath(string xPath)
        {
            return _xDoc.Document.XPathSelectElement(xPath);
        }

        public string TransformByXsl(string xslDocumentResourceName)
        {
            return TransformByXsl(Assembly.GetAssembly(this.GetType()), xslDocumentResourceName, null);
        }

        public string TransformByXsl(string xslDocumentResourceName, XsltArgumentList args)
        {
            return TransformByXsl(Assembly.GetAssembly(this.GetType()), xslDocumentResourceName, args);
        }

        public string TransformByXsl(Assembly asm, string xslDocumentResourceName, XsltArgumentList args)
        {
            Stream xslStream = ResourceManager.GetEmeddedResource(asm, xslDocumentResourceName);
            string transformed = XMLUtilities.TransformXMLString(GetXml(), xslStream, args);
            return transformed;
        }
        public virtual void LoadFromXmlAndBind<T>(string xml, T target)
            where T : new()
        {
            ObjectBinder binder = new ObjectBinder();
            T source = XMLUtilities.XmlDeserializeGeneric<T>(xml);
            binder.Copy<T, T>(source, target);
        }
#endif
      }
}
