using System;
using UGF.Types.Editor;
using UGF.Types.Editor.IMGUI;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace UGF.Code.Generate.Editor.Container.External
{
    public abstract class CodeGenerateContainerExternalAssetImporterEditor : ScriptedImporterEditor
    {
        public override bool showImportedObject { get; } = false;
        protected override bool useAssetDrawPreview { get; } = false;

        protected CodeGenerateContainerExternalAssetImporterBase Importer { get; private set; }
        protected SerializedProperty InfoSerializedProperty { get; private set; }
        protected virtual ICodeGenerateContainerValidation Validation { get; } = new CodeGenerateContainerExternalValidation();

        private SerializedProperty m_propertyScript;
        private SerializedProperty m_propertyTypeName;
        private SerializedProperty m_propertyMembers;
        private TypesDropdown m_dropdown;
        private bool m_validType;
        private Styles m_styles;

        private sealed class Styles
        {
            public readonly GUIContent TypeLabelContent = new GUIContent("Type");
            public readonly GUIContent TypeDropdownNone = new GUIContent("None");
            public readonly GUIStyle Box = new GUIStyle("Box");
        }

        public override void OnEnable()
        {
            base.OnEnable();

            Importer = (CodeGenerateContainerExternalAssetImporterBase)target;
            InfoSerializedProperty = serializedObject.FindProperty("m_info");

            m_propertyScript = serializedObject.FindProperty("m_Script");
            m_propertyTypeName = InfoSerializedProperty.FindPropertyRelative("m_typeName");
            m_propertyMembers = InfoSerializedProperty.FindPropertyRelative("m_members");

            ValidateType(m_propertyTypeName.stringValue);
        }

        public override void OnInspectorGUI()
        {
            if (m_styles == null)
            {
                m_styles = new Styles();
            }

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(m_propertyScript);
            }

            OnDrawTypeSelection(m_propertyTypeName);
            OnDrawTypeInfo(m_propertyTypeName);
            OnDrawMembers(m_propertyMembers);

            ApplyRevertGUI();
        }

        protected virtual void OnDrawTypeSelection(SerializedProperty propertyTypeName)
        {
            Rect rect = EditorGUILayout.GetControlRect();
            Rect rectButton = EditorGUI.PrefixLabel(rect, m_styles.TypeLabelContent);

            Type type = Type.GetType(propertyTypeName.stringValue);
            GUIContent typeButtonContent = type != null ? new GUIContent(type.Name) : m_styles.TypeDropdownNone;

            if (EditorGUI.DropdownButton(rectButton, typeButtonContent, FocusType.Keyboard))
            {
                ShowDropdown(rectButton);
            }
        }

        protected virtual void OnDrawTypeInfo(SerializedProperty propertyTypeName)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Type Info", EditorStyles.boldLabel);

            if (m_validType)
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
        }

        protected virtual void OnDrawMembers(SerializedProperty propertyMembers)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Members", EditorStyles.boldLabel);

            using (new GUILayout.VerticalScope(m_styles.Box))
            {
                for (int i = 0; i < propertyMembers.arraySize; i++)
                {
                    OnDrawMember(propertyMembers, i);
                }

                if (propertyMembers.arraySize == 0)
                {
                    EditorGUILayout.LabelField("Empty");
                }

                OnDrawMembersBottomControls();
            }
        }

        protected virtual void OnDrawMember(SerializedProperty propertyMembers, int index)
        {
            SerializedProperty propertyMember = propertyMembers.GetArrayElementAtIndex(index);
            SerializedProperty propertyName = propertyMember.FindPropertyRelative("m_name");
            SerializedProperty propertyActive = propertyMember.FindPropertyRelative("m_active");

            propertyActive.boolValue = EditorGUILayout.Toggle(propertyName.stringValue, propertyActive.boolValue);
        }

        protected virtual void OnDrawMembersBottomControls()
        {
            EditorGUILayout.Space();

            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Select all"))
                {
                    SetAllMembersActive(true);
                }

                if (GUILayout.Button("Deselect all"))
                {
                    SetAllMembersActive(false);
                }

                GUILayout.FlexibleSpace();
            }
        }

        protected virtual void OnTypeChanged(Type type)
        {
            CodeGenerateContainerExternalInfo info = CodeGenerateContainerExternalEditorUtility.CreateInfo(type, Validation);

            m_propertyMembers.ClearArray();

            for (int i = 0; i < info.Members.Count; i++)
            {
                CodeGenerateContainerExternalMemberInfo member = info.Members[i];

                m_propertyMembers.InsertArrayElementAtIndex(m_propertyMembers.arraySize);

                OnTypeChangedSetupDefaultMemberInfo(m_propertyMembers, m_propertyMembers.GetArrayElementAtIndex(i), member, i);
            }
        }

        protected virtual void OnTypeChangedSetupDefaultMemberInfo(SerializedProperty propertyMembers, SerializedProperty propertyMember, CodeGenerateContainerExternalMemberInfo memberInfo, int index)
        {
            SerializedProperty propertyName = propertyMember.FindPropertyRelative("m_name");
            SerializedProperty propertyActive = propertyMember.FindPropertyRelative("m_active");

            propertyName.stringValue = memberInfo.Name;
            propertyActive.boolValue = true;
        }

        protected void SetAllMembersActive(bool state)
        {
            SetAllMembersActive(m_propertyMembers, state);
        }

        protected void SetAllMembersActive(SerializedProperty propertyMembers, bool state)
        {
            for (int i = 0; i < propertyMembers.arraySize; i++)
            {
                SerializedProperty propertyMember = propertyMembers.GetArrayElementAtIndex(i);
                SerializedProperty propertyActive = propertyMember.FindPropertyRelative("m_active");

                propertyActive.boolValue = state;
            }
        }

        protected override void Apply()
        {
            base.Apply();

            Importer.Save();
        }

        private void ShowDropdown(Rect rect)
        {
            if (m_dropdown == null)
            {
                m_dropdown = TypesEditorGUIUtility.GetTypesDropdown(OnDropdownValidateType);
                m_dropdown.Selected += OnDropdownTypeSelected;
            }

            m_dropdown.Show(rect);
        }

        private bool OnDropdownValidateType(Type type)
        {
            return Validation.Validate(type);
        }

        private void OnDropdownTypeSelected(Type type)
        {
            m_propertyTypeName.stringValue = type.AssemblyQualifiedName;

            OnTypeChanged(type);

            ValidateType(m_propertyTypeName.stringValue);
        }

        private void ValidateType(string typeName)
        {
            Type type = Type.GetType(typeName);

            m_validType = type != null && Validation.Validate(type);
        }
    }
}
