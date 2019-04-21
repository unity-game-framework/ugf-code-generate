using System;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;

namespace UGF.Code.Generate.Editor.Container
{
    public static class CodeGenerateContainerEditorUtility
    {
        public static SyntaxNode CreateUnit(CSharpCompilation compilation, SyntaxGenerator generator, Type type)
        {
            CodeGenerateContainer container = Create(compilation, type);

            return CreateUnit(generator, container, type.Namespace);
        }

        public static SyntaxNode CreateUnit(SyntaxGenerator generator, CodeGenerateContainer container, string namespaceRoot)
        {
            SyntaxNode unit = generator.CompilationUnit();

            if (!string.IsNullOrEmpty(namespaceRoot))
            {
                SyntaxNode namespaceDeclaration = generator.NamespaceDeclaration(namespaceRoot);

                namespaceDeclaration = generator.AddMembers(namespaceDeclaration, container.Generate(generator));
                unit = generator.AddMembers(unit, namespaceDeclaration);
            }
            else
            {
                unit = generator.AddMembers(unit, container.Generate(generator));
            }

            return unit;
        }

        public static CodeGenerateContainer Create(CSharpCompilation compilation, Type type)
        {
            if (compilation == null) throw new ArgumentNullException(nameof(compilation));
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (!IsValidType(type)) throw new ArgumentException("The specified type not valid.", nameof(type));

            var container = new CodeGenerateContainer(type.Name, type.IsValueType);

            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];

                if (IsValidField(field) && TryCreateField(compilation, field.Name, field.FieldType, false, out CodeGenerateContainerField containerField))
                {
                    container.Fields.Add(containerField);
                }
            }

            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];

                if (IsValidProperty(property) && TryCreateField(compilation, property.Name, property.PropertyType, true, out CodeGenerateContainerField containerField))
                {
                    container.Fields.Add(containerField);
                }
            }

            return container;
        }

        public static bool TryCreateField(CSharpCompilation compilation, string name, Type returnType, bool asAutoProperty, out CodeGenerateContainerField field)
        {
            if (compilation == null) throw new ArgumentNullException(nameof(compilation));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (returnType == null) throw new ArgumentNullException(nameof(returnType));

            if (compilation.TryGetTypeByMetadataName(returnType, out INamedTypeSymbol typeSymbol))
            {
                string typeName = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                field = new CodeGenerateContainerField(name, typeName, null, asAutoProperty);
                return true;
            }

            field = null;
            return false;
        }

        public static bool IsValidType(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            bool isObject = (type.IsClass || type.IsValueType) && !type.IsEnum;
            bool isPublic = type.IsPublic;
            bool isGeneric = type.IsGenericTypeDefinition || type.IsGenericParameter;
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
