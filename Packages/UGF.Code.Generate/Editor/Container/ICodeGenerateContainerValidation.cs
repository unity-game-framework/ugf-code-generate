using System;
using System.Collections.Generic;
using System.Reflection;

namespace UGF.Code.Generate.Editor.Container
{
    /// <summary>
    /// Represents container validation to determines what types can be container and what members valid for generation.
    /// </summary>
    public interface ICodeGenerateContainerValidation
    {
        /// <summary>
        /// Determines whether specified type valid to generate container.
        /// </summary>
        /// <param name="type">The type to validate.</param>
        bool Validate(Type type);

        /// <summary>
        /// Determines whether specified field is valid to use in generation.
        /// </summary>
        /// <param name="field">The field to validate.</param>
        bool Validate(FieldInfo field);

        /// <summary>
        /// Determines whether specified property is valid to use in generation.
        /// </summary>
        /// <param name="property">The property to validate.</param>
        bool Validate(PropertyInfo property);

        /// <summary>
        /// Gets field for validation.
        /// <para>
        /// This method returns available fields that later should be validated.
        /// </para>
        /// </summary>
        /// <param name="type">The type to get fields.</param>
        IEnumerable<FieldInfo> GetFields(Type type);

        /// <summary>
        /// Gets properties for validation.
        /// <para>
        /// This method returns available properties that later should be validated.
        /// </para>
        /// </summary>
        /// <param name="type">The type to get properties.</param>
        IEnumerable<PropertyInfo> GetProperties(Type type);
    }
}
