namespace UGF.Code.Generate.Editor.Container.Asset
{
    /// <summary>
    /// Provides utilities to work with container external types in the editor.
    /// </summary>
    public static class CodeGenerateContainerAssetEditorUtility
    {
        /// <summary>
        /// Gets the default container external type validation.
        /// </summary>
        public static CodeGenerateContainerAssetValidation DefaultValidation { get; } = new CodeGenerateContainerAssetValidation();
    }
}
