//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: tools/CAssetBundleData.proto
using System.Collections.Generic;
namespace Core.CAssetBundleData
{
    public partial class ABFileDesc
    {
        public ABFileDesc() { }

        public string m_gbName;

        public string m_bundleName;

        public int m_ressVersion;

    }

    public partial class ABSceneDesc
    {
        public ABSceneDesc() { }
        public string m_sceneName;
        public string m_bundleName;
        public int m_ressVersion;
    }

    public partial class ABDependencies 
    {
        public ABDependencies() { }

        public string m_abName;
        public List<string> m_dependenciesAb = new List<string>();
        public int m_ressVersion;
        public bool m_isStreamAsset;
        public string m_areaName;   //������ �����ҵ���ӦĿ¼
    }

    public partial class ABDesc 
    {
        public ABDesc() { }

        public List<ABFileDesc> m_files = new List<ABFileDesc>();

        public List<ABSceneDesc> m_scenes = new List<ABSceneDesc>();

        public List<ABDependencies> m_abDepends = new List<ABDependencies>();
    }

}