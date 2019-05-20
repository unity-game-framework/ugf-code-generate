using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            LiteralExpressionSyntax initializer = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(10));
            PropertyDeclarationSyntax node = m_generator.AutoPropertyDeclaration("Name", m_generator.TypeExpression(SpecialType.System_Int32), Accessibility.NotApplicable, DeclarationModifiers.None, initializer);

            string result0 = node.NormalizeWhitespace().ToFullString();
            string expected = @"int Name
{
    get;
    set;
}

= 10;";

            Assert.NotNull(node);
            Assert.AreEqual(expected, result0);
        }

        [Test]
        public void TryGetAnyTypeByMetadataName()
        {
            bool result0 = m_compilation.TryGetAnyTypeByMetadataName(typeof(int).FullName, out INamedTypeSymbol typeSymbol0);
            bool result1 = m_compilation.TryGetAnyTypeByMetadataName(typeof(Task<>).FullName, out INamedTypeSymbol typeSymbol1);

            Assert.True(result0);
            Assert.NotNull(typeSymbol0);
            Assert.True(result1);
            Assert.NotNull(typeSymbol1);
        }

        [Test]
        public void TryConstructTypeSymbol()
        {
            bool result0 = m_compilation.TryConstructTypeSymbol(typeof(int), out INamedTypeSymbol typeSymbol0);
            bool result1 = m_compilation.TryConstructTypeSymbol(typeof(Vector2), out INamedTypeSymbol typeSymbol1);
            bool result2 = m_compilation.TryConstructTypeSymbol(typeof(List<>), out INamedTypeSymbol typeSymbol2);
            bool result3 = m_compilation.TryConstructTypeSymbol(typeof(List<int>), out INamedTypeSymbol typeSymbol3);

            Assert.True(result0);
            Assert.NotNull(typeSymbol0);
            Assert.True(result1);
            Assert.NotNull(typeSymbol1);
            Assert.True(result2);
            Assert.NotNull(typeSymbol2);
            Assert.True(result3);
            Assert.NotNull(typeSymbol3);

            string name00 = typeSymbol0.ToDisplayString();
            string name01 = m_generator.TypeExpression(typeSymbol0).ToFullString();
            string name10 = typeSymbol1.ToDisplayString();
            string name11 = m_generator.TypeExpression(typeSymbol1).ToFullString();
            string name20 = typeSymbol2.ToDisplayString();
            string name21 = m_generator.TypeExpression(typeSymbol2).ToFullString();
            string name30 = typeSymbol3.ToDisplayString();
            string name31 = m_generator.TypeExpression(typeSymbol3).ToFullString();

            Assert.AreEqual("int", name00);
            Assert.AreEqual("global::System.Int32", name01);
            Assert.AreEqual("UnityEngine.Vector2", name10);
            Assert.AreEqual("global::UnityEngine.Vector2", name11);
            Assert.AreEqual("System.Collections.Generic.List<T>", name20);
            Assert.AreEqual("global::System.Collections.Generic.List<T>", name21);
            Assert.AreEqual("System.Collections.Generic.List<int>", name30);
            Assert.AreEqual("global::System.Collections.Generic.List<global::System.Int32>", name31);
        }

        [Test]
        public void TryConstructGenericTypeSymbol()
        {
            Type definition0 = typeof(List<>);
            Type[] arguments0 = { typeof(int) };
            Type definition1 = typeof(List<>);
            Type[] arguments1 = { typeof(List<List<int>>) };

            bool result0 = m_compilation.TryConstructGenericTypeSymbol(definition0, arguments0, out INamedTypeSymbol typeSymbol0);
            bool result1 = m_compilation.TryConstructGenericTypeSymbol(definition1, arguments1, out INamedTypeSymbol typeSymbol1);

            Assert.True(result0);
            Assert.NotNull(typeSymbol0);
            Assert.True(result1);
            Assert.NotNull(typeSymbol1);

            string name00 = typeSymbol0.ToDisplayString();
            string name01 = m_generator.TypeExpression(typeSymbol0).ToFullString();
            string name10 = typeSymbol1.ToDisplayString();
            string name11 = m_generator.TypeExpression(typeSymbol1).ToFullString();

            Assert.AreEqual("System.Collections.Generic.List<int>", name00);
            Assert.AreEqual("global::System.Collections.Generic.List<global::System.Int32>", name01);
            Assert.AreEqual("System.Collections.Generic.List<System.Collections.Generic.List<System.Collections.Generic.List<int>>>", name10);
            Assert.AreEqual("global::System.Collections.Generic.List<global::System.Collections.Generic.List<global::System.Collections.Generic.List<global::System.Int32>>>", name11);
        }

        [Test]
        public void TryGetTypeByMetadataName()
        {
#pragma warning disable 618
            bool result0 = m_compilation.TryGetTypeByMetadataName(typeof(List<int>), out INamedTypeSymbol typeSymbol0);
            bool result1 = m_compilation.TryGetTypeByMetadataName(typeof(Vector2), out INamedTypeSymbol typeSymbol1);
#pragma warning restore 618
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
#pragma warning disable 618
            bool result0 = m_compilation.TryGetGenericTypeByMetadataName(typeof(List<>).FullName, new[] { typeof(int).FullName }, out INamedTypeSymbol typeSymbol);
#pragma warning restore 618
            string result1 = typeSymbol.ToDisplayString();
            string result2 = m_generator.TypeExpression(typeSymbol).ToFullString();

            Assert.True(result0);
            Assert.NotNull(typeSymbol);
            Assert.AreEqual("System.Collections.Generic.List<int>", result1);
            Assert.AreEqual("global::System.Collections.Generic.List<global::System.Int32>", result2);
        }
    }
}
