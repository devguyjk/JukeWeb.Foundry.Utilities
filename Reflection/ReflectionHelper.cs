using System;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace JukeWeb.Foundry.Utilities.Reflection
{
    public class ReflectionHelper
    {
        static ReflectionHelper() { }

        public static void CallMethod(string assemblyName, string className, string methodName, object[] methodArgs, out object result)
        {
            CallMethod(assemblyName, className, methodName, methodArgs, null, out result);
        }

        public static void CallMethod(string assemblyName, string className, string methodName, object[] methodArgs, object[] constructorArgs, out object result)
        {
            Assembly asm = Assembly.Load(assemblyName);
            CallMethod(asm, className, methodName, methodArgs, constructorArgs, out result);
        }

        public static void CallMethod(Assembly asm, string className, string methodName, object[] methodArgs, object[] constructorArgs, out object result)
        {
            Type clsType = asm.GetType(className, false, true);
            object clsInstance = null;
            GetClassInstance(asm, className, constructorArgs, out clsInstance);

            BindingFlags flags =
            BindingFlags.Public |
            BindingFlags.Default |
            BindingFlags.OptionalParamBinding |
            BindingFlags.InvokeMethod |
            BindingFlags.Instance;

            result = clsType.InvokeMember(methodName, flags, null, clsInstance, methodArgs);
        }

        public static void CallMethod(string assemblyName, string className, string methodName, ref object classInstance, object[] methodArgs, out object result)
        {
            Assembly asm = Assembly.Load(assemblyName);
            CallMethod(asm, className, methodName, ref classInstance, methodArgs, out result);
        }

        public static void CallMethod(Assembly asm, string className, string methodName, ref object classInstance, object[] methodArgs, out object result)
        {
            Type clsType = asm.GetType(className);
            BindingFlags flags =
            BindingFlags.Public |
            BindingFlags.OptionalParamBinding |
            BindingFlags.Default |
            BindingFlags.InvokeMethod |
            BindingFlags.Instance;

            MethodInfo methodInfo = clsType.GetMethod(methodName,flags);

            if (methodArgs != null)
            {
                foreach (ParameterInfo parameter in methodInfo.GetParameters())
                {
                    methodArgs[parameter.Position] = ConvertObjectToValueType(parameter.ParameterType.ToString(), methodArgs[parameter.Position]);
                }
            }
            result = methodInfo.Invoke(classInstance,methodArgs);
        }

        public static void GetClassInstance(string assemblyName, string className, object[] constructorArgs, out object result)
        {
            Assembly asm = Assembly.Load(assemblyName);
            GetClassInstance(asm, className, constructorArgs, out result);
        }

        public static void GetClassInstance(Assembly asm, string className, object[] constructorArgs, out object result)
        {
            Type clsType = asm.GetType(className);
            object clsInstance = null;

            if (constructorArgs != null)
            {
                List<Type> constructorTypes = new List<Type>();
                BindingFlags constructorArgsFlag =
                    BindingFlags.Instance | 
                    BindingFlags.Public |
                    BindingFlags.OptionalParamBinding |
                    BindingFlags.InvokeMethod |
                    BindingFlags.Default;

                
                clsInstance = asm.CreateInstance(className, true, constructorArgsFlag, null, constructorArgs, null, null);
            }
            else
                clsInstance = asm.CreateInstance(className);

            result = clsInstance;
        }


        public static object GetPropertyValue(object obj, string propertyName)
        {
            PropertyInfo propertyInfo = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            return propertyInfo.GetValue(obj, null);
        }

        public static string rtnStr<T>(ICollection<T> col, string fieldName)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            string val;

            foreach (T t in col)
            {
                Type inputType = t.GetType();
                PropertyInfo propInfo = inputType.GetProperty(fieldName);
                val = (propInfo != null) ? (string)propInfo.GetValue(t, null) : "Property Not Found";
                sb.Append(String.Format("{0}{1}", val, i + 1 < col.Count ? "," : ""));
                i++;
            }

            return sb.ToString();
        }

        public static void LoadProperties(object obj, XElement element)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                string propertyName = property.PropertyType.Name;
                if (property.PropertyType.Name.IndexOf("Nullable") != -1)
                    propertyName = property.PropertyType.FullName.Substring(property.PropertyType.FullName.IndexOf("[[") + 2, ((property.PropertyType.FullName.IndexOf(",") - property.PropertyType.FullName.IndexOf("[[") - 2)));

                switch (propertyName)
                {
                    case "System.Guid":
                    case "Guid":
                        Guid id = Guid.Parse((string)element.Element(property.Name));
                        if (element.Element(property.Name) != null)
                            property.SetValue(obj, id, new object[] { });
                        break;
                    case "System.Int16":
                        if (element.Element(property.Name) != null)
                            property.SetValue(obj, Int16.Parse(element.Element(property.Name).Value), new object[] { });
                        break;
                    case "System.Int32":
                        if (element.Element(property.Name) != null)
                            property.SetValue(obj, Int32.Parse(element.Element(property.Name).Value), new object[] { });
                        break;
                    case "System.Int64":
                        if (element.Element(property.Name) != null)
                            property.SetValue(obj, Int64.Parse(element.Element(property.Name).Value), new object[] { });
                        break;
                    case "System.DateTime":
                        if (element.Element(property.Name) != null)
                            property.SetValue(obj, DateTime.Parse(element.Element(property.Name).Value), new object[] { });
                        break;
                    case "System.Boolean":
                        Boolean val = false;
                        if (element.Element(property.Name) != null)
                        {
                            val = (element.Element(property.Name).Value == "0") ? false : true;
                            property.SetValue(obj, val, new object[] { });
                        }
                        break;
                    default:
                        if (element.Element(property.Name) != null)
                            property.SetValue(obj, (object)element.Element(property.Name).Value, new object[] { });
                        break;

                }
            }
        }

        public static bool LoadProperties<T>(ref T obj, XElement element, int line, List<string> errors)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();

            string error = "{0} was not in correct format for line {1}";
            bool isError = false;
            try
            {
                for (int p = 0; p < properties.Length; p++)
                {
                    PropertyInfo property = properties[p];

                    // Only processing value types currently
                    if (!PropertyIsValueType(property) || !property.CanWrite)
                        continue;

                    bool isNullable = false;
                    string propertyName = property.PropertyType.Name;
                    if (property.PropertyType.Name.IndexOf("Nullable") != -1)
                    {
                        propertyName = property.PropertyType.FullName.Substring(property.PropertyType.FullName.IndexOf("[[") + 2, ((property.PropertyType.FullName.IndexOf(",") - property.PropertyType.FullName.IndexOf("[[") - 2)));
                        isNullable = true;
                    }

                    switch (propertyName.Replace("System.", ""))
                    {
                        case "Guid":
                            if (element.Element(property.Name) != null)
                            {
                                Guid propValue;
                                if (Guid.TryParse(element.Element(property.Name).Value, out propValue))
                                    property.SetValue(obj, propValue, new object[] { });
                                else
                                {
                                    if (!(isNullable && string.IsNullOrWhiteSpace(element.Element(property.Name).Value)))
                                    {
                                        errors.Add(string.Format(error, property.Name, line));
                                        isError = true;
                                    }

                                }
                            }
                            break;
                        case "Int16":
                            if (element.Element(property.Name) != null)
                            {
                                Int16 propValue;
                                if (Int16.TryParse(element.Element(property.Name).Value, out propValue))
                                    property.SetValue(obj, propValue, new object[] { });
                                else
                                {
                                    if (!(isNullable && string.IsNullOrWhiteSpace(element.Element(property.Name).Value)))
                                    {
                                        errors.Add(string.Format(error, property.Name, line));
                                        isError = true;
                                    }
                                }
                            }
                            break;
                        case "Int32":
                            if (element.Element(property.Name) != null)
                            {
                                Int32 propValue;
                                if (Int32.TryParse(element.Element(property.Name).Value, out propValue))
                                    property.SetValue(obj, propValue, new object[] { });
                                else
                                {
                                    if (!(isNullable && string.IsNullOrWhiteSpace(element.Element(property.Name).Value)))
                                    {
                                        errors.Add(string.Format(error, property.Name, line));
                                        isError = true;
                                    }
                                }
                            }
                            break;
                        case "Int64":
                            if (element.Element(property.Name) != null)
                            {
                                Int64 propValue;
                                if (Int64.TryParse(element.Element(property.Name).Value, out propValue))
                                    property.SetValue(obj, propValue, new object[] { });
                                else
                                {
                                    if (!(isNullable && string.IsNullOrWhiteSpace(element.Element(property.Name).Value)))
                                    {
                                        errors.Add(string.Format(error, property.Name, line));
                                        isError = true;
                                    }
                                }
                            }
                            break;
                        case "Double":
                            if (element.Element(property.Name) != null)
                            {
                                Double propValue;
                                if (Double.TryParse(element.Element(property.Name).Value, out propValue))
                                    property.SetValue(obj, propValue, new object[] { });
                                else
                                {
                                    if (!(isNullable && string.IsNullOrWhiteSpace(element.Element(property.Name).Value)))
                                    {
                                        errors.Add(string.Format(error, property.Name, line));
                                        isError = true;
                                    }
                                }
                            }
                            break;
                        case "DateTime":
                            if (element.Element(property.Name) != null)
                            {
                                DateTime propValue;
                                if (DateTime.TryParse(element.Element(property.Name).Value, out propValue))
                                    property.SetValue(obj, propValue, new object[] { });
                                else
                                {
                                    if (!(isNullable && string.IsNullOrWhiteSpace(element.Element(property.Name).Value)))
                                    {
                                        errors.Add(string.Format(error, property.Name, line));
                                        isError = true;
                                    }
                                }
                            }
                            break;
                        case "Boolean":
                            if (element.Element(property.Name) != null)
                            {
                                Boolean propValue = false;
                                if (Boolean.TryParse(element.Element(property.Name).Value, out propValue))
                                    property.SetValue(obj, propValue, new object[] { });
                                else
                                {
                                    if (!(isNullable && string.IsNullOrWhiteSpace(element.Element(property.Name).Value)))
                                    {
                                        errors.Add(string.Format(error, property.Name, line));
                                        isError = true;
                                    }
                                }
                            }
                            break;
                        default:
                            if (element.Element(property.Name) != null)
                                property.SetValue(obj, (object)element.Element(property.Name).Value, new object[] { });
                            break;

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return isError;
        }

        public static bool LoadProperties<T>(ref T obj, List<string> names, List<string> values, int line, List<string> errors)
        {
            if ((values.Count != names.Count))
            {
                errors.Add(string.Format("Columns and count for every row must match line {0}.", line));
                return false;
            }

            XDocument propertyXDoc = new XDocument(new XElement("Root"));
            for (int n = 0; n < names.Count; n++)
            {
                string name = names[n];
                string value = string.Empty;
                if ((values.Count > n))
                    value = values[n];
                propertyXDoc.Root.Add(new XElement(name, value));
            }

            return LoadProperties(ref obj, propertyXDoc.Root, line, errors);
        }

        public static object ConvertObjectToValueType(string propertyType, object value)
        {
            switch (propertyType)
            {
                case "System.Guid":
                case "Guid":
                    return Guid.Parse(value.ToString());
                case "System.Int16":
                    return Int16.Parse(value.ToString());
                case "System.Int32":
                    return Int32.Parse(value.ToString());
                case "System.Int64":
                    return Int64.Parse(value.ToString());
                case "System.DateTime":
                    return DateTime.Parse(value.ToString());
                case "System.Boolean":
                    return Convert.ToBoolean(value);
                default:
                    return value;

            }
        }


        public static bool PropertyIsValueType(PropertyInfo property)
        {
            string propertyName = property.PropertyType.Name;
            if (property.PropertyType.Name.IndexOf("Nullable") != -1)
                propertyName = property.PropertyType.FullName.Substring(property.PropertyType.FullName.IndexOf("[[") + 2, ((property.PropertyType.FullName.IndexOf(",") - property.PropertyType.FullName.IndexOf("[[") - 2)));

            switch (propertyName.Replace("System.", ""))
            {
                case "Guid":
                case "Int16":
                case "Int32":
                case "Int64":
                case "DateTime":
                case "Boolean":
                case "Double":
                case "Float":
                case "float":
                case "String":
                    return true;
                default:
                    return false;
            }
        }
    }
}
