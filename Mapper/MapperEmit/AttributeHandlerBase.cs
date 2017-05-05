using System;
using System.Collections.Generic;
using System.Reflection;
using MapperReflect;

namespace MapperEmit
{
    abstract class AttributeHandlerBase : Handler
    {
        private Type src;
        private Type dest;
        private Type attrType;
        public List<KeyValuePair<MemberInfo, MemberInfo>> attributeList;
        private MemberInfo[] memberInfos;
        private MemberInfo memberSrc;

        public AttributeHandlerBase(Type src, Type dest, Type attrType)
        {
            this.src = src;
            this.dest = dest;
            this.attrType = attrType;
            attributeList = new List<KeyValuePair<MemberInfo, MemberInfo>>();
            memberInfos = dest.GetMembers();

            foreach (MemberInfo memberDest in memberInfos)
            {
                Attribute attr = memberDest.GetCustomAttribute(attrType);
                if ((attr != null) && (memberSrc = src.GetMember(memberDest.Name)[0]) != null)
                {
                    attributeList.Add(new KeyValuePair<MemberInfo, MemberInfo>(memberSrc, memberDest));
                }
            }
        }

        public override void LinkMembers(string nameSrc, string nameDest)
        {
            throw new NotImplementedException();
        }

        public override Type GetKlass()
        {
            return typeof(AttributeHandler);
        }
    }
}
