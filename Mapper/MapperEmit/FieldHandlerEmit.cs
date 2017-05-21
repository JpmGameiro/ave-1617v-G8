using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using MapperReflect;

namespace MapperEmit
{
    class FieldHandlerEmit : HandlerEmit
    {
        private Type src;
        private Type dest;
        private ConstructorInfo ctor;
        private IEmitter emitter;
        public List<KeyValuePair<FieldInfo, FieldInfo>> fieldList;
        public Dictionary<KeyValuePair<FieldInfo, FieldInfo>, IMapperEmit> map;

        public FieldHandlerEmit(Type src, Type dest)
        {
            this.src = src;
            this.dest = dest;
            fieldList = new List<KeyValuePair<FieldInfo, FieldInfo>>();
            map = new Dictionary<KeyValuePair<FieldInfo, FieldInfo>, IMapperEmit>();
            foreach (FieldInfo fiDest in dest.GetFields())
            {
                FieldInfo fiSrc = src.GetField(fiDest.Name);
                if (fiSrc != null && fiDest.FieldType.IsAssignableFrom(fiSrc.FieldType))
                    fieldList.Add(new KeyValuePair<FieldInfo, FieldInfo>(fiSrc, fiDest));
            }
        }

        public override Type GetKlass()
        {
            return typeof(FieldInfo);
        }

        public override void LinkMembers(string nameSrc, string nameDest)
        {
            FieldInfo fInfoSrc = src.GetField(nameSrc);
            FieldInfo fInfoDest = dest.GetField(nameDest);
            if (fInfoSrc != null && fInfoDest != null)
            {
                if (fInfoSrc.FieldType.IsAssignableFrom(fInfoDest.FieldType))
                {
                    fieldList.Add(new KeyValuePair<FieldInfo, FieldInfo>(fInfoSrc, fInfoDest));
                }
                else if (!fInfoSrc.FieldType.IsPrimitive && !fInfoDest.FieldType.IsPrimitive &&
                         fInfoSrc.FieldType != typeof(string) && fInfoDest.FieldType != typeof(string))
                {
                    KeyValuePair<FieldInfo, FieldInfo> pair = new KeyValuePair<FieldInfo, FieldInfo>(fInfoSrc, fInfoDest);
                    IMapperEmit m = new MapperEmit(fInfoSrc.FieldType, fInfoDest.FieldType);
                    map.Add(pair, m);
                    fieldList.Add(pair);
                }
            }
        }

        public override object Copy(object objSrc)
        {
            IMapperEmit m = null;
            if (emitter != null)
            {
                emitter.Copy(objSrc);
            }
            TypeBuilder tBuilder = GetTypeBuilder("Field");
            MethodBuilder mBuilder = GetMethodBuilder(tBuilder);
            FieldBuilder fb = tBuilder.DefineField(
                                "target",
                                typeof(IMapperEmit),
                                FieldAttributes.Private);
            ConstructorBuilder cb = tBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.ExplicitThis,
                new Type[] { typeof(IMapperEmit) });

            ILGenerator cbIlGen = cb.GetILGenerator();
            cbIlGen.Emit(OpCodes.Ldarg_0);
            cbIlGen.Emit(OpCodes.Ldarg_1);
            cbIlGen.Emit(OpCodes.Stfld, fb);
            cbIlGen.Emit(OpCodes.Ret);

            ILGenerator ilGenerator = mBuilder.GetILGenerator();


            /*********************************** IL CODE ***************************/

            ilGenerator.Emit(OpCodes.Newobj, dest.GetConstructors()[0]);
            foreach (KeyValuePair<FieldInfo, FieldInfo> pair in fieldList)
            {
                if (pair.Key.FieldType.IsAssignableFrom(pair.Value.FieldType))
                {
                    ilGenerator.Emit(OpCodes.Dup);
                    ilGenerator.Emit(OpCodes.Ldarg_1);
                    ilGenerator.Emit(OpCodes.Ldfld, ((FieldInfo)pair.Key));
                    ilGenerator.Emit(OpCodes.Stfld, ((FieldInfo)pair.Value));
                }
                else
                {
                    
                    if (map.TryGetValue(pair,out m))
                    {
                        ilGenerator.Emit(OpCodes.Dup);
                        ilGenerator.Emit(OpCodes.Ldarg_1);
                        ilGenerator.Emit(OpCodes.Ldfld, pair.Key);
                        ilGenerator.Emit(OpCodes.Ldfld, fb);
                        ilGenerator.Emit(OpCodes.Callvirt, typeof(IMapperEmit).GetMethod("Map"));
                        ilGenerator.Emit(OpCodes.Stfld, pair.Value);

                    }
                }             
            }

            ilGenerator.Emit(OpCodes.Ret);
            /*********************************** END ***************************/

            Type t = tBuilder.CreateType();
            emitter = (IEmitter)Activator.CreateInstance(t,m);
            asm.Save(tBuilder.Name + ".dll");
            return emitter.Copy(objSrc);

        }
    }
}
