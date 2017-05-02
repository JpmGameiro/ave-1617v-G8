namespace MapperReflect
{
    public interface IMapper
    {
        object Map(object src);
        object[] Map(object[] src);
        Mapper Bind(Mapping m );
        Mapper Match(string nameFrom, string nameDest);
    }
}
