/************************************************************
//     文件名      : NativeCode.cs
//     功能描述    : 
//     负责人      : blackzhou
//     参考文档    : 无
//     创建日期    : 2016/07/12.
//     Copyright   : Copyright 2016 EZFun Inc.
**************************************************************/
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using System.IO;

//[RegisterSystem(typeof(NativeCode), true)]
public class NativeCode :  IInitializeable, IUpdateable, IFixedUpdateable, ILateUpdateable//TCoreSystem<NativeCode>,
{
    #region C++ 动态链接库回调接口

    //日志输出类型
    public const int LOG_LEVEL_INFO = 0;
    public const int LOG_LEVEL_WARNING = 1;
    public const int LOG_LEVEL_ERROR = 2;

    //回调函数类型声明

    //输出调试日志
    public delegate void FnDebugLog([MarshalAs(UnmanagedType.LPStr)] string pszLog, int nLogLevel);

    //释放 C# Managed Object GCHandle
    public delegate void FnFreeObjectHandle(int ptrObject);

    //激活 GameObject
    public delegate void FnGameObjectSetActive(int ptrGameObject, bool bActive);

    //销毁 GameObject
    public delegate void FnGameObjectDestroy(int ptrGameObject);

    //创建 GameObject
    public delegate void FnLoadResource(string path, int callBackType, string strPara, bool bSync, bool bOutOfOrderCallback);

    // C# 回调函数指针结构体
    [StructLayout(LayoutKind.Sequential)]
    public struct ManagedCallbacks
    {
        public IntPtr DebugLog;
        public IntPtr FreeObjectHandle;
        public IntPtr GameObjectSetActive;
        public IntPtr GameObjectDestroy;
    };

    // C# Object 的 C++ IntPtr 句柄查询表
    protected static Dictionary<int, GCHandle> m_DictGCHandles = new Dictionary<int, GCHandle>();

    //分配 C# Object 的 C++ IntPtr 句柄（当不再使用后必须调用 FreeNativeObjectIntPtr 释放句柄）
    public static int AllocObjectNativeIntPtr(System.Object obj)
    {
        GCHandle objGCHandle = GCHandle.Alloc(obj, GCHandleType.Normal);
        if (objGCHandle == null)
        {
            return -1;
        }

        int objIntPtr = GCHandle.ToIntPtr(objGCHandle).ToInt32();

        if (m_DictGCHandles.ContainsKey(objIntPtr))
        {
            Debug.LogErrorFormat("Duplicated IntPtr value {0} of GCHandle. Object : {1}", objIntPtr, obj.ToString());
            return -1;
        }

        m_DictGCHandles.Add(objIntPtr, objGCHandle);

        return objIntPtr;
    }

    //释放 C++ IntPtr 句柄
    public static void FreeNativeObjectIntPtr(int nativeIntPtr)
    {
        GCHandle objGCHandle;
        if (!m_DictGCHandles.TryGetValue(nativeIntPtr, out objGCHandle))
        {
            Debug.LogErrorFormat("Couldn't find GCHandle for IntPtr value {0}.", nativeIntPtr);
            return;
        }

        objGCHandle.Free();
    }

    //释放所有 C++ IntPtr 句柄
    public static void FreeAllNativeObjectIntPtrs()
    {
        var enumeratorGCHandle = m_DictGCHandles.GetEnumerator();
        while (enumeratorGCHandle.MoveNext())
        {
            var objGCHandle = enumeratorGCHandle.Current.Value;
            objGCHandle.Free();
        }

        m_DictGCHandles.Clear();
    }

    // C++ IntPtr 转换为 C# Object
    public static System.Object NativeIntPtrToObject(int nativeIntPtr)
    {
        var intptr = new System.IntPtr(nativeIntPtr);
        if (intptr == null)
        {
            return null;
        }

        var gcHandle = GCHandle.FromIntPtr(intptr);
        if (gcHandle == null)
        {
            return null;
        }

        return gcHandle.Target;
    }

