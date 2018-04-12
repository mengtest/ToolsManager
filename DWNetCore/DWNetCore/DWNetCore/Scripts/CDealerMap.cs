
using System.Collections.Generic;
using System;

public class CDealerCB
{
    public System.Delegate cb;
    public CDealerCB(System.Delegate c)
    {
        cb = c;
    }
    public virtual bool EqualsTarget(object target)
    {
        return cb.Target == target;
    }
    public virtual bool EqualsType<T>()
    {
        return cb.Target.GetType() == typeof(T);
    }
}

public class CDealerMap<TID, TCB> where TCB : CDealerCB
{
    public Dictionary<TID, List<TCB>> m_dealerMap = new Dictionary<TID, List<TCB>>();
    private Dictionary<object, List<TID>> m_targetMap = new Dictionary<object, List<TID>>();

    public void AddHandle(TID id, TCB _cb)
    {
        /*
		if (!typeof(TCB).IsSubclassOf(typeof(Delegate)))
		{
			throw new InvalidOperationException(typeof(TCB).Name + " is not a delegate type");
		}
		*/

        if (!m_dealerMap.ContainsKey(id))
        {
            m_dealerMap[id] = new List<TCB>();
        }

        var list = m_dealerMap[id];
        TCB ocb = list.Find((TCB mcb) =>
        {
            return mcb.cb.Equals(_cb.cb);
        });
        if (ocb != null)
        {
            return;
        }

        list.Add(_cb);

        System.Delegate cb = _cb.cb;
        if (cb != null && cb.Target != null)
        {
            if (!m_targetMap.ContainsKey(cb.Target))
            {
                m_targetMap[cb.Target] = new List<TID>();
            }
            var tlist = m_targetMap[cb.Target];
            tlist.Add(id);
        }
    }
    public void RemoveHandleByTarget(object target)
    {
        List<TID> tlist = null;
        m_targetMap.TryGetValue(target, out tlist);
        if (tlist != null)
        {
            for (int i = tlist.Count - 1; i >= 0; --i)
            {
                TID id = tlist[i];
                List<TCB> clist = null;
                m_dealerMap.TryGetValue(id, out clist);
                if (clist != null)
                {
                    clist.RemoveAll((TCB cb) =>
                    {
                        return cb.EqualsTarget(target);
                    });
                    if (clist.Count == 0)
                    {
                        m_dealerMap.Remove(id);
                    }
                }
            }
            m_targetMap.Remove(target);
        }
    }

    public void RemoveHandleByType<T>()
    {
        List<object> keys = new List<object>();
        var enm = m_targetMap.GetEnumerator();
        while (enm.MoveNext())
        {
            var kv = enm.Current;
            if (kv.Key.GetType() == typeof(T))
            {
                var tlist = m_targetMap[kv.Key];
                for (int i = tlist.Count - 1; i >= 0; --i)
                {
                    var id = tlist[i];
                    if (m_dealerMap.ContainsKey(id))
                    {
                        var clist = m_dealerMap[id];
                        clist.RemoveAll((TCB cb) =>
                        {
                            return cb.EqualsType<T>();
                        });
                        if (clist.Count == 0)
                        {
                            m_dealerMap.Remove(id);
                        }
                    }
                }
                keys.Add(kv.Key);
            }
        }
        for (int i = 0; i < keys.Count; ++i)
        {
            m_targetMap.Remove(keys[i]);
        }
    }

    public void RemoveHandleById(TID id)
    {
        RemoveHandleById(id, null);
    }


    public void RemoveHandleById(TID id, TCB tcb)
    {
        List<TCB> list = null;
        m_dealerMap.TryGetValue(id, out list);

        if (list != null)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                var _cb = list[i];
                System.Delegate cb = _cb.cb;
                if (cb != null && ((tcb!= null && tcb.cb == cb) 
                    || (tcb == null && cb.Target != null)))
                {
                    List<TID> idlist = null;
                    m_targetMap.TryGetValue(cb.Target, out idlist);
                    if (idlist != null)
                    {
                        idlist.RemoveAll((TID obj) =>
                        {
                            return id.Equals(obj);
                        });
                        if (idlist.Count == 0)
                        {
                            m_targetMap.Remove(cb.Target);
                        }
                    }
                }
            }
            if (tcb == null)
            {
                list.Clear();
            }
            else
            {
                list.RemoveAll((TCB obj) =>
                {
                    return tcb.cb.Equals(obj.cb);
                });
            }
        }
        if (list == null || list.Count == 0)
        {
            m_dealerMap.Remove(id);
        }
    }

    public List<TCB> GetDealer(TID id)
    {
        List<TCB> list = null;
        m_dealerMap.TryGetValue(id, out list);
        return list;
    }
}
