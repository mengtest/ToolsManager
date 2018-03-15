using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;


public class XGame2DLL
{
#if UNITY_EDITOR
    const string LUADLL = "XGame2";
#elif UNITY_IPHONE
        const string LUADLL = "__Internal";
#else
        const string LUADLL = "XGame2";
#endif
    /// <summary>
    /// 没有实现，给NativeCode调用来确保vm被锁的
    /// </summary>
    public static void Init() { }

    /// <summary>
    /// 静态调用 vm加锁
    /// </summary>
    static XGame2DLL()
    {
        InitBitCoverter();
    }

    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    private static extern int InitBitCoverter();



    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int EncryToInt32(int data);

    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DecryToInt32(int data);

    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern byte EncryToBoolean(byte isTrue);

    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern byte DecryToBoolean(byte data);

    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int EncryToFloat(float data);

    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern float DecryToFloat(int data);

    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern long EncryToDouble(double data);

    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern double DecryToDouble(long data);

    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern long EncryToLong(long data);

    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern long DecryToLong(long data);


    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int ToInt32(byte[] data);

    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool ToBoolean(byte[] data);

    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern float ToFloat(byte[] data);

    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern double ToDouble(byte[] data);

    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern long ToLong(byte[] data);

    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void GetBoolBytes(bool value, byte[] data);


    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void GetIntBytes(int value, byte[] data);


    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void GetLongBytes(long value, byte[] data);


    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void GetFloatBytes(float value, byte[] data);


    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void GetDoubleBytes(double value, byte[] data);


    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int DecrypteToInt32(byte[] data);

    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool DecrypteToBoolean(byte[] data);

    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern float DecrypteToFloat(byte[] data);

    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern double DecrypteToDouble(byte[] data);

    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern long DecrypteToLong(byte[] data);

    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void EncrypteGetBoolBytes(bool value, byte[] data);


    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void EncrypteGetIntBytes(int value, byte[] data);

    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void EncrypteGetLongBytes(long value, byte[] data);


    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void EncrypteGetFloatBytes(float value, byte[] data);


    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void EncrypteGetDoubleBytes(double value, byte[] data);


    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void Scene_CreateNode(float x, float y, float z, float sx, float sy, float sz, int nodeId, int nx, int ny, float distance);

    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int Scene_Update(float x, float y, float z);

    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void Scene_Get(int[] value, out int activeLeng, out int loadResourceLenth, out int disableLength);

    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void Scene_Clear();

    [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void Scene_InitData(int nodeWidget, int MaxWidgetNum, int MaxLengthNum, float minX, float minZ);
}
