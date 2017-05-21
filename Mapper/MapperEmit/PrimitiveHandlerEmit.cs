using System;
using System.Reflection.Emit;
using MapperReflect;

namespace MapperEmit
{
    class PrimitiveHandlerEmit : HandlerEmit
    {
        private Type klassDest;
        private Type klassSrc;
        private IEmitter emitter;

        public PrimitiveHandlerEmit(Type klassSrc, Type klassDest)
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
            if (emitter != null)
            {
                return emitter.Copy(objSrc);
            }
            TypeBuilder tBuilder = GetTypeBuilder("Primitive");
            MethodBuilder mBuilder = GetMethodBuilder(tBuilder);

            ILGenerator ilGenerator = mBuilder.GetILGenerator();

            /*********************************** IL CODE ***************************/

            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Ret);

            /*********************************** END ***************************/

            Type t = tBuilder.CreateType();
            emitter = (IEmitter)Activator.CreateInstance(t);
            asm.Save(tBuilder.Name + ".dll");
            return emitter.Copy(objSrc);
        }
    }
}
