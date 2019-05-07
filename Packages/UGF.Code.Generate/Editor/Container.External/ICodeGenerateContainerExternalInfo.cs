using System;

namespace UGF.Code.Generate.Editor.Container.External
{
    /// <summary>
    /// Represents container external type information.
    /// </summary>
    public interface ICodeGenerateContainerExternalInfo
    {
        /// <summary>
        /// Tries to get member info by the specified name.
        /// </summary>
        /// <param name="name">The name of the member to find.</param>
        /// <param name="member">The found member.</param>
        bool TryGetMember(string name, out CodeGenerateContainerExternalMemberInfo member);

        /// <summary>
        /// Tries to get target type.
        /// </summary>
        /// <param name="type">The result type.</param>
        bool TryGetTargetType(out Type type);
    }
}
