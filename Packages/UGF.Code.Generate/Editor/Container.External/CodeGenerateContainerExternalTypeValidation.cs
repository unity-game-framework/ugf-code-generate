using System;
using System.Reflection;
using Object = UnityEngine.Object;

namespace UGF.Code.Generate.Editor.Container.External
{
    public class CodeGenerateContainerExternalTypeValidation
    {
        public bool IsContainer { get; set; } = true;
        public bool HasDefaultConstructor { get; set; } = true;
        public bool IsNotAttribute { get; set; } = true;
        public bool IsNotUnity { get; set; } = true;
        public bool IsNotObsolete { get; set; } = true;
        public bool IsNotSpecial { get; set; } = true;
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

            if (IsNotAttribute && IsTypeAttribute(type))
            {
                return false;
            }

            if (IsNotUnity && IsTypeUnity(type))
            {
                return false;
            }

            if (IsNotObsolete && IsTypeObsolete(type))
            {
                return false;
            }

            if (IsNotSpecial && IsTypeSpecial(type))
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

        protected virtual bool IsTypeContainer(Type type)
        {
            return CodeGenerateContainerEditorUtility.IsValidType(type);
        }

        protected virtual bool IsTypeHasDefaultConstructor(Type type)
        {
            return type.IsValueType || type.GetConstructor(Type.EmptyTypes) != null;
        }

        protected virtual bool IsTypeAttribute(Type type)
        {
            return typeof(Attribute).IsAssignableFrom(type);
        }

        protected virtual bool IsTypeUnity(Type type)
        {
            return typeof(Object).IsAssignableFrom(type);
        }

        protected virtual bool IsTypeObsolete(Type type)
        {
            return type.IsDefined(typeof(ObsoleteAttribute));
        }

        protected virtual bool IsTypeSpecial(Type type)
        {
            return type.IsSpecialName;
        }

        protected virtual bool IsTypeHasAnyValidFields(Type type)
        {
            FieldInfo[] fields = CodeGenerateContainerEditorUtility.GetFields(type);

            for (int i = 0; i < fields.Length; i++)
            {
                if (IsFieldValid(fields[i]))
                {
                    return true;
                }
            }

            return false;
        }

        protected virtual bool IsTypeHasAnyValidProperties(Type type)
        {
            PropertyInfo[] properties = CodeGenerateContainerEditorUtility.GetProperties(type);

            for (int i = 0; i < properties.Length; i++)
            {
                if (IsPropertyValid(properties[i]))
                {
                    return true;
                }
            }

            return false;
        }

        protected virtual bool IsFieldValid(FieldInfo field)
        {
            return CodeGenerateContainerEditorUtility.IsValidField(field);
        }

        protected virtual bool IsPropertyValid(PropertyInfo property)
        {
            return CodeGenerateContainerEditorUtility.IsValidProperty(property);
        }
    }
}
