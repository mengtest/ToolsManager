using System;
using LuaInterface;

public class TimerSysWrap
{
	public static LuaMethod[] regs = new LuaMethod[]
	{
		new LuaMethod("Init", Init),
		new LuaMethod("Release", Release),
		new LuaMethod("GetLocalTime", GetLocalTime),
		new LuaMethod("GetLocalMilliseconds", GetLocalMilliseconds),
		new LuaMethod("Add5AMEvent", Add5AMEvent),
		new LuaMethod("Reset", Reset),
		new LuaMethod("Update", Update),
		new LuaMethod("GetCurrentHMSTime", GetCurrentHMSTime),
		new LuaMethod("GetCurrentDateTime", GetCurrentDateTime),
		new LuaMethod("GetCurrentDoubleTime", GetCurrentDoubleTime),
		new LuaMethod("GetCurrentDateMTimes", GetCurrentDateMTimes),
		new LuaMethod("UtcNeedSeconds", UtcNeedSeconds),
		new LuaMethod("GetCurrentDateTimeLong", GetCurrentDateTimeLong),
		new LuaMethod("GetCurrentDateTimeDouble", GetCurrentDateTimeDouble),
		new LuaMethod("GetCurrentDate", GetCurrentDate),
		new LuaMethod("GetaServerUtcStart", GetaServerUtcStart),
		new LuaMethod("GetCurDateTimeLocalLong", GetCurDateTimeLocalLong),
		new LuaMethod("GetTimeStr", GetTimeStr),
		new LuaMethod("GetCurrentTimeStrHM", GetCurrentTimeStrHM),
		new LuaMethod("GetCurrentTimeStrMin", GetCurrentTimeStrMin),
		new LuaMethod("GetCurrentTimeStrHour", GetCurrentTimeStrHour),
		new LuaMethod("GetCurrentTimeStr", GetCurrentTimeStr),
		new LuaMethod("GetCurrentWeekOfDay", GetCurrentWeekOfDay),
		new LuaMethod("GetCurrentYear", GetCurrentYear),
		new LuaMethod("GetCurrentMonth", GetCurrentMonth),
		new LuaMethod("GetCurrentDay", GetCurrentDay),
		new LuaMethod("GetMonthAddSecends", GetMonthAddSecends),
		new LuaMethod("GetDayAddSecends", GetDayAddSecends),
		new LuaMethod("UpdateCurrentDataTime", UpdateCurrentDataTime),
		new LuaMethod("BeginSendHeart", BeginSendHeart),
		new LuaMethod("AddTimerEventByDestTime", AddTimerEventByDestTime),
		new LuaMethod("AddTimerEventByLeftTime", AddTimerEventByLeftTime),
		new LuaMethod("AddRepeatTimerEvent", AddRepeatTimerEvent),
		new LuaMethod("AddWaitUntilEvent", AddWaitUntilEvent),
		new LuaMethod("RemoveTimerEvt", RemoveTimerEvt),
		new LuaMethod("AddFrameEvtByLeftFrame", AddFrameEvtByLeftFrame),
		new LuaMethod("RemoveFrameEvtByLeftFrame", RemoveFrameEvtByLeftFrame),
		new LuaMethod("RemoveWaitUntilEvent", RemoveWaitUntilEvent),
		new LuaMethod("GetDateByUtcTime", GetDateByUtcTime),
		new LuaMethod("GetUtcTime", GetUtcTime),
		new LuaMethod("New", _CreateTimerSys),
		new LuaMethod("GetClassType", GetClassType),
	};

