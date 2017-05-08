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
            //ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldc_I4, parameterInfos.Length);
            ilGenerator.Emit(OpCodes.Newarr, typeof(object));
            ilGenerator.Emit(OpCodes.Stloc_0);
            ilGenerator.Emit(OpCodes.Stloc_1, 0);
            foreach (KeyValuePair<MemberInfo, ParameterInfo> pair in parameterList)
            {
                ilGenerator.Emit(OpCodes.Ldloc_0);
                ilGenerator.Emit(OpCodes.Ldloc_1);

                if (pair.Key is FieldInfo)
                {
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    ilGenerator.Emit(OpCodes.Ldfld, (FieldInfo) pair.Key);
                    ilGenerator.Emit(OpCodes.Stfld);
                }
                else if (pair.Key is PropertyInfo)
                {
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    ilGenerator.Emit(OpCodes.Call, klassSrc.GetProperty(pair.Key.Name).GetGetMethod());

                }
                ilGenerator.Emit(OpCodes.Ldloc_1);
                ilGenerator.Emit(OpCodes.Ldc_I4, 1);
                ilGenerator.Emit(OpCodes.Add);
                ilGenerator.Emit(OpCodes.Stloc_1);
            }
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Newobj, constructorInfo);
            ilGenerator.Emit(OpCodes.Ret);


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

        private static void method(MemberInfo pairKey)
        {
            
        }

        public static Handler generateRuntimePropertyHandler(Type klassSrc, Type klassDest)
        {
            TypeBuilder tBuilder = GetTypeBuilder("Property", typeof(PropertyHandlerBase));
            MethodBuilder mBuilder = GetMethodBuilder(tBuilder);

            ILGenerator ilGenerator = mBuilder.GetILGenerator();
            /*********************************** IL CODE ***************************/




            /*********************************** END ***************************/

//            object objDest = Activator.CreateInstance(dest);
//            if (propertyList != null)
//            {
//                foreach (KeyValuePair<PropertyInfo, PropertyInfo> pair in propertyList)
//                {
//                    PropertyInfo propertyValue = pair.Value;
//                    PropertyInfo propertyKey = pair.Key;
//                    if (propertyKey.PropertyType.IsAssignableFrom(propertyValue.PropertyType))
//                    {
//                        propertyValue.SetValue(objDest, propertyKey.GetValue(objSrc));
//                    }
//                    else
//                    {
//                        IMapper m;
//                        if (map.TryGetValue(pair, out m))
//                        {
//                            propertyValue.SetValue(objDest, m.Map(propertyKey.GetValue(objSrc)));
//                        }
//                    }
//                }
//            }
//            return objDest;



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
//            return objDest;

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

//            object objDest = Activator.CreateInstance(dest);
//            if (attributeList != null)
//            {
//                foreach (KeyValuePair<MemberInfo, MemberInfo> pair in attributeList)
//                {
//                    if (pair.Key is PropertyInfo)
//                    {
//                        ((PropertyInfo)pair.Value).SetValue(objDest, ((PropertyInfo)pair.Key).GetValue(objSrc));
//                    }
//                    else if (pair.Key is FieldInfo)
//                        ((FieldInfo)pair.Value).SetValue(objDest, ((FieldInfo)pair.Key).GetValue(objSrc));
//                }
//            }
//            return objDest;

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