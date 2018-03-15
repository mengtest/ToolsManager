#include "PreInclude.h"
#include "ActorAttribute.h"
#include "tea.h"
#include <cstdio>
#include <string.h>

extern "C"
{
	// C# 回调函数指针结构体
	ManagedCallbacks g_ManagedCallbacks;

	// Native 库初始化及 C# 回调函数注册
	NATIVE_LIB_DLL int NATIVE_LIB_API OnInitialize(ManagedCallbacks managedCallbacks)
	{
		g_ManagedCallbacks = managedCallbacks;
		InitAttribute();
		//返回 0 表示初始化成功
		return 0;
	}

	// Native 库释放及 C# 回调函数注销
	NATIVE_LIB_DLL void NATIVE_LIB_API OnUnInitialize()
	{
		ReleaseAllActorAttributes();

		memset(&g_ManagedCallbacks, 0, sizeof(ManagedCallbacks));
	}

	// Unity Update 函数调用
	NATIVE_LIB_DLL void NATIVE_LIB_API OnUpdate(float fDeltaTime)
	{
	}

	// Unity FixedUpdate 函数调用
	NATIVE_LIB_DLL void NATIVE_LIB_API OnFixedUpdate(float fDeltaTime)
	{
	}

	// Unity LateUpdate 函数调用
	NATIVE_LIB_DLL void NATIVE_LIB_API OnLateUpdate(float fDeltaTime)
	{
	}

	// Unity LateUpdate 函数调用
	NATIVE_LIB_DLL void NATIVE_LIB_API Test()
	{
		xxtea_long key[4] = { 0x68697071, 0x65646172, 0x6d6f635f, 0x6c697665 };
		char buf[64];
		//short i = 9535;
		//uint32_t len = sizeof(i);

		//uint8_t* dest = new uint8_t[len];
		//memcpy(dest, &i, len);

		//encryptBlock(dest, &len, key);
		////sprintf(buf, "%u", i);
		//g_ManagedCallbacks.DebugLog("123", 0);
		////decryptBlock((uint8_t*)&i, &len, key);
		////sprintf(buf, "%u", i);
		////g_ManagedCallbacks.DebugLog("456", 0);

		int i = 95653;
		xxtea_long v[2];
		v[0] = i;
		xxtea_long_encrypt(v, 2, key);
		xxtea_long v1[2];
		memcpy(v1, v, sizeof(v));
		xxtea_long_decrypt(v1, 2, key);
		sprintf(buf, "%d", (int)v1[0]);
		g_ManagedCallbacks.DebugLog(buf, 0);

		//decryptBlock(shellcode, &len, key);
		//g_ManagedCallbacks.DebugLog((const char*)shellcode, 0);
	}
}

void Test2()
{
	InitAttribute();
	unsigned int slot = CreateActorAttribute();
	int intValue = 123123123;
	SetActorIntAttribute(slot, 1, 1, intValue);
	int newIntValue = 0;
	GetActorIntAttribute(slot, 1, 1, &newIntValue);

	double doubleValue = 123123123.0;
	SetActorDoubleAttribute(slot, 1, 1, doubleValue);
	double newdoubleValue = 0;
	GetActorDoubleAttribute(slot, 1, 1, &newdoubleValue);

	unsigned int uIntValue = 123123123;
	SetActorUIntAttribute(slot, 1, 1, uIntValue);
	unsigned int newUIntValue = 0;
	GetActorUIntAttribute(slot, 1, 1, &newUIntValue);


	float ufloatValue = 123123123.0;
	SetActorFloatAttribute(slot, 1, 1, ufloatValue);
	float  newufloatValue = 0;
	GetActorFloatAttribute(slot, 1, 1, &newufloatValue);

	int64_t ulongValue = 123123123.0;
	SetActorLongAttribute(slot, 1, 1, ulongValue);
	int64_t  newlongValue = 0;
	GetActorLongAttribute(slot, 1, 1, &newlongValue);

	uint64_t uulongValue = 123123123.0;
	SetActorULongAttribute(slot, 1, 1, uulongValue);
	uint64_t  newulongValue = 0;
	GetActorULongAttribute(slot, 1, 1, &newulongValue);
}
