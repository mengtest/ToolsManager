using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class ABAssetInfo
{
    /// <summary>
    /// 是否忽略md5不一致的更新
    /// </summary>
    public bool m_isIgnore
    {
        get
        {
            var data = X2AssetsBundleEditor.GetOldData(AssetPath);
            if (data == null)
            {
                return m_cacheData.isIgnoreMd5;
            }
            m_cacheData.isIgnoreMd5 = data.isIgnoreMd5;
            return data.isIgnoreMd5;
        }
        set
        {
            X2AssetsBundleEditor.m_isIgnoreDic[AssetPath] = value;
            m_cacheData.isIgnoreMd5 = value;
            var data = X2AssetsBundleEditor.GetOldData(AssetPath);
            if (data != null)
            {
                data.isIgnoreMd5 = value;
            }
        }
    }


    public bool m_hasChildrenUpdate = false;

    public ABAssetData m_cacheData = new ABAssetData();

    /// <summary>
    /// 父引用 别人引用它
    /// </summary>
    public HashSet<ABAssetInfo> m_parent = new HashSet<ABAssetInfo>();

    /// <summary>
    /// 子引用 它引用别人
    /// </summary>
    public HashSet<ABAssetInfo> m_children = new HashSet<ABAssetInfo>();


    public ABAssetInfo(ABAssetData cacheData)
    {
        m_cacheData = cacheData;
    }

    public ABAssetData ToCacheData()
    {
        m_cacheData.depends = m_children.ToList().ConvertAll<string>((item) => { return item.AssetPath; }).ToArray();
        return m_cacheData;
    }

    public string AssetPath
    {
        get
        {
            return m_cacheData.filePath;
        }
    }

    public bool IsCaseUpdate
    {
        get
        {
            var data = X2AssetsBundleEditor.GetOldData(AssetPath);
            if (data == null)
            {
                return false;
            }
            if (m_isIgnore)
            {
                return false;
            }
            return !this.m_cacheData.Equals(data);
        }
    }

    public int Version
    {
        get
        {
            return m_cacheData.version;
        }
    }


    public int ChildCount
    {
        get { return m_children.Count; }
    }

    public int ParentCount
    {
        get { return m_parent.Count; }
    }

    public void AddChild(ABAssetInfo child)
    {
        if (!m_children.Contains(child))
        {
            m_children.Add(child);
        }
        if (!child.m_parent.Contains(this))
        {
            child.m_parent.Add(this);
        }
    }

    public void AddParent(ABAssetInfo parent)
    {
        if (!m_parent.Contains(parent))
        {
            m_parent.Add(parent);
        }
        if (!parent.m_children.Contains(this))
        {
            parent.m_children.Add(this);
        }
    }

    public void RemoveChild(ABAssetInfo child)
    {
        m_children.Remove(child);
        child.m_parent.Remove(this);
    }

    public void RemoveParent(ABAssetInfo parent)
    {
        m_parent.Remove(parent);
        parent.m_children.Remove(this);
    }

    public override int GetHashCode()
    {
        return m_cacheData.filePath.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj is ABAssetInfo)
        {
            return m_isIgnore || (obj as ABAssetInfo).AssetPath == AssetPath;
        }
        return false;
    }
}
