﻿using MapperReflect;

namespace MapperEmit
{
    public interface IMapperEmit
    {
        object Map(object src);
        object[] Map(object[] src);
        MapperEmit Bind(MappingEmit m);
        MapperEmit Match(string nameFrom, string nameDest);
    }
}