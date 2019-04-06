using System.IO;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;
using NUnit.Framework;
using UGF.Code.Analysis.Editor;
using UnityEngine;

namespace UGF.Code.Generate.Editor.Tests
{
    public class TestCodeGenerateRewriterAddAttributeToKind
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

        private void AddIdent(StringBuilder builder, int depth)
        {
            for (int i = 0; i < depth; i++)
            {
                builder.Append("    ");
            }
        }

        [Test]
        public void Visit()
        {
            string sourceWith = File.ReadAllText(m_targetWithAttribute);
            string sourceWithout = File.ReadAllText(m_targetWithoutAttribute);

            SyntaxNode attribute = m_generator.Attribute(m_compilation, typeof(TestAttribute));
            SyntaxNode node = SyntaxFactory.ParseSyntaxTree(sourceWithout).GetRoot();

            var rewriter = new CodeGenerateRewriterAddAttributeToKind(CodeAnalysisEditorUtility.Generator, attribute, SyntaxKind.ClassDeclaration);
            var format = new CodeGenerateRewriterFormatAttributeList();

            node = rewriter.Visit(node);
            node = format.Visit(node);

            string result = node.ToFullString();

            Debug.Log(result);
            Debug.Log(GetSyntaxTree(result));

            Assert.AreEqual(sourceWith, result);
        }

        [Test]
        public void PrintSyntaxTree()
        {
            string source = File.ReadAllText(m_targetWithoutAttribute);

            Debug.Log(GetSyntaxTree(source));
        }

        private string GetSyntaxTree(string source)
        {
            SyntaxTree tree = SyntaxFactory.ParseSyntaxTree(source);
            var builder = new StringBuilder();

            PrintSyntaxNode(builder, tree.GetRoot(), 0);

            return builder.ToString();
        }

        private void PrintSyntaxNode(StringBuilder builder, SyntaxNode node, int depth)
        {
            foreach (SyntaxTrivia trivia in node.GetLeadingTrivia())
            {
                AddIdent(builder, depth);

                builder.Append("[");
                builder.Append(trivia.GetType().Name);
                builder.Append(", ");
                builder.Append(trivia.Kind());
                builder.Append("]");
                builder.Append(" (Leading)");
                builder.AppendLine();
            }

            AddIdent(builder, depth);

            builder.Append("[");
            builder.Append(node.GetType().Name);
            builder.Append("]");
            builder.AppendLine();

            foreach (SyntaxTrivia trivia in node.GetTrailingTrivia())
            {
                AddIdent(builder, depth);

                builder.Append("[");
                builder.Append(trivia.GetType().Name);
                builder.Append(", ");
                builder.Append(trivia.Kind());
                builder.Append("]");
                builder.Append(" (Trailing)");
                builder.AppendLine();
            }

            foreach (SyntaxNode child in node.ChildNodes())
            {
                PrintSyntaxNode(builder, child, depth + 1);
            }
        }
    }
}
