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

        private TypesDropdown m_dropdown;
        private Styles m_styles;
        private bool m_typeValid;

        private sealed class Styles
        {
            public GUIContent TypeLabelContent { get; } = new GUIContent("Type");
            public GUIContent TypeNoneContent { get; } = new GUIContent("None");
            public GUIStyle Box { get; } = "Box";
        }

        public override void OnEnable()
        {
            base.OnEnable();

            SerializedProperty propertyTypeName = extraDataSerializedObject.FindProperty("m_info.m_typeName");

            ValidateType(propertyTypeName.stringValue);

            m_dropdown = new TypesDropdown(OnDropdownTypeCollector);
            m_dropdown.Selected += OnDropdownTypeSelected;
        }

        protected override void OnDrawInfo()
        {
            if (m_styles == null)
            {
                m_styles = new Styles();
            }

            extraDataSerializedObject.UpdateIfRequiredOrScript();

            SerializedProperty propertyTypeName = extraDataSerializedObject.FindProperty("m_info.m_typeName");
            SerializedProperty propertyMembers = extraDataSerializedObject.FindProperty("m_info.m_members");

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(InfoName, EditorStyles.boldLabel);

            Rect rect = EditorGUILayout.GetControlRect(true);
            Rect rectButton = EditorGUI.PrefixLabel(rect, m_styles.TypeLabelContent);

            var type = Type.GetType(propertyTypeName.stringValue);
            GUIContent typeButtonContent = type != null ? new GUIContent(type.Name) : m_styles.TypeNoneContent;

            if (EditorGUI.DropdownButton(rectButton, typeButtonContent, FocusType.Keyboard))
            {
                m_dropdown.Show(rectButton);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Type Info", EditorStyles.boldLabel);

            if (m_typeValid)
            {
                EditorGUILayout.HelpBox(propertyTypeName.stringValue, MessageType.None);
            }
            else
            {
                if (string.IsNullOrEmpty(propertyTypeName.stringValue))
                {
                    EditorGUILayout.HelpBox("Type not specified.", MessageType.None);
                }
                else
                {
                    EditorGUILayout.HelpBox($"Invalid type: `{propertyTypeName.stringValue}`.", MessageType.Warning);
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Members", EditorStyles.boldLabel);

            using (new GUILayout.VerticalScope(m_styles.Box))
            {
                for (int i = 0; i < propertyMembers.arraySize; i++)
                {
                    SerializedProperty propertyMember = propertyMembers.GetArrayElementAtIndex(i);
                    SerializedProperty propertyActive = propertyMember.FindPropertyRelative("m_active");
                    SerializedProperty propertyName = propertyMember.FindPropertyRelative("m_name");

                    propertyActive.boolValue = EditorGUILayout.Toggle(propertyName.stringValue, propertyActive.boolValue);
                }

                if (propertyMembers.arraySize == 0)
                {
                    EditorGUILayout.LabelField("Empty");
                }

                EditorGUILayout.Space();

                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Select all"))
                    {
                        SetAllMembersActive(propertyMembers, true);
                    }

                    if (GUILayout.Button("Deselect all"))
                    {
                        SetAllMembersActive(propertyMembers, false);
                    }

                    GUILayout.FlexibleSpace();
                }
            }

            extraDataSerializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnTypeChanged(Type type = null)
        {
            SerializedProperty propertyMembers = extraDataSerializedObject.FindProperty("m_info.m_members");

            propertyMembers.ClearArray();

            if (type != null)
            {
                CodeGenerateContainerInfo info = CodeGenerateContainerInfoEditorUtility.CreateInfo(type, GetContainerValidation());

                for (int i = 0; i < info.Members.Count; i++)
                {
                    CodeGenerateContainerInfo.MemberInfo memberInfo = info.Members[i];

                    propertyMembers.InsertArrayElementAtIndex(propertyMembers.arraySize);

                    SerializedProperty propertyMember = propertyMembers.GetArrayElementAtIndex(i);
                    SerializedProperty propertyActive = propertyMember.FindPropertyRelative("m_active");
                    SerializedProperty propertyName = propertyMember.FindPropertyRelative("m_name");

                    propertyActive.boolValue = true;
                    propertyName.stringValue = memberInfo.Name;
                }
            }

            propertyMembers.serializedObject.ApplyModifiedProperties();
        }

        protected virtual ICodeGenerateContainerValidation GetContainerValidation()
        {
            return CodeGenerateContainerAssetEditorUtility.DefaultValidation;
        }

        private IEnumerable<Type> OnDropdownTypeCollector()
        {
            ICodeGenerateContainerValidation validation = GetContainerValidation();

            foreach (Type type in TypeCache.GetTypesDerivedFrom<object>())
            {
                if (validation.Validate(type))
                {
                    yield return type;
                }
            }
        }

        private void OnDropdownTypeSelected(Type type)
        {
            SerializedProperty propertyTypeName = extraDataSerializedObject.FindProperty("m_info.m_typeName");

            propertyTypeName.stringValue = type != null ? type.AssemblyQualifiedName : string.Empty;
            propertyTypeName.serializedObject.ApplyModifiedProperties();

            OnTypeChanged(type);
            ValidateType(propertyTypeName.stringValue);
        }

        private void ValidateType(string typeName)
        {
            var type = Type.GetType(typeName);

            m_typeValid = type != null && GetContainerValidation().Validate(type);
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
    }
}
