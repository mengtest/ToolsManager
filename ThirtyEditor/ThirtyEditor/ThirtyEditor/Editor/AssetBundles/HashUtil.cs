

using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;


public class HashUtil
{

    static Dictionary<string, string> m_fileHashDic = new Dictionary<string, string>();

    public static void ClearHash()
    {
        m_fileHashDic.Clear();
    }

    public static string GetByPath(string path)
    {
        string fileHash;
        if (!m_fileHashDic.TryGetValue(path, out fileHash))
        {
            using (FileStream fs = new FileStream(path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read))
            {
                fileHash = Get(fs);
                m_fileHashDic.Add(path, fileHash);
            }
        }
        return fileHash;
    }

    public static string Get(Stream fs)
    {
        HashAlgorithm ha = HashAlgorithm.Create();
        byte[] bytes = ha.ComputeHash(fs);
        fs.Close();
        return ToHexString(bytes);
    }

    public static string Get(string s)
    {
        return Get(Encoding.UTF8.GetBytes(s));
    }

    public static string Get(byte[] data)
    {
        HashAlgorithm ha = HashAlgorithm.Create();
        byte[] bytes = ha.ComputeHash(data);
        return ToHexString(bytes);
    }

    public static string ToHexString(byte[] bytes)
    {
        string hexString = string.Empty;
        if (bytes != null)
        {
            StringBuilder strB = new StringBuilder();

            for (int i = 0; i < bytes.Length; i++)
            {
                strB.Append(bytes[i].ToString("X2"));
            }
            hexString = strB.ToString().ToLower();
        }
        return hexString;
    }
}
