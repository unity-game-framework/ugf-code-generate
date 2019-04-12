using System;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace UGF.Code.Generate.Editor.Container
{
    public static class CodeGenerateContainerEditorUtility
    {
        public static CodeGenerateContainer Create(CSharpCompilation compilation, Type type)
        {
            if (compilation == null) throw new ArgumentNullException(nameof(compilation));
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (!IsValidType(type)) throw new ArgumentException("The specified type not valid.", nameof(type));

            var container = new CodeGenerateContainer(type.Name, type.IsValueType);

            FieldInfo[] fields = type.GetFields();
            PropertyInfo[] properties = type.GetProperties();

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];

                if (IsValidField(field) && compilation.TryGetTypeByMetadataName(field.FieldType, out INamedTypeSymbol typeSymbol))
                {
                    string typeName = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                    container.Fields.Add(new CodeGenerateContainerField(field.Name, typeName));
                }
            }

            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];

                if (IsValidProperty(property) && compilation.TryGetTypeByMetadataName(property.PropertyType, out INamedTypeSymbol typeSymbol))
                {
                    string typeName = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                    container.Fields.Add(new CodeGenerateContainerField(property.Name, typeName, null, true));
                }
            }

            return container;
        }

        public static bool IsValidType(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            bool isObject = type.IsClass || type.IsValueType;
            bool isPublic = type.IsPublic || type.IsNestedPublic;
            bool isGeneric = type.IsGenericType || type.IsGenericTypeDefinition || type.IsGenericParameter || type.IsConstructedGenericType;
            bool isOther = type.IsAbstract || type.IsAbstract && type.IsSealed;

            return isObject && isPublic && !isGeneric && !isOther;
        }

        public static bool IsValidField(FieldInfo field)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));

            return field.IsPublic && !field.IsStatic && !field.IsLiteral && !field.IsInitOnly;
        }

        public static bool IsValidProperty(PropertyInfo property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));

            MethodInfo get = property.GetMethod;
            MethodInfo set = property.SetMethod;

            bool isGetValid = get != null && get.IsPublic && !get.IsStatic && !get.IsAbstract;
            bool isSetValid = set != null && set.IsPublic && !set.IsStatic && !set.IsAbstract;

            return property.GetIndexParameters().Length == 0 && isGetValid && isSetValid;
        }
    }
}
