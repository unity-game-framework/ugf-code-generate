using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using UGF.Code.Analysis.Editor;
using UnityEngine;

namespace UGF.Code.Generate.Editor.Container.External
{
    public static class CodeGenerateContainerExternalEditorUtility
    {
        public static CodeGenerateContainerExternalValidation DefaultValidation { get; } = new CodeGenerateContainerExternalValidation();

        public static CodeGenerateContainer CreateContainer(ICodeGenerateContainerExternalInfo info, ICodeGenerateContainerValidation validation = null, CSharpCompilation compilation = null)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            if (validation == null) validation = DefaultValidation;
            if (compilation == null) compilation = CodeAnalysisEditorUtility.ProjectCompilation;

            if (!info.TryGetTargetType(out Type type))
            {
                throw new ArgumentException("The specified container external info not valid.", nameof(info));
            }

            var container = new CodeGenerateContainer(type.Name, type.IsValueType);
            IEnumerable<FieldInfo> fields = validation.GetFields(type);
            IEnumerable<PropertyInfo> properties = validation.GetProperties(type);

            foreach (FieldInfo field in fields)
            {
                if (CodeGenerateContainerEditorUtility.IsValidField(field))
                {
                    if (info.TryGetMember(field.Name, out CodeGenerateContainerExternalMemberInfo member) && member.Active)
                    {
                        if (CodeGenerateContainerEditorUtility.TryCreateField(compilation, field.Name, field.FieldType, false, out CodeGenerateContainerField containerField))
                        {
                            container.Fields.Add(containerField);
                        }
                    }
                }
            }

            foreach (PropertyInfo property in properties)
            {
                if (CodeGenerateContainerEditorUtility.IsValidProperty(property))
                {
                    if (info.TryGetMember(property.Name, out CodeGenerateContainerExternalMemberInfo member) && member.Active)
                    {
                        if (CodeGenerateContainerEditorUtility.TryCreateField(compilation, property.Name, property.PropertyType, true, out CodeGenerateContainerField containerField))
                        {
                            container.Fields.Add(containerField);
                        }
                    }
                }
            }

            return container;
        }

        public static bool TryGetInfoFromAssetPath<T>(string path, out T info) where T : class, ICodeGenerateContainerExternalInfo
        {
            if (TryGetInfoFromAssetPath(path, typeof(T), out ICodeGenerateContainerExternalInfo interfaceInfo))
            {
                info = interfaceInfo as T;

                return info != null;
            }

            info = default;
            return false;
        }

        public static bool TryGetInfoFromAssetPath(string path, Type assetType, out ICodeGenerateContainerExternalInfo info)
        {
            if (File.Exists(path))
            {
                string text = File.ReadAllText(path);

                info = JsonUtility.FromJson(text, assetType) as ICodeGenerateContainerExternalInfo;

                return info != null;
            }

            info = null;
            return false;
        }
    }
}
