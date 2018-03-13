using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class TextureAlphaEditor
{

    //private static string defaultWhiteTexPath_relative = "Assets/Default_Alpha.png";
    //private static Texture2D defaultWhiteTex = null;

    [MenuItem("EZFun/UI/分离图集alpha通道")]
    public static void SeperateAllTexturesRGBandAlphaChannel()
    {
        Debug.Log("Start Departing.");
        if (!GetDefaultWhiteTexture())
        {
            return;
        }
        string[] paths = Directory.GetFiles(Application.dataPath + "/XGame/Data/UI/AltasImg/Release/", "*.*", SearchOption.AllDirectories);
        // string[] paths = Directory.GetFiles(Application.dataPath + "/XGame/Resources/EZFunUI/Texture/BgImg/map/", "*.*", SearchOption.AllDirectories);
        foreach (string path in paths)
        {
            if (!string.IsNullOrEmpty(path) && IsTextureFile(path) && !IsTextureConverted(path))   //full name    
            {
                SeperateRGBAandlphaChannel(path);
            }
        }
        AssetDatabase.Refresh();    //Refresh to ensure new generated RBA and Alpha textures shown in Unity as well as the meta file  
        Debug.Log("Finish Departing.");
    }


    [MenuItem("Assets/分离指定图集alpha通道")]
    public static void SeperateRGBandAlphaChannel()
    {
        if (!GetDefaultWhiteTexture())
        {
            return;
        }
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (!string.IsNullOrEmpty(path) && IsTextureFile(path) && !IsTextureConverted(path))   //full name    
        {
            SeperateRGBAandlphaChannel(path);
        }
        AssetDatabase.Refresh();
        Debug.Log("Finish Departing.");
    }

    /// <summary>
    /// 生成的pre要手动apply一下 不然运行会丢关联 感觉是unity的锅
    /// </summary>
    [MenuItem("EZFun/UI/分离map alpha通道")]
    public static void SeperateMapTexturesRGBandAlphaChannel()
    {
        Debug.Log("Start Departing.");
        NGUITools.mGetTextureCB = (name) =>
        {
            return Resources.Load("/XGame/Resources/EZFunUI/Texture/BgImg/map/mapTexture/" + name + ".png", typeof(UnityEngine.Texture)) as UnityEngine.Texture;
        };
        if (!GetDefaultWhiteTexture())
        {
            return;
        }
        string[] paths = Directory.GetFiles(Application.dataPath + "/XGame/Resources/EZFunUI/Texture/BgImg/map/mapTexture", "*.*", SearchOption.AllDirectories);
        foreach (string path in paths)
        {
            if (!string.IsNullOrEmpty(path) && IsTextureFile(path) && !IsTextureConverted(path))   //full name    
            {
                SeperateMapRGBAandlphaChannel(path);
            }
        }
        AssetDatabase.Refresh();    //Refresh to ensure new generated RBA and Alpha textures shown in Unity as well as the meta file  
        Debug.Log("Finish Departing.");
    }

    #region process texture
    static void ApplyPrefab(string _texPath)
    {
        string assetRelativePath = GetRelativeAssetPath(_texPath);
        string materialPath = System.IO.Path.ChangeExtension(assetRelativePath, "mat");
        Material mat = AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material)) as Material;
        string name = System.IO.Path.GetFileNameWithoutExtension(_texPath);
        string prefabName = "NO_USE_ref_" + name;
        string path = "Assets/XGame/Data/UI/AltasRef/" + prefabName + ".prefab";
        GameObject replace = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
        UIAtlas atlas = null;
        if (replace == null)
        {
            return;
            //replace = new GameObject(prefabName);
            //atlas = replace.AddComponent<UIAtlas>();
            //UnityEditor.PrefabUtility.CreatePrefab(path, replace);
            //AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            //replace = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
        }
        atlas = replace.GetComponent<UIAtlas>();
        atlas.spriteMaterial = mat;
        AssetDatabase.SaveAssets();
    }

    static void ApplyMaterial(string _texPath)
    {
        string assetRelativePath = GetRelativeAssetPath(_texPath);
        string materialPath = System.IO.Path.ChangeExtension(assetRelativePath, "mat");
        string rgbPath = GetRelativeAssetPath(GetRGBTexPath(_texPath));
        string alphaPath = GetRelativeAssetPath(GetAlphaTexPath(_texPath));

        // If the material doesn't exist, create it
        //if (mat == null)
        Material mat = new Material(Shader.Find("Unlit/Transparent Colored AlphaChannel(R(X) G(Y) B(Z))"));

        // Save the material
        AssetDatabase.CreateAsset(mat, materialPath);
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        // Load the material so it's usable
        mat = AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material)) as Material;
        Texture2D rgbTex = AssetDatabase.LoadAssetAtPath(rgbPath, typeof(Texture2D)) as Texture2D;
        Texture2D alphaTex = AssetDatabase.LoadAssetAtPath(alphaPath, typeof(Texture2D)) as Texture2D;
        mat.SetTexture("_MainTex", rgbTex);
        mat.SetTexture("_AlphaTex", alphaTex);

        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        AssetDatabase.SaveAssets();
    }

    static void SeperateRGBAandlphaChannel(string _texPath)
    {
        string assetRelativePath = GetRelativeAssetPath(_texPath);
        SetTextureReadableEx(assetRelativePath);    //set readable flag and set textureFormat TrueColor  
        Texture2D sourcetex = AssetDatabase.LoadAssetAtPath(assetRelativePath, typeof(Texture2D)) as Texture2D;  //not just the textures under Resources file    
        if (!sourcetex)
        {
            Debug.LogError("Load Texture Failed : " + assetRelativePath);
            return;
        }

        TextureImporter ti = null;
        try
        {
            ti = (TextureImporter)TextureImporter.GetAtPath(assetRelativePath);
        }
        catch
        {
            Debug.LogError("Load Texture failed: " + assetRelativePath);
            return;
        }
        if (ti == null)
        {
            return;
        }

        Texture2D rgbTex = new Texture2D(sourcetex.width, sourcetex.height, TextureFormat.RGB24, false);
        rgbTex.SetPixels(sourcetex.GetPixels());
        rgbTex.Apply();

        Color[] colors = sourcetex.GetPixels();
        Color[] colorsAlpha = new Color[colors.Length];

        for (int i = 0; i < colors.Length; ++i)
        {
            colorsAlpha[i].r = colors[i].a;
            colorsAlpha[i].g = colors[i].a;
            colorsAlpha[i].b = colors[i].a;
            colorsAlpha[i].a = colors[i].a;
        }
        Texture2D alphaTex = new Texture2D(sourcetex.width, sourcetex.height, TextureFormat.RGB24, false);
        alphaTex.SetPixels(colorsAlpha);
        alphaTex.Apply();
        byte[] alphabytes = alphaTex.EncodeToPNG();
        string alphaTexRelativePath = GetAlphaTexPath(_texPath);
        byte[] bytes = rgbTex.EncodeToPNG();

        File.WriteAllBytes(GetRGBTexPath(assetRelativePath), bytes);
        File.WriteAllBytes(alphaTexRelativePath, alphabytes);
        ReImportAsset(GetRelativeAssetPath(alphaTexRelativePath), sourcetex.width, sourcetex.height);
        ReImportAsset(GetRGBTexPath(assetRelativePath), sourcetex.width, sourcetex.height);

        ApplyMaterial(_texPath);
        ApplyPrefab(_texPath);
        //Object.DestroyImmediate(rgbTex);
        //Object.DestroyImmediate(alphaTex);
    }

    //地图用
    static void SeperateMapRGBAandlphaChannel(string _texPath)
    {
        string assetRelativePath = GetRelativeAssetPath(_texPath);
        SetTextureReadableEx(assetRelativePath);    //set readable flag and set textureFormat TrueColor  
        Texture2D sourcetex = AssetDatabase.LoadAssetAtPath(assetRelativePath, typeof(Texture2D)) as Texture2D;  //not just the textures under Resources file    
        if (!sourcetex)
        {
            Debug.LogError("Load Texture Failed : " + assetRelativePath);
            return;
        }
        TextureImporter ti = null;
        try
        {
            ti = (TextureImporter)TextureImporter.GetAtPath(assetRelativePath);
        }
        catch
        {
            Debug.LogError("Load Texture failed: " + assetRelativePath);
            return;
        }
        if (ti == null)
        {
            return;
        }

        Texture2D rgbTex = new Texture2D(sourcetex.width, sourcetex.height, TextureFormat.RGB24, false);
        rgbTex.SetPixels(sourcetex.GetPixels());
        rgbTex.Apply();

        Color[] colors = sourcetex.GetPixels();
        Color[] colorsAlpha = new Color[colors.Length];

        for (int i = 0; i < colors.Length; ++i)
        {
            colorsAlpha[i].r = colors[i].a;
            colorsAlpha[i].g = colors[i].a;
            colorsAlpha[i].b = colors[i].a;
            colorsAlpha[i].a = colors[i].a;
        }
        Texture2D alphaTex = new Texture2D(sourcetex.width, sourcetex.height, TextureFormat.RGB24, false);
        alphaTex.SetPixels(colorsAlpha);
        alphaTex.Apply();
        byte[] alphabytes = alphaTex.EncodeToPNG();
        string alphaTexRelativePath = GetAlphaTexPath(_texPath);
        byte[] bytes = rgbTex.EncodeToPNG();

        File.WriteAllBytes(GetRGBTexPath(assetRelativePath), bytes);
        File.WriteAllBytes(alphaTexRelativePath, alphabytes);
        ReImportAsset(GetRelativeAssetPath(alphaTexRelativePath), sourcetex.width, sourcetex.height);
        ReImportAsset(GetRGBTexPath(assetRelativePath), sourcetex.width, sourcetex.height);

        ApplyMaterial(_texPath);
        ApplyMapPre(_texPath, assetRelativePath);
    }

    static void ApplyMapPre(string _texPath,string assetRelativePath)
    {
        string materialPath = System.IO.Path.ChangeExtension(assetRelativePath, "mat");
        Material mat = AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material)) as Material;
        string name = System.IO.Path.GetFileNameWithoutExtension(_texPath);
        string path = "Assets/XGame/Resources/EZFunUI/Texture/BgImg/map/" + name + ".prefab";
        GameObject replace = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
        UITexture atlas = null;
        if (replace == null)
        {
            replace = new GameObject(name);
            atlas = replace.AddComponent<UITexture>();
            UnityEditor.PrefabUtility.CreatePrefab(path, replace);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            replace = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
        }
        string rgbPath = GetRelativeAssetPath(GetRGBTexPath(_texPath));
        Texture2D rgbTex = AssetDatabase.LoadAssetAtPath(rgbPath, typeof(Texture2D)) as Texture2D;

        atlas = replace.GetComponent<UITexture>();
        atlas.mainTexture = rgbTex;
        atlas.material = mat;
        AssetDatabase.SaveAssets();
    }

    static void ReImportAsset(string path, int width, int height)
    {
        try
        {
            AssetDatabase.ImportAsset(path);
        }
        catch
        {
            Debug.LogError("Import Texture failed: " + path);
            return;
        }

        TextureImporter importer = null;
        try
        {
            importer = (TextureImporter)TextureImporter.GetAtPath(path);
        }
        catch
        {
            Debug.LogError("Load Texture failed: " + path);
            return;
        }
        if (importer == null)
        {
            return;
        }
        importer.maxTextureSize = Mathf.Max(width, height);
        importer.anisoLevel = 0;
        importer.filterMode = FilterMode.Bilinear;
        importer.isReadable = false;  //increase memory cost if readable is true  
        importer.textureFormat = TextureImporterFormat.AutomaticCompressed;
#if UNITY_5_5 || UNITY_5_6
        importer.textureType = TextureImporterType.Default;
#else
        importer.textureType = TextureImporterType.Advanced;
#endif
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = false;
        AssetDatabase.ImportAsset(path);
    }


    static void SetTextureReadableEx(string _relativeAssetPath)    //set readable flag and set textureFormat TrueColor  
    {
        TextureImporter ti = null;
        try
        {
            ti = (TextureImporter)TextureImporter.GetAtPath(_relativeAssetPath);
        }
        catch
        {
            Debug.LogError("Load Texture failed: " + _relativeAssetPath);
            return;
        }
        if (ti == null)
        {
            return;
        }
        ti.isReadable = true;
        ti.textureFormat = TextureImporterFormat.AutomaticTruecolor;      //this is essential for departing Textures for ETC1. No compression format for following operation.  
        AssetDatabase.ImportAsset(_relativeAssetPath);
    }

    static bool GetDefaultWhiteTexture()
    {
        return true;
        //defaultWhiteTex = AssetDatabase.LoadAssetAtPath(defaultWhiteTexPath_relative, typeof(Texture2D)) as Texture2D;  //not just the textures under Resources file    
        //if (!defaultWhiteTex)
        //{
        //    Debug.LogError("Load Texture Failed : " + defaultWhiteTexPath_relative);
        //    return false;
        //}
        //return true;
    }

    #endregion

    #region string or path helper

    public static bool IsTextureFile(string _path)
    {
        string path = _path.ToLower();
        return path.EndsWith(".psd") || path.EndsWith(".tga") || path.EndsWith(".png") || path.EndsWith(".jpg") || path.EndsWith(".bmp") || path.EndsWith(".tif") || path.EndsWith(".gif");
    }

    static bool IsTextureConverted(string _path)
    {
        return _path.Contains("_RGB.") || _path.Contains("_Alpha.");
    }

    static string GetRGBTexPath(string _texPath)
    {
        return GetTexPath(_texPath, "_RGB.");
    }

    static string GetAlphaTexPath(string _texPath)
    {
        return GetTexPath(_texPath, "_Alpha.");
    }

    static string GetTexPath(string _texPath, string _texRole)
    {
        string dir = System.IO.Path.GetDirectoryName(_texPath);
        string filename = System.IO.Path.GetFileNameWithoutExtension(_texPath);
        string result = dir + "/" + filename + _texRole + "png";
        return result;
    }

    public static string GetRelativeAssetPath(string _fullPath)
    {
        _fullPath = GetRightFormatPath(_fullPath);
        int idx = _fullPath.IndexOf("Assets");
        string assetRelativePath = _fullPath.Substring(idx);
        return assetRelativePath;
    }

    static string GetRightFormatPath(string _path)
    {
        return _path.Replace("\\", "/");
    }

    #endregion
}
