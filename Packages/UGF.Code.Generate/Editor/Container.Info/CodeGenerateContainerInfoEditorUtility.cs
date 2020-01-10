using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using UGF.Code.Analysis.Editor;

namespace UGF.Code.Generate.Editor.Container.Info
{
    /// <summary>
    /// Provides utilities to work with container info.
    /// </summary>
    public static class CodeGenerateContainerInfoEditorUtility
    {
        /// <summary>
        /// Creates compilation syntax unit from the specified external type information and validation.
        /// </summary>
        /// <param name="info">The container external type information.</param>
        /// <param name="validation">The container type validation to use.</param>
        /// <param name="compilation">The project compilation.</param>
        /// <param name="generator">The syntax generator.</param>
        public static SyntaxNode CreateUnit(ICodeGenerateContainerInfo info, ICodeGenerateContainerValidation validation, Compilation compilation = null, SyntaxGenerator generator = null)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            if (validation == null) throw new ArgumentNullException(nameof(validation));
            if (compilation == null) compilation = CodeAnalysisEditorUtility.ProjectCompilation;
            if (generator == null) generator = CodeAnalysisEditorUtility.Generator;

            if (!info.TryGetTargetType(out Type type))
            {
                throw new ArgumentException("The specified container external type info has invalid target type information.", nameof(info));
            }

            CodeGenerateContainer container = CreateContainer(info, validation, compilation);

            return CodeGenerateContainerEditorUtility.CreateUnit(container, generator, type.Namespace);
        }

        /// <summary>
        /// Creates container external type information from the specified type and validation.
        /// </summary>
        /// <param name="type">The target type.</param>
        /// <param name="validation">The container type validation to use.</param>
        public static CodeGenerateContainerInfo CreateInfo(Type type, ICodeGenerateContainerValidation validation)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (validation == null) throw new ArgumentNullException(nameof(validation));

            var info = new CodeGenerateContainerInfo { TypeName = type.AssemblyQualifiedName };
            IEnumerable<FieldInfo> fields = validation.GetFields(type);
            IEnumerable<PropertyInfo> properties = validation.GetProperties(type);

            foreach (FieldInfo field in fields)
            {
                if (validation.Validate(field))
                {
                    info.Members.Add(new CodeGenerateContainerInfo.MemberInfo
                    {
                        Active = true,
                        Name = field.Name
                    });
                }
            }

            foreach (PropertyInfo property in properties)
            {
                if (validation.Validate(property))
                {
                    info.Members.Add(new CodeGenerateContainerInfo.MemberInfo
                    {
                        Active = true,
                        Name = property.Name
                    });
                }
            }

            return info;
        }

        /// <summary>
        /// Creates container from the specified external type information and validation.
        /// </summary>
        /// <param name="info">The container external type information.</param>
        /// <param name="validation">The container type validation to use.</param>
        /// <param name="compilation">The project compilation.</param>
        public static CodeGenerateContainer CreateContainer(ICodeGenerateContainerInfo info, ICodeGenerateContainerValidation validation, Compilation compilation = null)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            if (validation == null) throw new ArgumentNullException(nameof(validation));
            if (compilation == null) compilation = CodeAnalysisEditorUtility.ProjectCompilation;

            if (!info.TryGetTargetType(out Type type))
            {
                throw new ArgumentException("The specified container external type info has invalid target type information.", nameof(info));
            }

            var container = new CodeGenerateContainer(type.Name, type.IsValueType);
            IEnumerable<FieldInfo> fields = validation.GetFields(type);
            IEnumerable<PropertyInfo> properties = validation.GetProperties(type);

            foreach (FieldInfo field in fields)
            {
                if (info.TryGetMember(field.Name, out CodeGenerateContainerInfo.MemberInfo member) && member.Active)
                {
                    if (CodeGenerateContainerEditorUtility.TryCreateField(compilation, field.Name, field.FieldType, false, out CodeGenerateContainerField containerField))
                    {
                        container.Fields.Add(containerField);
                    }
                }
            }

            foreach (PropertyInfo property in properties)
            {
                if (info.TryGetMember(property.Name, out CodeGenerateContainerInfo.MemberInfo member) && member.Active)
                {
                    if (CodeGenerateContainerEditorUtility.TryCreateField(compilation, property.Name, property.PropertyType, true, out CodeGenerateContainerField containerField))
                    {
                        container.Fields.Add(containerField);
                    }
                }
            }

            return container;
        }
    }
}
