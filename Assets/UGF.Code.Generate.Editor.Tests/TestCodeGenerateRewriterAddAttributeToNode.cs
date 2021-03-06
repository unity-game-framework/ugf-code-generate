using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;
using NUnit.Framework;
using UGF.Code.Analysis.Editor;

namespace UGF.Code.Generate.Editor.Tests
{
    public class TestCodeGenerateRewriterAddAttributeToNode
    {
        private readonly string m_targetWithAttribute = "Assets/UGF.Code.Generate.Editor.Tests/TestTargetWithAttribute.txt";
        private readonly string m_targetWithoutAttribute = "Assets/UGF.Code.Generate.Editor.Tests/TestTargetWithoutAttribute.txt";
        private CSharpCompilation m_compilation;
        private SyntaxGenerator m_generator;

        [SetUp]
        public void Setup()
        {
            m_compilation = CodeAnalysisEditorUtility.ProjectCompilation;
            m_generator = CodeAnalysisEditorUtility.Generator;
        }

        [Test]
        public void Visit()
        {
            string sourceWith = File.ReadAllText(m_targetWithAttribute);
            string sourceWithout = File.ReadAllText(m_targetWithoutAttribute);

            ITypeSymbol typeSymbol = m_compilation.GetTypeByMetadataName(typeof(TestAttribute).FullName);
            SyntaxNode attribute = m_generator.Attribute(m_generator.TypeExpression(typeSymbol));
            SyntaxNode node = SyntaxFactory.ParseSyntaxTree(sourceWithout).GetRoot();

            var rewriter = new CodeGenerateRewriterAddAttributeToNode(CodeAnalysisEditorUtility.Generator, attribute, syntaxNode => syntaxNode.Kind() == SyntaxKind.ClassDeclaration);
            var format = new CodeGenerateRewriterFormatAttributeList();

            node = rewriter.Visit(node);
            node = format.Visit(node);

            string result = node.ToFullString();

            Assert.AreEqual(sourceWith, result);
        }
    }
}
