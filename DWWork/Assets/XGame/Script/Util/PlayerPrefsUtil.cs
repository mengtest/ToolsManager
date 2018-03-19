using UnityEngine;
using System.Collections;

public class PlayerDataRefUtil{
    public static void SetInt(string key ,int value)
    {
        PlayerPrefs.SetInt(key, value);
    }
    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }
    public static void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }

    public static int GetInt(string key,int defalutValue)
    {
        return PlayerPrefs.GetInt(key,defalutValue);
    }
    public static string GetString(string key,string defalutValue)
    {
        return PlayerPrefs.GetString(key,defalutValue);
    }
    public static float GetFloat(string key,float defalutValue)
    {
        return PlayerPrefs.GetFloat(key, defalutValue);
    }
    public static void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }
   
}
