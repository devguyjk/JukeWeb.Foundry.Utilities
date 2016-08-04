using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JukeWeb.Foundry.Exceptions;

namespace JukeWeb.Foundry.Utilities.Common
{
    public partial class FoundryGeneralException : GeneralException
    {

        public FoundryGeneralException(string message) : base(message) { }
        public FoundryGeneralException(string message, Exception innerException) : base(message, innerException) { }
        public FoundryGeneralException(string message, ExceptionTypes exceptionType, Exception innerException)
            : base(message, exceptionType, innerException)
        { }


        protected override void InvokeLogging()
        {
            base.InvokeLogging();
        }
    }
}
