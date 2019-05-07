using UnityEditor.Experimental.AssetImporters;

namespace UGF.Code.Generate.Editor.Container.External
{
    /// <summary>
    /// Represents base abstract asset importer for the container external type.
    /// </summary>
    public abstract class CodeGenerateContainerExternalAssetImporterBase : ScriptedImporter
    {
        public abstract void Save();
    }
}
