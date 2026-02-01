using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SimpleFolderIcon.Editor
{
    public class IconDictionaryCreator : AssetPostprocessor
    {
        private const string AssetsPath = "FolderIcons/Icons";
        internal static Dictionary<string, Texture> IconDictionary;

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths
        )
        {
            if (
                !ContainsIconAsset(importedAssets)
                && !ContainsIconAsset(deletedAssets)
                && !ContainsIconAsset(movedAssets)
                && !ContainsIconAsset(movedFromAssetPaths)
            )
            {
                return;
            }

            BuildDictionary();
        }

        private static bool ContainsIconAsset(string[] assets)
        {
            foreach (string str in assets)
            {
                if (ReplaceSeparatorChar(Path.GetDirectoryName(str)) == "Assets/" + AssetsPath)
                {
                    return true;
                }
            }
            return false;
        }

        private static string ReplaceSeparatorChar(string path)
        {
            return path.Replace("\\", "/");
        }

        internal static void BuildDictionary()
        {
            var dictionary = new Dictionary<string, Texture>();

            var dir = new DirectoryInfo(Application.dataPath + "/" + AssetsPath);

            FileInfo[] infoSO = dir.GetFiles("*.asset");
            foreach (FileInfo f in infoSO)
            {
                var folderIconSO = (FolderObject)
                    AssetDatabase.LoadAssetAtPath(
                        $"Assets/FolderIcons/Icons/{f.Name}",
                        typeof(FolderObject)
                    );

                if (folderIconSO != null)
                {
                    var texture = (Texture)folderIconSO.texture;

                    foreach (string folderName in folderIconSO.names)
                    {
                        if (folderName != null)
                        {
                            dictionary.TryAdd(folderName, texture);
                        }
                    }
                }
            }

            IconDictionary = dictionary;
        }
    }
}
