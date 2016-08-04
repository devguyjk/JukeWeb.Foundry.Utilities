using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization;
using System.Reflection;
using System.Collections;
using System.Collections.ObjectModel;

namespace JukeWeb.Foundry.Utilities
{
    public class XMLUtilities
    {
        #region XMLSerialization Utilities

        public static object XmlDeserializeObject<T>(string xml)
        {
            MemoryStream memStream = null;
            object obj = null;

            try
            {
                //byte[] bytes = new byte[xml.Length];
                byte[] bytes = null;
                //Changed UTF8 to ASCII because I was getting the error 'The output byte buffer is too small'
                //if (xml.Contains("encoding=\"utf-16\""))
                //{
                //    bytes = new byte[Encoding.Unicode.GetByteCount(xml)];
                //    Encoding.Unicode.GetBytes(xml, 0, xml.Length, bytes, 0);
                //}
                //else
                //{
                //    bytes = new byte[Encoding.ASCII.GetByteCount(xml)];
                //    Encoding.ASCII.GetBytes(xml, 0, xml.Length, bytes, 0);
                //}
                bytes = new byte[Encoding.Unicode.GetByteCount(xml)];
                Encoding.Unicode.GetBytes(xml, 0, xml.Length, bytes, 0);

                XmlAttributes atts = new XmlAttributes();
                XmlAttributeOverrides attOverride = new XmlAttributeOverrides();
                atts.Xmlns = false;
                attOverride.Add(typeof(T), atts);

                XmlSerializer serializer = new XmlSerializer(typeof(T), attOverride);
                using (memStream = new MemoryStream(bytes))
                {
                    obj = serializer.Deserialize(memStream);
                }
            }
            catch (Exception e)
            {
                //		EventLogger.WriteWarning("Error in xml serialization, msg:" + e.Message, e.Source);
            }
            finally
            {
                if (memStream != null) memStream.Close();

            }
            return obj;
        }


        public static T XmlDeserializeGeneric<T>(string xml) where T : new()
        {
            T obj = (T)XmlDeserializeObject<T>(xml);
            return obj;
        }

        public static string XmlSerializeObject<T>(object obj)
        {


            String xmlResult = null;
            try
            {
                XmlSerializer xsSubmit = new XmlSerializer(typeof(T));
                //StringWriter sww = new StringWriter();
                //XmlWriter writer = XmlWriter.Create(sww);
                //xsSubmit.Serialize(writer, obj);
                //return sww.ToString();

                MemoryStream stream = new MemoryStream();
                var enc = new UnicodeEncoding(false, false);
                XmlTextWriter writer = new XmlTextWriter(stream, enc);

                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                xsSubmit.Serialize(writer, obj, ns);
                return Encoding.Unicode.GetString(stream.GetBuffer(), 0, (int)stream.Length);

                /*
                   XmlSerializer serializer = new XmlSerializer(typeof(T));
                   using (MemoryStream stream = new MemoryStream())
                   {
                       var enc = new UTF8Encoding(false);
                       using (XmlTextWriter writer = new XmlTextWriter(stream, enc))
                       {
                           serializer.Serialize(writer, typeof(T));
                       }
                       return Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
                   }*/
            }
            catch (Exception e)
            {
                EventLogger.WriteWarning("Error in xml  serialization, msg:" + e.Message, e.Source);
            }
            return xmlResult;
        }

        public static XElement XElementSerializeObject<T>(object obj, string title = "Root")
        {
            XElement root = new XElement(title);
            //try
            //{
            foreach (PropertyInfo property in obj.GetType().GetProperties())
            {
                if (property.GetCustomAttributes(typeof(XmlIgnoreAttribute), true).Count() > 0)
                    continue;
                if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                {
                    ICollection collection = (ICollection)property.GetValue(obj, null);
                    XElement element = new XElement(property.Name);

                    foreach (object ob in collection)
                    {
                        if (!ob.GetType().IsClass || ob.GetType() == typeof(string))
                        {
                            string value = null;
                            if (ob != null)
                                value = ob.ToString();
                            element.Add(new XElement(property.Name + "Item", value));
                        }
                        else
                            element.Add(XElementSerializeObject<T>(ob, property.Name + "Item"));
                    }

                    root.Add(element);
                }
                else
                {
                    var propertyName = property.Name;
                    var parameterInfo = property.GetIndexParameters();
                    object value = null;
                    if (parameterInfo.Count() == 0)
                        value = property.GetValue(obj, null);

                    if (value != null)
                    {
                        if (!property.PropertyType.IsClass || property.PropertyType == typeof(string))
                            root.Add(new XElement(property.Name, value));
                        else
                            root.Add(XElementSerializeObject<T>(value, property.Name));
                    }
                }
            }

            return root;
            //}
            //catch (Exception e)
            //{
            //    EventLogger.WriteWarning("Error in xml  serialization, msg:" + e.Message, e.Source);
            //}

            //return root;
        }

