using System;
using System.Collections.Generic;
using UnityEngine;

namespace UGF.Code.Generate.Editor.Container.External
{
    /// <summary>
    /// Represents abstract implementation of the container external type information.
    /// </summary>
    public abstract class CodeGenerateContainerExternalInfoBase<TMemberInfo> : ICodeGenerateContainerExternalInfo where TMemberInfo : CodeGenerateContainerExternalMemberInfo
    {
        [SerializeField] private string m_typeName;
        [SerializeField] private List<TMemberInfo> m_members = new List<TMemberInfo>();

        /// <summary>
        /// Gets or sets the name of the target type.
        /// </summary>
        public string TypeName { get { return m_typeName; } set { m_typeName = value; } }

        /// <summary>
        /// Gets collection of the members.
        /// </summary>
        public List<TMemberInfo> Members { get { return m_members; } }

        public bool TryGetMember<T>(string name, out T member) where T : CodeGenerateContainerExternalMemberInfo
        {
            if (TryGetMember(name, out TMemberInfo memberInfo) && memberInfo is T cast)
            {
                member = cast;
                return true;
            }

            member = default;
            return false;
        }

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
