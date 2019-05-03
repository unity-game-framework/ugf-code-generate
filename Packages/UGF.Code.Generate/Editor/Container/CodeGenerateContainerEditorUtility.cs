using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;
using UGF.Code.Analysis.Editor;

namespace UGF.Code.Generate.Editor.Container
{
    /// <summary>
    /// Provides utilities to work with code generated containers.
    /// </summary>
    public static partial class CodeGenerateContainerEditorUtility
    {
        public static CodeGenerateContainerValidation DefaultValidation { get; } = new CodeGenerateContainerValidation();

        public static SyntaxNode CreateUnit(Type type, ICodeGenerateContainerValidation validation = null, CSharpCompilation compilation = null, SyntaxGenerator generator = null)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (validation == null) validation = DefaultValidation;
            if (compilation == null) compilation = CodeAnalysisEditorUtility.ProjectCompilation;
            if (generator == null) generator = CodeAnalysisEditorUtility.Generator;

            CodeGenerateContainer container = Create(type, validation, compilation);

            return CreateUnit(container, generator, type.Namespace);
        }

        public static SyntaxNode CreateUnit(CodeGenerateContainer container, SyntaxGenerator generator = null, string namespaceRoot = null)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (generator == null) generator = CodeAnalysisEditorUtility.Generator;

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

        public static CodeGenerateContainer Create(Type type, ICodeGenerateContainerValidation validation = null, CSharpCompilation compilation = null)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (validation == null) validation = DefaultValidation;
            if (compilation == null) compilation = CodeAnalysisEditorUtility.ProjectCompilation;

            var container = new CodeGenerateContainer(type.Name, type.IsValueType);

            IEnumerable<FieldInfo> fields = validation.GetFields(type);
            IEnumerable<PropertyInfo> properties = validation.GetProperties(type);

            foreach (FieldInfo field in fields)
            {
                if (validation.Validate(field))
                {
                    if (TryCreateField(compilation, field.Name, field.FieldType, false, out CodeGenerateContainerField containerField))
                    {
                        container.Fields.Add(containerField);
                    }
                }
            }

            foreach (PropertyInfo property in properties)
            {
                if (validation.Validate(property))
                {
                    if (TryCreateField(compilation, property.Name, property.PropertyType, true, out CodeGenerateContainerField containerField))
                    {
                        container.Fields.Add(containerField);
                    }
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
