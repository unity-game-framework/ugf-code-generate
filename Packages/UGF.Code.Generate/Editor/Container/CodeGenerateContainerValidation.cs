using System;
using System.Collections.Generic;
using System.Reflection;

namespace UGF.Code.Generate.Editor.Container
{
    /// <summary>
    /// Represents default implementation of the container validation to determines what types can be container and what members valid for generation.
    /// </summary>
    public class CodeGenerateContainerValidation : ICodeGenerateContainerValidation
    {
        /// <summary>
        /// Gets or sets value that determines whether to check whether specified type is a container. (Default value is true)
        /// <para>
        /// By default use default container rules for the type.
        /// </para>
        /// </summary>
        public bool IsContainer { get; set; } = true;

        /// <summary>
        /// Gets or sets value that determines whether to check whether specified type is value type or class that has default constructor. (Default value is true)
        /// </summary>
        public bool HasDefaultConstructor { get; set; } = true;

        /// <summary>
        /// Gets or sets value that determines whether to check whether specified type has any field that pass validation. (Default value is true)
        /// </summary>
        public bool HasAnyValidFields { get; set; } = true;

        /// <summary>
        /// Gets or sets value that determines whether to check whether specified type has any property that pass validation. (Default value is true)
        /// </summary>
        public bool HasAnyValidProperties { get; set; } = true;

        public virtual bool Validate(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (IsContainer && !IsTypeContainer(type))
            {
                return false;
            }

            if (HasDefaultConstructor && !IsTypeHasDefaultConstructor(type))
            {
                return false;
            }

            bool hasAnyValidFields = !HasAnyValidFields || IsTypeHasAnyValidFields(type);
            bool hasAnyValidProperties = !HasAnyValidProperties || IsTypeHasAnyValidProperties(type);

            return hasAnyValidFields || hasAnyValidProperties;
        }

        public virtual bool Validate(FieldInfo field)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));

            return CodeGenerateContainerEditorUtility.IsValidField(field);
        }

        public virtual bool Validate(PropertyInfo property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));

            return CodeGenerateContainerEditorUtility.IsValidProperty(property);
        }

        public virtual IEnumerable<FieldInfo> GetFields(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            return CodeGenerateContainerEditorUtility.GetFields(type);
        }

        public virtual IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            return CodeGenerateContainerEditorUtility.GetProperties(type);
        }

        /// <summary>
        /// Determines whether specified type is container.
        /// </summary>
        /// <param name="type">The type to check.</param>
        protected virtual bool IsTypeContainer(Type type)
        {
            return CodeGenerateContainerEditorUtility.IsValidType(type);
        }

        /// <summary>
        /// Determines whether specified type has default constructor.
        /// </summary>
        /// <param name="type">The type to check.</param>
        protected virtual bool IsTypeHasDefaultConstructor(Type type)
        {
            return type.IsValueType || type.GetConstructor(Type.EmptyTypes) != null;
        }

        /// <summary>
        /// Determines whether specified type has any field that pass validation.
        /// </summary>
        /// <param name="type">The type to check.</param>
        protected virtual bool IsTypeHasAnyValidFields(Type type)
        {
            foreach (FieldInfo field in GetFields(type))
            {
                if (Validate(field))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether specified type has any property that pass validation.
        /// </summary>
        /// <param name="type">The type to check.</param>
        protected virtual bool IsTypeHasAnyValidProperties(Type type)
        {
            foreach (PropertyInfo property in GetProperties(type))
            {
                if (Validate(property))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
