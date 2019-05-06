using UnityEditor;
using UnityEngine;

namespace UGF.Code.Generate.Editor.Tests.Container.External
{
    public static class TestCodeGenerateContainerExternalEditorMenu
    {
        [MenuItem("Assets/Create/Test/CodeGenerateContainerExternalInfo", false, 0)]
        private static void Menu()
        {
            Texture2D icon = AssetPreview.GetMiniTypeThumbnail(typeof(TextAsset));
            string extension = "test-external";

            ProjectWindowUtil.CreateAssetWithContent($"New External Type Info.{extension}", "{}", icon);
        }
    }
}
