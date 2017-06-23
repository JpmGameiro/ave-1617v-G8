using System;
using System.Collections.Generic;
using System.IO.Pipes;

namespace MapperEmit
{
    public class CacheStructure
    {
        public Dictionary<KeyValuePair<Type,Type>, IMapperEmit> cacheDictionary;

        public CacheStructure()
        {
            cacheDictionary = new Dictionary<KeyValuePair<Type,Type>, IMapperEmit>();
        }

        public void Add(Type src, Type dest, IMapperEmit mapper)
        {
            KeyValuePair<Type,Type> pair = new KeyValuePair<Type, Type>(src,dest);
            cacheDictionary.Add(pair, mapper);
        }

        public IMapperEmit GetMapper(Type src, Type dest)
        {
            IMapperEmit mapper;
            KeyValuePair<Type,Type> pair = new KeyValuePair<Type, Type>(src,dest);
            if (cacheDictionary.TryGetValue(pair, out mapper))
            {
                return mapper;
            }
            return null;
        }
    }

}
