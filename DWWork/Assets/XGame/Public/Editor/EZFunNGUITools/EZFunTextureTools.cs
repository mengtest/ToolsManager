using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// 收集图片到 Resources/EZFunUI/Altas/textureList.prefab中去
/// </summary>
public class EZFunTextureTools
{
    [MenuItem("EZFun/UI/生成贴图列表")]
    public static void HandleTexturesToPrefab()
    {
        var gb = new GameObject("textureList");
        UITextureList texture = gb.AddComponent<UITextureList>();

        var dir = new DirectoryInfo(Application.dataPath + "/XGame/Resources/EZFunUI/Texture/");

        var files = dir.GetFiles("*.*", SearchOption.AllDirectories);

        List<string> textureNames = new List<string>();
        List<string> texturePaths = new List<string>();
        for (int i = 0; i < files.Length; i++)
        {
            if (!files[i].Name.EndsWith("png", System.StringComparison.OrdinalIgnoreCase) 
                && !files[i].Name.EndsWith("jpg", System.StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }
            textureNames.Add(files[i].Name.Substring(0, files[i].Name.IndexOf('.')));

            var fullPath = files[i].FullName.Replace("\\", "/");
            Debug.LogError(fullPath);
            texturePaths.Add(fullPath.Substring(fullPath.IndexOf("Assets")));
        }

        texture.m_sprites = textureNames;
        texture.m_spritePath = texturePaths;

        List<string> atlasNames = new List<string>();
        List<string> atlasPaths = new List<string>();
        var atlasDir = new DirectoryInfo(Application.dataPath + "/XGame/Resources/EZFunUI/Altas/Release/");
        var atlasFiles = atlasDir.GetFiles("*.prefab", SearchOption.AllDirectories);

        for (int i = 0; i < atlasFiles.Length; i++)
        {
            atlasNames.Add(atlasFiles[i].Name.Substring(0, atlasFiles[i].Name.IndexOf('.')));

            var fullPath = atlasFiles[i].FullName.Replace("\\", "/");
            atlasPaths.Add(fullPath.Substring(fullPath.IndexOf("Assets")));
        }
        texture.m_atlas = atlasNames;
        texture.m_atlasPath = atlasPaths;

        PrefabUtility.CreatePrefab("Assets/XGame/Resources/EZFunUI/Altas/textureList.prefab", gb);
        GameObject.DestroyImmediate(gb);
    }

    [MenuItem("EZFun/UI/处理地形Mask贴图")]
    public static void HandleTerrainMaskTexture()
    {
        string[] paths = Directory.GetFiles(Application.dataPath + "/T4MOBJ/Terrains/Texture/", "*.*", SearchOption.AllDirectories);
        foreach (string path in paths)
        {
            if (!string.IsNullOrEmpty(path) && TextureAlphaEditor.IsTextureFile(path))   //full name    
            {
                string assetRelativePath = TextureAlphaEditor.GetRelativeAssetPath(path);
                TextureImporter ti = null;
                try
                {
                    ti = (TextureImporter)TextureImporter.GetAtPath(assetRelativePath);
                }
                catch
                {
                    Debug.LogError("Load Texture failed: " + assetRelativePath);
                    continue;
                }

                ti.isReadable = false;
                AssetDatabase.ImportAsset(assetRelativePath);
            }
        }
        AssetDatabase.Refresh();    //Refresh to ensure new generated RBA and Alpha textures shown in Unity as well as the meta file  
        Debug.Log("Finish Departing.");
    }
    /*
    [MenuItem("NGUI/Texture/ReplaceTextureToPrefab")]
    public static void ReplaceTexture()
    {
        DirectoryInfo windows = new DirectoryInfo(Application.dataPath + "/XGame/Resources/EZFunUI/Window");

        DirectoryInfo items = new DirectoryInfo(Application.dataPath + "/XGame/Resources/EZFunUI/Item");

        var files = items.GetFiles("*.prefab", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            ReplaceAtlas(files[i].FullName);
        }

        files = windows.GetFiles("*.prefab", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            ReplaceAtlas(files[i].FullName);
        }
    }

    private static void ReplaceAtlas(string filePath)
    {
        filePath = filePath.Replace("\\", "/");
        var prefabPath = filePath.Substring(filePath.IndexOf("Assets"));
        filePath = filePath.Substring(filePath.IndexOf("EZFunUI"));
        string parefabpath = filePath.Substring(0,
            filePath.LastIndexOf("."));
        var srcGb = Resources.Load<GameObject>(parefabpath);
        var Gb = GameObject.Instantiate(srcGb);
        try
        {
            var sprites = Gb.GetComponentsInChildren<UISprite>(true);
            for (int i = 0; i < sprites.Length; i++)
            {
                if (sprites[i].atlas != null && sprites[i].atlas.name.Contains("item"))
                {
                    var texture = EZFunTools.GetOrAddComponent<UITexture>(sprites[i].gameObject);
                    var sprite = sprites[i];
                    texture.width = sprite.width;
                    texture.height = sprite.height;
                    texture.depth = sprite.depth;
                    texture.color = sprite.color;
                   /// texture.mainTexture = CUIRessLoadSys.Instance.GetUITexture(sprite.spriteName);
                    GameObject.Destroy(sprite);
                }
            }
            PrefabUtility.CreatePrefab(prefabPath, Gb, ReplacePrefabOptions.ConnectToPrefab);
            GameObject.Destroy(Gb);
        }
        finally
        {
            GameObject.Destroy(Gb);
        }
    }
    */
}
