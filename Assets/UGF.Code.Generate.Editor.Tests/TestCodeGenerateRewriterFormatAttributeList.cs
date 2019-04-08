using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

namespace UGF.Code.Generate.Editor.Tests
{
    public class TestCodeGenerateRewriterFormatAttributeList
    {
        private readonly string m_targetWithAttributeFormat = "Assets/UGF.Code.Generate.Editor.Tests/TestTargetWithAttributeFormat.txt";
        private readonly string m_targetWithAttributeNoFormat = "Assets/UGF.Code.Generate.Editor.Tests/TestTargetWithAttributeNoFormat.txt";

        [Test]
        public void Visit()
        {
            string sourceFormat = File.ReadAllText(m_targetWithAttributeFormat);
            string sourceNoFormat = File.ReadAllText(m_targetWithAttributeNoFormat);

            SyntaxNode node = SyntaxFactory.ParseSyntaxTree(sourceNoFormat).GetRoot();

            var rewriter = new CodeGenerateRewriterFormatAttributeList();

            node = rewriter.Visit(node);

            string result = node.ToFullString();

            Assert.AreEqual(sourceFormat, result);
        }
    }
}
