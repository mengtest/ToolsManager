using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class DWIAPLib
{

#if UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern void InitUnlegalCountry(string list);
	public static void _InitUnlegalCountry(string list)
	{
		InitUnlegalCountry(list);
	}
	[DllImport("__Internal")]
	private static extern bool CanPurchase();
	public static bool _CanPurchase()
	{
		return CanPurchase();
	}
	[DllImport("__Internal")]
	private static extern bool PurchaseSucceed(string pid, string tid);
	public static bool _PurchaseSucceed(string pid, string  tid)
	{
		return PurchaseSucceed(pid, tid);
	}

	[DllImport("__Internal")]
	private static extern void BuyIAP(string id);
	public static void _BuyIAP(string id)
	{
		BuyIAP(id);
	}

	[DllImport("__Internal")]
	private static extern void Restore();
	public static void _Restore()
	{
		Restore();
	}
#else
    public static void _InitUnlegalCountry(string list)
	{
	}
    public static bool _CanPurchase()
	{
        return false;
	}

	public static bool _PurchaseSucceed(string pid, string  tid)
	{
        return false;
	}


	public static void _BuyIAP(string id)
	{
	}


	public static void _Restore()
	{
	}

#endif

 }
