using System.Collections;
using UnityEngine;
using System;

//#if UNITY_IPHONE || UNITY_IOS
//using System.Collections.Generic;
//using System.Runtime.InteropServices;
//using System.Runtime.Serialization.Formatters.Binary;
//#endif

public class RBDViewController 
{
    public static void BeginDetecation()
    {
//#if UNITY_IPHONE || UNITY_IOS
//         BeginDetecation_IOS();
//#endif
    }

    public static bool isMuted()
    {
//#if UNITY_IPHONE || UNITY_IOS
//        return GetIsMuted_IOS();
//#endif
        return false;
    }

//#if UNITY_IPHONE || UNITY_IOS
//    [DllImport("__Internal")]
//    public static extern void BeginDetecation_IOS();

//     [DllImport("__Internal")]
//    public static extern bool GetIsMuted_IOS();
//#endif
}