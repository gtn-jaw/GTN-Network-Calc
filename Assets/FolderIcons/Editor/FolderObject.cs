using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "FolderObject", menuName = "FolderScriptableObject", order = 1)]
public class FolderObject : ScriptableObject
{
    [Header("Texture")]
    [Tooltip("Folder Icon")]
    public Texture2D texture;

    [Space(8)]
    [Header("List of names")]
    [Tooltip("List of names you wish for the icon to apply to")]
    public List<string> names = new();

    private void OnValidate()
    {
        if (texture != null)
        {
            if (names.Count <= 0)
            {
                names.Add(texture.name);
            }
            else
            {
                names[0] = texture.name;
            }
        }
        else
        {
            if (AssetDatabase.FindAssets(name + " t:texture2D").Length > 0)
            {
                texture = (Texture2D)
                    AssetDatabase.LoadAssetAtPath(
                        AssetDatabase.GUIDToAssetPath(
                            AssetDatabase.FindAssets(
                                name + " t:texture2D",
                                new[]
                                {
                                    "Assets/FolderIcons",
                                    "Assets/FolderIcons/Icons",
                                    "Assets/FolderIcons/Textures",
                                }
                            )[0]
                        ),
                        typeof(Texture2D)
                    );
                if (names.Count <= 0)
                {
                    names.Add(texture.name);
                }
                else
                {
                    names[0] = texture.name;
                }
            }
            else
            {
                Debug.LogWarning(
                    @"Error: Couldn't find Texture2D sharing the name """
                        + name
                        + @""", please assign one manually."
                );
            }
        }
    }

    private void OnEnable()
    {
        if (texture != null)
        {
            if (names.Count <= 0)
            {
                names.Add(texture.name);
            }
            else
            {
                names[0] = texture.name;
            }
        }
        else
        {
            if (AssetDatabase.FindAssets(name + " t:texture2D").Length > 0)
            {
                texture = (Texture2D)
                    AssetDatabase.LoadAssetAtPath(
                        AssetDatabase.GUIDToAssetPath(
                            AssetDatabase.FindAssets(
                                name + " t:texture2D",
                                new[]
                                {
                                    "Assets/FolderIcons",
                                    "Assets/FolderIcons/Icons",
                                    "Assets/FolderIcons/Textures",
                                }
                            )[0]
                        ),
                        typeof(Texture2D)
                    );
                if (names.Count <= 0)
                {
                    names.Add(texture.name);
                }
                else
                {
                    names[0] = texture.name;
                }
            }
            else
            {
                Debug.LogWarning(
                    @"Error: Couldn't find Texture2D sharing the name """
                        + name
                        + @""", please assign one manually."
                );
            }
        }
    }
}

[CustomEditor(typeof(FolderObject))]
public class FolderObjectEditor : UnityEditor.Editor
{
    public static void CreateAsset<FolderObject>()
        where FolderObject : ScriptableObject
    {
        FolderObject asset = ScriptableObject.CreateInstance<FolderObject>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);

        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(
                Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)),
                ""
            );
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(
            path + "/New " + typeof(FolderObject).ToString() + ".asset"
        );

        AssetDatabase.CreateAsset(asset, assetPathAndName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    public override Texture2D RenderStaticPreview(
        string assetPath,
        Object[] subAssets,
        int width,
        int height
    )
    {
        FolderObject FolderObject = (FolderObject)target;

        if (FolderObject == null || FolderObject.texture == null)
            return null;

        // FolderObject.texture must be a supported format: ARGB32, RGBA32, RGB24,
        // Alpha8 or one of float formats
        Texture2D tex = new Texture2D(width, height);
        EditorUtility.CopySerialized(FolderObject.texture, tex);

        return tex;
    }
}
