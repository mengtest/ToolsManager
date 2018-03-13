using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;

public class ABAssetDataUtil
{
    public static ABAssetDataList ReloadCache(BuildTarget buildTarget)
    {
        string data = "";
        var path = X2AssetsBundleEditor.PreOutPath(buildTarget);
        using (StreamReader file = File.OpenText(path + "/AssetDatas.json"))
        {
            data = file.ReadToEnd();
        }
        ABAssetDataList cache = LitJson.JsonMapper.ToObject<ABAssetDataList>(data);
        return cache;
    }

    public static void SaveAssetCache(BuildTarget buildTarget, ABAssetDataList cache)
    {
        var data = LitJson.JsonMapper.ToJson(cache);
        var path = X2AssetsBundleEditor.PreOutPath(buildTarget);
        using (Stream file = File.Create(path + "/AssetDatas.json"))
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(data);
            file.Write(bytes, 0, bytes.Length);
            file.Close();
        }
    }
}

