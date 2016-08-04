using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JukeWeb.Foundry.Utilities
{
    public static partial class ExtensionMethods
    {

        #region Int Extensions
        
        public static T ParseToEnum<T>(this int o)
        {
            return (T)Enum.Parse(typeof(T), o.ToString());
        }

        public static char ToChar(this int num)
        {
            return (char)((int)'a' + num);
        }

        public static string ToOrdinal(this int num)
        {
            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num + "th";
            }

            switch (num % 10)
            {
                case 1:
                    return num + "st";
                case 2:
                    return num + "nd";
                case 3:
                    return num + "rd";
            }

            return num + "th";
        }

        #endregion
    }
}
