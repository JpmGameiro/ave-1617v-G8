﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using MapperReflect;

namespace MapperEmit
{
    class AttributeHandlerEmit : HandlerEmit
    {
        private Type src;
        private Type dest;
        private Type attrType;
        private IEmitter emitter;
        public List<KeyValuePair<MemberInfo, MemberInfo>> attributeList;
        private MemberInfo[] memberInfos;
        private MemberInfo memberSrc;

        public AttributeHandlerEmit(Type src, Type dest, Type attrType)
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

        public override object Copy(object objSrc)
        {
            if (emitter != null)
            {
                return emitter.Copy(objSrc);
            }
            TypeBuilder tBuilder = GetTypeBuilder("Attribute");
            MethodBuilder mBuilder = GetMethodBuilder(tBuilder);

            ILGenerator ilGenerator = mBuilder.GetILGenerator();

            /*********************************** IL CODE ***************************/

            ilGenerator.Emit(OpCodes.Newobj, dest.GetConstructors()[0]);
            foreach (KeyValuePair<MemberInfo, MemberInfo> pair in attributeList)
            {
                if (pair.Key is PropertyInfo)
                {
                    ilGenerator.Emit(OpCodes.Dup);
                    ilGenerator.Emit(OpCodes.Ldarg_1);
                    ilGenerator.Emit(OpCodes.Callvirt, ((PropertyInfo) pair.Key).GetGetMethod());
                    ilGenerator.Emit(OpCodes.Callvirt, ((PropertyInfo) pair.Value).GetSetMethod());
                }
                else if (pair.Key is FieldInfo)
                {
                    ilGenerator.Emit(OpCodes.Dup);
                    ilGenerator.Emit(OpCodes.Ldarg_1);
                    ilGenerator.Emit(OpCodes.Ldfld, ((FieldInfo) pair.Key));
                    ilGenerator.Emit(OpCodes.Stfld, ((FieldInfo) pair.Value));
                }
                ilGenerator.Emit(OpCodes.Ret);
            }
            Type t = tBuilder.CreateType();
            emitter = (IEmitter)Activator.CreateInstance(t);
            asm.Save(tBuilder.Name + ".dll");
            return emitter.Copy(objSrc);
        }

        /*********************************** END ***************************/
    }
}
