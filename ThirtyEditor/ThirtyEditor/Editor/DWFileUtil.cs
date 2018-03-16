using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;

public class DWFileUtil
{

    public static byte[] ReadFileStream(string path)
    {
        byte[] b = null;
        if (File.Exists(path))
        {
            using (Stream file = File.OpenRead(path))
            {
                b = new byte[(int)file.Length];
                file.Read(b, 0, b.Length);
                file.Close();
                file.Dispose();
            }
        }
        if (b == null)
            Debug.LogError("ReadFileStream Read file failed! " + path);
        return b;
    }

}
