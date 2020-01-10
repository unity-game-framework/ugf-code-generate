using System;
using System.IO;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using UGF.AssetPipeline.Editor.Asset.Info;
using UGF.Code.Generate.Editor.Container;
using UGF.Code.Generate.Editor.Container.Asset;
using UGF.Code.Generate.Editor.Container.Info;

namespace UGF.Code.Generate.Editor.Tests.Container.External
{
    public class TestCodeGenerateContainerExternalEditorUtility
    {
        private readonly string m_target = "Assets/UGF.Code.Generate.Editor.Tests/Container/TestTargetContainer3.txt";
        private readonly ICodeGenerateContainerValidation m_validation = CodeGenerateContainerAssetEditorUtility.DefaultValidation;

        [Test]
        public void CreateInfo()
        {
            CodeGenerateContainerInfo info = CodeGenerateContainerInfoEditorUtility.CreateInfo(typeof(TestTargetContainerExternal), m_validation);

            Assert.NotNull(info);

            bool result1 = info.TryGetTargetType(out Type type);

            Assert.True(result1);
            Assert.NotNull(type);
            Assert.AreEqual(typeof(TestTargetContainerExternal), type);
            Assert.AreEqual(3, info.Members.Count);
            Assert.True(info.Members.Exists(x => x.Name == "Field"));
            Assert.True(info.Members.Exists(x => x.Name == "Field2"));
            Assert.True(info.Members.Exists(x => x.Name == "Property"));
        }

        [Test]
        public void CreateUnit()
        {
            CodeGenerateContainerInfo info = CodeGenerateContainerInfoEditorUtility.CreateInfo(typeof(Target), m_validation);

            Assert.NotNull(info);

            SyntaxNode unit = CodeGenerateContainerInfoEditorUtility.CreateUnit(info, m_validation);

            Assert.NotNull(unit);

            string result = unit.NormalizeWhitespace().ToFullString();
            string expected = File.ReadAllText(m_target);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void CreateContainer()
        {
            var info = new CodeGenerateContainerInfo
            {
                TypeName = typeof(TestTargetContainerExternal).AssemblyQualifiedName,
                Members =
                {
                    new CodeGenerateContainerInfo.MemberInfo
                    {
                        Name = "Field"
                    },
                    new CodeGenerateContainerInfo.MemberInfo
                    {
                        Active = false,
                        Name = "Field2"
                    },
                    new CodeGenerateContainerInfo.MemberInfo
                    {
                        Name = "Property"
                    }
                }
            };

            CodeGenerateContainer container = CodeGenerateContainerInfoEditorUtility.CreateContainer(info, m_validation);

            Assert.NotNull(container);
            Assert.AreEqual(2, container.Fields.Count);
            Assert.True(container.Fields.Exists(x => x.Name == "Field"));
            Assert.False(container.Fields.Exists(x => x.Name == "Field2"));
            Assert.True(container.Fields.Exists(x => x.Name == "Property"));
        }

        [Test]
        public void TryGetInfoFromAssetPath()
        {
            string path = "Assets/UGF.Code.Generate.Editor.Tests/Container.External/TestExternalInfo.json";

            var info = AssetInfoEditorUtility.LoadInfo<CodeGenerateContainerInfo>(path);

            Assert.NotNull(info);

            bool result1 = info.TryGetTargetType(out Type type);

            Assert.True(result1);
            Assert.NotNull(type);
            Assert.AreEqual(typeof(TestTargetContainerExternal), type);
            Assert.AreEqual(3, info.Members.Count);
            Assert.True(info.Members.Exists(x => x.Name == "Field"));
            Assert.True(info.Members.Exists(x => x.Name == "Field2"));
            Assert.True(info.Members.Exists(x => x.Name == "Property"));
        }
    }

    public class TestTargetContainerExternal
    {
        public bool Field;
        public bool Field2;

        public bool Property { get; set; }
    }
}
