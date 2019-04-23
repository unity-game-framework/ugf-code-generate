using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UGF.Code.Generate.Editor
{
    /// <summary>
    /// Represents walker that collects using directives from the specified syntax node.
    /// </summary>
    public class CodeGenerateWalkerCollectUsingDirectives : CSharpSyntaxWalker
    {
        /// <summary>
        /// Gets collection of the collected directives.
        /// </summary>
        public List<UsingDirectiveSyntax> UsingDirectives { get; } = new List<UsingDirectiveSyntax>();

        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
            base.VisitUsingDirective(node);

            UsingDirectives.Add(node);
        }
    }
}
