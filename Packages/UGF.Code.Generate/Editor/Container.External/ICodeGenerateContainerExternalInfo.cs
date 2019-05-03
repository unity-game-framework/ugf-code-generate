using System;

namespace UGF.Code.Generate.Editor.Container.External
{
    public interface ICodeGenerateContainerExternalInfo
    {
        bool TryGetMember(string name, out CodeGenerateContainerExternalMemberInfo member);
        bool TryGetTargetType(out Type type);
    }
}
