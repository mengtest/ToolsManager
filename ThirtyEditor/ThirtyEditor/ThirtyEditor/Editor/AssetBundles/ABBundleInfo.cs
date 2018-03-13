

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

public class ABBundleInfo
{

    public HashSet<string> fileList = new HashSet<string>();

    public BundleID bundleId { get; set; }

    public int firstVersion;

    private string m_bundleName;

    public AssetBundleBuild GetBundleBuild()
    {
        AssetBundleBuild build = new AssetBundleBuild();
        build.assetBundleName = BundleName;
        build.assetNames = fileList.ToArray();

        return build;
    }

    public int Version
    {
        get
        {
            return this.bundleId.version;
        }
    }

    public string BundleName
    {
        get
        {
            if (bundleId.buildType == EnumAB.shaders_ab)
            {
                m_bundleName =  bundleId.buildType.ToString();
                return m_bundleName;
            }
            else
            {
                m_bundleName =  bundleId.version + "_" + bundleId.buildType + 
                    (string.IsNullOrEmpty(bundleId.sceneName) ? "": ("_"+ bundleId.sceneName.ToLower())) ;
            }
            return m_bundleName;
        }
    }

    public int Count
    {
        get
        {
            return fileList.Count;
        }
    }
}

