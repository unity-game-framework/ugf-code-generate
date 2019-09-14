using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
            CSharpCompilation compilation = CodeAnalysisEditorUtility.ProjectCompilation;
            SyntaxGenerator generator = CodeAnalysisEditorUtility.Generator;

            compilation.TryConstructTypeSymbol(typeof(List<int>), out ITypeSymbol typeSymbol);

            var container = new CodeGenerateContainer("Container");

            container.Fields.Add(new CodeGenerateContainerField("IntValue", "int"));
            container.Fields.Add(new CodeGenerateContainerField("FloatValue", "float"));
            container.Fields.Add(new CodeGenerateContainerField("BoolValue", "bool", null, true));
            container.Fields.Add(new CodeGenerateContainerField("ListValue", typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)));

            SyntaxNode node = container.Generate(generator);

            Assert.NotNull(node);

            string result = node.NormalizeWhitespace().ToFullString();
            string expected = File.ReadAllText(m_target);

            Assert.AreEqual(expected, result);
        }
    }
}
