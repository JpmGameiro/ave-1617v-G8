using System;
using System.Collections.Generic;
using System.Reflection;

namespace MapperReflect
{
    public class PropertyHandler : Handler
    {
        private Type src;
        private Type dest;
        private ConstructorInfo ctor;
        public List <KeyValuePair<PropertyInfo, PropertyInfo>> propertyList;
        public Dictionary<KeyValuePair<PropertyInfo, PropertyInfo>, IMapper> map;

        public PropertyHandler(Type src, Type dest)
        {
            this.src = src;
            this.dest = dest;
            propertyList = new List<KeyValuePair<PropertyInfo, PropertyInfo>>();
            map = new Dictionary<KeyValuePair<PropertyInfo, PropertyInfo>, IMapper>();
            foreach (PropertyInfo piDest in dest.GetProperties())
            {
                PropertyInfo piSrc = src.GetProperty(piDest.Name);
                if (piSrc != null && piDest.PropertyType.IsAssignableFrom( piSrc.PropertyType))
                    propertyList.Add(new KeyValuePair<PropertyInfo, PropertyInfo>(piSrc, piDest));
            }
        }

        public override object Copy(object objSrc)
        {
            object objDest = Activator.CreateInstance(dest);
            if (propertyList != null)
            {
                foreach (KeyValuePair<PropertyInfo, PropertyInfo> pair in propertyList)
                {
                    PropertyInfo propertyValue = pair.Value;
                    PropertyInfo propertyKey = pair.Key;
                    if (propertyKey.PropertyType.IsAssignableFrom(propertyValue.PropertyType))
                    {
                        propertyValue.SetValue(objDest, propertyKey.GetValue(objSrc));
                    }
                    else
                    {
                        IMapper m;
                        if (map.TryGetValue(pair, out m))
                        {
                            propertyValue.SetValue(objDest, m.Map(propertyKey.GetValue(objSrc)));
                        }
                    }     
                }
            }
            return objDest;
        }

        public override Type GetKlass()
        {
            return typeof(PropertyInfo);
        }

        public override void LinkMembers(string nameSrc, string nameDest)
        {
            PropertyInfo pInfoSrc = src.GetProperty(nameSrc);
            PropertyInfo pInfoDest = dest.GetProperty(nameDest);
            if (pInfoSrc != null && pInfoDest != null )
            {
                if (pInfoSrc.PropertyType.IsAssignableFrom(pInfoDest.PropertyType))
                {
                    propertyList.Add(new KeyValuePair<PropertyInfo, PropertyInfo>(pInfoSrc, pInfoDest));
                }
                else if (!pInfoSrc.PropertyType.IsPrimitive && !pInfoDest.PropertyType.IsPrimitive &&
                         pInfoSrc.PropertyType != typeof(string) && pInfoDest.PropertyType != typeof(string))
                {
                    IMapper m = new Mapper(pInfoSrc.PropertyType, pInfoDest.PropertyType);
                    KeyValuePair<PropertyInfo, PropertyInfo> pair =
                        new KeyValuePair<PropertyInfo, PropertyInfo>(pInfoSrc, pInfoDest);
                    map.Add(pair , m);
                    propertyList.Add(pair);
                }
            }
        }
    }
}