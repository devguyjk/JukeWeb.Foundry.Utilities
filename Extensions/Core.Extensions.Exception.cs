using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class ExceptionExtensions
    {
        public static string AllMessages(this Exception exception)
        {
            StringBuilder builder = new StringBuilder();
            while (exception != null)
            {
                builder.AppendLine(exception.Message);
                builder.AppendLine();
                builder.AppendLine(exception.StackTrace);
                builder.AppendLine();
                exception = exception.InnerException;
            }

            return builder.ToString();
        }
    }
}
