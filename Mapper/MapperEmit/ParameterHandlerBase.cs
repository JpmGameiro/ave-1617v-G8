using System;
using System.Collections.Generic;
using System.Reflection;
using MapperReflect;

namespace MapperEmit
{
    abstract class ParameterHandlerBase : Handler
    {
        private Type src;
        private Type dest;
        private ConstructorInfo ctor;
        public ParameterInfo[] parameterInfos;
        public List<KeyValuePair<MemberInfo, ParameterInfo>> parameterList;

        public ParameterHandlerBase(Type src, Type dest, ConstructorInfo ctor)
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
                    if (string.Equals(mInfo.Name, paramInfo.Name, StringComparison.CurrentCultureIgnoreCase))
                        parameterList.Add(new KeyValuePair<MemberInfo, ParameterInfo>(mInfo, paramInfo));
                }
            }
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
