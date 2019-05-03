using System;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;

namespace UGF.Code.Generate.Editor.Container
{
    /// <summary>
    /// Provides utilities to work with code generated containers.
    /// </summary>
    public static class CodeGenerateContainerEditorUtility
    {
        /// <summary>
        /// Creates container from the specified type as compilation unit.
        /// <para>
        /// This method will generate container inside type namespace, if presents.
        /// </para>
        /// </summary>
        /// <param name="compilation">The project compilation.</param>
        /// <param name="generator">The syntax generator.</param>
        /// <param name="type">The target type of the container.</param>
        public static SyntaxNode CreateUnit(CSharpCompilation compilation, SyntaxGenerator generator, Type type)
        {
            if (compilation == null) throw new ArgumentNullException(nameof(compilation));
            if (generator == null) throw new ArgumentNullException(nameof(generator));
            if (type == null) throw new ArgumentNullException(nameof(type));

            CodeGenerateContainer container = Create(compilation, type);

            return CreateUnit(generator, container, type.Namespace);
        }

        /// <summary>
        /// Creates compilation unit from the specified container.
        /// </summary>
        /// <param name="generator">The syntax generator.</param>
        /// <param name="container">The container used to generate compilation unit.</param>
        /// <param name="namespaceRoot">The namespace of the container.</param>
        public static SyntaxNode CreateUnit(SyntaxGenerator generator, CodeGenerateContainer container, string namespaceRoot = null)
        {
            if (generator == null) throw new ArgumentNullException(nameof(generator));
            if (container == null) throw new ArgumentNullException(nameof(container));

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

        /// <summary>
        /// Creates container from the specified valid type.
        /// <para>
        /// This method will generates container from the valid type with the valid fields and properties.
        /// </para>
        /// </summary>
        /// <param name="compilation">The project compilation.</param>
        /// <param name="type">The target type to generate container from.</param>
        /// <exception cref="ArgumentException">The specified type must be valid to generate container.</exception>
        public static CodeGenerateContainer Create(CSharpCompilation compilation, Type type)
        {
            if (compilation == null) throw new ArgumentNullException(nameof(compilation));
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (!IsValidType(type)) throw new ArgumentException("The specified type not valid.", nameof(type));

            var container = new CodeGenerateContainer(type.Name, type.IsValueType);

            FieldInfo[] fields = GetFields(type);
            PropertyInfo[] properties = GetProperties(type);

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

        /// <summary>
        /// Tries to create field from with the specified name and return type.
        /// <para>
        /// This method will create field if the type symbol for the specified return type can be got.
        /// </para>
        /// </summary>
        /// <param name="compilation">The project compilation.</param>
        /// <param name="name">The name of the field.</param>
        /// <param name="returnType">The return type of the field.</param>
        /// <param name="asAutoProperty">The value determines whether field will be create as auto property.</param>
        /// <param name="field">The created field.</param>
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

        /// <summary>
        /// Determines whether the specified type valid to generate container.
        /// <para>
        /// The type can be valid for generating container only if:
        /// <para>- Type is public non nested class or struct.</para>
        /// <para>- Type is not generic type.</para>
        /// <para>- Type is not abstract or static.</para>
        /// </para>
        /// </summary>
        /// <param name="type">The type to validate.</param>
        public static bool IsValidType(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            bool isObject = (type.IsClass || type.IsValueType) && !type.IsEnum;
            bool isPublic = type.IsPublic;
            bool isGeneric = type.IsGenericTypeDefinition || type.IsGenericParameter;
            bool isOther = type.IsAbstract || type.IsAbstract && type.IsSealed;

            return isObject && isPublic && !isGeneric && !isOther;
        }

        /// <summary>
        /// Determines whether the specified field is valid to generate field container.
        /// <para>
        /// The field can be valid for generating field container only if:
        /// <para>- Field is public.</para>
        /// <para>- Field is not static.</para>
        /// <para>- Field is not readonly or const.</para>
        /// </para>
        /// </summary>
        /// <param name="field">The field to validate.</param>
        public static bool IsValidField(FieldInfo field)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));

            return field.IsPublic && !field.IsStatic && !field.IsLiteral && !field.IsInitOnly;
        }

        /// <summary>
        /// Determines whether the specified property is valid to generate field container.
        /// <para>
        /// The property can be valid for generating field container only if:
        /// <para>- Property is public.</para>
        /// <para>- Property is not static.</para>
        /// <para>- Property is not abstract.</para>
        /// <para>- Property is can read and write.</para>
        /// </para>
        /// </summary>
        /// <param name="property">The property to validate.</param>
        public static bool IsValidProperty(PropertyInfo property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));

            MethodInfo get = property.GetMethod;
            MethodInfo set = property.SetMethod;

            bool isGetValid = get != null && get.IsPublic && !get.IsStatic && !get.IsAbstract;
            bool isSetValid = set != null && set.IsPublic && !set.IsStatic && !set.IsAbstract;

            return property.GetIndexParameters().Length == 0 && isGetValid && isSetValid;
        }

        /// <summary>
        /// Gets public non static fields from the specified type.
        /// </summary>
        /// <param name="type">The type to get fields from.</param>
        public static FieldInfo[] GetFields(Type type)
        {
            return type.GetFields(BindingFlags.Instance | BindingFlags.Public);
        }

        /// <summary>
        /// Gets public non static properties from the specified type.
        /// </summary>
        /// <param name="type">The type to get properties from.</param>
        public static PropertyInfo[] GetProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }
    }
}
