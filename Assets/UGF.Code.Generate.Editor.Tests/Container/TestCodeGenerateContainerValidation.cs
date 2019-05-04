using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UGF.Code.Generate.Editor.Container;

namespace UGF.Code.Generate.Editor.Tests.Container
{
    public class TestCodeGenerateContainerValidation
    {
        private readonly CodeGenerateContainerValidation m_validation = CodeGenerateContainerEditorUtility.DefaultValidation;
        private readonly TestValidation m_testValidation = new TestValidation();

        private class TestValidation : CodeGenerateContainerValidation
        {
            public new bool IsTypeContainer(Type type)
            {
                return base.IsTypeContainer(type);
            }

            public new bool IsTypeHasDefaultConstructor(Type type)
            {
                return base.IsTypeHasDefaultConstructor(type);
            }

            public new bool IsTypeHasAnyValidFields(Type type)
            {
                return base.IsTypeHasAnyValidFields(type);
            }

            public new bool IsTypeHasAnyValidProperties(Type type)
            {
                return base.IsTypeHasAnyValidProperties(type);
            }
        }

        [Test]
        public void ValidateType()
        {
            bool result = m_validation.Validate(typeof(TestValidationTarget));

            Assert.True(result);
        }

        [Test]
        public void ValidateField()
        {
            FieldInfo field = typeof(TestValidationTarget).GetField("Field");

            bool result = m_validation.Validate(field);

            Assert.True(result);
        }

        [Test]
        public void ValidateProperty()
        {
            PropertyInfo property = typeof(TestValidationTarget).GetProperty("Property");

            bool result = m_validation.Validate(property);

            Assert.True(result);
        }

        [Test]
        public void GetFields()
        {
            IEnumerable<FieldInfo> fields = m_validation.GetFields(typeof(TestValidationTarget));

            Assert.NotNull(fields);
            Assert.AreEqual(1, fields.Count());
            Assert.True(fields.First().Name == "Field");
        }

        [Test]
        public void GetProperties()
        {
            IEnumerable<PropertyInfo> properties = m_validation.GetProperties(typeof(TestValidationTarget));

            Assert.NotNull(properties);
            Assert.AreEqual(1, properties.Count());
            Assert.True(properties.First().Name == "Property");
        }

        [Test]
        public void IsTypeContainer()
        {
            bool result = m_testValidation.IsTypeContainer(typeof(TestValidationTarget));

            Assert.True(result);
        }

        [Test]
        public void IsTypeHasDefaultConstructor()
        {
            bool result = m_testValidation.IsTypeHasDefaultConstructor(typeof(TestValidationTarget));

            Assert.True(result);
        }

        [Test]
        public void IsTypeHasAnyValidFields()
        {
            bool result = m_testValidation.IsTypeHasAnyValidFields(typeof(TestValidationTarget));

            Assert.True(result);
        }

        [Test]
        public void IsTypeHasAnyValidProperties()
        {
            bool result = m_testValidation.IsTypeHasAnyValidProperties(typeof(TestValidationTarget));

            Assert.True(result);
        }
    }

    public class TestValidationTarget
    {
        public bool Field;

        public bool Property { get; set; }
    }
}
