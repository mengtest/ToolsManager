#pragma once
#include "PreInclude.h"
#include "tea.h"
#ifndef DATA_LEN
#define DATA_LEN 2
#endif
#ifndef DATA_BYTE_LEN
#define DATA_BYTE_LEN 8
#endif

extern "C"
{
	extern xxtea_long DataValue[4];
	extern unsigned char encryBytes[256];
	extern unsigned char decryBytes[256];
	struct CryptoData
	{
		unsigned char m_Buffer[DATA_BYTE_LEN];
	};

	enum DataType
	{
		DataInt		= 0,
		DataUInt	,
		DataLong	,
		DataULong	,
		DataFloat	,
		DataDouble	,
		DataCount 
	};

	struct AttributeMap
	{
		map<int, CryptoData> m_MapAttribute[DataCount];
	};


	void InitAttribute();

	//创建角色属性对象（返回索引角色属性数据对象的 SlotID。）
	extern NATIVE_LIB_DLL unsigned int NATIVE_LIB_API CreateActorAttribute(); 

	//释放角色属性对象
	extern NATIVE_LIB_DLL void NATIVE_LIB_API ReleaseActorAttribute(unsigned int nSlot);

	//释放所有角色属性对象
	extern NATIVE_LIB_DLL void NATIVE_LIB_API ReleaseAllActorAttributes();

	//设置 int 类型属性值
	extern NATIVE_LIB_DLL bool NATIVE_LIB_API SetActorIntAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, int nValue);

	//获取 int 类型属性值
	extern NATIVE_LIB_DLL bool NATIVE_LIB_API GetActorIntAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, int* pValue);

	//设置 unsigned int 类型属性值
	extern NATIVE_LIB_DLL bool NATIVE_LIB_API SetActorUIntAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, unsigned int nValue);

	//获取 unsigned int 类型属性值
	extern NATIVE_LIB_DLL bool NATIVE_LIB_API GetActorUIntAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, unsigned int* pValue);

	//设置 long 类型属性值
	extern NATIVE_LIB_DLL bool NATIVE_LIB_API SetActorLongAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, int64_t nValue);

	//获取 long 类型属性值
	extern NATIVE_LIB_DLL bool NATIVE_LIB_API GetActorLongAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, int64_t* pValue);

	//设置 unsinged long 类型属性值
	extern NATIVE_LIB_DLL bool NATIVE_LIB_API SetActorULongAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, uint64_t nValue);

	//获取 unsinged long 类型属性值
	extern NATIVE_LIB_DLL bool NATIVE_LIB_API GetActorULongAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, uint64_t* pValue);

	//设置 float 类型属性值
	extern NATIVE_LIB_DLL bool NATIVE_LIB_API SetActorFloatAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, float nValue);

	//获取 float 类型属性值
	extern NATIVE_LIB_DLL bool NATIVE_LIB_API GetActorFloatAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, float* pValue);

	//设置 double 类型属性值
	extern NATIVE_LIB_DLL bool NATIVE_LIB_API SetActorDoubleAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, double nValue);

	//获取 double 类型属性值
	extern NATIVE_LIB_DLL bool NATIVE_LIB_API GetActorDoubleAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, double* pValue);
};

template <typename Type> __inline bool GetData(map<int, CryptoData>& refMap, int nAttributeType, int nAttributeSlot, Type* pValue)
{
	map<int, CryptoData>::iterator it = refMap.find(nAttributeSlot);
	if (it == refMap.end())
	{
		*pValue = 0;
		return false;
	}
	else
	{
		//用double做存储空间，可以多线程并发
		double tempData;
		unsigned char* bytes = (unsigned char*)&tempData;
		CryptoData& data = it->second;
		memcpy(bytes, data.m_Buffer, sizeof(tempData));
		for (int i = 0; i < DATA_BYTE_LEN; i++)
		{
			bytes[i] = (unsigned char)(decryBytes[bytes[i]]);
		}
		memcpy(pValue, bytes, sizeof(Type));
		return true;
	}
}

template <typename Type> __inline void SetData(map<int, CryptoData>& refMap, int nAttributeType, int nAttributeSlot, Type nValue)
{
	CryptoData& data = refMap[nAttributeSlot];
	memset(data.m_Buffer, 0, DATA_BYTE_LEN);
	memcpy(data.m_Buffer, &nValue, sizeof(Type));
	for (int i = 0; i < DATA_BYTE_LEN; i++)
	{
		data.m_Buffer[i] = (unsigned char)(encryBytes[data.m_Buffer[i]]);
	}
}


void __inline EncryBytes(unsigned char * bytes, int len)
{
	for (int i = 0; i < len; i++)
	{
		bytes[i] = (unsigned char)(encryBytes[bytes[i]]);
	}
}

void __inline DecryBytes(unsigned char* bytes, int len)
{
	for (int i = 0; i < len; i++)
	{
		bytes[i] = (unsigned char)(decryBytes[bytes[i]]);
	}
}