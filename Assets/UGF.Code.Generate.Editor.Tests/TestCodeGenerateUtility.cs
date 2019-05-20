using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using UGF.Code.Analysis.Editor;

namespace UGF.Code.Generate.Editor.Tests
{
    public class TestCodeGenerateUtility
    {
        private readonly string m_targetWithGeneratedCodeLeadingTrivia = "Assets/UGF.Code.Generate.Editor.Tests/TestTargetWithGeneratedCodeLeadingTrivia.txt";
        private readonly string m_targetWithoutGeneratedCodeLeadingTrivia = "Assets/UGF.Code.Generate.Editor.Tests/TestTargetWithoutGeneratedCodeLeadingTrivia.txt";
        private readonly string m_targetWithAttribute = "Assets/UGF.Code.Generate.Editor.Tests/TestTargetWithAttribute.txt";
        private CSharpCompilation m_compilation;

        [SetUp]
        public void Setup()
        {
            m_compilation = CodeAnalysisEditorUtility.ProjectCompilation;
        }

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

        [Test]
        public void CheckAttributeFromScriptFromType()
        {
            bool result = CodeGenerateEditorUtility.CheckAttributeFromScript(m_compilation, m_targetWithAttribute, typeof(TestAttribute));

            Assert.True(result);
        }

        [Test]
        public void CheckAttributeFromScript()
        {
            INamedTypeSymbol typeSymbol = m_compilation.GetTypeByMetadataName(typeof(TestAttribute).FullName);

            bool result = CodeGenerateEditorUtility.CheckAttributeFromScript(m_compilation, m_targetWithAttribute, typeSymbol);

            Assert.True(result);
        }

        [Test]
        public void GetPathForGeneratedScript()
        {
            string path0 = CodeGenerateEditorUtility.GetPathForGeneratedScript("Assets/Code/Script.cs");
            string path1 = CodeGenerateEditorUtility.GetPathForGeneratedScript("Assets/Code/Script.cs", "Label");

            Assert.AreEqual("Assets/Code/Script.Generated.cs", path0);
            Assert.AreEqual("Assets/Code/Script.Label.Generated.cs", path1);
        }
    }
}
