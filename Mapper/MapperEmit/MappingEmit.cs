using System;
using System.Reflection;

namespace MapperEmit
{
    public class MappingEmit
    {
        public Type klass;
        public MappingEmit(Type klass)
        {
            this.klass = klass;
        }
        public static MappingEmit CustomAttributes = new MappingEmit(typeof(ToMapAttribute));
        public static MappingEmit Fields = new MappingEmit(typeof(FieldInfo));
        public static MappingEmit Properties = new MappingEmit(typeof(PropertyInfo));
    }
}
