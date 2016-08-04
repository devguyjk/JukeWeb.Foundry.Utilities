using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JukeWeb.Foundry.Utilities.Reflection
{
    public class ObjectBinderReflection : ObjectBinderBase
    {
        private readonly Dictionary<string, PropertyMap[]> _maps = new Dictionary<string, PropertyMap[]>();
        public override void MapTypes<T, TU>()        
        {
            var source = typeof(T);
            var target = typeof(TU);
            var key = GetMapKey<T, TU>();
            if (_maps.ContainsKey(key))
                return;
            var props = GetMatchingProperties<T, TU>();
            _maps.Add(key, props.ToArray());
        }

        public override void Copy<T, TU>(T source, TU target)
        {
            var key = GetMapKey<T, TU>();
            if (!_maps.ContainsKey(key))
                MapTypes<T, TU>();
            var propMap = _maps[key];
            for (var i = 0; i < propMap.Length; i++)
            {
                var prop = propMap[i];
                var sourceValue = prop.SourceProperty.GetValue(source, null);
                prop.TargetProperty.SetValue(target, sourceValue, null);
            }
        }
    }
}
