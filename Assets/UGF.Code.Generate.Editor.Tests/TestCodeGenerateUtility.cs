using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using UGF.Code.Analysis.Editor;
using UnityEngine;

namespace UGF.Code.Generate.Editor.Tests
{
    public class TestCodeGenerateUtility
    {
        [Test]
        public void Test()
        {
            CSharpCompilation compilation = CodeAnalysisEditorUtility.ProjectCompilation;
            PropertyDeclarationSyntax property = CodeGenerateEditorUtility.PropertyDeclaration(compilation, "Property", typeof(string), Accessibility.Public);

            Debug.Log(property.NormalizeWhitespace().ToFullString());
        }
    }
}
