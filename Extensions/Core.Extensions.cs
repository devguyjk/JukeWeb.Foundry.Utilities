using System;
using System.Runtime.Serialization;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using JukeWeb.Foundry.Utilities.Common.Attributes;
using JukeWeb.Foundry.Utilities.Interfaces;

using JukeWeb.Foundry.Utilities.Common;

namespace JukeWeb.Foundry.Utilities
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public sealed class MultiDescription : DescriptionAttribute
    {
        public MultiDescription(string description)
        {
            this.DescriptionValue = description;
        }
    }

    public static partial class ExtensionMethods
    {
        #region Object Extensions

        public static object GetValueOrDefault(this Object o, object defaultValue)
        {
            return (o == null) ? defaultValue : o;
        }

        public static string GetDescriptionAttributeValue(this Object o)
        {
            try
            {
                var enumBit = (Enum)Enum.Parse(o.GetType(), o.ToString());
                List<string> returnValues = new List<string>();
                foreach (Enum enumVal in GetFlags(enumBit))
                {
                    FieldInfo field = enumVal.GetType().GetField(enumVal.ToString());
                    var attributes = Attribute.GetCustomAttributes(field, typeof(DescriptionAttribute));
                    foreach (var attribute in attributes)
                    {
                        if (attribute.GetType() == typeof(DescriptionAttribute))
                            returnValues.Add((attribute as DescriptionAttribute).Description);
                    }
                    var multiAttributes = Attribute.GetCustomAttributes(field, typeof(MultiDescription));
                    foreach (var attribute in multiAttributes)
                    {
                        if (attribute.GetType() == typeof(MultiDescription))
                            returnValues.Add((attribute as MultiDescription).Description);
                    }
                }
                return String.Join(", ",returnValues);

                /*FieldInfo field = o.GetType().GetField(o.ToString());

                var attributes = Attribute.GetCustomAttributes(field, typeof(DescriptionAttribute));

                foreach(var attribute in attributes)
                {
                    if (attribute.GetType() == typeof(DescriptionAttribute))
                        return (attribute as DescriptionAttribute).Description;
                }

                return o.ToString();*/
            }
            catch (Exception ex)
            {
                return o.ToString();
            }
        }

        public static T ToType<T>(this Object o)
        {
            return (T)o;
        }

        #endregion

        #region Guid Extensions 
        public static string ToMD5Hash(this Guid o)
        {
            return EncryptionUtils.CalculateMD5Hash(o.ToString());
        }
        #endregion

        #region NameValueCollectionExtensions
        public static bool HasKey(this NameValueCollection o, string keyName)
        {
            for(int i=0;i<o.Keys.Count;i++)
            {
                string key = o[i].ToLower();
                if (key == keyName.ToLower())
                    return true;
            }
            return false;
        }
        #endregion

        #region IEnumerable Extensions

        public static IReport ToReport<T>(this IEnumerable<T> o, List<string> headerOverrides = null)
        {
            return null;
        }

        public static IReportCollection ToReport(this IEnumerable<IReport> r)
        {
            //ReportCOllection
            return null;
        }

        public static string Delimited<T>(this IEnumerable<T> o, string delimiter)
        {
            return Delimited(o, delimiter, null);
        }

        public static string Delimited<T>(this IEnumerable<T> o, string delimiter, string qualifier)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;

            foreach (T t in o)
            {
                sb.Append(String.Format("{2}{0}{2}{1}", t, delimiter, (string.IsNullOrEmpty(qualifier)) ? "" : qualifier));
                i++;
            }

            return sb.ToString(0, sb.Length - (delimiter.Length));

        }

        public static XDocument ToXDocument<T>(this IEnumerable<T> o)
        {
            return ToXDocument<T>(o, typeof(T).Name + "s");
        }
        
        public static XDocument ToXDocument<T>(this IEnumerable<T> o, string rootName)
        {
            XDocument xDoc = new XDocument(new XElement(rootName));
            foreach (T t in o)
            {
                xDoc.Root.Add(XElement.Parse(XMLUtilities.DataContractSerializeObject<T>(t)));
            }
            return xDoc;
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> o)
        {
            ObservableCollection<T> col = new ObservableCollection<T>();

            foreach (T t in o)
            {
                col.Add(t);
            }
            return col;
        }

        public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TSource, bool> comparer)
        {
            return first.Except(second, new LambdaComparer<TSource>(comparer));
        }

        public static IEnumerable<TSource> DistinctBy<TSource>(this IEnumerable<TSource> items, Func<TSource, TSource, bool> equalityComparer) where TSource : class
        {
            return items.Distinct(new LambdaComparer<TSource>(equalityComparer));
        }

        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> o, Func<T, IEnumerable<T>> childSelector)
        {
            var stack = new Stack<T>(o);
            while (stack.Count > 0)
            {
                T t = stack.Pop();

                yield return t;

                foreach (T child in childSelector(t)) stack.Push(child);
            }
        }

        #endregion

        #region IDictionary Extensions

        public static string ToNameValueQueryString(this IDictionary<string, object> o)
        {
            return ToNameValueQueryString(o, ",");
        }

        public static string ToNameValueQueryString(this IDictionary<string, string> o)
        {
            return ToNameValueQueryString(o, ",");
        }

        public static string ToNameValueQueryString(this IDictionary<string, string> o, string delimiter)
        {
            List<string> returnAry = new List<string>();
            foreach (string key in o.Keys)
                returnAry.Add(string.Format("{0}={1}", key, o[key]));

            return (returnAry.Count > 0) ? string.Join(delimiter, returnAry.ToArray()) : string.Empty;
        }

        public static string ToFormattedMessage(this IDictionary<string, object> o)
        {
            return ToFormattedMessage(o, "------------------------------------------------------");
        }

        public static string ToFormattedMessage(this IDictionary<string, object> o, string itemSeperator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string key in o.Keys)
                sb.AppendFormat("{0}:{1}", key, Environment.NewLine).AppendFormat("{0}{1}{2}", o[key], Environment.NewLine, (o.Count > 1)?itemSeperator:"").AppendLine();

            return sb.ToString();
        }

        public static string ToNameValueQueryString(this IDictionary<string, object> o, string delimiter)
        {
            List<string> returnAry = new List<string>();
            foreach (string key in o.Keys)
                returnAry.Add(string.Format("{0}={1}", key, o[key]));

            return (returnAry.Count > 0)?string.Join(delimiter, returnAry.ToArray()):string.Empty;
        }
        
        public static string ToElementsXML(this IDictionary<string, string> o)
        {
            XDocument xDoc = XDocument.Parse("<Elements></Elements>", LoadOptions.PreserveWhitespace);
            foreach (string key in o.Keys)
            {
                XElement elem = new XElement("Element",  new XAttribute("key", key), new XAttribute("value", o[key]));
                xDoc.Add(elem);
            }

            return xDoc.ToString();
        }

        #endregion

        #region IQueryable Extensions
        public static ObservableCollection<T> ToObservableCollection<T>(this IQueryable<T> o)
        {
            ObservableCollection<T> col = new ObservableCollection<T>();

            foreach (T t in o)
            {
                col.Add(t);
            }
            return col;
        }
        #endregion
        
        #region DateTime Extensions

        public static string GetMSTimeStamp(this DateTime o)
        {
            return o.ToString("hh:mm:ss.FFF");
        }

        public static string GetMSTimeStampWithLapse(this DateTime o)
        {
            return GetMSTimeStampWithLapse(o, DateTime.Now);
        }


        public static string GetMSTimeStampWithLapse(this DateTime o, DateTime lapsedTime)
        { 
            return o.Add(new TimeSpan(o.Subtract(lapsedTime).Ticks *-1)).ToString("hh:mm:ss.FFF");
        }

        #endregion

      

    }
}
