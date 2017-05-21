using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using MapperReflect;

namespace MapperEmit
{
    class ParameterHandlerEmit : Handler
    {
        private Type src;
        private Type dest;
        private ConstructorInfo ctor;
        private IEmitter emitter;
        public ParameterInfo[] parameterInfos;
        public List<KeyValuePair<MemberInfo, ParameterInfo>> parameterList;

        public ParameterHandlerEmit(Type src, Type dest, ConstructorInfo ctor)
        {
            this.src = src;
            this.dest = dest;
            this.ctor = ctor;
            parameterList = new List<KeyValuePair<MemberInfo, ParameterInfo>>();
            parameterInfos = ctor.GetParameters();
            foreach (ParameterInfo paramInfo in parameterInfos)
            {
                MemberInfo[] mInfos = src.GetMembers();
                foreach (MemberInfo mInfo in mInfos)
                {
                    if (string.Equals(mInfo.Name, paramInfo.Name, StringComparison.CurrentCultureIgnoreCase))
                        parameterList.Add(new KeyValuePair<MemberInfo, ParameterInfo>(mInfo, paramInfo));
                }
            }
        }

        public override Type GetKlass()
        {
            return typeof(ParameterInfo);
        }

        public override void LinkMembers(string nameSrc, string nameDest)
        {
            throw new NotImplementedException();
        }

        public override object Copy(object objSrc)
        {
            if (emitter != null)
            {
                emitter.Copy(objSrc);
            }
            TypeBuilder tBuilder = GetTypeBuilder("Parameter");
            MethodBuilder mBuilder = GetMethodBuilder(tBuilder);

            ILGenerator ilGenerator = mBuilder.GetILGenerator();

            /*********************************** IL CODE ***************************/

            LocalBuilder arr = ilGenerator.DeclareLocal(typeof(object));
            LocalBuilder i = ilGenerator.DeclareLocal(typeof(int));
            LocalBuilder Iobjs = ilGenerator.DeclareLocal(typeof(int));
            ilGenerator.Emit(OpCodes.Ldc_I4, parameterInfos.Length);
            ilGenerator.Emit(OpCodes.Newarr, typeof(object));
            ilGenerator.Emit(OpCodes.Stloc, arr);                          //new object [] 
            ilGenerator.Emit(OpCodes.Ldc_I4, 0);
            ilGenerator.Emit(OpCodes.Stloc, i);                          //i = 0
            foreach (KeyValuePair<MemberInfo, ParameterInfo> pair in parameterList)
            {
 
                if (pair.Key is FieldInfo)
                {
                    ilGenerator.Emit(OpCodes.Ldloc, arr);
                    ilGenerator.Emit(OpCodes.Ldloc, i);
                    ilGenerator.Emit(OpCodes.Ldarg_1);
                    ilGenerator.Emit(OpCodes.Ldfld, (FieldInfo)pair.Key);
                    ilGenerator.Emit(OpCodes.Stelem_Ref);
                }
                else if (pair.Key is PropertyInfo)
                {
                    ilGenerator.Emit(OpCodes.Ldloc, arr);
                    ilGenerator.Emit(OpCodes.Ldloc, i);
                    ilGenerator.Emit(OpCodes.Ldarg_1);
                    ilGenerator.Emit(OpCodes.Callvirt, src.GetProperty(pair.Key.Name).GetGetMethod());
                    ilGenerator.Emit(OpCodes.Stelem_Ref);
                }

                ilGenerator.Emit(OpCodes.Ldloc, i);
                ilGenerator.Emit(OpCodes.Ldc_I4, 1);
                ilGenerator.Emit(OpCodes.Add);
                ilGenerator.Emit(OpCodes.Stloc, i);
            }
            ilGenerator.Emit(OpCodes.Ldloc, arr);
            //ilGenerator.Emit(OpCodes.Ldloc, dest);
//            ilGenerator.Emit(OpCodes.Callvirt, typeof(Type).GetMethod("GetConstructors", Type.EmptyTypes));
//            ilGenerator.Emit(OpCodes.Ldc_I4, 0);
//            ilGenerator.Emit(OpCodes.Stloc, Iobjs);
//            ilGenerator.Emit(OpCodes.Ldloc, Iobjs);
//            ilGenerator.Emit(OpCodes.Ldelem_Ref);
//            ilGenerator.Emit(OpCodes.Callvirt, typeof(ConstructorInfo).GetMethod("Invoke", new []{typeof(object[])}));
            ilGenerator.Emit(OpCodes.Newobj, ctor);
            ilGenerator.Emit(OpCodes.Ret);

            /*********************************** END ***************************/

            Type t = tBuilder.CreateType();
            emitter = (IEmitter)Activator.CreateInstance(t);
            asm.Save(tBuilder.Name + ".dll");
            return emitter.Copy(objSrc);
        }
    }
}
