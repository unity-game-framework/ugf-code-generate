using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using NUnit.Framework;
using UGF.Code.Analysis.Editor;
using UGF.Code.Generate.Editor.Experimental;

namespace UGF.Code.Generate.Editor.Tests.Experimental
{
    public class TestCodeGenerateRewriterAddAttributeFromGenericArgument
    {
        private readonly string m_withoutAttributes = "Assets/UGF.Code.Generate.Editor.Tests/Experimental/TestTargetWithoutAttributes.txt";
        private readonly string m_withAttributes = "Assets/UGF.Code.Generate.Editor.Tests/Experimental/TestTargetWithAttributes.txt";

        [Test]
        public void Visit()
        {
            string sourceWithout = File.ReadAllText(m_withoutAttributes);
            string sourceWith = File.ReadAllText(m_withAttributes);

            CSharpCompilation compilation = CodeAnalysisEditorUtility.ProjectCompilation;
            SyntaxGenerator generator = CodeAnalysisEditorUtility.Generator;

            INamedTypeSymbol attributeTypeSymbol = compilation.GetTypeByMetadataName(typeof(TestAttribute).FullName);
            var attributeType = (TypeSyntax)generator.TypeExpression(attributeTypeSymbol);

            var rewriter = new CodeGenerateRewriterAddAttributeFromGenericArgument(generator, attributeType, "GenericClass");
            SyntaxTree tree = SyntaxFactory.ParseSyntaxTree(sourceWithout);

            string result = rewriter.Visit(tree.GetRoot()).ToFullString();

            Assert.AreEqual(sourceWith, result);
        }
    }
}
