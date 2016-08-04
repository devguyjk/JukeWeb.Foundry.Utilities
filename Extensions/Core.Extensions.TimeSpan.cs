using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class TimeSpanExtensions
    {
        public static string ToFriendlyString(this TimeSpan t)
        {
            if (t.TotalMinutes < 1)
                return string.Format("{0} (secs)", t.Seconds);
            string explanation = "secs";
            string timePart = t.ToString("ss");
            if (t.TotalMinutes >= 1)
            {
                timePart = t.ToString("mm") + ":" + timePart;
                explanation = "mins:" + explanation;
            }
            if (t.TotalHours >= 1)
            {
                timePart = t.ToString("hh") + ":" + timePart;
                explanation = "hours:" + explanation;
            }
            if(t.TotalDays >= 1)
                timePart = t.Days + " days and " + timePart;

            return string.Format("{0} ({1})", timePart, explanation);
        }
    }
}
