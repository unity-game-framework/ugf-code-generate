using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;
using NUnit.Framework;
using UGF.Code.Analysis.Editor;

namespace UGF.Code.Generate.Editor.Tests
{
    public class TestCodeGenerateWalkerCheckAttribute
    {
        private readonly string m_targetWithAttribute = "Assets/UGF.Code.Generate.Editor.Tests/TestTargetWithAttribute.txt";
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
            string source = File.ReadAllText(m_targetWithAttribute);
            SyntaxTree tree = SyntaxFactory.ParseSyntaxTree(source);
            SemanticModel model = m_compilation.AddSyntaxTrees(tree).GetSemanticModel(tree);
            ITypeSymbol typeSymbol = m_compilation.GetTypeByMetadataName(typeof(TestAttribute).FullName);

            var walker = new CodeGenerateWalkerCheckAttribute(model, typeSymbol);

            walker.Visit(tree.GetRoot());

            bool result = walker.Result;

            Assert.True(result);
        }
    }
}
