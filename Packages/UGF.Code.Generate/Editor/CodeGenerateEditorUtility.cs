using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace UGF.Code.Generate.Editor
{
    public static class CodeGenerateEditorUtility
    {
        public static T AddGeneratedCodeLeadingTrivia<T>(T node) where T : SyntaxNode
        {
            if (node == null) throw new ArgumentNullException(nameof(node));

            node = node.WithLeadingTrivia(SyntaxFactory.Comment("// THIS IS A GENERATED CODE. DO NOT EDIT."),
                SyntaxFactory.CarriageReturnLineFeed,
                SyntaxFactory.Comment("// ReSharper disable all"),
                SyntaxFactory.CarriageReturnLineFeed,
                SyntaxFactory.CarriageReturnLineFeed);

            return node;
        }
    }
}
