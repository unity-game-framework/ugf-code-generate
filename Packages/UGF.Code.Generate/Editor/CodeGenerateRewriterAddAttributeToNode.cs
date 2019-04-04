using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;

namespace UGF.Code.Generate.Editor
{
    public class CodeGenerateRewriterAddAttributeToNode : CSharpSyntaxRewriter
    {
        public SyntaxGenerator Generator { get; }
        public SyntaxNode Attribute { get; }
        public CodeGenerateRewriterAddAttributeToNodeValidate Validate { get; }

        public CodeGenerateRewriterAddAttributeToNode(SyntaxGenerator generator, SyntaxNode attribute, CodeGenerateRewriterAddAttributeToNodeValidate validate, bool visitIntoStructuredTrivia = false) : base(visitIntoStructuredTrivia)
        {
            Generator = generator ?? throw new ArgumentNullException(nameof(generator));
            Attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
            Validate = validate ?? throw new ArgumentNullException(nameof(validate));
        }

        public override SyntaxNode DefaultVisit(SyntaxNode node)
        {
            return Validate(node) ? Generator.AddAttributes(Attribute) : base.DefaultVisit(node);
        }
    }
}
