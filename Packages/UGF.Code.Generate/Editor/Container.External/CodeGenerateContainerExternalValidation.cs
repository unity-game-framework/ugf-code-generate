using System;
using System.Reflection;
using Object = UnityEngine.Object;

namespace UGF.Code.Generate.Editor.Container.External
{
    public class CodeGenerateContainerExternalValidation : CodeGenerateContainerValidation
    {
        public bool IsNotAttribute { get; set; } = true;
        public bool IsNotUnity { get; set; } = true;
        public bool IsNotObsolete { get; set; } = true;
        public bool IsNotSpecial { get; set; } = true;

        public override bool Validate(Type type)
        {
            if (!base.Validate(type))
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

            return true;
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
    }
}
