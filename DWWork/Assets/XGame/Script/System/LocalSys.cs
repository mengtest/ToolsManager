using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 各种本地化的参数都放在这儿
/// </summary>
public class LocalSys
{
    private LocalSys()
    {
    }

    private static LocalSys _instance;

    public static LocalSys Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LocalSys();
            }
            return _instance;
        }
    }

    /// <summary>
    /// 主界面下面的系统的行距
    /// </summary>
    public int MainUIBottomWidth { get; set; }

    public bool IsVTC { get; set; }

    public void Init(int platId)
    {
        IsVTC = false; ;
        MainUIBottomWidth = 74;
    }


    public bool CheckIsPrePublish()
    {
		return false;
        //return Version.Instance.m_isPrePublish;
    }
}
