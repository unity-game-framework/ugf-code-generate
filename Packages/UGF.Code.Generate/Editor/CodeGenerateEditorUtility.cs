using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UGF.Code.Generate.Editor
{
    public static class CodeGenerateEditorUtility
    {
        public static T AddGeneratedCodeLeadingTrivia<T>(T node) where T : SyntaxNode
        {
            if (node == null) throw new ArgumentNullException(nameof(node));

            return node.WithLeadingTrivia(SyntaxFactory.Comment("// THIS IS A GENERATED CODE. DO NOT EDIT."),
                SyntaxFactory.CarriageReturnLineFeed,
                SyntaxFactory.Comment("// ReSharper disable all"),
                SyntaxFactory.CarriageReturnLineFeed,
                SyntaxFactory.CarriageReturnLineFeed);
        }

        public static HashSet<string> UsingDirectivesCollectUniqueNames(IEnumerable<UsingDirectiveSyntax> directives)
        {
            if (directives == null) throw new ArgumentNullException(nameof(directives));

            var hashset = new HashSet<string>();

            foreach (UsingDirectiveSyntax directive in directives)
            {
                hashset.Add(directive.Name.GetText().ToString());
            }

            return hashset;
        }
    }
}
