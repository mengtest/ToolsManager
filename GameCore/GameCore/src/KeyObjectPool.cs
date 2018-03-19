/************************************************************
//     文件名      : KeyObjectPool.cs
//     功能描述    : Key Object对象池，负责对象的加载、回收等管理
//     负责人      : blackzhou
//     参考文档    : 无
//     创建日期    : 2016/07/01.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/

using UnityEngine;
using System;
using System.Collections.Generic;

//该对象池会自动回收不再使用的对象实例，当再次需要时直接根据键值取出闲置的对象实例，以避免资源的重复加载。
//该对象池允许对象使用相同的键值，以实现对同类资源多个实例的回收管理。
public class KeyObjectPool <TKey, TObject>
{
    //创建对象回调函数
    public delegate TObject TCreateObjectCallback(TKey key, System.Object param);

    //销毁对象回调函数
    public delegate void TDestroyObjectCallback(TObject obj);

    //重用对象回调函数
    public delegate void TReuseObjectCallback(TObject obj, System.Object param);

    //回收对象回调函数
    public delegate void TRecycleObjectCallback(TObject obj, System.Object param);

    /// <summary>
    /// 初始化对象池
    /// </summary>
    /// <param name="maxCachedObjects">最大缓存的对象数量</param>
    /// <param name="createObjectCallback">创建对象回调函数</param>
    /// <param name="destroyObjectCallback">销毁对象回调函数</param>
    /// <param name="recycleObjectCallback">回收对象回调函数</param>
    /// <param name="reuseObjectCallback">重用对象回调函数</param>
    public void Initialize(int maxCachedObjects, TCreateObjectCallback createObjectCallback, TDestroyObjectCallback destroyObjectCallback = null,
        TReuseObjectCallback reuseObjectCallback = null, TRecycleObjectCallback recycleObjectCallback = null)
    {
        m_MaxCachedObjects = maxCachedObjects;
        m_CreateObjectCallback = createObjectCallback;
        m_DestroyObjectCallback = destroyObjectCallback;
        m_ReuseObjectCallback = reuseObjectCallback;
        m_RecycleObjectCallback = recycleObjectCallback;
    }

    public void Release()
    {
        Clear();

        m_MaxCachedObjects = 0;
        m_CreateObjectCallback = null;
        m_DestroyObjectCallback = null;
        m_ReuseObjectCallback = null;
        m_RecycleObjectCallback = null;
    }

    /// <summary>
    /// 根据指定键值加载对象（指定键值的对象在对象池中则直接返回缓存的对象，不在对象池中则调用创建对象回调函数创建新实例。）
    /// </summary>
    /// <param name="key">指定加载对象的键值</param>
    /// <param name="param">传给初始化回调函数使用的自定义参数</param>
    /// <returns>已加载的对象</returns>
    public TObject LoadObject(TKey key, System.Object param = null)
    {
        if (key == null)
        {
            Debug.LogError("KeyObjectPool.LoadObject: key is null");
        }
        LinkedListNode<ObjectNode> objectListNode = null;

        //在回收对象池中找到键值相同的可复用对象
        if (m_ObjectDict.TryGetValue(key, out objectListNode))
        {
            //从空闲列表中删除对象
            _RemoveObjectFromUnusedList(objectListNode);

            //调用对象重用函数
            TObject reusedObject = objectListNode.Value.m_Object;
            if (m_ReuseObjectCallback != null)
            {
                m_ReuseObjectCallback(reusedObject, param);
            }

            return reusedObject;
        }
        //未找到可复用的回收对象
        else
        {
            //创建新的对象实例
            TObject newObject = m_CreateObjectCallback(key, param);
            return newObject;
        }
    }
    /// <summary>
    /// 卸载掉键值为key的所有缓存
    /// </summary>
    /// <param name="key"></param>
    public void UnLoadAllObject(TKey key)
    {
        if (key == null)
        {
            Debug.LogError("KeyObjectPool.LoadObject: key is null");
        }
        LinkedListNode<ObjectNode> objectListNode = null;

        //在回收对象池中找到键值相同的可复用对象
        while(m_ObjectDict.TryGetValue(key, out objectListNode))
        {
            _RemoveObjectFromUnusedList(objectListNode);

            //销毁对象
            if (m_DestroyObjectCallback != null)
            {
                m_DestroyObjectCallback(objectListNode.Value.m_Object);
            }
        }
    }
    /// <summary>
    /// 卸载指定键值的对象
    /// </summary>
    /// <param name="key">卸载对象的键值</param>
    /// <param name="obj">卸载对象的引用</param>
    /// <param name="param">传给回收回调函数使用的自定义参数</param>
    public void UnloadObject(TKey key, TObject obj, System.Object param = null)
    {
        //如果允许缓存对象
        if (m_MaxCachedObjects > 0)
        {
            //如果超过了最大缓存对象数量，则删除较老的缓存对象。
            while (m_UnusedObjectList.Count >= m_MaxCachedObjects)
            {
                //直接删除缓存链表头的对象
                LinkedListNode<ObjectNode> objectListNode = m_UnusedObjectList.First;
                if (objectListNode != null)
                {
                    //从空闲列表中删除对象
                    _RemoveObjectFromUnusedList(objectListNode);

                    //销毁对象
                    if (m_DestroyObjectCallback != null)
                    {
                        m_DestroyObjectCallback(objectListNode.Value.m_Object);
                    }
                }
            }

            //调用对象回收函数
            if (m_RecycleObjectCallback != null)
            {
                m_RecycleObjectCallback(obj, param);
            }

            //创建闲置对象链表节点
            ObjectNode objectNode = new ObjectNode();
            objectNode.m_Object = obj;
            objectNode.m_Key = key;
            objectNode.m_PrevDictNode = null;
            objectNode.m_NextDictNode = null;

            LinkedListNode<ObjectNode> newObjectLinkedListNode = new LinkedListNode<ObjectNode>(objectNode);

            //记录回收时间
            objectNode.m_RecycleTime = Time.realtimeSinceStartup;

            //如果还有键值相同的已回收对象
            _AddObjectToUnusedList(key, newObjectLinkedListNode);
        }
        //如果不允许缓存对象
        else
        {
            //销毁对象
            if (m_DestroyObjectCallback != null)
            {
                m_DestroyObjectCallback(obj);
            }
        }
    }

    /// <summary>
    /// 根据未使用时间销毁池中的闲置对象
    /// </summary>
    /// <param name="deltaTime">经过时间的时差，一般指定为帧延时。</param>
    /// <param name="destroyThresholdTime">已回收对象的最大缓存时长，超过该值后则强制销毁对象。</param>
    public void DestroyObjectsByTime(float deltaTime, float destroyThresholdTime)
    {
        //从链表头部开始遍历所有闲置对象
        LinkedListNode<ObjectNode> objectListNode = m_UnusedObjectList.First;
        while (objectListNode != null)
        {
            LinkedListNode<ObjectNode> nextNode = objectListNode.Next;

            //计算闲置时间
            float fUnusedTime = Time.realtimeSinceStartup - objectListNode.Value.m_RecycleTime;

            //如果闲置时间已小于对象销毁极限时间则退出循环（越往后回收时间越晚）
            if (fUnusedTime < destroyThresholdTime)
            {
                break;
            }

            //从空闲列表中删除对象
            _RemoveObjectFromUnusedList(objectListNode);

            //销毁对象
            if (m_DestroyObjectCallback != null)
            {
                m_DestroyObjectCallback(objectListNode.Value.m_Object);
            }

            objectListNode = nextNode;
        }
    }

    /// <summary>
    /// 强制清空对象池
    /// </summary>
    public void Clear()
    {
        m_ObjectDict.Clear();

        if (m_DestroyObjectCallback != null)
        {
            LinkedListNode<ObjectNode> objectListNode = m_UnusedObjectList.First;
            while (objectListNode != null)
            {
                m_DestroyObjectCallback(objectListNode.Value.m_Object);
                objectListNode = objectListNode.Next;
            }
        }

        m_UnusedObjectList.Clear();
    }

    /// <summary>
    /// 获取对象数量
    /// </summary>
    /// <returns>对象数量</returns>
    public int GetObjectCount()
    {
        return m_ObjectDict.Count;
    }

    //从空闲对象列表中删除对象节点
    protected void _RemoveObjectFromUnusedList(LinkedListNode<ObjectNode> objectListNodeToRemove)
    {
        //如果还有键值相同的前续链表对象
        if (objectListNodeToRemove.Value.m_PrevDictNode != null)
        {
            objectListNodeToRemove.Value.m_PrevDictNode.Value.m_NextDictNode = objectListNodeToRemove.Value.m_NextDictNode;
        }

        //如果还有键值相同的后续链表对象
        if (objectListNodeToRemove.Value.m_NextDictNode != null)
        {
            objectListNodeToRemove.Value.m_NextDictNode.Value.m_PrevDictNode = objectListNodeToRemove.Value.m_PrevDictNode;
        }

        //如果当前删除的节点是查找表入口条目
        LinkedListNode<ObjectNode> dictEntryNode = null;
        if (m_ObjectDict.TryGetValue(objectListNodeToRemove.Value.m_Key, out dictEntryNode) && dictEntryNode == objectListNodeToRemove)
        {
            //如果存在后续节点则将后续节点作为查找表入口
            if (objectListNodeToRemove.Value.m_NextDictNode != null)
            {
                m_ObjectDict[objectListNodeToRemove.Value.m_Key] = objectListNodeToRemove.Value.m_NextDictNode;
            }
            //否则删除查找表条目
            else
            {
                m_ObjectDict.Remove(objectListNodeToRemove.Value.m_Key);
            }
        }

        objectListNodeToRemove.Value.m_PrevDictNode = null;
        objectListNodeToRemove.Value.m_NextDictNode = null;

        //从链表中删除对象
        m_UnusedObjectList.Remove(objectListNodeToRemove);
    }

    //将对象节点添加到闲置对象列表中
    protected void _AddObjectToUnusedList(TKey key, LinkedListNode<ObjectNode> objectListNodeToAdd)
    {
        //如果还有键值相同的已回收对象
        LinkedListNode<ObjectNode> existObjectLinkedListNode = null;
        if (m_ObjectDict.TryGetValue(key, out existObjectLinkedListNode))
        {
            m_ObjectDict[key] = objectListNodeToAdd;

            //将其加到原节点之前
            existObjectLinkedListNode.Value.m_PrevDictNode = objectListNodeToAdd;
            objectListNodeToAdd.Value.m_NextDictNode = existObjectLinkedListNode;
        }
        else
        {
            m_ObjectDict.Add(key, objectListNodeToAdd);
        }

        //将闲置对象节点添加到链表尾部
        m_UnusedObjectList.AddLast(objectListNodeToAdd);
    }

    public class ObjectNode
    {
        public TObject                      m_Object;           //被回收的对象
        public TKey                         m_Key;              //对象键值
        public float                        m_RecycleTime;     //回收时间
        public LinkedListNode<ObjectNode>   m_PrevDictNode;     //上一个键值相同的回收对象节点
        public LinkedListNode<ObjectNode>   m_NextDictNode;     //下一个键值相同的回收对象节点
    }

    //闲置对象查找表
    protected Dictionary<TKey, LinkedListNode<ObjectNode>> m_ObjectDict = new Dictionary<TKey, LinkedListNode<ObjectNode>>();

    //闲置对象链表
    protected LinkedList<ObjectNode> m_UnusedObjectList = new LinkedList<ObjectNode>();

    public int m_MaxCachedObjects = 0;
    public TCreateObjectCallback m_CreateObjectCallback = null;
    public TDestroyObjectCallback m_DestroyObjectCallback = null;
    public TReuseObjectCallback m_ReuseObjectCallback = null;
    public TRecycleObjectCallback m_RecycleObjectCallback = null;
}
