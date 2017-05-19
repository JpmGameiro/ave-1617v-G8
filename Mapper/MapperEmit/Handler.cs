using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MapperEmit
{
    public abstract class Handler
    {
        public abstract void LinkMembers(string nameSrc, string nameDest);
        public abstract Type GetKlass();
        public abstract object Copy(object objSrc);

        public static TypeBuilder GetTypeBuilder(string asmName)
        {
            string name = asmName + "CopyEmitter";
            AssemblyBuilder asm = CreateAsm(name);
            ModuleBuilder mb = asm.DefineDynamicModule(name, name + ".dll");
            TypeBuilder typeBuilder = mb.DefineType(name, TypeAttributes.Public);
            typeBuilder.AddInterfaceImplementation(typeof(IEmitter));
            return typeBuilder;
        }

        public static AssemblyBuilder CreateAsm(string name)
        {
            AssemblyName aName = new AssemblyName(name);
            AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.RunAndSave);
            return ab;
        }

        public static MethodBuilder GetMethodBuilder(TypeBuilder tb)
        {
            return tb.DefineMethod("Copy",
                MethodAttributes.Public | MethodAttributes.ReuseSlot, typeof(object), new[] { typeof(object) });
        }
    }
}
