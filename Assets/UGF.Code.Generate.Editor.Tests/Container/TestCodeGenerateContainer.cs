using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using NUnit.Framework;
using UGF.Code.Analysis.Editor;
using UGF.Code.Generate.Editor.Container;

namespace UGF.Code.Generate.Editor.Tests.Container
{
    public class TestCodeGenerateContainer
    {
        private readonly string m_target = "Assets/UGF.Code.Generate.Editor.Tests/Container/TestTargetContainer.txt";

        [Test]
        public void Generate()
        {
            SyntaxGenerator generator = CodeAnalysisEditorUtility.Generator;

            var container = new CodeGenerateContainer("Container");

            container.Fields.Add(new CodeGenerateContainerField("IntValue", generator.TypeExpression(SpecialType.System_Int32)));
            container.Fields.Add(new CodeGenerateContainerField("FloatValue", generator.TypeExpression(SpecialType.System_Single)));
            container.Fields.Add(new CodeGenerateContainerField("BoolValue", generator.TypeExpression(SpecialType.System_Boolean), null, true));

            SyntaxNode node = container.Generate(generator);

            Assert.NotNull(node);

            string result = node.NormalizeWhitespace().ToFullString();
            string expected = File.ReadAllText(m_target);

            Assert.AreEqual(expected, result);
        }
    }
}
