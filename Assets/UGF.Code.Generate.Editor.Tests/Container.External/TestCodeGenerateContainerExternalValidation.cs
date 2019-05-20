using System;
using NUnit.Framework;
using UGF.Code.Generate.Editor.Container.External;
using Object = UnityEngine.Object;

namespace UGF.Code.Generate.Editor.Tests.Container.External
{
    public class TestCodeGenerateContainerExternalValidation
    {
        private readonly CodeGenerateContainerExternalValidation m_validation = new CodeGenerateContainerExternalValidation();
        private readonly TestValidation m_testValidation = new TestValidation();

        private class TestValidation : CodeGenerateContainerExternalValidation
        {
            public new bool IsTypeAttribute(Type type)
            {
                return base.IsTypeAttribute(type);
            }

            public new bool IsTypeUnity(Type type)
            {
                return base.IsTypeUnity(type);
            }

            public new bool IsTypeObsolete(Type type)
            {
                return base.IsTypeObsolete(type);
            }

            public new bool IsTypeSpecial(Type type)
            {
                return base.IsTypeSpecial(type);
            }
        }

        [Test]
        public void ValidateType()
        {
            bool result0 = m_validation.Validate(typeof(TestValidationTargetExternal));
            bool result1 = m_validation.Validate(typeof(TestAttribute));
            bool result2 = m_validation.Validate(typeof(Object));
#pragma warning disable 612
            bool result3 = m_validation.Validate(typeof(TestValidationTargetObsolete));
#pragma warning restore 612

            Assert.True(result0);
            Assert.False(result1);
            Assert.False(result2);
            Assert.False(result3);
        }

        [Test]
        public void IsTypeAttribute()
        {
            bool result = m_testValidation.IsTypeAttribute(typeof(TestAttribute));

            Assert.True(result);
        }

        [Test]
        public void IsTypeUnity()
        {
            bool result = m_testValidation.IsTypeUnity(typeof(Object));

            Assert.True(result);
        }

        [Test]
        public void IsTypeObsolete()
        {
#pragma warning disable 612
            bool result = m_testValidation.IsTypeObsolete(typeof(TestValidationTargetObsolete));
#pragma warning restore 612

            Assert.True(result);
        }

        [Test]
        public void IsTypeSpecial()
        {
            Assert.Ignore();
        }
    }

    public class TestValidationTargetExternal
    {
        public bool Field;

        public bool Property { get; set; }
    }

    [Obsolete]
    public class TestValidationTargetObsolete
    {
        public bool Field;

        public bool Property { get; set; }
    }
}
