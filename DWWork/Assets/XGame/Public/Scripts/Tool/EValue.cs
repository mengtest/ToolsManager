using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.ComponentModel;
using System.Text;
using System.IO;
using UnityEngine;
/// <summary>
/// 线程安全
/// </summary>
public class EValue
{
    private EValue()
    {
    }

    private static byte[] encryBytes = null;
    private static byte[] decryBytes = null;

    static EValue()
    {

        System.Random random = new System.Random();
        encryBytes = new byte[256];
        decryBytes = new byte[256];
        for (int j = 0; j < 256; j++)
        {
            encryBytes[j] = (byte)j;
            decryBytes[j] = (byte)j;
        }
        byte i = (byte)random.Next(256);
        byte temp = 0;
        for (int j = 0; j < 256; j++)
        {
            temp = encryBytes[i];
            encryBytes[i] = encryBytes[j];
            encryBytes[j] = temp;
            i = (byte)random.Next(256);
        }

        for (int j = 0; j < 256; j++)
        {
            decryBytes[encryBytes[j]] = (byte)j;
        }

    }

    public static byte[] DecryptDES(byte[] bytes)
    {
        byte[] tempBytes = new byte[bytes.Length];
        for (int i = 0; i < bytes.Length; i++)
        {
            tempBytes[i] = (byte)(decryBytes[bytes[i]]);
        }
        return tempBytes;
    }

    public static byte[] EncryptDES(byte[] bytes)
    {
        byte[] tempBytes = new byte[bytes.Length];
        for (int i = 0; i < bytes.Length; i++)
        {
            tempBytes[i] = (byte)(encryBytes[bytes[i]]);
        }
        return tempBytes;
    }

    //int
    public static byte[] DecryptIntDES(byte[] bytes, byte[] outbytes)
    {
        for (int i = 0; i < bytes.Length; i++)
        {
            outbytes[i] = (byte)(decryBytes[bytes[i]]);
        }
        return outbytes;
    }
    public static byte[] EncryptIntDES(byte[] bytes)
    {
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = (byte)(encryBytes[bytes[i]]);
        }
        return bytes;
    }

    public static void GetDeLs(List<EvalueInit> ls, ref List<int> returnLs)
    {
        returnLs.Clear();
        for (int i = 0; i < ls.Count; i++)
        {
            returnLs.Add(ls[i].m_value);
        }
    }

    public static void InitEnLs(List<int> ls, ref List<EvalueInit> returnLs)
    {
        returnLs.Clear();
        for (int i = 0; i < ls.Count; i++)
        {
            returnLs.Add(new EvalueInit(ls[i]));
        }
    }

    public static void GetDeLs(List<EvalueFloat> ls, ref List<float> returnLs)
    {
        returnLs.Clear();
        for (int i = 0; i < ls.Count; i++)
        {
            returnLs.Add(ls[i].m_value);
        }
    }

    public static void InitEnLs(List<float> ls, ref List<EvalueFloat> returnLs)
    {
        returnLs.Clear();
        for (int i = 0; i < ls.Count; i++)
        {
            returnLs.Add(new EvalueFloat(ls[i]));
        }
    }
}

public class EvalueListInt
{
    private List<EvalueInit> _m_value = new List<EvalueInit>();
    private List<int> _m_value_int = new List<int>();
    public List<int> m_value
    {
        get
        {
            EValue.GetDeLs(_m_value, ref _m_value_int);
            return _m_value_int;
        }
        set
        {
            EValue.InitEnLs(value, ref _m_value);
        }
    }

    public EvalueListInt()
    {
    }

    public EvalueListInt(List<int> value)
    {
        EValue.InitEnLs(value, ref _m_value);
    }
}

public class EvalueListFloat
{
    private List<EvalueFloat> _m_value = new List<EvalueFloat>();
    private List<float> _m_value_float = new List<float>();
    public List<float> m_value
    {
        get
        {
            EValue.GetDeLs(_m_value, ref _m_value_float);
            return _m_value_float;
        }
        set
        {
            EValue.InitEnLs(value, ref _m_value);
        }
    }

    public EvalueListFloat()
    {
    }

    public EvalueListFloat(List<float> value)
    {
        EValue.InitEnLs(value, ref _m_value);
    }
}

public struct EvalueInit
{
    private int _m_value;

    public EvalueInit(int value)
    {
        _m_value = XGame2DLL.EncryToInt32(value);
    }

    public int m_value
    {
        get
        {
            return XGame2DLL.DecryToInt32(_m_value);
        }
        set
        {
            _m_value = XGame2DLL.EncryToInt32(value);
        }
    }
}

public struct EvalueFloat
{
    private int _m_value;

    public EvalueFloat(float value)
    {
        _m_value = XGame2DLL.EncryToFloat(value);
    }

    public float m_value
    {
        get
        {
            //这么弄是为了少点内存啊
            return XGame2DLL.DecryToFloat(_m_value);
        }
        set
        {
            _m_value = XGame2DLL.EncryToFloat(value);
        }
    }
}




public struct EvalueLone
{
    private long _m_value;


    public EvalueLone(long value)
    {
        _m_value = XGame2DLL.EncryToLong(value);
    }

    public long m_value
    {
        get
        {
            //这么弄是为了少点内存啊
            return XGame2DLL.DecryToLong(_m_value);
        }
        set
        {
            _m_value = XGame2DLL.EncryToLong(value);
        }
    }
}


public struct EvalueBool
{
    private byte _m_value;


    public EvalueBool(bool value)
    {
        _m_value = XGame2DLL.EncryToBoolean(value ? (byte)99 : (byte)37);
    }

    public bool m_value
    {
        get
        {
            return XGame2DLL.DecryToBoolean(_m_value) == 99;
        }
        set
        {
            _m_value = XGame2DLL.EncryToBoolean(value ? (byte)99 : (byte)37);
        }
    }
}


public struct EvalueDouble
{
    private long _m_value;

    public EvalueDouble(double value)
    {
        _m_value = XGame2DLL.EncryToDouble(value);
    }

    public double m_value
    {
        get
        {
            return XGame2DLL.DecryToDouble(_m_value);
        }
        set
        {
            _m_value = XGame2DLL.EncryToDouble(value);
        }
    }
}

public struct EvalueVector2
{
    private EvalueFloat _m_x;
    private EvalueFloat _m_Y;

    public EvalueVector2(float _x, float _y)
    {
        _m_x = new EvalueFloat();
        _m_Y = new EvalueFloat();
        _m_x.m_value = _x;
        _m_Y.m_value = _y;
    }

    public float x
    {
        get { return _m_x.m_value; }
        set { _m_x.m_value = value; }
    }

    public float y
    {
        get { return _m_Y.m_value; }
        set { _m_Y.m_value = value; }
    }
}