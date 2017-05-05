using System;
using System.Collections.Generic;

namespace MapperReflect
{
    public class CacheStructure
    {
        private Dictionary<Type, object> cacheDictionary;

        public CacheStructure()
        {
            cacheDictionary = new Dictionary<Type, object>();
        }

        public void Add(Type t, object o)
        {
            cacheDictionary.Add(t,o);
        }

        public object GetValue(Type t)
        {
            object o;
            if (cacheDictionary.TryGetValue(t, out o))
            {
                return o;
            }
            return null;
        }
    }

}
