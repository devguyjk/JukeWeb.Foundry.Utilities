using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using JukeWeb.Foundry.Utilities.Common.Attributes;

namespace JukeWeb.Foundry.Utilities
{
    public static partial class ExtensionMethods
    {
        public static List<List<T>> SplitSubList<T>(this IList<T> source, int sublistSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / sublistSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        public static bool IndexExists<T>(this IList<T> source, int index)
        {
            return index >= 0 && index <= source.Count() ? true : false;
        }
        

        public static DataTable ToDataTable<T>(this IList<T> items, DataTable table=null, string  commaDelimListOfIncludedBaseClassFields=null)
        {
            try
            {
                DataTable tb = null;
                bool addProperty = true;
                Type thisType = typeof(T);

                PropertyInfo[] props = thisType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                List<PropertyInfo> usedProps = new List<PropertyInfo>();
                Type propertyType = null;
                bool isNullable = false;
                DataColumn column = null;

                if (table == null)
                {
                    tb = new DataTable(typeof(T).Name);
                    foreach (var prop in props)
                    {
                        propertyType = prop.PropertyType;
                        isNullable = false;
                        if (prop.PropertyType.IsGenericType &&
                            prop.PropertyType.GetGenericTypeDefinition() == typeof (Nullable<>))
                        {
                            propertyType = prop.PropertyType.GetGenericArguments()[0];
                            isNullable = true;
                        }

                        addProperty = true;
                        var bcpAttr =
                            prop.GetCustomAttributes(typeof (BCPAttribute), true)
                                .FirstOrDefault()
                                .ToType<BCPAttribute>();
                        if (bcpAttr != null)
                        {
                            addProperty = bcpAttr.IsIncluded;
                        }

                        if (prop.PropertyType.Name != "String" && !prop.PropertyType.IsValueType)
                            addProperty = false;

                        if (!prop.DeclaringType.Equals(thisType))
                        {
                            addProperty = false;
                            if (!string.IsNullOrEmpty(commaDelimListOfIncludedBaseClassFields))
                            {
                                string[] baseClassPropertiesToInclude =
                                    commaDelimListOfIncludedBaseClassFields.Split(',');
                                if (baseClassPropertiesToInclude.Contains(prop.Name))
                                    addProperty = true;
                            }
                        }
                        
                        if (addProperty)
                        {
                            column = new DataColumn(prop.Name, prop.PropertyType);
                            column.AllowDBNull = isNullable;
                            tb.Columns.Add(column);
                            usedProps.Add(prop);
                        }
                    }
                }
                else
                {
                    tb = table;
                    foreach (DataColumn dc in tb.Columns)
                    {
                        var prop = props.Where(x => x.Name == dc.ColumnName).FirstOrDefault();

                        if(prop != null)
                            usedProps.Add(prop);
                    }
                }

                object value = null;
                foreach (var item in items)
                {
                    var values = new object[usedProps.Count];
                    for (var i = 0; i < usedProps.Count; i++)
                    {
                        value = usedProps[i].GetValue(item, null);
                        values[i] =(value==null)?DBNull.Value:value;
                    }

                    tb.Rows.Add(values);
                }

                return tb;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }
}
