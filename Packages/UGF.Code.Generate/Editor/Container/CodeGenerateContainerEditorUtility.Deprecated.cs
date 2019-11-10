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

        /// <summary>
        /// Gets public non static fields from the specified type.
        /// </summary>
        /// <param name="type">The type to get fields from.</param>
        [Obsolete("GetFields has been deprecated.")]
        public static FieldInfo[] GetFields(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            return type.GetFields(BindingFlags.Instance | BindingFlags.Public);
        }

        /// <summary>
        /// Gets public non static properties from the specified type.
        /// </summary>
        /// <param name="type">The type to get properties from.</param>
        [Obsolete("GetProperties has been deprecated.")]
        public static PropertyInfo[] GetProperties(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
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
        [Obsolete("IsValidField has been deprecated.")]
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
        [Obsolete("IsValidProperty has been deprecated.")]
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
