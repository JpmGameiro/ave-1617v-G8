using System;

namespace MapperEmit
{
    public class AutoMapperEmit
    {
        public static IMapperEmit Build(Type klassSrc, Type klassDest)
        {
            return new MapperEmit(klassSrc, klassDest);
        }

        public static Mapper<TSrc, TDest> Build <TSrc, TDest> ()  
        {
            return new Mapper<TSrc, TDest>(typeof(TSrc),typeof(TDest));
        }
    }
}

