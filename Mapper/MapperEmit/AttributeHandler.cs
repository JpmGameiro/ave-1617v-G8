using System;
using System.Collections.Generic;
using System.Reflection;
using MapperReflect;

namespace MapperEmit
{
    public class AttributeHandler : Handler
    {
        private Type src;
        private Type dest;
        private Type attrType;
        public List<KeyValuePair<MemberInfo , MemberInfo>> attributeList;
        private MemberInfo[] memberInfos;
        private MemberInfo memberSrc;

        public AttributeHandler(Type src, Type dest, Type attrType)
        {
            this.src = src;
            this.dest = dest;
            this.attrType = attrType;
            attributeList = new List<KeyValuePair<MemberInfo, MemberInfo>>();
            memberInfos = dest.GetMembers();

            foreach (MemberInfo memberDest in memberInfos)
            {
                Attribute attr = memberDest.GetCustomAttribute(attrType);          
                if ((attr != null) && (memberSrc = src.GetMember(memberDest.Name)[0]) !=null)
                {
                    attributeList.Add(new KeyValuePair<MemberInfo, MemberInfo>(memberSrc,memberDest));
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

        public override object Copy(object objSrc)
        {
            object objDest = Activator.CreateInstance(dest);
            if (attributeList != null)
            {
                foreach (KeyValuePair<MemberInfo, MemberInfo> pair in attributeList)
                {
                    if (pair.Key is PropertyInfo)
                    {
                        ((PropertyInfo) pair.Value).SetValue(objDest, ((PropertyInfo) pair.Key).GetValue(objSrc));
                    }
                    else if (pair.Key is FieldInfo)
                        ((FieldInfo) pair.Value).SetValue(objDest, ((FieldInfo) pair.Key).GetValue(objSrc));
                }
            }
            return objDest;
        }
    }
}
