using System;
using System.Collections.Generic;

namespace MapperReflect
{
    public class CacheStructure
    {
        public Dictionary<KeyValuePair<Type, Type>, IMapper> cacheDictionary;

        public CacheStructure()
        {
            cacheDictionary = new Dictionary<KeyValuePair<Type, Type>, IMapper>();
        }

        public void Add(Type src, Type dest, IMapper mapper)
        {
            KeyValuePair<Type, Type> pair = new KeyValuePair<Type, Type>(src, dest);
            cacheDictionary.Add(pair, mapper);
        }

        public IMapper GetMapper(Type src, Type dest)
        {
            IMapper mapper;
            KeyValuePair<Type, Type> pair = new KeyValuePair<Type, Type>(src, dest);
            if (cacheDictionary.TryGetValue(pair, out mapper))
            {
                return mapper;
            }
            return null;
        }
    }

}
