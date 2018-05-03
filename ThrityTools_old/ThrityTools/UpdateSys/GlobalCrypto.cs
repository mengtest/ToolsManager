/************************************************************
     file      : GlobalCrypto.cs
     author    : blackzhou 
     version   : 1.0
     date      : 2016/07/05.
     copyright : Copyright 2016 EZFun Inc.
**************************************************************/
using System;
using System.Security.Cryptography;
using System.IO;
using ComponentAce.Compression.Libs.zlib;

public class GlobalCrypto
{
    private static NetCrypte m_netCrypte = new NetCrypte();
    private static byte[] m_tempBuffer = new byte[4 * 1024];

    static GlobalCrypto()
    {
        m_netCrypte.pkgLen = 4;
        m_netCrypte.isCrypte = true;
#if UNITY_EDITOR
        InitCry("dd7fd4a156d28bade96f816db1d18609", "dd7fd4a156d28bade96f816db1d18609");
#endif
    }

    public static void InitCry(string iv, string key)
    {
        m_netCrypte.InitAes();
        m_netCrypte.setIV(iv);
        m_netCrypte.setKey(key);
    }

    public static byte[] LoadDLL(Stream stream)
    {
        if (stream == null)
        {
            return null;
        }
        byte[] lenBytes = new byte[4];
        stream.Read(lenBytes, 0, 4);
        int len =  System.Net.IPAddress.NetworkToHostOrder((int)BitConverter.ToUInt32(lenBytes, 0));
        stream.Read(lenBytes, 0, 4);
        int decryLen = System.Net.IPAddress.NetworkToHostOrder((int)BitConverter.ToUInt32(lenBytes, 0));
        var trans = m_netCrypte.GetCryptoTransform();
        byte[] srcBytes = new byte[len];
        byte[] dbytes = new byte[len];
        stream.Read(srcBytes, 0, (int)stream.Length - 8);
        trans.TransformBlock(srcBytes, 0, (int)stream.Length - 8, dbytes, 0);

        MemoryStream ms = new MemoryStream(dbytes, 0, decryLen, false);
        var zos = new ZInputStream(ms);
        MemoryStream output = new MemoryStream(srcBytes, true);
        int tlen = 0;
        while ((tlen = zos.read(m_tempBuffer, 0, m_tempBuffer.Length)) > 0)
        {
            output.Write(m_tempBuffer, 0, tlen);
        };
        zos.Close();
        return srcBytes;
    }

    private static byte[] Decompress(byte[] input, int ilen = 0)
    {
        MemoryStream ms = new MemoryStream(input, 0, ilen == 0 ? input.Length : ilen, false);
        var zos = new ZInputStream(ms);
        MemoryStream output = new MemoryStream(input.Length * 5);
        int len = 0;
        while ((len = zos.read(m_tempBuffer, 0, m_tempBuffer.Length)) > 0)
        {
            output.Write(m_tempBuffer, 0, len);
        };
        zos.Close();

        return output.ToArray();
    }

    public static byte[] Decompress(byte[] input)
    {
        return Decompress(input, 0);
    }

    public static byte[] Encrypte(byte[] inByte)
    {
        byte[] outByte = m_netCrypte.Encrypte(inByte);
        return outByte;
    }

    public static byte[] Decrypte(byte[] b)
    {
        byte[] zdata = m_netCrypte.Decrypte(b);
        return zdata;
    }

    public static byte[] Decrypte(byte[] b, int inoffset, int inlength)
    {
        byte[] zdata = m_netCrypte.Decrypte(b, inoffset, inlength);
        return zdata;
    }

    public static byte[] Decrypte(byte[] b, out int len)
    {
        byte[] zdata = m_netCrypte.Decrypte(b, out len);
        return zdata;
    }
}


