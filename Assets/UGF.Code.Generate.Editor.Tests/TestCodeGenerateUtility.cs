using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

namespace UGF.Code.Generate.Editor.Tests
{
    public class TestCodeGenerateUtility
    {
        private readonly string m_targetWithGeneratedCodeLeadingTrivia = "Assets/UGF.Code.Generate.Editor.Tests/TestTargetWithGeneratedCodeLeadingTrivia.txt";
        private readonly string m_targetWithoutGeneratedCodeLeadingTrivia = "Assets/UGF.Code.Generate.Editor.Tests/TestTargetWithoutGeneratedCodeLeadingTrivia.txt";

        [Test]
        public void AddGeneratedCodeLeadingTrivia()
        {
            string sourceWith = File.ReadAllText(m_targetWithGeneratedCodeLeadingTrivia);
            string sourceWithout = File.ReadAllText(m_targetWithoutGeneratedCodeLeadingTrivia);

            SyntaxNode node = SyntaxFactory.ParseSyntaxTree(sourceWithout).GetRoot();

            node = CodeGenerateEditorUtility.AddGeneratedCodeLeadingTrivia(node);

            string result = node.ToFullString();

            Assert.AreEqual(sourceWith, result);
        }
    }
}
