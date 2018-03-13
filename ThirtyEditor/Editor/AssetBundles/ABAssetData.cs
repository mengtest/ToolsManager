using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABAssetData
{
    public string filePath;

    public EnumAB bundleType;

    public string sceneName;

    public string fileHash;

    public int version;

    public string[] depends;

    public bool isIgnoreMd5 = false;

    public override int GetHashCode()
    {
        return this.filePath.GetHashCode();
    }

    public BundleID bundleId
    {
        get
        {
            return new BundleID(version, bundleType, sceneName);
        }
        set
        {
            this.version = value.version;
            this.bundleType = value.buildType;
            this.sceneName = value.sceneName;
        }
    }
    public override bool Equals(object obj)
    {
        ABAssetData mountId = obj as ABAssetData;
        if (mountId != null)
        {
            if (mountId.isIgnoreMd5)
            {
                return true;
            }
            return (mountId.fileHash == fileHash);
        }
        else if (X2AssetsBundleEditor.m_isIgnoreDic.ContainsKey(filePath))
        {
            return X2AssetsBundleEditor.m_isIgnoreDic[filePath];
        }
        return false;
    }
}

public class ABAssetDataList
{
    public int newVersion = 0;

    public int firstVersion = 0;

    public List<ABAssetData> assetList = new List<ABAssetData>();
}
