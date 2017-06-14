using System;
using System.Collections.Generic;
using System.Reflection;
using MapperReflect;

namespace MapperEmit
{
    public class MapperEmit : IMapperEmit
    {
        public static void Main () { }
        private Type klassSrc;
        private Type klassDest;
        private HandlerEmit handler;
        private ConstructorInfo [] ctorDest;
        private ParameterInfo[] parameterInfos;
        CacheStructure cache;
        public bool mappingArray;
        public List<KeyValuePair<string, Func<object>>> map;

        public MapperEmit(Type klassSrc, Type klassDest)
        {         
            this.klassSrc = klassSrc;
            this.klassDest = klassDest;
            map = new List<KeyValuePair<string, Func<object>>>();
            mappingArray = false;
            ctorDest = klassDest.GetConstructors();
            cache = new CacheStructure();
            if (klassSrc.IsPrimitive && klassDest.IsPrimitive)
            {
                handler = new PrimitiveHandlerEmit(klassSrc, klassDest);
            }
            else
            {
                foreach (ConstructorInfo constructorInfo in ctorDest)
                {
                    if (constructorInfo.GetParameters().Length == 0)
                    {
                        handler = new PropertyHandlerEmit(klassSrc, klassDest);
                        break;
                    }
                }
                if (handler == null)
                {
                    handler =  new ParameterHandlerEmit(klassSrc, klassDest, ctorDest[0]);
                }
            }
        }

        public MapperEmit Bind (MappingEmit m)
        {
            if (m.klass != handler.GetKlass())
            {
                if (m.klass == typeof(FieldInfo))
                {
                    handler = new FieldHandlerEmit(klassSrc, klassDest);
                }
                else handler = new AttributeHandlerEmit(klassSrc, klassDest, m.klass);
            }         
            return this;
        }

        public MapperEmit Match(string nameFrom, string nameDest)
        {
            handler.LinkMembers(nameFrom,nameDest);
            return this;
        }

        public object Map(object objSrc)
        {
            if (map.Count != 0)
            {
                foreach (KeyValuePair<string, Func<object>> pair in map)
                {
                    PropertyInfo pInfo = objSrc.GetType().GetProperty(pair.Key);
                    if (pInfo != null)
                    {
                        pInfo.SetValue(objSrc, pair.Value.Invoke());
                    }
                }
            }
            object o = cache.GetValue(klassDest);
            if (o == null)
            {
                object toRet = handler.Copy(objSrc);
                cache.Add(klassDest, toRet);
                return toRet;
            }
            if (mappingArray)
            {
                object toRet = handler.Copy(objSrc);
                return toRet;
            }
            return o;
        }

        public object[] Map(object[] objSrc)
        {
            mappingArray = true;
            Array array = Array.CreateInstance(klassDest,objSrc.Length);
            for (int i = 0; i < array.Length; i++)
            {
                array.SetValue(this.Map(objSrc[i]), i);
            }
            return (object[])array;
        }
    }
}