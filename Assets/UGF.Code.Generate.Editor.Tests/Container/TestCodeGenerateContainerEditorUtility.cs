using System.Collections.Generic;
using System.IO;
using System.Reflection;
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

        private class TargetClass
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

        [Test]
        public void Create()
        {
            CSharpCompilation compilation = CodeAnalysisEditorUtility.ProjectCompilation;
            SyntaxGenerator generator = CodeAnalysisEditorUtility.Generator;
            CodeGenerateContainer container = CodeGenerateContainerEditorUtility.Create(compilation, typeof(Target));

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

            Assert.False(result0);
            Assert.True(result1);
            Assert.False(result2);
            Assert.True(result3);
            Assert.False(result4);
            Assert.False(result5);
        }

        [Test]
        public void IsValidField()
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;

            FieldInfo field0 = typeof(Target).GetField("PublicField");
            FieldInfo field1 = typeof(Target).GetField("InternalField", flags);
            FieldInfo field2 = typeof(Target).GetField("ProtectedField", flags);
            FieldInfo field3 = typeof(Target).GetField("m_privateField", flags);
            FieldInfo field4 = typeof(Target).GetField("StaticField");
            FieldInfo field5 = typeof(Target).GetField("ReadOnlyField");
            FieldInfo field6 = typeof(Target).GetField("ConstField");

            bool result0 = CodeGenerateContainerEditorUtility.IsValidField(field0);
            bool result1 = CodeGenerateContainerEditorUtility.IsValidField(field1);
            bool result2 = CodeGenerateContainerEditorUtility.IsValidField(field2);
            bool result3 = CodeGenerateContainerEditorUtility.IsValidField(field3);
            bool result4 = CodeGenerateContainerEditorUtility.IsValidField(field4);
            bool result5 = CodeGenerateContainerEditorUtility.IsValidField(field5);
            bool result6 = CodeGenerateContainerEditorUtility.IsValidField(field6);

            Assert.True(result0);
            Assert.False(result1);
            Assert.False(result2);
            Assert.False(result3);
            Assert.False(result4);
            Assert.False(result5);
            Assert.False(result6);
        }

        [Test]
        public void IsValidProperty()
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;

            PropertyInfo property0 = typeof(Target).GetProperty("PublicProp");
            PropertyInfo property1 = typeof(Target).GetProperty("PrivateProp", flags);
            PropertyInfo property2 = typeof(Target).GetProperty("OnlySetProp");
            PropertyInfo property3 = typeof(Target).GetProperty("OnlyGetProp");
            PropertyInfo property4 = typeof(Target).GetProperty("StaticProp");
            PropertyInfo property5 = typeof(Target).GetProperty("InternalProp", flags);
            PropertyInfo property6 = typeof(Target).GetProperty("Item");

            bool result0 = CodeGenerateContainerEditorUtility.IsValidProperty(property0);
            bool result1 = CodeGenerateContainerEditorUtility.IsValidProperty(property1);
            bool result2 = CodeGenerateContainerEditorUtility.IsValidProperty(property2);
            bool result3 = CodeGenerateContainerEditorUtility.IsValidProperty(property3);
            bool result4 = CodeGenerateContainerEditorUtility.IsValidProperty(property4);
            bool result5 = CodeGenerateContainerEditorUtility.IsValidProperty(property5);
            bool result6 = CodeGenerateContainerEditorUtility.IsValidProperty(property6);

            Assert.True(result0);
            Assert.False(result1);
            Assert.False(result2);
            Assert.False(result3);
            Assert.False(result4);
            Assert.False(result5);
            Assert.False(result6);
        }
    }
}
