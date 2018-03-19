#include "ActorAttribute.h"
#include "time.h"

extern "C"
{
	typedef map<int, AttributeMap> ActorAttribute;
	typedef map<unsigned int, ActorAttribute*> MapActorAttribute;
	static MapActorAttribute g_MapActorAttributes;
	//角色属性 Slot 索引计数
	static unsigned int g_nSlotIndex = 0;
	static bool m_isInitMap = false;
	unsigned char encryBytes[256];
	unsigned char decryBytes[256];

	xxtea_long DataValue[4] = { 0x68607071, 0x65636151, 0x6d6f635f, 0x6c69726c };

	int Random(int max)
	{
		return rand() % (max);
	}

	void InitAttribute()
	{
		if (m_isInitMap)
		{
			return;
		}
		m_isInitMap = true;
		srand((unsigned)time(NULL));
		for (int j = 0; j < 256; j++)
		{
			encryBytes[j] = (unsigned char)j;
			decryBytes[j] = (unsigned char)j;
		}
		unsigned char i = (unsigned char)Random(256);
		unsigned char temp = 0;
		for (int j = 0; j < 256; j++)
		{
			temp = encryBytes[i];
			encryBytes[i] = encryBytes[j];
			encryBytes[j] = temp;
			i = (unsigned char)Random(256);
		}
		for (int j = 0; j < 256; j++)
		{
			decryBytes[encryBytes[j]] = (unsigned char)j;
		}
	}

	NATIVE_LIB_DLL unsigned int NATIVE_LIB_API CreateActorAttribute()
	{
		ActorAttribute* pAttribute = new ActorAttribute();
		if (pAttribute == NULL)
		{
#ifndef ENABLE_DEBUG_LOG
			char strMessage[256];
			DebugWarningFormat(strMessage, sizeof(strMessage), "Failed to new an attribute object of actor.");
#endif
			return 0;
		}

		const unsigned int nSlot = ++g_nSlotIndex;

		pair<MapActorAttribute::iterator, bool> result = g_MapActorAttributes.insert(pair<int, ActorAttribute*>(nSlot, pAttribute));
		if (!result.second)
		{
#ifndef ENABLE_DEBUG_LOG
			char strMessage[256];
			DebugWarningFormat(strMessage, sizeof(strMessage), "The actor (Slot = %d) attribute object has already been created, couldn't create it again.", nSlot);
#endif
		}

		return nSlot;
	}

	NATIVE_LIB_DLL void NATIVE_LIB_API ReleaseActorAttribute(unsigned int nSlot)
	{
		MapActorAttribute::iterator it = g_MapActorAttributes.find(nSlot);
		if (it == g_MapActorAttributes.end())
		{
			return;
		}

		ActorAttribute* pAttribute = it->second;
		SafeDelete(pAttribute);

		g_MapActorAttributes.erase(it);
	}

	NATIVE_LIB_DLL void NATIVE_LIB_API ReleaseAllActorAttributes()
	{
		MapActorAttribute::iterator iterCur = g_MapActorAttributes.begin();
		MapActorAttribute::iterator iterEnd = g_MapActorAttributes.end();
		for (; iterCur != iterEnd; ++iterCur)
		{
			ActorAttribute* pAttribute = iterCur->second;
			SafeDelete(pAttribute);
		}

		g_MapActorAttributes.clear();
	}

	//根据角色 ID 查找角色属性对象
	ActorAttribute* NATIVE_LIB_API _FindActorAttribute(unsigned int nSlot)
	{
		MapActorAttribute::iterator it = g_MapActorAttributes.find(nSlot);
		if (it == g_MapActorAttributes.end())
		{
#ifndef ENABLE_DEBUG_LOG
			char strMessage[256];
			DebugWarningFormat(strMessage, sizeof(strMessage), "Couldn't find out the attribute of actor (Slot = %d).", nSlot);
#endif
			return NULL;
		}

		ActorAttribute* pAttribute = it->second;
		return pAttribute;
	}

	NATIVE_LIB_DLL bool NATIVE_LIB_API SetActorIntAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, int nValue)
	{
		ActorAttribute* pAttribute = _FindActorAttribute(nSlot);
		if (pAttribute == NULL)
		{
			return false;
		}

		SetData<int>((*pAttribute)[nAttributeType].m_MapAttribute[DataInt], nAttributeType, nAttributeSlot, nValue);
		return true;
	}

	NATIVE_LIB_DLL bool NATIVE_LIB_API GetActorIntAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, int* pValue)
	{
		ActorAttribute* pAttribute = _FindActorAttribute(nSlot);
		if (pAttribute == NULL)
		{
			return false;
		}

		return GetData<int>((*pAttribute)[nAttributeType].m_MapAttribute[DataInt], nAttributeType, nAttributeSlot, pValue);
	}

	NATIVE_LIB_DLL bool NATIVE_LIB_API SetActorUIntAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, unsigned int nValue)
	{
		ActorAttribute* pAttribute = _FindActorAttribute(nSlot);
		if (pAttribute == NULL)
		{
			return false;
		}

		SetData<unsigned int>((*pAttribute)[nAttributeType].m_MapAttribute[DataUInt], nAttributeType, nAttributeSlot, nValue);
		return true;
	}

	NATIVE_LIB_DLL bool NATIVE_LIB_API GetActorUIntAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, unsigned int* pValue)
	{
		ActorAttribute* pAttribute = _FindActorAttribute(nSlot);
		if (pAttribute == NULL)
		{
			return false;
		}

		return GetData<unsigned int>((*pAttribute)[nAttributeType].m_MapAttribute[DataUInt], nAttributeType, nAttributeSlot, pValue);
	}

	NATIVE_LIB_DLL bool NATIVE_LIB_API SetActorLongAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, int64_t nValue)
	{
		ActorAttribute* pAttribute = _FindActorAttribute(nSlot);
		if (pAttribute == NULL)
		{
			return false;
		}

		SetData<int64_t>((*pAttribute)[nAttributeType].m_MapAttribute[DataLong], nAttributeType, nAttributeSlot, nValue);
		return true;
	}

	NATIVE_LIB_DLL bool NATIVE_LIB_API GetActorLongAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, int64_t* pValue)
	{
		ActorAttribute* pAttribute = _FindActorAttribute(nSlot);
		if (pAttribute == NULL)
		{
			return false;
		}

		return GetData<int64_t>((*pAttribute)[nAttributeType].m_MapAttribute[DataLong], nAttributeType, nAttributeSlot, pValue);
	}

	NATIVE_LIB_DLL bool NATIVE_LIB_API SetActorULongAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, uint64_t nValue)
	{
		ActorAttribute* pAttribute = _FindActorAttribute(nSlot);
		if (pAttribute == NULL)
		{
			return false;
		}

		SetData<uint64_t>((*pAttribute)[nAttributeType].m_MapAttribute[DataULong], nAttributeType, nAttributeSlot, nValue);
		return true;
	}

	NATIVE_LIB_DLL bool NATIVE_LIB_API GetActorULongAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, uint64_t* pValue)
	{
		ActorAttribute* pAttribute = _FindActorAttribute(nSlot);
		if (pAttribute == NULL)
		{
			return false;
		}

		return GetData<uint64_t>((*pAttribute)[nAttributeType].m_MapAttribute[DataULong], nAttributeType, nAttributeSlot, pValue);
	}

	NATIVE_LIB_DLL bool NATIVE_LIB_API SetActorFloatAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, float nValue)
	{
		ActorAttribute* pAttribute = _FindActorAttribute(nSlot);
		if (pAttribute == NULL)
		{
			return false;
		}

		SetData<float>((*pAttribute)[nAttributeType].m_MapAttribute[DataFloat], nAttributeType, nAttributeSlot, nValue);
		return true;
	}

	NATIVE_LIB_DLL bool NATIVE_LIB_API GetActorFloatAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, float* pValue)
	{
		ActorAttribute* pAttribute = _FindActorAttribute(nSlot);
		if (pAttribute == NULL)
		{
			return false;
		}

		return GetData<float>((*pAttribute)[nAttributeType].m_MapAttribute[DataFloat], nAttributeType, nAttributeSlot, pValue);
	}

	NATIVE_LIB_DLL bool NATIVE_LIB_API SetActorDoubleAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, double nValue)
	{
		ActorAttribute* pAttribute = _FindActorAttribute(nSlot);
		if (pAttribute == NULL)
		{
			return false;
		}

		SetData<double>((*pAttribute)[nAttributeType].m_MapAttribute[DataDouble], nAttributeType, nAttributeSlot, nValue);
		return true;
	}

	NATIVE_LIB_DLL bool NATIVE_LIB_API GetActorDoubleAttribute(unsigned int nSlot, int nAttributeType, int nAttributeSlot, double* pValue)
	{
		ActorAttribute* pAttribute = _FindActorAttribute(nSlot);
		if (pAttribute == NULL)
		{
			return false;
		}

		return GetData<double>((*pAttribute)[nAttributeType].m_MapAttribute[DataDouble], nAttributeType, nAttributeSlot, pValue);

	}
};
