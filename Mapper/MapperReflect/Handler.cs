using System;

namespace MapperReflect
{
    public abstract class Handler
    {
        public abstract void LinkMembers(string nameSrc, string nameDest);
        public abstract Type GetKlass();
        public abstract object Copy(object objSrc);
    }
}
