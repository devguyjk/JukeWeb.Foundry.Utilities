using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class DoubleExtensions
    {
        public static string ToFriendlyString(this double d)
        {
            if (d == (int)d)
                return ((int)d).ToString();
            if (d > 0 && d < 1)
                return d.ToString(".00");
            return d.ToString("0.00");
        }
    }
}
