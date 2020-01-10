using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;
using NUnit.Framework;
using UGF.Code.Analysis.Editor;
using UGF.Code.Generate.Editor.Container;
using UnityEngine;

namespace UGF.Code.Generate.Editor.Tests.Container
{
    public class TestCodeGenerateContainerEditorUtility
    {
        private readonly string m_target = "Assets/UGF.Code.Generate.Editor.Tests/Container/TestTargetContainer2.txt";
        private readonly string m_target3 = "Assets/UGF.Code.Generate.Editor.Tests/Container/TestTargetContainer3.txt";

        [Test]
        public void CreateUnit()
        {
            CodeGenerateContainerValidation validation = CodeGenerateContainerEditorUtility.DefaultValidation;
            CSharpCompilation compilation = CodeAnalysisEditorUtility.ProjectCompilation;
            SyntaxGenerator generator = CodeAnalysisEditorUtility.Generator;

            SyntaxNode unit = CodeGenerateContainerEditorUtility.CreateUnit(typeof(Target), validation, compilation, generator);

            string result = unit.NormalizeWhitespace().ToFullString();
            string expected = File.ReadAllText(m_target3);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Create()
        {
            CodeGenerateContainerValidation validation = CodeGenerateContainerEditorUtility.DefaultValidation;
            CSharpCompilation compilation = CodeAnalysisEditorUtility.ProjectCompilation;
            SyntaxGenerator generator = CodeAnalysisEditorUtility.Generator;
            CodeGenerateContainer container = CodeGenerateContainerEditorUtility.Create(typeof(Target), validation, compilation);

            Assert.NotNull(container);

            string result = container.Generate(generator).NormalizeWhitespace().ToFullString();
            string expected = File.ReadAllText(m_target);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void IsValidType()
        {
            bool result0 = CodeGenerateContainerEditorUtility.IsValidType(typeof(TargetClass));
            bool result1 = CodeGenerateContainerEditorUtility.IsValidType(typeof(TargetStruct));
            bool result2 = CodeGenerateContainerEditorUtility.IsValidType(typeof(TargetStaticClass));
            bool result3 = CodeGenerateContainerEditorUtility.IsValidType(typeof(Target));
            bool result4 = CodeGenerateContainerEditorUtility.IsValidType(typeof(TargetAbstractClass));
            bool result5 = CodeGenerateContainerEditorUtility.IsValidType(typeof(TargetGenericClass<>));
            bool result6 = CodeGenerateContainerEditorUtility.IsValidType(typeof(TestDelegate));

            Assert.True(result0);
            Assert.True(result1);
            Assert.False(result2);
            Assert.True(result3);
            Assert.False(result4);
            Assert.False(result5);
            Assert.False(result6);
        }
    }

    public class Target
    {
        public bool PublicField;
#pragma warning disable 414
        internal bool InternalField = false;
#pragma warning restore 414
        protected bool ProtectedField;
        private bool m_privateField;
        public static bool StaticField;
        public readonly bool ReadOnlyField;
        public const bool ConstField = false;

        public bool PublicProp { get; set; }
        private bool PrivateProp { get; set; }
        public bool OnlySetProp { set { } }
        public bool OnlyGetProp { get; }
        public static bool StaticProp { get; set; }
        internal bool InternalProp { get; set; }
        public bool this[int index] { get { return false; } set { } }

        public List<Vector2> List { get; set; }
    }

    public class TargetClass
    {
    }

    public struct TargetStruct
    {
    }

    public static class TargetStaticClass
    {
    }

    public abstract class TargetAbstractClass
    {
        public abstract bool AbstractProp { get; set; }
    }

    public class TargetGenericClass<T>
    {
    }

    public delegate void TestDelegate();
}
