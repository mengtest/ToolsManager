using System;
using System.Collections;
using System.Collections.Generic;

public class DWPlatformInfo
{
	
	static public int rssi = 0;//wifi信号强度
	static public int netType = 0;//网络类型
	static public double battery = 0;//电池电量
	static public string deviceid = "";//设备id
	static public string  country = "";//国家
	static public string  countrycode = "";//国别码
	static public string  subLocality = "";//区
	static public string  street = "";//街道
	static public string  state = "";//省
	static public string  name = "";//具体名称，如果没有局势街道地址
	static public string  latitude = "";//纬度
	static public string  longitude = "";//经度
	static public string  thoroughfare = "";//路
	static public string  subThoroughfare = "";//号
	static public string  city = "";//市
	static public string  formattedAddressLine = "";//全地址
}