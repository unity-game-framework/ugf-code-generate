using System;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;

namespace UGF.Code.Generate.Editor.Container
{
    public static partial class CodeGenerateContainerEditorUtility
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
        [Obsolete("CreateUnit has been deprecated. Use overload with validation instead.")]
        public static SyntaxNode CreateUnit(Compilation compilation, SyntaxGenerator generator, Type type)
        {
            if (compilation == null) throw new ArgumentNullException(nameof(compilation));
            if (generator == null) throw new ArgumentNullException(nameof(generator));
            if (type == null) throw new ArgumentNullException(nameof(type));

            CodeGenerateContainer container = Create(compilation, type);

            return CreateUnit(container, generator, type.Namespace);
        }

        /// <summary>
        /// Creates compilation unit from the specified container.
        /// </summary>
        /// <param name="generator">The syntax generator.</param>
        /// <param name="container">The container used to generate compilation unit.</param>
        /// <param name="namespaceRoot">The namespace of the container.</param>
        [Obsolete("CreateUnit has been deprecated. Use overload with validation instead.")]
        public static SyntaxNode CreateUnit(SyntaxGenerator generator, CodeGenerateContainer container, string namespaceRoot = null)
        {
            if (generator == null) throw new ArgumentNullException(nameof(generator));
            if (container == null) throw new ArgumentNullException(nameof(container));

            return CreateUnit(container, generator, namespaceRoot);
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
        [Obsolete("Create has been deprecated. Use overload with validation instead.")]
        public static CodeGenerateContainer Create(Compilation compilation, Type type)
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
    }
}
