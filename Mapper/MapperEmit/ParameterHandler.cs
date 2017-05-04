using System;
using System.Collections.Generic;
using System.Reflection;
using MapperReflect;

namespace MapperEmit
{
    class ParameterHandler : Handler
    {
        private Type src;
        private Type dest;
        private ConstructorInfo ctor;
        private ParameterInfo[] parameterInfos;
        public List<KeyValuePair<MemberInfo , ParameterInfo>> parameterList;    

        public ParameterHandler(Type src, Type dest, ConstructorInfo ctor)
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
                    if(string.Equals(mInfo.Name, paramInfo.Name, StringComparison.CurrentCultureIgnoreCase))
                        parameterList.Add(new KeyValuePair<MemberInfo, ParameterInfo>(mInfo, paramInfo)); 
                }
            }
        }
        public override object Copy(object objSrc)
        {
            object[] objs = new object[parameterInfos.Length];
            int idx = 0;
            foreach (KeyValuePair<MemberInfo, ParameterInfo> pair in parameterList)
            {
                objs [idx++] = extractValueFromMember(objSrc,pair.Key);                
            }
            return ctor.Invoke(objs);
        }

        private object extractValueFromMember(object objSrc, MemberInfo pairKey)
        {
            return pairKey is PropertyInfo
                ? ((PropertyInfo) pairKey).GetValue(objSrc)
                : ((FieldInfo) pairKey).GetValue(objSrc);
        }

        public override Type GetKlass()
        {
            return typeof(ParameterInfo);
        } 

        public override void LinkMembers(string nameSrc, string nameDest)
        {
            throw new NotImplementedException();
        }   
    }
}