	static LuaField[] fields = new LuaField[]
	{
		new LuaField("m_seq_", get_m_seq_, set_m_seq_),
		new LuaField("UTCZone", get_UTCZone, set_UTCZone),
	};

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateTimerSys(IntPtr L)
	{
		int count = LuaDLL.lua_gettop(L);

		if (count == 0)
		{
			TimerSys obj = new TimerSys();
			LuaScriptMgr.PushObject(L, obj);
			return 1;
		}
		else
		{
			LuaDLL.luaL_error(L, "invalid arguments to method: TimerSys.New");
		}

		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, typeof(TimerSys));
		return 1;
	}

	public static void Register(IntPtr L)
	{
		LuaScriptMgr.RegisterLib(L, "TimerSys", typeof(TimerSys), regs, fields, "TCoreSystem`1[TimerSys]");
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_seq_(IntPtr L)
	{
		LuaScriptMgr.Push(L, TimerSys.m_seq_);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_UTCZone(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name UTCZone");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index UTCZone on a nil value");
			}
		}

		TimerSys obj = (TimerSys)o;
		LuaScriptMgr.Push(L, obj.UTCZone);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_seq_(IntPtr L)
	{
		TimerSys.m_seq_ = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_UTCZone(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);

		if (o == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name UTCZone");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index UTCZone on a nil value");
			}
		}

		TimerSys obj = (TimerSys)o;
		obj.UTCZone = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Init(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		obj.Init();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Release(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		obj.Release();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetLocalTime(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		double o = obj.GetLocalTime();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetLocalMilliseconds(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		double o = obj.GetLocalMilliseconds();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Add5AMEvent(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		obj.Add5AMEvent();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Reset(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		obj.Reset();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Update(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		obj.Update();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCurrentHMSTime(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		int o = obj.GetCurrentHMSTime();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCurrentDateTime(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		int o = obj.GetCurrentDateTime();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCurrentDoubleTime(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		double o = obj.GetCurrentDoubleTime();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCurrentDateMTimes(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		long o = obj.GetCurrentDateMTimes();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int UtcNeedSeconds(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		int o = obj.UtcNeedSeconds();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCurrentDateTimeLong(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		long o = obj.GetCurrentDateTimeLong();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCurrentDateTimeDouble(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		string o = obj.GetCurrentDateTimeDouble();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCurrentDate(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		DateTime o = obj.GetCurrentDate();
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetaServerUtcStart(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		DateTime o = obj.GetaServerUtcStart();
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCurDateTimeLocalLong(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		long o = obj.GetCurDateTimeLocalLong();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetTimeStr(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
		string o = obj.GetTimeStr(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCurrentTimeStrHM(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		string o = obj.GetCurrentTimeStrHM();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCurrentTimeStrMin(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		string o = obj.GetCurrentTimeStrMin();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCurrentTimeStrHour(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		string o = obj.GetCurrentTimeStrHour();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCurrentTimeStr(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		string o = obj.GetCurrentTimeStr();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCurrentWeekOfDay(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		int o = obj.GetCurrentWeekOfDay();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCurrentYear(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		int o = obj.GetCurrentYear();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCurrentMonth(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		int o = obj.GetCurrentMonth();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetCurrentDay(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		int o = obj.GetCurrentDay();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetMonthAddSecends(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
		int o = obj.GetMonthAddSecends(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetDayAddSecends(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
		int o = obj.GetDayAddSecends(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int UpdateCurrentDataTime(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		long arg0 = (long)LuaScriptMgr.GetNumber(L, 2);
		obj.UpdateCurrentDataTime(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int BeginSendHeart(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		obj.BeginSendHeart();
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddTimerEventByDestTime(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		TimerSys.CallBack arg0 = LuaScriptMgr.GetNetObject<TimerSys.CallBack>(L, 2);
		double arg1 = (double)LuaScriptMgr.GetNumber(L, 3);
		int o = obj.AddTimerEventByDestTime(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddTimerEventByLeftTime(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		TimerSys.CallBack arg0 = LuaScriptMgr.GetNetObject<TimerSys.CallBack>(L, 2);
		float arg1 = (float)LuaScriptMgr.GetNumber(L, 3);
		int o = obj.AddTimerEventByLeftTime(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddRepeatTimerEvent(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 5);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		TimerSys.CallBack arg0 = LuaScriptMgr.GetNetObject<TimerSys.CallBack>(L, 2);
		float arg1 = (float)LuaScriptMgr.GetNumber(L, 3);
		int arg2 = (int)LuaScriptMgr.GetNumber(L, 4);
		bool arg3 = LuaScriptMgr.GetBoolean(L, 5);
		int o = obj.AddRepeatTimerEvent(arg0,arg1,arg2,arg3);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddWaitUntilEvent(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		TimerSys.CallBack arg0 = LuaScriptMgr.GetNetObject<TimerSys.CallBack>(L, 2);
		Func<bool> arg1 = LuaScriptMgr.GetNetObject<Func<bool>>(L, 3);
		int o = obj.AddWaitUntilEvent(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RemoveTimerEvt(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
		obj.RemoveTimerEvt(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddFrameEvtByLeftFrame(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		TimerSys.CallBack arg0 = LuaScriptMgr.GetNetObject<TimerSys.CallBack>(L, 2);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
		int o = obj.AddFrameEvtByLeftFrame(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RemoveFrameEvtByLeftFrame(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
		obj.RemoveFrameEvtByLeftFrame(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RemoveWaitUntilEvent(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
		obj.RemoveWaitUntilEvent(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetDateByUtcTime(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		int arg0 = (int)LuaScriptMgr.GetNumber(L, 2);
		DateTime o = obj.GetDateByUtcTime(arg0);
		LuaScriptMgr.PushValue(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetUtcTime(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 5);
		TimerSys obj = LuaScriptMgr.GetNetObject<TimerSys>(L, 1);
		bool arg0 = LuaScriptMgr.GetBoolean(L, 2);
		int arg1 = (int)LuaScriptMgr.GetNumber(L, 3);
		int arg2 = (int)LuaScriptMgr.GetNumber(L, 4);
		int arg3 = (int)LuaScriptMgr.GetNumber(L, 5);
		int o = obj.GetUtcTime(arg0,arg1,arg2,arg3);
		LuaScriptMgr.Push(L, o);
		return 1;
	}
}

