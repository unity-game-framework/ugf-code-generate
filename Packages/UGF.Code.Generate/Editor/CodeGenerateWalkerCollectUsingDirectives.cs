using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UGF.Code.Generate.Editor
{
    public class CodeGenerateWalkerCollectUsingDirectives : CSharpSyntaxWalker
    {
        public List<UsingDirectiveSyntax> UsingDirectives { get; } = new List<UsingDirectiveSyntax>();

        public CodeGenerateWalkerCollectUsingDirectives(SyntaxWalkerDepth depth = SyntaxWalkerDepth.Node) : base(depth)
        {
        }

        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
            base.VisitUsingDirective(node);

            UsingDirectives.Add(node);
        }
    }
}
