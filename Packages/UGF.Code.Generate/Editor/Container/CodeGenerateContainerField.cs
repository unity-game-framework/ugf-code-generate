using System;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;

namespace UGF.Code.Generate.Editor.Container
{
    public class CodeGenerateContainerField
    {
        public string Name { get; }
        public SyntaxNode Type { get; }
        [CanBeNull]
        public SyntaxNode Initializer { get; }
        public bool AsAutoProperty { get; }

        public CodeGenerateContainerField(string name, SyntaxNode type, SyntaxNode initializer = null, bool asAutoProperty = false)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            Name = name;
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Initializer = initializer;
            AsAutoProperty = asAutoProperty;
        }

        public SyntaxNode Generate(SyntaxGenerator generator)
        {
            if (generator == null) throw new ArgumentNullException(nameof(generator));

            return AsAutoProperty
                ? generator.AutoPropertyDeclaration(Name, Type, Accessibility.Public, DeclarationModifiers.None, Initializer)
                : generator.FieldDeclaration(Name, Type, Accessibility.Public, DeclarationModifiers.None, Initializer);
        }
    }
}
