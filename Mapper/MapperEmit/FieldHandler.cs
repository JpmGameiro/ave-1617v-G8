using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using MapperReflect;

namespace MapperEmit
{
    public class FieldHandler : Handler
    {
        private Type src;
        private Type dest;
        private ConstructorInfo ctor;
        public List<KeyValuePair<FieldInfo, FieldInfo>> fieldList;
        public Dictionary<KeyValuePair<FieldInfo, FieldInfo>, IMapper> map;
        public delegate object GetInstance(Type t); 

        public FieldHandler(Type src, Type dest)
        {
            this.src = src;
            this.dest = dest;
            fieldList = new List<KeyValuePair<FieldInfo, FieldInfo>>();
            map = new Dictionary<KeyValuePair<FieldInfo, FieldInfo>, IMapper>();
            foreach (FieldInfo fiDest in dest.GetFields())
            {
                FieldInfo fiSrc = src.GetField(fiDest.Name);
                if (fiSrc != null && fiDest.FieldType.IsAssignableFrom(fiSrc.FieldType))
                    fieldList.Add(new KeyValuePair<FieldInfo, FieldInfo>(fiSrc, fiDest));
            }
        }

        public override object Copy(object objSrc)
        {
            //criar instancia de obj de um determinado tipo
            /*PSEUDO CODIGO PARA EMIT DA INSTANCIA DE UM OBJECTO DE UM CERTO TYPE*/
            DynamicMethod dMethod = new DynamicMethod("GetInstance", typeof(object), new []{ typeof(Type) });
            ILGenerator generator = dMethod.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, typeof(Activator).GetMethod("CreateInstance"));
            generator.Emit(OpCodes.Ret);
           // ObjectGeneratorEmit(generator, ctor);

            GetInstance method = dMethod.CreateDelegate;
            object objDest = method.Invoke(dest);

            if (fieldList != null)
            {
                foreach (KeyValuePair<FieldInfo, FieldInfo> pair in fieldList)
                {
                    //COLOCAR PAR CHAVE VALOR NA STACK
                    generator.Emit(OpCodes.Stfld, pair.Value);
                    generator.Emit(OpCodes.Stfld, pair.Key);


                }
            }
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
            return null;
        }

        private void ObjectGeneratorEmit(ILGenerator generator, ConstructorInfo ctor)
        {
            generator.Emit(OpCodes.Newobj, ctor);
            generator.Emit(OpCodes.Ret);
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
                    IMapper m = new Mapper(fInfoSrc.FieldType, fInfoDest.FieldType);
                    map.Add(pair , m);
                    fieldList.Add(pair);
                }           
            }
        }
    }
}