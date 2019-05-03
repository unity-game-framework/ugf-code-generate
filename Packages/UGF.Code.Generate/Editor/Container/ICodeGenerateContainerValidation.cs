using System;
using System.Collections.Generic;
using System.Reflection;

namespace UGF.Code.Generate.Editor.Container
{
    public interface ICodeGenerateContainerValidation
    {
        bool Validate(Type type);
        bool Validate(FieldInfo field);
        bool Validate(PropertyInfo property);
        IEnumerable<FieldInfo> GetFields(Type type);
        IEnumerable<PropertyInfo> GetProperties(Type type);
    }
}
