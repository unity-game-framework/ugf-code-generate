using System;
using System.IO;
using NUnit.Framework;
using UGF.Code.Generate.Editor.Container.Info;
using UnityEngine;

namespace UGF.Code.Generate.Editor.Tests.Container.External
{
    public class TestCodeGenerateContainerInfo
    {
        private readonly string m_path = "Assets/UGF.Code.Generate.Editor.Tests/Container.External/TestExternalInfo.json";

        private readonly CodeGenerateContainerInfo m_info = new CodeGenerateContainerInfo
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

        [Test]
        public void Serialize()
        {
            string expected = File.ReadAllText(m_path);
            string actual = JsonUtility.ToJson(m_info, true).Replace("\n", "\r\n");

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Deserialize()
        {
            string text = File.ReadAllText(m_path);
            var info = JsonUtility.FromJson<CodeGenerateContainerInfo>(text);

            Assert.NotNull(info);

            bool result = info.TryGetTargetType(out Type type);

            Assert.True(result);
            Assert.NotNull(type);
            Assert.AreEqual(typeof(TestTargetContainerExternal), type);
            Assert.AreEqual(3, info.Members.Count);
            Assert.True(info.Members.Exists(x => x.Name == "Field"));
            Assert.True(info.Members.Exists(x => x.Name == "Field2"));
            Assert.True(info.Members.Exists(x => x.Name == "Property"));
        }

        [Test]
        public void TryGetMember()
        {
            bool result0 = m_info.TryGetMember("Field", out CodeGenerateContainerInfo.MemberInfo member0);
            bool result1 = m_info.TryGetMember("Field2", out CodeGenerateContainerInfo.MemberInfo member1);
            bool result2 = m_info.TryGetMember("Property", out CodeGenerateContainerInfo.MemberInfo member2);
            bool result3 = m_info.TryGetMember("Property2", out CodeGenerateContainerInfo.MemberInfo member3);

            Assert.True(result0);
            Assert.NotNull(member0);
            Assert.AreEqual("Field", member0.Name);
            Assert.True(result1);
            Assert.NotNull(member1);
            Assert.AreEqual("Field2", member1.Name);
            Assert.True(result2);
            Assert.NotNull(member2);
            Assert.AreEqual("Property", member2.Name);
            Assert.False(result3);
            Assert.Null(member3);
        }

        [Test]
        public void TryGetTargetType()
        {
            bool result = m_info.TryGetTargetType(out Type type);

            Assert.True(result);
            Assert.NotNull(type);
            Assert.AreEqual(typeof(TestTargetContainerExternal), type);
        }
    }
}
