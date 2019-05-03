using System;
using UnityEngine;

namespace UGF.Code.Generate.Editor.Container.External
{
    [Serializable]
    public class CodeGenerateContainerExternalMemberInfo
    {
        [SerializeField] private string m_name;
        [SerializeField] private bool m_active;

        public string Name { get { return m_name; } set { m_name = value; } }
        public bool Active { get { return m_active; } set { m_active = value; } }
    }
}
