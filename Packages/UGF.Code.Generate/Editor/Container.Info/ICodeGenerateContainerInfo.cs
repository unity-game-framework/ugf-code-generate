using System;

namespace UGF.Code.Generate.Editor.Container.Info
{
    /// <summary>
    /// Represents container external type information.
    /// </summary>
    public interface ICodeGenerateContainerInfo
    {
        /// <summary>
        /// Tries to get member info by the specified name.
        /// </summary>
        /// <param name="name">The name of the member to find.</param>
        /// <param name="member">The found member.</param>
        bool TryGetMember(string name, out CodeGenerateContainerInfo.MemberInfo member);

        /// <summary>
        /// Tries to get target type.
        /// </summary>
        /// <param name="type">The result type.</param>
        bool TryGetTargetType(out Type type);
    }
}
