using System;
using System.Collections.Generic;
using UnityEngine;

namespace UGF.Code.Generate.Editor.Container.External
{
    public abstract class CodeGenerateContainerExternalInfoBase<TMemberInfo> : ICodeGenerateContainerExternalInfo where TMemberInfo : CodeGenerateContainerExternalMemberInfo
    {
        [SerializeField] private string m_typeName;
        [SerializeField] private List<TMemberInfo> m_members = new List<TMemberInfo>();

        public string TypeName { get { return m_typeName; } set { m_typeName = value; } }
        public List<TMemberInfo> Members { get { return m_members; } }

        public bool TryGetMember(string name, out TMemberInfo member)
        {
            for (int i = 0; i < m_members.Count; i++)
            {
                member = m_members[i];

                if (member.Name == name)
                {
                    return true;
                }
            }

            member = null;
            return false;
        }

        public bool TryGetTargetType(out Type type)
        {
            type = Type.GetType(m_typeName);

            return type != null;
        }

        bool ICodeGenerateContainerExternalInfo.TryGetMember(string name, out CodeGenerateContainerExternalMemberInfo member)
        {
            if (TryGetMember(name, out TMemberInfo memberInfo))
            {
                member = memberInfo;
                return true;
            }

            member = null;
            return false;
        }
    }
}
