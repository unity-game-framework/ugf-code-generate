using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using NUnit.Framework;
using UGF.Code.Analysis.Editor;
using UGF.Code.Generate.Editor.Container;

namespace UGF.Code.Generate.Editor.Tests.Container
{
    public class TestCodeGenerateContainerField
    {
        [Test]
        public void GenerateField()
        {
            SyntaxGenerator generator = CodeAnalysisEditorUtility.Generator;

            var field = new CodeGenerateContainerField("Field", "int");

            SyntaxNode node = field.Generate(generator);

            Assert.NotNull(node);

            string result = node.NormalizeWhitespace().ToFullString();

            Assert.AreEqual("public int Field;", result);
        }

        [Test]
        public void GenerateFieldWithInitializer()
        {
            SyntaxGenerator generator = CodeAnalysisEditorUtility.Generator;

            LiteralExpressionSyntax initializer = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(10));
            var field = new CodeGenerateContainerField("Field", "int", initializer);

            SyntaxNode node = field.Generate(generator);

            Assert.NotNull(node);

            string result = node.NormalizeWhitespace().ToFullString();

            Assert.AreEqual("public int Field = 10;", result);
        }

        [Test]
        public void GenerateProperty()
        {
            SyntaxGenerator generator = CodeAnalysisEditorUtility.Generator;

            var field = new CodeGenerateContainerField("Field", "int", null, true);

            SyntaxNode node = field.Generate(generator);

            Assert.NotNull(node);

            string result = node.NormalizeWhitespace().ToFullString();
            string expected = @"public int Field
{
    get;
    set;
}";

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GeneratePropertyWithInitializer()
        {
            SyntaxGenerator generator = CodeAnalysisEditorUtility.Generator;

            LiteralExpressionSyntax initializer = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(10));
            var field = new CodeGenerateContainerField("Field", "int", initializer, true);

            SyntaxNode node = field.Generate(generator);

            Assert.NotNull(node);

            string result = node.NormalizeWhitespace().ToFullString();
            string expected = @"public int Field
{
    get;
    set;
}

= 10;";

            Assert.AreEqual(expected, result);
        }
    }
}
