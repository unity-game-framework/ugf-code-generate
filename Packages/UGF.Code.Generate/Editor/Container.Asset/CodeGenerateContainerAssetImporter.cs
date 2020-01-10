using UGF.AssetPipeline.Editor.Asset.Info;
using UGF.Code.Generate.Editor.Container.Info;
using UnityEditor;

namespace UGF.Code.Generate.Editor.Container.Asset
{
    public abstract class CodeGenerateContainerAssetImporter<TInfo> : AssetInfoTextImporter<TInfo> where TInfo : CodeGenerateContainerInfo, new()
    {
        protected override string OnCreateTextAsset(TInfo info)
        {
            return EditorJsonUtility.ToJson(info, true);
        }
    }
}
