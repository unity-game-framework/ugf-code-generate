using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;

namespace UGF.Code.Generate.Editor
{
    public class CodeGenerateRewriterAddAttributeToKind : CSharpSyntaxRewriter
    {
        public SyntaxGenerator Generator { get; }
        public SyntaxNode Attribute { get; }
        public SyntaxKind Kind { get; }

        public CodeGenerateRewriterAddAttributeToKind(SyntaxGenerator generator, SyntaxNode attribute, SyntaxKind kind, bool visitIntoStructuredTrivia = false) : base(visitIntoStructuredTrivia)
        {
            Generator = generator ?? throw new ArgumentNullException(nameof(generator));
            Attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
            Kind = kind;
        }

        public override SyntaxNode DefaultVisit(SyntaxNode node)
        {
            return node.Kind() == Kind ? Generator.AddAttributes(node, Attribute) : base.DefaultVisit(node);
        }
    }
}
