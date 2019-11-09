using System;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;

namespace UGF.Code.Generate.Editor.Container
{
    /// <summary>
    /// Presents container field.
    /// </summary>
    public class CodeGenerateContainerField
    {
        /// <summary>
        /// Gets name of the field.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets type of the field.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Gets initializer of the field.
        /// </summary>
        [CanBeNull]
        public SyntaxNode Initializer { get; }

        /// <summary>
        /// Gets value determines whether this filed will be generated as auto property.
        /// </summary>
        public bool AsAutoProperty { get; }

        /// <summary>
        /// Create container field with the specified name and type.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <param name="type">The type of the field.</param>
        /// <param name="initializer">The initializer of the field.</param>
        /// <param name="asAutoProperty">The value determines whether this field will be generated as auto property.</param>
        public CodeGenerateContainerField(string name, string type, SyntaxNode initializer = null, bool asAutoProperty = false)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Value cannot be null or empty.", nameof(name));
            if (string.IsNullOrEmpty(type)) throw new ArgumentException("Value cannot be null or empty.", nameof(type));

            Name = name;
            Type = type;
            Initializer = initializer;
            AsAutoProperty = asAutoProperty;
        }

        /// <summary>
        /// Generates container field syntax node using the specified syntax generator.
        /// </summary>
        /// <param name="generator">The syntax generator to use.</param>
        public SyntaxNode Generate(SyntaxGenerator generator)
        {
            if (generator == null) throw new ArgumentNullException(nameof(generator));

            return AsAutoProperty
                ? generator.AutoPropertyDeclaration(Name, SyntaxFactory.ParseTypeName(Type), Accessibility.Public, DeclarationModifiers.None, Initializer)
                : generator.FieldDeclaration(Name, SyntaxFactory.ParseTypeName(Type), Accessibility.Public, DeclarationModifiers.None, Initializer);
        }
    }
}
