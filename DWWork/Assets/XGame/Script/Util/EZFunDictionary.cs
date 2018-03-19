//========================================================================
// Copyright(C): EZFun
//
// CLR Version : 4.0.30319.42000
// Machinename : USER-20160222CX
// NameSpace   : Assets.XGame.Script.Util
// FileName    : EZFunDictionary
//
// Created by  : dhf at 2016/7/1 11:47:28
//
// Function    : 可用for的dictionary类型
//
//========================================================================


using System;
using System.Collections.Generic;
using UnityEngine;

public class EZFunDictionary<TKey, TValue>
{
    private Action<TKey, TValue> m_addCB;
    private Action<TKey, TValue> m_removeCB;
    private Dictionary<TKey, TValue> _m_dict = new Dictionary<TKey, TValue>();
    private List<TKey> _m_keys = new List<TKey>();
    private Dictionary<TKey, TValue> m_dict
    {
        get
        {
            NullCheck();
            return _m_dict;
        }
        set
        {
            _m_dict = value;
        }
    }
    private List<TKey> m_keys
    {
        get
        {
            NullCheck();
            return _m_keys;
        }
        set
        {
            _m_keys = value;
        }
    }

    private void NullCheck()
    {
        if (_m_dict == null || _m_keys == null)
        {
            _m_dict = new Dictionary<TKey, TValue>();
            _m_keys = new List<TKey>();
        }
    }

    public int Count
    {
        get
        {
            return m_dict.Count;
        }
    }

    public EZFunDictionary(Action<TKey, TValue> addCB = null, Action<TKey, TValue> removeCB = null)
    {
        m_addCB = addCB;
        m_removeCB = removeCB;
    }
    public EZFunDictionary(Dictionary<TKey, TValue> dictionary)
    {
        List<TKey> keys = new List<TKey>(dictionary.Keys);
        m_dict = dictionary;
        m_keys = keys;
    }

    public void Add(TKey key, TValue value)
    {
        if (m_keys.Contains(key))
        {
            m_dict[key] = value;
            return;
        }
        m_dict.Add(key, value);
        m_keys.Add(key);
        if(m_addCB != null)
        {
            m_addCB(key, value);
        }
    }

    public void Add(Dictionary<TKey, TValue> dictionary)
    {
        List<TKey> keys = new List<TKey>(dictionary.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            Add(keys[i], dictionary[keys[i]]);
        }
    }

    public void Remove(TKey key)
    {
        if (m_removeCB != null)
        {
            m_dict.Remove(key);
            m_keys.Remove(key);
            m_removeCB(key, m_dict[key]);
        }
        else
        {
            m_dict.Remove(key);
            m_keys.Remove(key);
        }
    }

    public void RemoveAt(int index)
    {
        if (m_removeCB != null)
        {
            m_dict.Remove(m_keys[index]);
            m_keys.RemoveAt(index);
            m_removeCB(m_keys[index], m_dict[m_keys[index]]);
        }
        else
        {
            m_dict.Remove(m_keys[index]);
            m_keys.RemoveAt(index);
        }
    }

    public void Clear()
    {
        if(m_removeCB != null)
        {
            for(int i = 0; i < m_keys.Count; ++i)
            {
                m_removeCB(m_keys[i], m_dict[m_keys[i]]);
            }
        }
        m_dict.Clear();
        m_keys.Clear();
    }

    public bool ContainsKey(TKey key)
    {
        return m_keys.Contains(key);
    }

    public bool ContainsValue(TValue value)
    {
        return m_dict.ContainsValue(value);
    }

    public TValue this[TKey key]
    {
        get
        {
            return m_dict[key];
        }
        set
        {
            m_dict[key] = value;
        }
    }

    public TKey GetKey(int index)
    {
        TKey key = default(TKey);
        if (m_keys.Count > index)
        {
            key = m_keys[index];
        }
        else
        {
            Debug.LogError(EZFunString.LinkString("EZFunDictionary index ", index.ToString(), "error"));
        }
        return key;
    }

    public TValue GetValue(int index)
    {
        return this[GetKey(index)];
    }

    public void CopyTo(ref EZFunDictionary<TKey, TValue> toDict)
    {
       if(toDict == null)
       {
           toDict = new EZFunDictionary<TKey, TValue>();
       }
        toDict.Clear();
        for(int i = 0; i < m_keys.Count; ++i)
        {
            toDict.Add(m_keys[i], m_dict[m_keys[i]]);
        }
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
		value = default(TValue);
		for (int i=0; i<_m_keys.Count; i++) 
		{
			if (key.Equals(m_keys[i]))
			{
				value = m_dict[m_keys[i]];
				return true;
			}
		}
		return false;
    }

    public void SortByValue(Comparison<TValue> cmp)
    {
        m_keys.Sort((TKey lkey, TKey rkey) => 
        {
            return cmp(m_dict[lkey], m_dict[rkey]);
        });
    }

    public List<KeyValuePair<TKey, TValue>> ToList()
    {
        return new List<KeyValuePair<TKey, TValue>>(m_dict);
    }

    public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
    {
        return m_dict.GetEnumerator();
    }
}
