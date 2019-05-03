using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;
using UGF.Code.Analysis.Editor;
using UnityEngine;

namespace UGF.Code.Generate.Editor.Container.External
{
    public static class CodeGenerateContainerExternalEditorUtility
    {
        public static void GenerateExternalContainers(Type infoType, IReadOnlyList<string> paths, ICollection<string> externals, CSharpCompilation compilation = null, SyntaxGenerator generator = null)
        {
            if (infoType == null) throw new ArgumentNullException(nameof(infoType));
            if (paths == null) throw new ArgumentNullException(nameof(paths));
            if (externals == null) throw new ArgumentNullException(nameof(externals));
            if (compilation == null) compilation = CodeAnalysisEditorUtility.ProjectCompilation;
            if (generator == null) generator = CodeAnalysisEditorUtility.Generator;

            for (int i = 0; i < paths.Count; i++)
            {
                string path = paths[i];

                if (TryGetInfoFromAssetPath(path, infoType, out ICodeGenerateContainerExternalInfo info))
                {
                    if (info.TryGetTargetType(out Type type))
                    {
                        CodeGenerateContainer container = CreateContainer(info, compilation);
                        SyntaxNode unit = CodeGenerateContainerEditorUtility.CreateUnit(generator, container, type.Namespace);

                        string source = unit.NormalizeWhitespace().ToFullString();

                        externals.Add(source);
                    }
                }
            }
        }

        public static CodeGenerateContainer CreateContainer(ICodeGenerateContainerExternalInfo info, CSharpCompilation compilation = null)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            if (compilation == null) compilation = CodeAnalysisEditorUtility.ProjectCompilation;

            if (!info.TryGetTargetType(out Type type))
            {
                throw new ArgumentException("The specified container external info not valid.", nameof(info));
            }

            var container = new CodeGenerateContainer(type.Name, type.IsValueType);
            FieldInfo[] fields = CodeGenerateContainerEditorUtility.GetFields(type);
            PropertyInfo[] properties = CodeGenerateContainerEditorUtility.GetProperties(type);

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];

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

            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];

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

        public static bool TryGetInfoFromAssetPath<T>(string path, out T info) where T : ICodeGenerateContainerExternalInfo
        {
            if (TryGetInfoFromAssetPath(path, typeof(T), out ICodeGenerateContainerExternalInfo interfaceInfo))
            {
                info = (T)interfaceInfo;
                return true;
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
