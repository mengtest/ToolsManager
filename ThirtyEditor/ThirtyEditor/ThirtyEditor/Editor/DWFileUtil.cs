using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;

public class DWFileUtil
{

    public static byte[] ReadFile(string filename)
    {
        int index = filename.IndexOf("/assets/");
        if (index > 0)
            filename = filename.Substring(index + "/assets/".Length);
        int strlen = LoadFileSize(filename);
        if (strlen == -1)
        {
            Debug.LogError("xgame2  nulll" + filename);
            return null;
        }
        byte[] data = new byte[strlen];
        LoadFileByData(filename, data);
        return data;
    }

    public static bool IsExsits(string filename)
    {
        int index = filename.IndexOf("/assets/");
        if (index > 0)
            filename = filename.Substring(index + "/assets/".Length);
        int strlen = LoadFileSize(filename);
        return strlen != -1;
    }

    [DllImport("XGame2File", CallingConvention = CallingConvention.Cdecl)]
    public static extern void LoadFileByData(string fileName, byte[] data);

    [DllImport("XGame2File", CallingConvention = CallingConvention.Cdecl)]
    public static extern int LoadFileSize(string fileName);

}
