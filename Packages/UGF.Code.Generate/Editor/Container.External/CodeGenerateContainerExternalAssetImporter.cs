using System.IO;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace UGF.Code.Generate.Editor.Container.External
{
    /// <summary>
    /// Represents abstract implementation of the asset importer for the container external type information.
    /// </summary>
    public abstract class CodeGenerateContainerExternalAssetImporter<TInfo> : CodeGenerateContainerExternalAssetImporterBase where TInfo : class, ICodeGenerateContainerExternalInfo, new()
    {
        [SerializeField] private TInfo m_info = new TInfo();

        /// <summary>
        /// Gets importer information.
        /// </summary>
        public TInfo Info { get { return m_info; } }

        public override void OnImportAsset(AssetImportContext context)
        {
            string source = File.ReadAllText(context.assetPath);
            var asset = new TextAsset(source);

            EditorJsonUtility.FromJsonOverwrite(source, m_info);

            context.AddObjectToAsset("main", asset);
            context.SetMainObject(asset);
        }

        /// <summary>
        /// Saves information as Json to the asset path of the importer.
        /// </summary>
        public override void Save()
        {
            string source = EditorJsonUtility.ToJson(m_info, true);

            File.WriteAllText(assetPath, source);

            SaveAndReimport();
        }
    }
}
