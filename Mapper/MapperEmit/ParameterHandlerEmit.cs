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

            ilGenerator.Emit(OpCodes.Ldc_I4, parameterInfos.Length);
            ilGenerator.Emit(OpCodes.Newarr, typeof(object));
            ilGenerator.Emit(OpCodes.Stloc_0);                              //new object [] 
            ilGenerator.Emit(OpCodes.Stloc_1, 0);                          //i = 0
            foreach (KeyValuePair<MemberInfo, ParameterInfo> pair in parameterList)
            {
                ilGenerator.Emit(OpCodes.Ldloc_0);
                ilGenerator.Emit(OpCodes.Ldloc_1);

                if (pair.Key is FieldInfo)
                {
                    ilGenerator.Emit(OpCodes.Ldarg_1);
                    ilGenerator.Emit(OpCodes.Ldfld, (FieldInfo)pair.Key);
                    ilGenerator.Emit(OpCodes.Stfld);
                }
                else if (pair.Key is PropertyInfo)
                {
                    ilGenerator.Emit(OpCodes.Ldarg_1);
                    ilGenerator.Emit(OpCodes.Callvirt, src.GetProperty(pair.Key.Name).GetGetMethod());
                }

                ilGenerator.Emit(OpCodes.Ldloc_1);
                ilGenerator.Emit(OpCodes.Ldc_I4, 1);
                ilGenerator.Emit(OpCodes.Add);
                ilGenerator.Emit(OpCodes.Stloc_1);
            }
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Newobj, ctor);
            ilGenerator.Emit(OpCodes.Ret);

            /*********************************** END ***************************/

            Type t = tBuilder.CreateType();
            emitter = (IEmitter)Activator.CreateInstance(t);
            return emitter.Copy(objSrc);
        }
    }
}
