using System;
using MapperReflect;

namespace MapperEmit
{
    public class PrimitiveHandler : Handler
    {
        private Type klassDest;
        private Type klassSrc;

        public PrimitiveHandler(Type klassSrc, Type klassDest)
        {
            this.klassSrc = klassSrc;
            this.klassDest = klassDest;
        }

        public override void LinkMembers(string nameSrc, string nameDest)
        {
            throw new NotImplementedException();
        }

        public override Type GetKlass()
        {
            return typeof(PrimitiveHandler);
        }

        public override object Copy(object objSrc)
        {
            object o = objSrc;
            return o;
        }
    }
}