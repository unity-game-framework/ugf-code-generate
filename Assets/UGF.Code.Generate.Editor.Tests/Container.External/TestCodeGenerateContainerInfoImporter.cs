using UGF.Code.Generate.Editor.Container.Asset;
using UGF.Code.Generate.Editor.Container.Info;
using UnityEditor.Experimental.AssetImporters;

namespace UGF.Code.Generate.Editor.Tests.Container.External
{
    [ScriptedImporter(0, "test-external")]
    public class TestCodeGenerateContainerInfoImporter : CodeGenerateContainerAssetImporter<CodeGenerateContainerInfo>
    {
    }
}
