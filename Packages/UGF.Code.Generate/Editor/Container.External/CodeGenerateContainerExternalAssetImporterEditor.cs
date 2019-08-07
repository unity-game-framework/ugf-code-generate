using System;
using UGF.Types.Editor;
using UGF.Types.Editor.IMGUI;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace UGF.Code.Generate.Editor.Container.External
{
    /// <summary>
    /// Represents abstract implementation of the container external type information asset importer.
    /// </summary>
    public abstract class CodeGenerateContainerExternalAssetImporterEditor : ScriptedImporterEditor
    {
        public override bool showImportedObject { get; } = false;
        protected override bool useAssetDrawPreview { get; } = false;

        /// <summary>
        /// Gets the target importer of the editor.
        /// </summary>
        protected CodeGenerateContainerExternalAssetImporterBase Importer { get { return (CodeGenerateContainerExternalAssetImporterBase)serializedObject.targetObject; } }

        /// <summary>
        /// Gets the container external type information as serialized property.
        /// </summary>
        protected SerializedProperty InfoSerializedProperty { get; private set; }

        /// <summary>
        /// Gets the container type validation.
        /// </summary>
        protected virtual ICodeGenerateContainerValidation Validation { get; } = new CodeGenerateContainerExternalValidation();

        /// <summary>
        /// Gets value that determines whether target information has valid target type information.
        /// </summary>
        protected bool IsTypeValid { get; private set; }

        private SerializedProperty m_propertyScript;
        private SerializedProperty m_propertyTypeName;
        private SerializedProperty m_propertyMembers;
        private TypesDropdown m_dropdown;
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

            serializedObject.UpdateIfRequiredOrScript();

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(m_propertyScript);
            }

            OnDrawTypeSelection(m_propertyTypeName);
            OnDrawTypeInfo(m_propertyTypeName);
            OnDrawMembers(m_propertyMembers);

            serializedObject.ApplyModifiedProperties();

            ApplyRevertGUI();
        }

        /// <summary>
        /// Draws type selection GUI.
        /// </summary>
        /// <param name="propertyTypeName">The serialized property of the type name field.</param>
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

        /// <summary>
        /// Draws type information.
        /// </summary>
        /// <param name="propertyTypeName">The serialized property of the type name member.</param>
        protected virtual void OnDrawTypeInfo(SerializedProperty propertyTypeName)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Type Info", EditorStyles.boldLabel);

            if (IsTypeValid)
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

        /// <summary>
        /// Draw members from the container external information.
        /// </summary>
        /// <param name="propertyMembers">The serialized property of the members.</param>
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

        /// <summary>
        /// Draws member info from the container external information.
        /// </summary>
        /// <param name="propertyMembers">The serialized property of the members.</param>
        /// <param name="index">The index in the members collection.</param>
        protected virtual void OnDrawMember(SerializedProperty propertyMembers, int index)
        {
            SerializedProperty propertyMember = propertyMembers.GetArrayElementAtIndex(index);
            SerializedProperty propertyName = propertyMember.FindPropertyRelative("m_name");
            SerializedProperty propertyActive = propertyMember.FindPropertyRelative("m_active");

            propertyActive.boolValue = EditorGUILayout.Toggle(propertyName.stringValue, propertyActive.boolValue);
        }

        /// <summary>
        /// Draws members selection control at the bottom of the members collection.
        /// </summary>
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

        /// <summary>
        /// Occurs after type dropdown change type.
        /// </summary>
        /// <param name="type">The selected type.</param>
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

        /// <summary>
        /// Occurs after type dropdown change type and require to setup member information from the specified info.
        /// </summary>
        /// <param name="propertyMembers">The serialized property of the members.</param>
        /// <param name="propertyMember">The serialized property of the member, that require to setup.</param>
        /// <param name="memberInfo">The member information from the type.</param>
        /// <param name="index">The index of the member.</param>
        protected virtual void OnTypeChangedSetupDefaultMemberInfo(SerializedProperty propertyMembers, SerializedProperty propertyMember, CodeGenerateContainerExternalMemberInfo memberInfo, int index)
        {
            SerializedProperty propertyName = propertyMember.FindPropertyRelative("m_name");
            SerializedProperty propertyActive = propertyMember.FindPropertyRelative("m_active");

            propertyName.stringValue = memberInfo.Name;
            propertyActive.boolValue = true;
        }

        /// <summary>
        /// Sets active state to the all members.
        /// </summary>
        /// <param name="state">The state of the member.</param>
        protected void SetAllMembersActive(bool state)
        {
            SetAllMembersActive(m_propertyMembers, state);
        }

        /// <summary>
        /// Sets active state to the all members from the specified serialized property.
        /// </summary>
        /// <param name="propertyMembers">The serialized property of the members to change.</param>
        /// <param name="state">The state of the member.</param>
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

            serializedObject.ApplyModifiedProperties();
        }

        private void ValidateType(string typeName)
        {
            Type type = Type.GetType(typeName);

            IsTypeValid = type != null && Validation.Validate(type);
        }
    }
}
