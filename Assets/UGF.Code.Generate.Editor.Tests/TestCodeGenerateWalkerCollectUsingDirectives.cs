using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

namespace UGF.Code.Generate.Editor.Tests
{
    public class TestCodeGenerateWalkerCollectUsingDirectives
    {
        private readonly string m_targetWithUsings = "Assets/UGF.Code.Generate.Editor.Tests/TestTargetWithUsings.txt";

        [Test]
        public void Visit()
        {
            string source = File.ReadAllText(m_targetWithUsings);
            SyntaxNode node = SyntaxFactory.ParseSyntaxTree(source).GetRoot();

            var walker = new CodeGenerateWalkerCollectUsingDirectives();

            walker.Visit(node);

            bool result0 = walker.UsingDirectives.Exists(x => x.Name.ToString() == "System");
            bool result1 = walker.UsingDirectives.Exists(x => x.Name.ToString() == "UnityEngine");

            Assert.True(result0);
            Assert.True(result1);
        }
    }
}
