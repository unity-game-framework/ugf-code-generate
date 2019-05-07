using System;
using UnityEngine;

namespace UGF.Code.Generate.Editor.Container.External
{
    /// <summary>
    /// Represents member info of the container external type info.
    /// </summary>
    [Serializable]
    public class CodeGenerateContainerExternalMemberInfo
    {
        [SerializeField] private string m_name;
        [SerializeField] private bool m_active;

        /// <summary>
        /// Gets or sets name of the member.
        /// </summary>
        public string Name { get { return m_name; } set { m_name = value; } }

        /// <summary>
        /// Gets or sets value that determines whether to use or not this member in generation.
        /// </summary>
        public bool Active { get { return m_active; } set { m_active = value; } }
    }
}
