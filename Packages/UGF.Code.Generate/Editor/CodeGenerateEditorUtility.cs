using System;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace UGF.Code.Generate.Editor
{
    /// <summary>
    /// Provides utilities to work with code generation in editor.
    /// </summary>
    public static class CodeGenerateEditorUtility
    {
        /// <summary>
        /// Adds leading trivia indicates that specified code was generated.
        /// <para>
        /// Also adds "ReSharper" ignorance.
        /// </para>
        /// </summary>
        /// <param name="node">The syntax node to modify.</param>
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

        /// <summary>
        /// Checks source script at the specified path for attribute.
        /// <para>
        /// This method will determines whether any declaration in the specified source script contains an attribute with the specified type.
        /// </para>
        /// </summary>
        /// <param name="compilation">The project compilation.</param>
        /// <param name="path">The path of the source script.</param>
        /// <param name="attributeType">The type of the attribute to check.</param>
        public static bool CheckAttributeFromScript(CSharpCompilation compilation, string path, Type attributeType)
        {
            if (compilation == null) throw new ArgumentNullException(nameof(compilation));
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (attributeType == null) throw new ArgumentNullException(nameof(attributeType));

            INamedTypeSymbol typeSymbol = compilation.GetTypeByMetadataName(attributeType.FullName);

            if (typeSymbol == null) throw new ArgumentException("Can not resolve type symbol from the specified type.", nameof(attributeType));

            return CheckAttributeFromScript(compilation, path, typeSymbol);
        }

        /// <summary>
        /// Checks source script at the specified path for attribute.
        /// <para>
        /// This method will determines whether any declaration in the specified source script contains an attribute with the specified type.
        /// </para>
        /// </summary>
        /// <param name="compilation">The project compilation.</param>
        /// <param name="path">The path of the source script.</param>
        /// <param name="attributeTypeSymbol">The type symbol of the attribute to check.</param>
        public static bool CheckAttributeFromScript(CSharpCompilation compilation, string path, ITypeSymbol attributeTypeSymbol)
        {
            if (compilation == null) throw new ArgumentNullException(nameof(compilation));
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            if (attributeTypeSymbol == null) throw new ArgumentNullException(nameof(attributeTypeSymbol));

            SyntaxTree tree = SyntaxFactory.ParseSyntaxTree(File.ReadAllText(path));
            SemanticModel model = compilation.AddSyntaxTrees(tree).GetSemanticModel(tree);

            var walker = new CodeGenerateWalkerCheckAttribute(model, attributeTypeSymbol);

            walker.Visit(tree.GetRoot());

            return walker.Result;
        }
    }
}
