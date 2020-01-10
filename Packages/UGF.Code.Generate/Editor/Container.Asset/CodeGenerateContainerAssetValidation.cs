using System;
using System.Reflection;
using Object = UnityEngine.Object;

namespace UGF.Code.Generate.Editor.Container.Asset
{
    /// <summary>
    /// Represents external type validation to determines what types can be container and what members valid for generation.
    /// </summary>
    public class CodeGenerateContainerAssetValidation : CodeGenerateContainerValidation
    {
        /// <summary>
        /// Gets or sets value that determines whether to check whether specified type is not an attribute. (Default value is true)
        /// </summary>
        public bool IsNotAttribute { get; set; } = true;

        /// <summary>
        /// Gets or sets value that determines whether to check whether specified type is not an Unity Object. (Default value is true)
        /// </summary>
        public bool IsNotUnity { get; set; } = true;

        /// <summary>
        /// Gets or sets value that determines whether to check whether specified type is not deprecated. (Default value is true)
        /// </summary>
        public bool IsNotObsolete { get; set; } = true;

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

            if (type.IsSpecialName)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether specified type is attribute type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        protected virtual bool IsTypeAttribute(Type type)
        {
            return typeof(Attribute).IsAssignableFrom(type);
        }

        /// <summary>
        /// Determines whether specified type is Unity Object.
        /// </summary>
        /// <param name="type">The type to check.</param>
        protected virtual bool IsTypeUnity(Type type)
        {
            return typeof(Object).IsAssignableFrom(type);
        }

        /// <summary>
        /// Determines whether specified type contains obsolete attribute.
        /// </summary>
        /// <param name="type">The type to check.</param>
        protected virtual bool IsTypeObsolete(Type type)
        {
            return type.IsDefined(typeof(ObsoleteAttribute));
        }
    }
}
