using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace UGF.Code.Generate.Editor.Experimental
{
    public class CodeGenerateRewriterAddAttributeFromGenericArgument : CSharpSyntaxRewriter
    {
        public SyntaxGenerator Generator { get; }
        public TypeSyntax AttributeType { get; }
        public string GenericTypeIdentifier { get; }

        public CodeGenerateRewriterAddAttributeFromGenericArgument(SyntaxGenerator generator, TypeSyntax attributeType, string genericTypeIdentifier)
        {
            Generator = generator ?? throw new ArgumentNullException(nameof(generator));
            AttributeType = attributeType ?? throw new ArgumentNullException(nameof(attributeType));
            GenericTypeIdentifier = genericTypeIdentifier ?? throw new ArgumentNullException(nameof(genericTypeIdentifier));
        }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            if (node.BaseList != null)
            {
                for (int i = 0; i < node.BaseList.Types.Count; i++)
                {
                    TypeSyntax type = node.BaseList.Types[i].Type;

                    if (type.TryGetGenericNameSyntax(out GenericNameSyntax genericNameSyntax))
                    {
                        if (genericNameSyntax.Identifier.Text == GenericTypeIdentifier && genericNameSyntax.TypeArgumentList.Arguments.Count == 1)
                        {
                            TypeSyntax targetType = genericNameSyntax.TypeArgumentList.Arguments[0];
                            SyntaxNode attribute = Generator.Attribute(AttributeType, new[]
                            {
                                Generator.TypeOfExpression(targetType)
                            });

                            return Generator.AddAttributes(node, attribute);
                        }
                    }
                }
            }

            return base.VisitClassDeclaration(node);
        }
    }
}
