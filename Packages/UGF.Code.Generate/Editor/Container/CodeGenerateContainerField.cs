using System;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;

namespace UGF.Code.Generate.Editor.Container
{
    public class CodeGenerateContainerField
    {
        public string Name { get; }
        public string Type { get; }
        [CanBeNull]
        public SyntaxNode Initializer { get; }
        public bool AsAutoProperty { get; }

        public CodeGenerateContainerField(string name, string type, SyntaxNode initializer = null, bool asAutoProperty = false)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrEmpty(type)) throw new ArgumentNullException(nameof(type));

            Name = name;
            Type = type;
            Initializer = initializer;
            AsAutoProperty = asAutoProperty;
        }

        public SyntaxNode Generate(SyntaxGenerator generator)
        {
            if (generator == null) throw new ArgumentNullException(nameof(generator));

            return AsAutoProperty
                ? generator.AutoPropertyDeclaration(Name, SyntaxFactory.ParseTypeName(Type), Accessibility.Public, DeclarationModifiers.None, Initializer)
                : generator.FieldDeclaration(Name, SyntaxFactory.ParseTypeName(Type), Accessibility.Public, DeclarationModifiers.None, Initializer);
        }
    }
}
