using System;
using System.Reflection;
using System.Reflection.Emit;
using MapperReflect;
using System.Collections.Generic;

namespace MapperEmit
{
    public class RuntimeHandlerGenerator
    {


        public static Handler generateRuntimePrimitiveHandler()
        {
            TypeBuilder tBuilder = GetTypeBuilder("Primitive", typeof(PrimitiveHandlerBase));
            MethodBuilder mBuilder = GetMethodBuilder(tBuilder);

            ILGenerator ilGenerator = mBuilder.GetILGenerator();
            /*********************************** IL CODE ***************************/
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Ret);
            /*********************************** END ***************************/

            Type t = tBuilder.CreateType();
            return (Handler) Activator.CreateInstance(t);
        }

        public static Handler generateRuntimeParameterHandler(Type klassSrc, Type klassDest, ConstructorInfo constructorInfo)
        {
            List<KeyValuePair<MemberInfo, ParameterInfo>> parameterList = new List<KeyValuePair<MemberInfo, ParameterInfo>>();
            ParameterInfo[] parameterInfos = constructorInfo.GetParameters();
            foreach (ParameterInfo paramInfo in parameterInfos)
            {
                MemberInfo[] mInfos = klassSrc.GetMembers();
                foreach (MemberInfo mInfo in mInfos)
                {
                    if (string.Equals(mInfo.Name, paramInfo.Name, StringComparison.CurrentCultureIgnoreCase))
                        parameterList.Add(new KeyValuePair<MemberInfo, ParameterInfo>(mInfo, paramInfo));
                }
            }

            TypeBuilder tBuilder = GetTypeBuilder("Parameter", typeof(ParameterHandlerBase));
            MethodBuilder mBuilder = GetMethodBuilder(tBuilder);

            ILGenerator ilGenerator = mBuilder.GetILGenerator();
            /*********************************** IL CODE ***************************/
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, typeof(ParameterHandlerBase).GetField("parameterInfos"));
            ilGenerator.Emit(OpCodes.Newarr, typeof(object));
            ilGenerator.Emit(OpCodes.Stloc_0);
            ilGenerator.Emit(OpCodes.Stloc_1, 0);
            foreach (KeyValuePair<MemberInfo, ParameterInfo> pair in parameterList)
            {
                ilGenerator.Emit(OpCodes.Ldloc, (Type) pair.Key);

            }   

            /*********************************** END ***************************/

            //        object[] objs = new object[parameterInfos.Length];
            //        int idx = 0;
            //        foreach (KeyValuePair<MemberInfo, ParameterInfo> pair in parameterList)
            //        {
            //            objs[idx++] = extractValueFromMember(objSrc, pair.Key);
            //        }
            //        return ctor.Invoke(objs);

            //        private object extractValueFromMember(object objSrc, MemberInfo pairKey)
            //        {
            //            return pairKey is PropertyInfo
            //                ? ((PropertyInfo)pairKey).GetValue(objSrc)
            //                : ((FieldInfo)pairKey).GetValue(objSrc);
            //        }






            Type t = tBuilder.CreateType();
            return (Handler)Activator.CreateInstance(t);
        }

        public static Handler generateRuntimePropertyHandler(Type klassSrc, Type klassDest)
        {
            TypeBuilder tBuilder = GetTypeBuilder("Property", typeof(PropertyHandlerBase));
            MethodBuilder mBuilder = GetMethodBuilder(tBuilder);

            ILGenerator ilGenerator = mBuilder.GetILGenerator();
            /*********************************** IL CODE ***************************/




            /*********************************** END ***************************/
            Type t = tBuilder.CreateType();
            return (Handler)Activator.CreateInstance(t);
        }

        public static Handler generateRuntimeFieldHandler(Type klassSrc, Type klassDest)
        {
            TypeBuilder tBuilder = GetTypeBuilder("Field", typeof(FieldHandlerBase));
            MethodBuilder mBuilder = GetMethodBuilder(tBuilder);

            ILGenerator ilGenerator = mBuilder.GetILGenerator();
            /*********************************** IL CODE ***************************/




            /*********************************** END ***************************/
            Type t = tBuilder.CreateType();
            return (Handler)Activator.CreateInstance(t);
        }

        public static Handler generateRuntimeAttributeHandler(Type klassSrc, Type klassDest, Type mKlass)
        {
            TypeBuilder tBuilder = GetTypeBuilder("Attribute", typeof(ParameterHandlerBase));
            MethodBuilder mBuilder = GetMethodBuilder(tBuilder);

            ILGenerator ilGenerator = mBuilder.GetILGenerator();
            /*********************************** IL CODE ***************************/




            /*********************************** END ***************************/
            Type t = tBuilder.CreateType();
            return (Handler)Activator.CreateInstance(t);
        }

        private static MethodBuilder GetMethodBuilder(TypeBuilder tb)
        {
            return tb.DefineMethod("Copy",
                MethodAttributes.Public | MethodAttributes.ReuseSlot, typeof(object), new[] {typeof(object)});
        }

        private static TypeBuilder GetTypeBuilder(string asmName, Type klassToExtend)
        {
            string name = asmName + "HandlerEmit";
            AssemblyBuilder asm = CreateAsm(name);
            ModuleBuilder mb = asm.DefineDynamicModule(name, name + ".dll");
            TypeBuilder typeBuilder = mb.DefineType(name, TypeAttributes.Public);
            typeBuilder.SetParent(klassToExtend);
            return typeBuilder;
        }

        private static AssemblyBuilder CreateAsm(string name)
        {
            AssemblyName aName = new AssemblyName(name);
            AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.RunAndSave);
            return ab;
        }
    }
}