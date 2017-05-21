using System;
using System.Reflection;
using MapperReflect;

namespace MapperEmit
{
    public class Mapping
    {
        public Type klass;
        public Mapping(Type klass)
        {
            this.klass = klass;
        }
        public static Mapping CustomAttributes = new Mapping(typeof(ToMapAttribute));       
        public static Mapping Fields = new Mapping(typeof(FieldInfo));
        public static Mapping Properties = new Mapping(typeof(PropertyInfo));
    }
}