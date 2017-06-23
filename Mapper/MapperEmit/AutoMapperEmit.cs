using System;
using System.Collections.Generic;

namespace MapperEmit
{
    public class AutoMapperEmit
    {
        private static CacheStructure cache;

        public static IMapperEmit Build(Type klassSrc, Type klassDest)
        {
            if (cache == null)
                cache = new CacheStructure();
                IMapperEmit mapper =  cache.GetMapper(klassSrc, klassDest);
                if (mapper == null)
                {
                    mapper = new MapperEmit(klassSrc, klassDest);
                    cache.Add(klassSrc, klassDest, mapper);
                }
                return mapper;
        }

        public static Mapper<TSrc, TDest> Build <TSrc, TDest> ()  
        {   
            if (cache == null)
                cache = new CacheStructure();
            IMapperEmit mapper = cache.GetMapper(typeof(TSrc), typeof(TDest));
            if (mapper == null)
            {
                mapper = new Mapper<TSrc, TDest>(typeof(TSrc),typeof(TDest));
                cache.Add(typeof(TSrc), typeof(TDest), mapper);
            }
            return (Mapper<TSrc,TDest>)mapper;
        }
    }
}

