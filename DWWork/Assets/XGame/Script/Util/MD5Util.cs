
using System.IO;
using System.Text;
using System.Security.Cryptography;

public class MD5Util
{
    static public string ComputeHash(Stream stream)
    {
        MD5 md5 = MD5.Create();
        return ConvertToStr(md5.ComputeHash(stream));
    }

    static public string ComputeHash(string str)
    {
        return ComputeHash(Encoding.UTF8.GetBytes(str));
    }

    static public string ComputeHash(byte[] buffer)
    {
        MD5 md5 = MD5.Create();
        return ConvertToStr(md5.ComputeHash(buffer));
    }

    static string ConvertToStr(byte[] md5Val)
    {
        string md5Str = "";

        for (int j = 0; j < md5Val.Length; ++j)
            md5Str += System.Convert.ToString(md5Val[j], 16).PadLeft(2, '0');

        return md5Str.PadLeft(32, '0');
    }
}