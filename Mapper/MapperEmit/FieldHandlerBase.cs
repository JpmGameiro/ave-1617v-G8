using System;
using System.Collections.Generic;
using System.Reflection;
using MapperReflect;

namespace MapperEmit
{
    abstract class FieldHandlerBase : Handler
    {
        private Type src;
        private Type dest;
        private ConstructorInfo ctor;
        public List<KeyValuePair<FieldInfo, FieldInfo>> fieldList;
        public Dictionary<KeyValuePair<FieldInfo, FieldInfo>, IMapperEmit> map;

        public FieldHandlerBase(Type src, Type dest)
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
    }
}
