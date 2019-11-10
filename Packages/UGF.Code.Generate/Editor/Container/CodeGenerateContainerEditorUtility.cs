using System;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using UGF.Code.Analysis.Editor;

namespace UGF.Code.Generate.Editor.Container
{
    /// <summary>
    /// Provides utilities to work with code generated containers.
    /// </summary>
    public static partial class CodeGenerateContainerEditorUtility
    {
        /// <summary>
        /// Gets default container type validation.
        /// </summary>
        public static CodeGenerateContainerValidation DefaultValidation { get; } = new CodeGenerateContainerValidation();

        /// <summary>
        /// Creates compilation unit from the specified container type using validation.
        /// </summary>
        /// <param name="type">The type of the container.</param>
        /// <param name="validation">The validation to use.</param>
        /// <param name="compilation">The project compilation.</param>
        /// <param name="generator">The syntax generator.</param>
        public static SyntaxNode CreateUnit(Type type, ICodeGenerateContainerValidation validation = null, Compilation compilation = null, SyntaxGenerator generator = null)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (validation == null) validation = DefaultValidation;
            if (compilation == null) compilation = CodeAnalysisEditorUtility.ProjectCompilation;
            if (generator == null) generator = CodeAnalysisEditorUtility.Generator;

            CodeGenerateContainer container = Create(type, validation, compilation);

            return CreateUnit(container, generator, type.Namespace);
        }

        /// <summary>
        /// Creates compilation unit from the specified container.
        /// </summary>
        /// <param name="container">The container used to generate compilation unit.</param>
        /// <param name="generator">The syntax generator.</param>
        /// <param name="namespaceRoot">The namespace of the container.</param>
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

        /// <summary>
        /// Creates container from the specified type using specified validation.
        /// <para>
        /// The specified validation will not be used to validate specified type.
        /// </para>
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <param name="validation">The type validation to use.</param>
        /// <param name="compilation">The project compilation.</param>
        public static CodeGenerateContainer Create(Type type, ICodeGenerateContainerValidation validation = null, Compilation compilation = null)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (validation == null) validation = DefaultValidation;
            if (compilation == null) compilation = CodeAnalysisEditorUtility.ProjectCompilation;

            var container = new CodeGenerateContainer(type.Name, type.IsValueType);

            foreach (FieldInfo field in validation.GetFields(type))
            {
                if (validation.Validate(field))
                {
                    if (TryCreateField(compilation, field.Name, field.FieldType, false, out CodeGenerateContainerField containerField))
                    {
                        container.Fields.Add(containerField);
                    }
                }
            }

            foreach (PropertyInfo property in validation.GetProperties(type))
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
        public static bool TryCreateField(Compilation compilation, string name, Type returnType, bool asAutoProperty, out CodeGenerateContainerField field)
        {
            if (compilation == null) throw new ArgumentNullException(nameof(compilation));
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Value cannot be null or empty.", nameof(name));
            if (returnType == null) throw new ArgumentNullException(nameof(returnType));

            if (compilation.TryConstructTypeSymbol(returnType, out ITypeSymbol typeSymbol))
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

            bool isObject = (type.IsClass || type.IsValueType) && !type.IsEnum && !typeof(Delegate).IsAssignableFrom(type);
            bool isPublic = type.IsPublic;
            bool isGeneric = type.IsGenericTypeDefinition || type.IsGenericParameter;
            bool isOther = type.IsAbstract || type.IsAbstract && type.IsSealed;

            return isObject && isPublic && !isGeneric && !isOther;
        }
    }
}
