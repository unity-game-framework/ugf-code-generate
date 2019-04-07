using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using NUnit.Framework;
using UGF.Code.Analysis.Editor;
using UnityEngine;

namespace UGF.Code.Generate.Editor.Tests
{
    public class TestCodeGenerateEditorExtensions
    {
        private CSharpCompilation m_compilation;
        private SyntaxGenerator m_generator;

        [SetUp]
        public void Setup()
        {
            m_compilation = CodeAnalysisEditorUtility.ProjectCompilation;
            m_generator = CodeAnalysisEditorUtility.Generator;
        }

        [Test]
        public void AutoPropertyDeclaration()
        {
            PropertyDeclarationSyntax node = m_generator.AutoPropertyDeclaration("Name", m_generator.TypeExpression(SpecialType.System_Int32));

            string result0 = node.NormalizeWhitespace().ToFullString();
            string expected = @"int Name
{
    get;
    set;
}";

            Assert.NotNull(node);
            Assert.AreEqual(expected, result0);
        }

        [Test]
        public void TryGetTypeByMetadataName()
        {
            bool result0 = m_compilation.TryGetTypeByMetadataName(typeof(List<int>), out INamedTypeSymbol typeSymbol0);
            bool result1 = m_compilation.TryGetTypeByMetadataName(typeof(Vector2), out INamedTypeSymbol typeSymbol1);
            string result2 = typeSymbol0.ToDisplayString();
            string result3 = m_generator.TypeExpression(typeSymbol0).ToFullString();
            string result4 = typeSymbol1.ToDisplayString();
            string result5 = m_generator.TypeExpression(typeSymbol1).ToFullString();

            Assert.True(result0);
            Assert.NotNull(typeSymbol0);
            Assert.True(result1);
            Assert.NotNull(typeSymbol1);
            Assert.AreEqual("System.Collections.Generic.List<int>", result2);
            Assert.AreEqual("global::System.Collections.Generic.List<global::System.Int32>", result3);
            Assert.AreEqual("UnityEngine.Vector2", result4);
            Assert.AreEqual("global::UnityEngine.Vector2", result5);
        }

        [Test]
        public void TryGetGenericTypeByMetadataName()
        {
            bool result0 = m_compilation.TryGetGenericTypeByMetadataName(typeof(List<>).FullName, new[] { typeof(int).FullName }, out INamedTypeSymbol typeSymbol);
            string result1 = typeSymbol.ToDisplayString();
            string result2 = m_generator.TypeExpression(typeSymbol).ToFullString();

            Assert.True(result0);
            Assert.NotNull(typeSymbol);
            Assert.AreEqual("System.Collections.Generic.List<int>", result1);
            Assert.AreEqual("global::System.Collections.Generic.List<global::System.Int32>", result2);
        }
    }
}
