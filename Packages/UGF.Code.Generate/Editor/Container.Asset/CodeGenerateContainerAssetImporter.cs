using UGF.AssetPipeline.Editor.Asset.Info;
using UGF.Code.Generate.Editor.Container.Info;

namespace UGF.Code.Generate.Editor.Container.Asset
{
    public abstract class CodeGenerateContainerAssetImporter<TInfo> : AssetInfoImporter<TInfo> where TInfo : CodeGenerateContainerInfo, new()
    {
    }
}
