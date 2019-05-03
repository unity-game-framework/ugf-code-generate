using System.IO;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace UGF.Code.Generate.Editor.Container.External
{
    public abstract class CodeGenerateContainerExternalAssetImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext context)
        {
            string source = File.ReadAllText(context.assetPath);
            var asset = new TextAsset(source);

            context.AddObjectToAsset("main", asset);
            context.SetMainObject(asset);
        }
    }
}