    //输出调试日志
    [MonoPInvokeCallback(typeof(FnDebugLog))]
    public static void DebugLog([MarshalAs(UnmanagedType.LPStr)] string pszLog, int nLogLevel)
    {
        switch (nLogLevel)
        {
            case LOG_LEVEL_INFO:
            default:
                {
                    Debug.Log(pszLog);
                    break;
                }
            case LOG_LEVEL_WARNING:
                {
                    Debug.LogWarning(pszLog);
                    break;
                }
            case LOG_LEVEL_ERROR:
                {
                    Debug.LogError(pszLog);
                    break;
                }
        }
    }

    //释放 C# Managed Object GCHandle
    [MonoPInvokeCallback(typeof(FnFreeObjectHandle))]
    public static void FreeObjectHandle(int ptrObject)
    {
        FreeNativeObjectIntPtr(ptrObject);
    }

    //激活 GameObject
    [MonoPInvokeCallback(typeof(FnGameObjectSetActive))]
    public static void GameObjectSetActive(int ptrGameObject, bool bActive)
    {
        GameObject gameObject = NativeIntPtrToObject(ptrGameObject) as GameObject;
        if (gameObject != null)
        {
            gameObject.SetActive(bActive);
        }
    }

    //销毁 GameObject
    [MonoPInvokeCallback(typeof(FnGameObjectDestroy))]
    public static void GameObjectDestroy(int ptrGameObject)
    {
        GameObject gameObject = NativeIntPtrToObject(ptrGameObject) as GameObject;
        GameObject.Destroy(gameObject);
        FreeNativeObjectIntPtr(ptrGameObject);
    }
    #endregion
    #region 基本 C++ 动态链接库导出接口

    // Native 库初始化及 C# 回调函数注册

    [DllImport(Constants.NATIVE_IMPORT)]
    public static extern int OnInitialize(ManagedCallbacks managedCallbacks);

    // Native 库释放及 C# 回调函数注销
    [DllImport(Constants.NATIVE_IMPORT)]
    public static extern void OnUnInitialize();

    // Unity Update 函数调用
    [DllImport(Constants.NATIVE_IMPORT)]
    public static extern void OnUpdate(float deltaTime);

    // Unity FixedUpdate 函数调用
    [DllImport(Constants.NATIVE_IMPORT)]
    public static extern void OnFixedUpdate(float deltaTime);

    // Unity LateUpdate 函数调用
    [DllImport(Constants.NATIVE_IMPORT)]
    public static extern void OnLateUpdate(float deltaTime);

    #endregion

    // Update is called once per frame
    public void Update()
    {
        OnUpdate(Time.deltaTime);
    }

    public void FixedUpdate()
    {
        OnFixedUpdate(Time.fixedDeltaTime);
    }

    public void LateUpdate()
    {
        OnLateUpdate(Time.deltaTime);
    }

    public void Init()
    {
        ManagedCallbacks managedCallbacks;
        managedCallbacks.DebugLog = Marshal.GetFunctionPointerForDelegate(new FnDebugLog(DebugLog));
        managedCallbacks.FreeObjectHandle = Marshal.GetFunctionPointerForDelegate(new FnFreeObjectHandle(FreeObjectHandle));
        managedCallbacks.GameObjectSetActive = Marshal.GetFunctionPointerForDelegate(new FnGameObjectSetActive(GameObjectSetActive));
        managedCallbacks.GameObjectDestroy = Marshal.GetFunctionPointerForDelegate(new FnGameObjectDestroy(GameObjectDestroy));
        //防止ActorAttribute.cpp InitAttribute被线程重复调用，所以这里强制调用这个，利用vm的锁确保同时只有一个调用InitAttribute()
        XGame2DLL.Init();
        if (0 != OnInitialize(managedCallbacks))
        {
            return;
        }
    }

    public void Release()
    {
        OnUnInitialize();
    }

}
