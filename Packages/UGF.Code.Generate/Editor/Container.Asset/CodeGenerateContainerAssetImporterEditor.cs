using System;
using System.Collections.Generic;
using UGF.AssetPipeline.Editor.Asset.Info;
using UGF.Code.Generate.Editor.Container.Info;
using UGF.EditorTools.Editor.IMGUI.Types;
using UnityEditor;
using UnityEngine;

namespace UGF.Code.Generate.Editor.Container.Asset
{
    [CustomEditor(typeof(CodeGenerateContainerAssetImporter<>), true)]
    public class CodeGenerateContainerAssetImporterEditor : AssetInfoImporterEditor
    {
        public override bool showImportedObject { get; } = false;

        private SerializedProperty m_propertyTypeName;
        private SerializedProperty m_propertyMembers;
        private TypesDropdownDrawer m_dropdown;
        private Styles m_styles;
        private bool m_typeValid;

        private sealed class Styles
        {
            public GUIContent TypeLabelContent { get; } = new GUIContent("Type");
            public GUIStyle Box { get; } = "Box";
        }

        public override void OnEnable()
        {
            base.OnEnable();

            m_propertyTypeName = extraDataSerializedObject.FindProperty("m_info.m_typeName");
            m_propertyMembers = extraDataSerializedObject.FindProperty("m_info.m_members");

            ValidateType(m_propertyTypeName.stringValue);

            m_dropdown = new TypesDropdownDrawer(m_propertyTypeName, OnDropdownTypeCollector);
            m_dropdown.Selected += OnDropdownTypeSelected;
        }

        protected override void OnDrawInfo()
        {
            if (m_styles == null)
            {
                m_styles = new Styles();
            }

            extraDataSerializedObject.UpdateIfRequiredOrScript();

            m_propertyTypeName = extraDataSerializedObject.FindProperty("m_info.m_typeName");
            m_propertyMembers = extraDataSerializedObject.FindProperty("m_info.m_members");

            // m_dropdown.DrawGUILayout(m_styles.TypeLabelContent);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Type Info", EditorStyles.boldLabel);

            if (m_typeValid)
            {
                EditorGUILayout.HelpBox(m_propertyTypeName.stringValue, MessageType.None);
            }
            else
            {
                if (string.IsNullOrEmpty(m_propertyTypeName.stringValue))
                {
                    EditorGUILayout.HelpBox("Type not specified.", MessageType.None);
                }
                else
                {
                    EditorGUILayout.HelpBox($"Invalid type: `{m_propertyTypeName.stringValue}`.", MessageType.Warning);
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Members", EditorStyles.boldLabel);

            using (new GUILayout.VerticalScope(m_styles.Box))
            {
                for (int i = 0; i < m_propertyMembers.arraySize; i++)
                {
                    SerializedProperty propertyMember = m_propertyMembers.GetArrayElementAtIndex(i);
                    SerializedProperty propertyActive = propertyMember.FindPropertyRelative("m_active");
                    SerializedProperty propertyName = propertyMember.FindPropertyRelative("m_name");

                    propertyActive.boolValue = EditorGUILayout.Toggle(propertyName.stringValue, propertyActive.boolValue);
                }

                if (m_propertyMembers.arraySize == 0)
                {
                    EditorGUILayout.LabelField("Empty");
                }

                EditorGUILayout.Space();

                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Select all"))
                    {
                        SetAllMembersActive(m_propertyMembers, true);
                    }

                    if (GUILayout.Button("Deselect all"))
                    {
                        SetAllMembersActive(m_propertyMembers, false);
                    }

                    GUILayout.FlexibleSpace();
                }
            }

            extraDataSerializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnTypeChanged(Type type)
        {
            CodeGenerateContainerInfo info = CodeGenerateContainerInfoEditorUtility.CreateInfo(type, GetContainerValidation());

            m_propertyMembers.ClearArray();

            for (int i = 0; i < info.Members.Count; i++)
            {
                CodeGenerateContainerInfo.MemberInfo memberInfo = info.Members[i];

                m_propertyMembers.InsertArrayElementAtIndex(m_propertyMembers.arraySize);

                SerializedProperty propertyMember = m_propertyMembers.GetArrayElementAtIndex(i);
                SerializedProperty propertyActive = propertyMember.FindPropertyRelative("m_active");
                SerializedProperty propertyName = propertyMember.FindPropertyRelative("m_name");

                propertyActive.boolValue = true;
                propertyName.stringValue = memberInfo.Name;
            }

            m_propertyMembers.serializedObject.ApplyModifiedProperties();
        }

        protected virtual ICodeGenerateContainerValidation GetContainerValidation()
        {
            return CodeGenerateContainerAssetEditorUtility.DefaultValidation;
        }

        private static void SetAllMembersActive(SerializedProperty propertyMembers, bool state)
        {
            for (int i = 0; i < propertyMembers.arraySize; i++)
            {
                SerializedProperty propertyMember = propertyMembers.GetArrayElementAtIndex(i);
                SerializedProperty propertyActive = propertyMember.FindPropertyRelative("m_active");

                propertyActive.boolValue = state;
            }

            propertyMembers.serializedObject.ApplyModifiedProperties();
        }

        private IEnumerable<Type> OnDropdownTypeCollector()
        {
            foreach (Type type in TypeCache.GetTypesDerivedFrom<object>())
            {
                if (GetContainerValidation().Validate(type))
                {
                    yield return type;
                }
            }
        }

        private void OnDropdownTypeSelected(Type type)
        {
            if (type == null)
            {
                m_typeValid = false;
                m_propertyTypeName.stringValue = string.Empty;
                m_propertyTypeName.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                m_propertyTypeName.stringValue = type.AssemblyQualifiedName;

                OnTypeChanged(type);

                ValidateType(m_propertyTypeName.stringValue);

                m_propertyTypeName.serializedObject.ApplyModifiedProperties();
            }
        }

        private void ValidateType(string typeName)
        {
            var type = Type.GetType(typeName);

            m_typeValid = type != null && GetContainerValidation().Validate(type);
        }
    }
}
