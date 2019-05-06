using UGF.Code.Generate.Editor.Container.External;
using UnityEditor.Experimental.AssetImporters;

namespace UGF.Code.Generate.Editor.Tests.Container.External
{
    [ScriptedImporter(0, "test-external")]
    public class TestCodeGenerateContainerExternalAssetImporter : CodeGenerateContainerExternalAssetImporter<CodeGenerateContainerExternalInfo>
    {
    }
}
