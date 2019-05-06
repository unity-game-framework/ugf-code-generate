using System.IO;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace UGF.Code.Generate.Editor.Container.External
{
    public abstract class CodeGenerateContainerExternalAssetImporter<TInfo> : CodeGenerateContainerExternalAssetImporterBase where TInfo : class, ICodeGenerateContainerExternalInfo, new()
    {
        [SerializeField] private TInfo m_info = new TInfo();

        public TInfo Info { get { return m_info; } }

        public override void OnImportAsset(AssetImportContext context)
        {
            string source = File.ReadAllText(context.assetPath);
            var asset = new TextAsset(source);

            JsonUtility.FromJsonOverwrite(source, m_info);

            context.AddObjectToAsset("main", asset);
            context.SetMainObject(asset);
        }

        public override void Save()
        {
            string source = JsonUtility.ToJson(m_info, true).Replace("\n", "\r\n");

            File.WriteAllText(assetPath, source);

            SaveAndReimport();
        }
    }
}