        private static bool IsXmlNodeExcluded(string nodeName, IEnumerable<string> excludes)
        {
            bool isObjectExcluded = false;
            if (excludes != null)
            {
                isObjectExcluded = (excludes.Contains(nodeName.Trim()));
                System.Diagnostics.Debug.WriteLine("{0} -> {1}", nodeName, isObjectExcluded.ToString());
            }
            return isObjectExcluded;
        }

        #endregion

        #region DataContract Serialization Utilities

        public static string DataContractSerializeObject<T>(object obj)
        {
            string xml = "";
            try
            {
                DataContractSerializer ser = new DataContractSerializer(typeof(T));
                MemoryStream ms = new MemoryStream();
                ser.WriteObject(ms, obj);
                xml = Encoding.UTF8.GetString(ms.ToArray());
            }
            catch (Exception e)
            {
                EventLogger.WriteWarning("Error in datacontract  serialization, msg:" + e.Message, e.Source);
            }
            return xml;
        }

        public static T DataContractDeserializeObject<T>(string xml)
        {
            object obj = null;
            try
            {
                DataContractSerializer ser = new DataContractSerializer(typeof(T));
                obj = (T)ser.ReadObject(XmlReader.Create(new StringReader(xml)), true);
            }
            catch (Exception e)
            {
                EventLogger.WriteWarning("Error in datacontract  serialization, msg:" + e.Message, e.Source);
            }
            return (T)obj;
        }


        #endregion

        #region XML/XSLT Transformation

        public static string TransformXMLString(string xml, string xsltNamePath, Assembly assembly)
        {
            Stream xslt = ResourceManager.GetEmeddedResource(assembly, xsltNamePath);
            return TransformXMLString(xml, xslt);
        }

        public static string TransformXMLString(string xml, string xsltNamePath, Assembly assembly, XsltArgumentList args)
        {
            Stream xslt = ResourceManager.GetEmeddedResource(assembly, xsltNamePath);
            return TransformXMLString(xml, xslt, args);
        }


        public static string TransformXMLString(string xml, Stream xslt)
        {
            return TransformXMLString(xml, xslt, null);
        }

        public static string TransformXMLString(string xml, Stream xsltStream, XsltArgumentList args)
        {
            XmlReader xmlR = XmlReader.Create(new StringReader(xml));
            XmlReader xsltR = XmlReader.Create(xsltStream);
            return TransformXMLString(xmlR, xsltR, args);
        }

        public static string TransformXMLString(XmlReader xmlR, XmlReader xsltR, XsltArgumentList args)
        {
            StringWriter sw = new StringWriter();
            XslCompiledTransform xslTrans = new XslCompiledTransform();

            XsltSettings settings = new XsltSettings();
            settings.EnableScript = true;
            xslTrans.Load(xsltR, settings, new XmlUrlResolver());
            XPathDocument xPathDoc = new XPathDocument(xmlR);
            xslTrans.Transform(xPathDoc, args, sw);

            return sw.ToString();
        }


        #endregion

        #region XDocument Helpers

        public static string GetXDocumentElementValue(XElement node, string name)
        {
            string val = "";
            if (node.Element(name) != null)
                val = node.Element(name).Value;

            return val;
        }

        public static string GetXDocumentAttributeValue(XElement node, string name)
        {
            string val = "";
            if (node.Attribute(name) != null)
                val = node.Attribute(name).Value;

            return val;
        }

        public static int[] XDocumentNodesToIntArray(XElement node, string nodeName)
        {
            List<int> ints = new List<int>();
            try
            {
                foreach (XElement elem in node.Descendants(nodeName))
                {
                    ints.Add(int.Parse(elem.Value));
                }
            }
            catch (Exception ex) { }
            return ints.ToArray();
        }

        public static string CleanInvalidXmlChars(string text)
        {
            // From xml spec valid chars: 
            // #x9 | #xA | #xD | [#x20-#xD7FF] | [#xE000-#xFFFD] | [#x10000-#x10FFFF]     
            // any Unicode character, excluding the surrogate blocks, FFFE, and FFFF. 
            string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
            return Regex.Replace(text, re, "");
        }

        #endregion
    }
}
