using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;

namespace UGF.Code.Generate.Editor.Container
{
    /// <summary>
    /// Represents code generated data container.
    /// </summary>
    public class CodeGenerateContainer
    {
        /// <summary>
        /// Gets name of the container.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets value that determines whether specified container will be generated as struct.
        /// </summary>
        public bool AsStruct { get; }

        /// <summary>
        /// Gets collection of the fields.
        /// </summary>
        public List<CodeGenerateContainerField> Fields { get; } = new List<CodeGenerateContainerField>();

        /// <summary>
        /// Creates container with the specified name.
        /// </summary>
        /// <param name="name">The name of the container.</param>
        /// <param name="asStruct">The value determines whether this container will be generated as struct.</param>
        public CodeGenerateContainer(string name, bool asStruct = false)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Value cannot be null or empty.", nameof(name));

            Name = name;
            AsStruct = asStruct;
        }

        /// <summary>
        /// Generates container syntax node using the specified syntax generator.
        /// </summary>
        /// <param name="generator">The syntax generator used to create container.</param>
        public SyntaxNode Generate(SyntaxGenerator generator)
        {
            if (generator == null) throw new ArgumentNullException(nameof(generator));

            SyntaxNode declaration = AsStruct
                ? generator.StructDeclaration(Name, null, Accessibility.Public)
                : generator.ClassDeclaration(Name, null, Accessibility.Public);

            for (int i = 0; i < Fields.Count; i++)
            {
                CodeGenerateContainerField field = Fields[i];

                declaration = generator.AddMembers(declaration, field.Generate(generator));
            }

            return declaration;
        }
    }
}
