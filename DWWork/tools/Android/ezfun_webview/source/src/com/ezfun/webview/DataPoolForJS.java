package com.ezfun.webview;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import android.content.Context;
import android.widget.Toast;

public class DataPoolForJS {

	private Context mContext;

	public DataPoolForJS(Context context){
		this.mContext = context;
	}
	
	//webview�е���toastԭ�����
	public void showToast(String toast) {
		Toast.makeText(mContext, toast, Toast.LENGTH_SHORT).show();
	}
	
	//��jsonʵ��webview��js֮������ݽ���
	public String jsontohtml(){
		JSONObject map;
		JSONArray array = new JSONArray();
		try {
			map = new JSONObject();
			map.put("name","aaron");
			map.put("age", 25);
			map.put("address", "�й��Ϻ�");
			array.put(map);
			
			map = new JSONObject();
			map.put("name","jacky");
			map.put("age", 22);
			map.put("address", "�й�����");
			array.put(map);
			
			map = new JSONObject();
			map.put("name","vans");
			map.put("age", 26);
			map.put("address", "�й�����");
			map.put("phone","13888888888");
			array.put(map);
		} catch (JSONException e) {
			e.printStackTrace();
		}
		return array.toString();
	}
	
	private int mActivityType = 0;
	private int mActivityID = 0;
	
	public void setActivityType(int type)
	{
		this.mActivityType = type;
	}
	
	public void setActivityID(int id)
	{
		mActivityID = id;
	}

	public int getActivityType()
	{
		return mActivityType;
	}

	public int getActivityID()
	{
		return mActivityID;
	}
}

