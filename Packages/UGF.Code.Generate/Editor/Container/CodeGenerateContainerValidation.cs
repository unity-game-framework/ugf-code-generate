using System;
using System.Collections.Generic;
using System.Reflection;

namespace UGF.Code.Generate.Editor.Container
{
    public class CodeGenerateContainerValidation : ICodeGenerateContainerValidation
    {
        public bool IsContainer { get; set; } = true;
        public bool HasDefaultConstructor { get; set; } = true;
        public bool HasAnyValidFields { get; set; } = true;
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

            if (HasAnyValidFields && !IsTypeHasAnyValidFields(type))
            {
                return false;
            }

            if (HasAnyValidProperties && !IsTypeHasAnyValidProperties(type))
            {
                return false;
            }

            return true;
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

        protected virtual bool IsTypeContainer(Type type)
        {
            return CodeGenerateContainerEditorUtility.IsValidType(type);
        }

        protected virtual bool IsTypeHasDefaultConstructor(Type type)
        {
            return type.IsValueType || type.GetConstructor(Type.EmptyTypes) != null;
        }

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
