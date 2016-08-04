
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JukeWeb.Foundry.Utilities.Reflection;
using JukeWeb.Foundry.Utilities;

namespace JukeWeb.Foundry.Utilities.Common.IO
{
    public class ObjectToCSV<T> : List<T> where T : new()
    {
        private static readonly List<T> Obj = new List<T>();



        #region Private Methods

        private List<PropertyInfo> GetPropertyInfo(T obj, bool limiteToWriteableValueTypes = false)
        {
            var properties = obj.GetType().GetProperties().ToList<PropertyInfo>();
            var results = properties.Where(y => ReflectionHelper.PropertyIsValueType(y) && y.CanWrite).ToList();
            return results;
        }

        private List<string> GetPropertyNames(T obj, bool limiteToWriteableValueTypes = false)
        {
            var names = new List<string>();
            var props = GetPropertyInfo(obj, limiteToWriteableValueTypes);
            props.ForEach(x => names.Add(x.Name));
            return names;
        }

#if !SILVERLIGHT
        private string ConvertObjectToDelimitedString(T obj, IEnumerable<string> colNames)
        {
            var propertyInfo = GetPropertyInfo(obj, true);
            var propertyValues = new List<string>();

            foreach (var columName in colNames)
            {
                var prop = propertyInfo.FirstOrDefault(p => p.Name.ToLower() == columName.ToLower());
                if (prop == null)
                    continue;

                if (ReflectionHelper.PropertyIsValueType(prop))
                {
                    var propertyValue = ReflectionHelper.GetPropertyValue(obj, prop.Name);
                    propertyValues.Add((propertyValue == null) ? "" : propertyValue.ToString());
                }
                else
                    propertyValues.Add("");
            }

            return propertyValues.AsEnumerable().Delimited(",", "\"");
        }
#endif

        private bool IsCSVHeaderValid(string csvHeader)
        {
            var objInstance = new T();
            var validHeaders = GetPropertyNames(objInstance, true);
            List<string> csvHeaders = csvHeader.Split(",", "\"", true).ToList<string>();

            return !validHeaders.Where((t, c) => t.ToLower() != csvHeaders[c].ToLower()).Any();
        }

        #endregion

        #region Public Methods

#if !SILVERLIGHT

        public List<string> SerializeToCSV()
        {
            return SerializeToCSV(Obj);
        }

        public List<string> SerializeToCSV(List<T> objs, List<string> headerOverrides = null)
        {
            var csv = new List<string>();
            var defaultColumnNames = new List<string>();
            var columnNames = new List<string>();
            // Export will only include writeable value types
            var propertyInfo = GetPropertyInfo(objs.FirstOrDefault(), true);
            string columns;

            //Set default object columns 
            propertyInfo.ForEach(y => defaultColumnNames.Add(y.Name));


            // Add column headers
            if (headerOverrides != null)
            {
                if (headerOverrides.Count != propertyInfo.Count)
                {
                    throw new Exception(
                        "SerializeToCSV - HeaderOverride must have the same column count with the object");
                }
                int i = 0;
                foreach (var prop in propertyInfo)
                {
                    columnNames.Add((headerOverrides[i] != string.Empty) ? headerOverrides[i] : prop.Name);
                    i++;
                }
                columns = columnNames.Delimited(",", "\"");
            }
            else
            {
                columns = defaultColumnNames.Delimited(",", "\"");
            }



            csv.Add(columns);

            // Loop through each object and append the data
            csv.AddRange(objs.Select(obj => ConvertObjectToDelimitedString(obj, defaultColumnNames)));

            return csv;
        }
#endif

        public List<T> DeserializeFromCSV(string filePath)
        {
            var errors = new List<string>();
            var contents = FileUtilities.GetFileContentLines(filePath);
            return DeserializeFromCSV(contents, errors);
        }

        public List<T> DeserializeFromCSV(List<string> contents, List<string> errors)
        {
            var objs = new List<T>();
            try
            {
                for (var i = 0; i < contents.Count; i++)
                {
                    string content = contents[i];
                    if (i == 0)
                    {
                        // Bad headers is a fatal exception, will not continue.
                        if (!IsCSVHeaderValid(content))
                        {
                            errors.Add(string.Format("Headers were not in correct order or one was not found:"));
                            return objs;
                        }
                    }
                    else
                    {
                        var obj = new T();
                        ReflectionHelper.LoadProperties<T>(ref obj, GetPropertyNames(obj),
                                                           content.Split(",", "\"", true).ToList<string>(), (i + 1), errors);
                        // if there are more than 20 errors, stop loading and tell the user to look into this problem
                        if ((errors.Count >= 20))
                            break;

                        objs.Add(obj);
                    }
                }
                return objs;
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                return objs;
            }
        }

        #endregion
    }
}
