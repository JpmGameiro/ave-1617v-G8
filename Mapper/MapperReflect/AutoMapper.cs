using System;

namespace MapperReflect
{
    public class AutoMapper
    {
        private static CacheStructure cache;

        public static IMapper Build(Type klassSrc, Type klassDest)
        {
            if (cache == null)
                cache = new CacheStructure();
            IMapper mapper = cache.GetMapper(klassSrc, klassDest);
            if (mapper == null)
            {
                mapper = new Mapper(klassSrc, klassDest);
                cache.Add(klassSrc, klassDest, mapper);
            }
            return mapper;
        }
    }
}
