using System;
using System.Reflection;
using System.Reflection.Emit;
using MapperReflect;

namespace MapperEmit
{
    public class RuntimeHandlerGenerator
    {
        private static MethodBuilder GetMethodBuilder(TypeBuilder tb)
        {
            return tb.DefineMethod("Copy",
                MethodAttributes.Public | MethodAttributes.ReuseSlot, typeof(object), new []{typeof(object)});
        }

        private static TypeBuilder GetTypeBuilder(string asmName, Type klassToExtend)
        {
            AssemblyBuilder asm = CreateAsm(asmName);
            ModuleBuilder mb = asm.DefineDynamicModule(asmName, asmName + ".dll");
            TypeBuilder typeBuilder = mb.DefineType(asmName, TypeAttributes.Public);
            typeBuilder.SetParent(klassToExtend);
            return typeBuilder;
        }

        private static AssemblyBuilder CreateAsm(string name)
        {
            AssemblyName aName = new AssemblyName(name);
            AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.RunAndSave);
            return ab;
        }

        public static Handler generateRuntimePrimitiveHandler(Type klassSrc, Type klassDest)
        {
            TypeBuilder tBuilder = GetTypeBuilder("Primitive", typeof(PrimitiveHandlerBase));
            MethodBuilder mBuilder = GetMethodBuilder(tBuilder);
            ///////////////////////////codigo il/////////////////////
//            object objDest = Activator.CreateInstance(dest);
//            if (fieldList != null)
//            {
//                foreach (KeyValuePair<FieldInfo, FieldInfo> pair in fieldList)
//                {
//                    FieldInfo fieldValue = pair.Value;
//                    FieldInfo fieldKey = pair.Key;
//                    if (fieldKey.FieldType.IsAssignableFrom(fieldValue.FieldType))
//                    {
//                        fieldValue.SetValue(objDest, fieldKey.GetValue(objSrc));
//                    }
//                    else
//                    {
//                        IMapper m;
//                        if (map.TryGetValue(pair, out m))
//                        {
//                            fieldValue.SetValue(objDest, m.Map(fieldKey.GetValue(objSrc)));
//                        }
//                    }
//                }
//            }
            ILGenerator ilGenerator = mBuilder.GetILGenerator();
            ilGenerator.Emit(OpCodes.Newobj, klassDest);


            Type t = tBuilder.CreateType();
            return (Handler)Activator.CreateInstance(t);
        }

        public static Handler generateRuntimePropertyHandler(Type klassSrc, Type klassDest)
        {
            throw new NotImplementedException();
        }

        public static Handler generateRuntimeParameterHandler(Type klassSrc, Type klassDest, ConstructorInfo constructorInfo)
        {
            throw new NotImplementedException();
        }

        public static Handler generateRuntimeFieldHandler(Type klassSrc, Type klassDest)
        {
            throw new NotImplementedException();
        }

        public static Handler generateRuntimeAttributeHandler(Type klassSrc, Type klassDest, Type mKlass)
        {
            throw new NotImplementedException();
        }
    }