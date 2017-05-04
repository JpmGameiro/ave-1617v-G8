using System;
using System.Reflection;

namespace MapperReflect
{
    public class Mapper : IMapper
    {
        public static void Main () { }
        private Type klassSrc;
        private Type klassDest;
        private Handler handler;
        private ConstructorInfo [] ctorDest;
        private ParameterInfo[] parameterInfos;
        CacheStructure cache;
        private bool mappingArray;

        public Mapper(Type klassSrc, Type klassDest)
        {         
            this.klassSrc = klassSrc;
            this.klassDest = klassDest;
            mappingArray = false;
            ctorDest = klassDest.GetConstructors();
            cache = new CacheStructure();
            if (klassSrc.IsPrimitive && klassDest.IsPrimitive)
            {
                handler = new PrimitiveHandler(klassSrc, klassDest);
            }
            else
            {
                foreach (ConstructorInfo constructorInfo in ctorDest)
                {
                    if (constructorInfo.GetParameters().Length == 0)
                    {
                        handler = new PropertyHandler(klassSrc, klassDest);
                        break;
                    }
                }
                if (handler == null)
                {
                    handler = new ParameterHandler(klassSrc, klassDest, ctorDest[0]);
                }
            }
        }

        public Mapper Bind (Mapping m)
        {
            if (m.klass != handler.GetKlass())
            {
                if (m.klass == typeof(FieldInfo))
                {
                    handler = new FieldHandler(klassSrc,klassDest);         
                }
                else handler = new AttributeHandler(klassSrc, klassDest, m.klass);
            }         
            return this;
        }

        public Mapper Match(string nameFrom, string nameDest)
        {
            handler.LinkMembers(nameFrom,nameDest);
            return this;
        }

        public object Map(object objSrc)
        {
            object o = cache.GetValue(klassDest);
            if (o == null)
            {
                object toRet = handler.Copy(objSrc);
                cache.Add(klassDest, toRet);
                return toRet;
            }
            else if (o != null && mappingArray)
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