using System;
using MapperReflect;

namespace MapperEmit
{
    abstract class PrimitiveHandlerBase : Handler
    {
        private Type klassDest;
        private Type klassSrc;

        public PrimitiveHandlerBase(Type klassSrc, Type klassDest)
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
    }
}
