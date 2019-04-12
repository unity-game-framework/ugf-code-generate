using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;

namespace UGF.Code.Generate.Editor.Container
{
    public class CodeGenerateContainer
    {
        public string Name { get; }
        public bool AsStruct { get; }
        public List<CodeGenerateContainerField> Fields { get; } = new List<CodeGenerateContainerField>();

        public CodeGenerateContainer(string name, bool asStruct = false)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            Name = name;
            AsStruct = asStruct;
        }

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
