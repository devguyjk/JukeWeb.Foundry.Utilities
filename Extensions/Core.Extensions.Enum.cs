using System;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using JukeWeb.Foundry.Utilities.Common.Attributes;

using JukeWeb.Foundry.Utilities.Common;

namespace JukeWeb.Foundry.Utilities
{
    public static partial class ExtensionMethods
    {
        #region Enum Extensions

        // ENUM: FLAG RELATED
        public static IEnumerable<Enum> GetFlags(this Enum value)
        {
            return GetFlags(value, Enum.GetValues(value.GetType()).Cast<Enum>().ToArray());
        }

        public static IEnumerable<Enum> GetIndividualFlags(this Enum value)
        {
            return GetFlags(value, GetFlagValues(value.GetType()).ToArray());
        }

        private static IEnumerable<Enum> GetFlags(Enum value, Enum[] values)
        {
            ulong bits = Convert.ToUInt64(value);
            List<Enum> results = new List<Enum>();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                ulong mask = Convert.ToUInt64(values[i]);
                if (i == 0 && mask == 0L)
                    break;
                if ((bits & mask) == mask)
                {
                    results.Add(values[i]);
                    bits -= mask;
                }
            }
            if (bits != 0L)
                return Enumerable.Empty<Enum>();
            if (Convert.ToUInt64(value) != 0L)
                return results.Reverse<Enum>();
            if (bits == Convert.ToUInt64(value) && values.Length > 0 && Convert.ToUInt64(values[0]) == 0L)
                return values.Take(1);
            return Enumerable.Empty<Enum>();
        }

        private static IEnumerable<Enum> GetFlagValues(Type enumType)
        {
            ulong flag = 0x1;
            foreach (var value in Enum.GetValues(enumType).Cast<Enum>())
            {
                ulong bits = Convert.ToUInt64(value);
                if (bits == 0L)
                    //yield return value;
                    continue; // skip the zero value
                while (flag < bits) flag <<= 1;
                if (flag == bits)
                    yield return value;
            }
        }


        public static int ConvertToBitFlags(this Enum en)
        {
            return (int)Enum.Parse(en.GetType(), en.ToString());
        }

        //ENUM : Attribute / Value / Name Related

        public static TAttribute GetAttribute<TAttribute>(this Enum value)
            where TAttribute : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name) // I prefer to get attributes this way
                .GetCustomAttributes(false)
                .OfType<TAttribute>()
                .SingleOrDefault();
        }

        public static Dictionary<int, string> ToDictionary(this Enum @enum)
        {
            return ToInts(@enum).ToDictionary(e => e, e => Enum.GetName(@enum.GetType(), e));
        }

        public static List<int> ToInts(this Enum @enum)
        {
            var type = @enum.GetType();
            return Enum.GetValues(type).Cast<int>().ToList();
        }

        public static List<string> ToStrings(this Enum @enum)
        {
            return ToInts(@enum).Select(x => Enum.GetName(@enum.GetType(), x)).ToList<string>();
        }

        public static Expected GetAttributeValue<T, Expected>(this Enum enumeration, Func<T, Expected> expression)
        where T : Attribute
        {
            T attribute = enumeration.GetType().GetMember(enumeration.ToString())[0].GetCustomAttributes(typeof(T), false).Cast<T>().SingleOrDefault();

            if (attribute == null)
                return default(Expected);

            return expression(attribute);
        }

        public static List<int> GetIndexAttributes(this Enum @enum)
        {
            List<int> returnValues = new List<int>();
            foreach(Enum enumVal in GetFlags(@enum))
            {
                returnValues.Add(GetAttribute<ArrayIndexAttribute>(enumVal).Index);
            }
            return returnValues;
        }

        public static List<Enum> GetEnumListByBit(this Object o, Type enumType, int bit)
        {
            var enumList = new List<Enum>();
            var enumNames = Enum.GetNames(enumType);

            foreach (var enumName in enumNames)
            {
                var enumBit = (Enum)Enum.Parse(enumType, enumName);
                var enumBitNum = (int)Enum.Parse(enumType, enumName);
                int check = bit & enumBitNum;
                if (check != 0)
                {
                    enumList.Add(enumBit);
                }
            }
            return enumList;
        }

        public static Enum GetEnumByDescription(this Object o, Type enumType, string enumDesc)
        {
            var enumNames = Enum.GetNames(enumType);

            foreach (var enumName in enumNames)
            {
                var memInfo = enumType.GetMember(enumName);
                var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute),
                    false);
                var enumDescription = ((DescriptionAttribute)attributes[0]).Description;
                if (enumDescription == enumDesc)
                {
                    return (Enum)Enum.Parse(enumType, enumName);
                }
            }
            return null;
        }
        #endregion
    }
}
