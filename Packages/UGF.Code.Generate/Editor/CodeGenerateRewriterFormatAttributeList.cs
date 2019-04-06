using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UGF.Code.Generate.Editor
{
    public class CodeGenerateRewriterFormatAttributeList : CSharpSyntaxRewriter
    {
        public CodeGenerateRewriterFormatAttributeList(bool visitIntoStructuredTrivia = false) : base(visitIntoStructuredTrivia)
        {
        }

        public override SyntaxNode VisitAttributeList(AttributeListSyntax node)
        {
            SyntaxTriviaList trivia = SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed);

            foreach (SyntaxTrivia syntaxTrivia in node.GetLeadingTrivia().Reverse())
            {
                if (syntaxTrivia.IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    trivia = trivia.Add(syntaxTrivia);
                }
                else
                {
                    break;
                }
            }

            node = node.WithTrailingTrivia(trivia);

            return base.VisitAttributeList(node);
        }
    }
}
