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

	//������ɫ���Զ��󣨷���������ɫ�������ݶ���� SlotID����
	extern NATIVE_LIB_DLL unsigned int NATIVE_LIB_API CreateActorAttribute(); 

	//�ͷŽ�ɫ���Զ���
	extern NATIVE_LIB_DLL void NATIVE_LIB_API ReleaseActorAttribute(unsigned int nSlot);

	//�ͷ����н�ɫ���Զ���
	extern NATIVE_LIB_DLL void NATIVE_LIB_API ReleaseAllActorAttributes();

	//���� int ��������ֵ
	extern NATIVE_LIB_DLL bool NATIVE_LIB_API SetActorIntAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, int nValue);

	//��ȡ int ��������ֵ
	extern NATIVE_LIB_DLL bool NATIVE_LIB_API GetActorIntAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, int* pValue);

	//���� unsigned int ��������ֵ
	extern NATIVE_LIB_DLL bool NATIVE_LIB_API SetActorUIntAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, unsigned int nValue);

	//��ȡ unsigned int ��������ֵ
	extern NATIVE_LIB_DLL bool NATIVE_LIB_API GetActorUIntAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, unsigned int* pValue);

	//���� long ��������ֵ
	extern NATIVE_LIB_DLL bool NATIVE_LIB_API SetActorLongAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, int64_t nValue);

	//��ȡ long ��������ֵ
	extern NATIVE_LIB_DLL bool NATIVE_LIB_API GetActorLongAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, int64_t* pValue);

	//���� unsinged long ��������ֵ
	extern NATIVE_LIB_DLL bool NATIVE_LIB_API SetActorULongAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, uint64_t nValue);

	//��ȡ unsinged long ��������ֵ
	extern NATIVE_LIB_DLL bool NATIVE_LIB_API GetActorULongAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, uint64_t* pValue);

	//���� float ��������ֵ
	extern NATIVE_LIB_DLL bool NATIVE_LIB_API SetActorFloatAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, float nValue);

	//��ȡ float ��������ֵ
	extern NATIVE_LIB_DLL bool NATIVE_LIB_API GetActorFloatAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, float* pValue);

	//���� double ��������ֵ
	extern NATIVE_LIB_DLL bool NATIVE_LIB_API SetActorDoubleAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, double nValue);

	//��ȡ double ��������ֵ
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
		//��double���洢�ռ䣬���Զ��̲߳���
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