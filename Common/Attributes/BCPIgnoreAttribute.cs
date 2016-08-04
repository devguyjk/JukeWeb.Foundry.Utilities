using System;
using System.ComponentModel;

namespace JukeWeb.Foundry.Utilities.Common.Attributes
{
    public class ArrayIndexAttribute : DescriptionAttribute
    {
        public ArrayIndexAttribute(int index) : base(index.ToString()) { }

        public int Index
        {
            get { return int.Parse(Description); }
        }
    }
}
