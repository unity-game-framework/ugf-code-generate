using System;
using System.Collections.Generic;
using UnityEngine;

namespace UGF.Code.Generate.Editor.Container.Info
{
    /// <summary>
    /// Represents container external type information.
    /// </summary>
    [Serializable]
    public class CodeGenerateContainerInfo : ICodeGenerateContainerInfo
    {
        [SerializeField] private string m_typeName;
        [SerializeField] private List<MemberInfo> m_members = new List<MemberInfo>();

        /// <summary>
        /// Gets or sets the name of the target type.
        /// </summary>
        public string TypeName { get { return m_typeName; } set { m_typeName = value; } }

        /// <summary>
        /// Gets collection of the members.
        /// </summary>
        public List<MemberInfo> Members { get { return m_members; } }

        /// <summary>
        /// Represents member info of the container external type info.
        /// </summary>
        [Serializable]
        public class MemberInfo
        {
            [SerializeField] private bool m_active = true;
            [SerializeField] private string m_name;

            /// <summary>
            /// Gets or sets value that determines whether to use or not this member in generation.
            /// </summary>
            public bool Active { get { return m_active; } set { m_active = value; } }

            /// <summary>
            /// Gets or sets name of the member.
            /// </summary>
            public string Name { get { return m_name; } set { m_name = value; } }
        }

        public bool TryGetMember(string name, out MemberInfo member)
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
            if (!string.IsNullOrEmpty(m_typeName))
            {
                type = Type.GetType(m_typeName);

                return type != null;
            }

            type = null;
            return false;
        }
    }
}
