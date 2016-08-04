using System;
using System.ComponentModel;

namespace JukeWeb.Foundry.Utilities.Common.Attributes
{
    public class BCPAttribute : Attribute
    {
        private bool _isIncluded;
        public BCPAttribute(bool isIncluded)
        {
            _isIncluded = isIncluded;
        }

        public bool IsIncluded
        {
            get { return _isIncluded; }
        }
    }
}
