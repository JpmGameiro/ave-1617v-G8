using System;
using System.Collections.Generic;
using System.Reflection;
using MapperReflect;

namespace MapperEmit
{
    abstract class PropertyHandlerBase : Handler
    {
        private Type src;
        private Type dest;
        private ConstructorInfo ctor;
        public List<KeyValuePair<PropertyInfo, PropertyInfo>> propertyList;
        public Dictionary<KeyValuePair<PropertyInfo, PropertyInfo>, IMapperEmit> map;

        public PropertyHandlerBase(Type src, Type dest)
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
    }
}
