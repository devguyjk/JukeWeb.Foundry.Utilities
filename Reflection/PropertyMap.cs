using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JukeWeb.Foundry.Utilities.Reflection
{
    public class PropertyMap
    {
        public PropertyInfo SourceProperty { get; set; }
        public PropertyInfo TargetProperty { get; set; }
    }
}
