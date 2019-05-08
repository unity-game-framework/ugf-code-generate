using System;
using NUnit.Framework;
using UGF.Code.Generate.Editor.Container;
using UGF.Code.Generate.Editor.Container.External;

namespace UGF.Code.Generate.Editor.Tests.Container.External
{
    public class TestCodeGenerateContainerExternalEditorUtility
    {
        [Test]
        public void CreateInfo()
        {
            CodeGenerateContainerExternalInfo info = CodeGenerateContainerExternalEditorUtility.CreateInfo(typeof(TestTargetContainerExternal));

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
        public void CreateContainer()
        {
            var info = new CodeGenerateContainerExternalInfo
            {
                TypeName = typeof(TestTargetContainerExternal).AssemblyQualifiedName,
                Members =
                {
                    new CodeGenerateContainerExternalMemberInfo
                    {
                        Name = "Field",
                        Active = true
                    },
                    new CodeGenerateContainerExternalMemberInfo
                    {
                        Name = "Field2",
                        Active = false
                    },
                    new CodeGenerateContainerExternalMemberInfo
                    {
                        Name = "Property",
                        Active = true
                    }
                }
            };

            CodeGenerateContainer container = CodeGenerateContainerExternalEditorUtility.CreateContainer(info);

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

            bool result = CodeGenerateContainerExternalEditorUtility.TryGetInfoFromAssetPath(path, out CodeGenerateContainerExternalInfo info);

            Assert.True(result);
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
