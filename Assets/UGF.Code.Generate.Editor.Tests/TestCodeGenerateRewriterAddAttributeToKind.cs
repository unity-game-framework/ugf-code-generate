using System.IO;
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

        [Test]
        public void Visit()
        {
            string sourceWith = File.ReadAllText(m_targetWithAttribute);
            string sourceWithout = File.ReadAllText(m_targetWithoutAttribute);
            
            SyntaxNode attribute = m_generator.Attribute(m_compilation, typeof(TestAttribute));
            SyntaxNode node = SyntaxFactory.ParseSyntaxTree(sourceWithout).GetRoot();
            
            var rewriter = new CodeGenerateRewriterAddAttributeToKind(CodeAnalysisEditorUtility.Generator, attribute, SyntaxKind.ClassDeclaration);

            node = rewriter.Visit(node);

            string result = node.ToFullString();
            
            Debug.Log(result);
            
            Assert.AreEqual(sourceWith, result);
        }
    }
}
