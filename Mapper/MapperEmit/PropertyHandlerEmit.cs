using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using MapperReflect;

namespace MapperEmit
{
    class PropertyHandlerEmit : Handler
    {
        private Type src;
        private Type dest;
        private ConstructorInfo ctor;
        private IEmitter emitter;
        public List<KeyValuePair<PropertyInfo, PropertyInfo>> propertyList;
        public Dictionary<KeyValuePair<PropertyInfo, PropertyInfo>, IMapperEmit> map;

        public PropertyHandlerEmit(Type src, Type dest)
        {
            this.src = src;
            this.dest = dest;
            propertyList = new List<KeyValuePair<PropertyInfo, PropertyInfo>>();
            map = new Dictionary<KeyValuePair<PropertyInfo, PropertyInfo>, IMapperEmit>();
            foreach (PropertyInfo piDest in dest.GetProperties())
            {
                PropertyInfo piSrc = src.GetProperty(piDest.Name);
                if (piSrc != null && piDest.PropertyType.IsAssignableFrom(piSrc.PropertyType))
                    propertyList.Add(new KeyValuePair<PropertyInfo, PropertyInfo>(piSrc, piDest));
            }
        }

        public override Type GetKlass()
        {
            return typeof(PropertyInfo);
        }

        public override void LinkMembers(string nameSrc, string nameDest)
        {
            PropertyInfo pInfoSrc = src.GetProperty(nameSrc);
            PropertyInfo pInfoDest = dest.GetProperty(nameDest);
            if (pInfoSrc != null && pInfoDest != null)
            {
                if (pInfoSrc.PropertyType.IsAssignableFrom(pInfoDest.PropertyType))
                {
                    propertyList.Add(new KeyValuePair<PropertyInfo, PropertyInfo>(pInfoSrc, pInfoDest));
                }
                else if (!pInfoSrc.PropertyType.IsPrimitive && !pInfoDest.PropertyType.IsPrimitive &&
                         pInfoSrc.PropertyType != typeof(string) && pInfoDest.PropertyType != typeof(string))
                {
                    IMapperEmit m = new MapperEmit(pInfoSrc.PropertyType, pInfoDest.PropertyType);
                    KeyValuePair<PropertyInfo, PropertyInfo> pair =
                        new KeyValuePair<PropertyInfo, PropertyInfo>(pInfoSrc, pInfoDest);
                    map.Add(pair, m);
                    propertyList.Add(pair);
                }
            }
        }

        public override object Copy(object objSrc)
        {
            IMapperEmit m = null;
            if (emitter != null)
            {
                return emitter.Copy(objSrc);
            }
            TypeBuilder tBuilder = GetTypeBuilder("Property");
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
            ilGenerator.Emit(OpCodes.Newobj, dest);
            ilGenerator.Emit(OpCodes.Stloc_0);
            foreach (KeyValuePair<PropertyInfo, PropertyInfo> pair in propertyList)
            {
                if (pair.Key.PropertyType.IsAssignableFrom(pair.Value.PropertyType))
                {
                    ilGenerator.Emit(OpCodes.Ldloc_0);
                    ilGenerator.Emit(OpCodes.Ldarg_1);
                    ilGenerator.Emit(OpCodes.Callvirt, pair.Key.GetGetMethod());
                    ilGenerator.Emit(OpCodes.Callvirt, pair.Value.GetSetMethod());
                }
                else
                {                   
                    if (map.TryGetValue(pair, out m))
                    {
                        ilGenerator.Emit(OpCodes.Ldloc_0);
                        ilGenerator.Emit(OpCodes.Ldarg_1);
                        ilGenerator.Emit(OpCodes.Callvirt, pair.Key.GetGetMethod());
                        ilGenerator.Emit(OpCodes.Ldfld, fb);
                        ilGenerator.Emit(OpCodes.Callvirt, typeof(IMapperEmit).GetMethod("Map"));
                        ilGenerator.Emit(OpCodes.Callvirt, pair.Value.GetSetMethod());
                   }
                }
                ilGenerator.Emit(OpCodes.Ldloc_0);
                ilGenerator.Emit(OpCodes.Ret);
            }

            Type t = tBuilder.CreateType();
            emitter = (IEmitter)Activator.CreateInstance(t,m);
            return emitter.Copy(objSrc);
        }
    }
}