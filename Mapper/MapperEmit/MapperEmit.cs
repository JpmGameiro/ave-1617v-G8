﻿using System;
using System.Reflection;
using MapperReflect;

namespace MapperEmit
{
    public class MapperEmit : IMapperEmit
    {
        public static void Main () { }
        private Type klassSrc;
        private Type klassDest;
        private Handler handler;
        private ConstructorInfo [] ctorDest;
        private ParameterInfo[] parameterInfos;
        CacheStructure cache;
        private bool mappingArray;

        public MapperEmit(Type klassSrc, Type klassDest)
        {         
            this.klassSrc = klassSrc;
            this.klassDest = klassDest;
            mappingArray = false;
            ctorDest = klassDest.GetConstructors();
            cache = new CacheStructure();
            if (klassSrc.IsPrimitive && klassDest.IsPrimitive)
            {
                handler = RuntimeHandlerGenerator.generateRuntimePrimitiveHandler();
            }
            else
            {
                foreach (ConstructorInfo constructorInfo in ctorDest)
                {
                    if (constructorInfo.GetParameters().Length == 0)
                    {
                        handler = RuntimeHandlerGenerator.generateRuntimePropertyHandler(klassSrc, klassDest);
                        break;
                    }
                }
                if (handler == null)
                {
                    handler = RuntimeHandlerGenerator.generateRuntimeParameterHandler(klassSrc, klassDest, ctorDest[0]);
                }
            }
        }

        public MapperEmit Bind (Mapping m)
        {
            if (m.klass != handler.GetKlass())
            {
                if (m.klass == typeof(FieldInfo))
                {
                    handler = RuntimeHandlerGenerator.generateRuntimeFieldHandler(klassSrc, klassDest);
                }
                else handler = RuntimeHandlerGenerator.generateRuntimeAttributeHandler(klassSrc, klassDest, m.klass);
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