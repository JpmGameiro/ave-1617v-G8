using System;

namespace MapperEmit
{
    public class AutoMapperEmit
    {
        public static IMapperEmit Build(Type klassSrc, Type klassDest)
        {
            return new MapperEmit(klassSrc, klassDest);
        }
    }
}

