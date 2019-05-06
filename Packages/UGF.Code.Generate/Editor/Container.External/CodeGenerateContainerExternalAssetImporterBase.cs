using UnityEditor.Experimental.AssetImporters;

namespace UGF.Code.Generate.Editor.Container.External
{
    public abstract class CodeGenerateContainerExternalAssetImporterBase : ScriptedImporter
    {
        public abstract void Save();
    }
}
