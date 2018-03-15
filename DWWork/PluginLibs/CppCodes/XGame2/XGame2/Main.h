#pragma once

#include "PreInclude.h"

#define MAX(a,b) a>b ? a : b
#define MIN(a,b) a<b ? a : b

extern "C"
{
	//日志输出类型
	const int LOG_LEVEL_INFO = 0;
	const int LOG_LEVEL_WARNING = 1;
	const int LOG_LEVEL_ERROR = 2;

	// C# 回调函数指针类型声明

	//输出调试日志
	typedef void (NATIVE_LIB_API *FnDebugLog)(const char* pszLog, int nLogLevel);

	//释放 C# Managed Object GCHandle
	typedef void (NATIVE_LIB_API *FnFreeObjectHandle)(intptr_t ptrObject);

	//激活 GameObject
	typedef void (NATIVE_LIB_API *FnGameObjectSetActive)(intptr_t ptrGameObject, bool bActive);

	//销毁 GameObject
	typedef void (NATIVE_LIB_API *FnGameObjectDestroy)(intptr_t ptrGameObject);

	// C# 回调函数指针结构体
	struct NATIVE_LIB_DLL ManagedCallbacks
	{
		FnDebugLog				DebugLog;
		FnFreeObjectHandle		FreeObjectHandle;
		FnGameObjectSetActive	GameObjectSetActive;
		FnGameObjectDestroy		GameObjectDestroy;
	};

	// C# 回调函数指针结构体
	extern NATIVE_LIB_DLL ManagedCallbacks g_ManagedCallbacks;

	// Native 库初始化及 C# 回调函数注册
	extern NATIVE_LIB_DLL int NATIVE_LIB_API OnInitialize(ManagedCallbacks managedCallbacks);

	// Native 库释放及 C# 回调函数注销
	extern NATIVE_LIB_DLL void NATIVE_LIB_API OnUnInitialize();

	// Unity Update 函数调用
	extern NATIVE_LIB_DLL void NATIVE_LIB_API OnUpdate(float fDeltaTime);

	// Unity FixedUpdate 函数调用
	extern NATIVE_LIB_DLL void NATIVE_LIB_API OnFixedUpdate(float fDeltaTime);

	// Unity LateUpdate 函数调用
	extern NATIVE_LIB_DLL void NATIVE_LIB_API OnLateUpdate(float fDeltaTime);

	// Unity LateUpdate 函数调用
	extern NATIVE_LIB_DLL void NATIVE_LIB_API Test();

	void Test2();
}
