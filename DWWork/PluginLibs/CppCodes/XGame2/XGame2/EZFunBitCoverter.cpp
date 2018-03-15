#include "PreInclude.h"
#include "ActorAttribute.h"
#include "tea.h"
#include <cstdio>
#include <string.h>

extern "C"
{
	bool AmILittleEndian()
	{
		// binary representations of 1.0:
		// big endian: 3f f0 00 00 00 00 00 00
		// little endian: 00 00 00 00 00 00 f0 3f
		// arm fpa little endian: 00 00 f0 3f 00 00 00 00
		double d = 1.0;
		char* b = (char*)&d;
		return (b[0] == 0);
	}

	bool DoubleWordsAreSwapped()
	{
		// binary representations of 1.0:
		// big endian: 3f f0 00 00 00 00 00 00
		// little endian: 00 00 00 00 00 00 f0 3f
		// arm fpa little endian: 00 00 f0 3f 00 00 00 00
		double d = 1.0;
		char* b = (char*)&d;
		return b[2] == 0xf0;
	}

	NATIVE_LIB_DLL int 		NATIVE_LIB_API  	InitBitCoverter()
	{
		InitAttribute();
		return 0;
	}

	void PutBytes(unsigned char* dst, unsigned char* src, int start_index, int count)
	{
		for (int i = 0; i < count; i++)
			dst[i] = src[i + start_index];
	}

	void GetBytes(unsigned char* ptr, int count, unsigned char* ret)
	{
		for (int i = 0; i < count; i++)
		{
			ret[i] = ptr[i];
		}
	}

	NATIVE_LIB_DLL int NATIVE_LIB_API  ToInt32(unsigned char* pszBuffer)
	{
		int ret;
		PutBytes((unsigned char*)&ret, pszBuffer, 0, 4);
		return ret;
	}

	NATIVE_LIB_DLL bool NATIVE_LIB_API ToBoolean(unsigned char* value, int startIndex)
	{
		if (value[startIndex] != 0)
			return true;
		return false;
	}

	NATIVE_LIB_DLL float NATIVE_LIB_API  ToFloat(unsigned char* pszBuffer)
	{
		float ret;
		PutBytes((unsigned char*)&ret, pszBuffer, 0, 4);
		return ret;
	}

	NATIVE_LIB_DLL double NATIVE_LIB_API  ToDouble(unsigned char* pszBuffer)
	{
		double ret;
		if (DoubleWordsAreSwapped())
		{
			unsigned char* p = (unsigned char*)&ret;
			p[0] = pszBuffer[4];
			p[1] = pszBuffer[5];
			p[2] = pszBuffer[6];
			p[3] = pszBuffer[7];
			p[4] = pszBuffer[0];
			p[5] = pszBuffer[1];
			p[6] = pszBuffer[2];
			p[7] = pszBuffer[3];
			return ret;
		}
		PutBytes((unsigned char*)&ret, pszBuffer, 0, 8);
		return ret;
	}

	NATIVE_LIB_DLL int64_t NATIVE_LIB_API  ToLong(unsigned char* pszBuffer)
	{
		int64_t ret;
		PutBytes((unsigned char*)&ret, pszBuffer, 0, 8);
		return ret;
	}



	NATIVE_LIB_DLL void NATIVE_LIB_API GetBoolBytes(bool value, unsigned char *ret)
	{
		GetBytes((unsigned char*)&value, 1, ret);
	}

	NATIVE_LIB_DLL void NATIVE_LIB_API GetIntBytes(int value, unsigned char* ret)
	{
		return GetBytes((unsigned char*)&value, 4, ret);
	}

	NATIVE_LIB_DLL void NATIVE_LIB_API GetLongBytes(int64_t value, unsigned char* ret)
	{
		return GetBytes((unsigned char*)&value, 8, ret);
	}


	NATIVE_LIB_DLL void NATIVE_LIB_API GetFloatBytes(float value, unsigned char* ret)
	{
		return GetBytes((unsigned char*)&value, 4, ret);
	}

	NATIVE_LIB_DLL void NATIVE_LIB_API GetDoubleBytes(double value, unsigned char * ret)
	{
		if (DoubleWordsAreSwapped())
		{
			unsigned char* p = (unsigned char*)&value;
			ret[0] = p[4];
			ret[1] = p[5];
			ret[2] = p[6];
			ret[3] = p[7];
			ret[4] = p[0];
			ret[5] = p[1];
			ret[6] = p[2];
			ret[7] = p[3];
		}
		else
		{
			GetBytes((unsigned char*)&value, 8, ret);
		}
	}



	void PutAndDecrypteBytes(unsigned char* dst, unsigned char* src, int start_index, int count)
	{
		for (int i = 0; i < count; i++)
			dst[i] = src[i + start_index];
		DecryBytes(dst, count);
	}

	void GetAndEncrypteBytes(unsigned char* ptr, int count, unsigned char* ret)
	{
		for (int i = 0; i < count; i++)
		{
			ret[i] = ptr[i];
		}
		EncryBytes(ret, count);
	}


	NATIVE_LIB_DLL int NATIVE_LIB_API  DecrypteToInt32(unsigned char* pszBuffer)
	{
		int ret;
		PutAndDecrypteBytes((unsigned char*)&ret, pszBuffer, 0, 4);
		return ret;
	}

	NATIVE_LIB_DLL bool NATIVE_LIB_API DecrypteToBoolean(unsigned char* pszBuffer)
	{
		unsigned char ret = 0;
		PutAndDecrypteBytes((unsigned char*)&ret, pszBuffer, 0, 1);
		return false;
	}

	NATIVE_LIB_DLL float NATIVE_LIB_API  DecrypteToFloat(unsigned char* pszBuffer)
	{
		float ret;
		PutAndDecrypteBytes((unsigned char*)&ret, pszBuffer, 0, 4);
		return ret;
	}

	NATIVE_LIB_DLL double NATIVE_LIB_API  DecrypteToDouble(unsigned char* pszBuffer)
	{
		double ret;
		if (DoubleWordsAreSwapped())
		{
			unsigned char* p = (unsigned char*)&ret;
			p[0] = pszBuffer[4];
			p[1] = pszBuffer[5];
			p[2] = pszBuffer[6];
			p[3] = pszBuffer[7];
			p[4] = pszBuffer[0];
			p[5] = pszBuffer[1];
			p[6] = pszBuffer[2];
			p[7] = pszBuffer[3];
			DecryBytes(p, 8);
			return ret;
		}
		PutAndDecrypteBytes((unsigned char*)&ret, pszBuffer, 0, 8);
		return ret;
	}

	NATIVE_LIB_DLL int64_t NATIVE_LIB_API  DecrypteToLong(unsigned char* pszBuffer)
	{
		int64_t ret;
		PutAndDecrypteBytes((unsigned char*)&ret, pszBuffer, 0, 8);
		return ret;
	}

	NATIVE_LIB_DLL void NATIVE_LIB_API EncrypteGetBoolBytes(bool value, unsigned char *ret)
	{
		GetAndEncrypteBytes((unsigned char*)&value, 1, ret);
	}

	NATIVE_LIB_DLL void NATIVE_LIB_API EncrypteGetIntBytes(int value, unsigned char* ret)
	{
		return GetAndEncrypteBytes((unsigned char*)&value, 4, ret);
	}

	NATIVE_LIB_DLL void NATIVE_LIB_API EncrypteGetLongBytes(int64_t value, unsigned char* ret)
	{
		return GetAndEncrypteBytes((unsigned char*)&value, 8, ret);
	}


	NATIVE_LIB_DLL void NATIVE_LIB_API EncrypteGetFloatBytes(float value,unsigned char* ret)
	{
		return GetAndEncrypteBytes((unsigned char*)&value, 4, ret);
	}

	NATIVE_LIB_DLL void NATIVE_LIB_API EncrypteGetDoubleBytes(double value, unsigned char * ret)
	{
		if (DoubleWordsAreSwapped())
		{
			unsigned char* p = (unsigned char*)&value;
			ret[0] = p[4];
			ret[1] = p[5];
			ret[2] = p[6];
			ret[3] = p[7];
			ret[4] = p[0];
			ret[5] = p[1];
			ret[6] = p[2];
			ret[7] = p[3];
			EncryBytes(ret, 8);
		}
		else
		{
			GetAndEncrypteBytes((unsigned char*)&value, 8, ret);
		}
	}

	void GetByteAndDecrypte(unsigned char* dst, int count)
	{
		DecryBytes(dst, count);
	}

	void GetBytesEncrypteBytes(unsigned char* ptr, int count)
	{
		EncryBytes(ptr, count);
	}

	NATIVE_LIB_DLL int 		NATIVE_LIB_API  	DecryToInt32(int value)
	{
		GetByteAndDecrypte((unsigned char*)&value, 4);
		return value;
	}
	NATIVE_LIB_DLL unsigned char 		NATIVE_LIB_API 		DecryToBoolean(unsigned char value)
	{
		GetByteAndDecrypte((unsigned char*)&value, 1);
		return value;
	}
	NATIVE_LIB_DLL float 	NATIVE_LIB_API  	DecryToFloat(int value)
	{
		GetByteAndDecrypte((unsigned char*)&value, 4);
		return *((float*)&value);
	}
	NATIVE_LIB_DLL double 	NATIVE_LIB_API  	DecryToDouble(int64_t value)
	{
		GetByteAndDecrypte((unsigned char*)&value, 8);
		return *((double*)&value);
	}
	NATIVE_LIB_DLL int64_t 	NATIVE_LIB_API  	DecryToLong(int64_t value)
	{
		GetByteAndDecrypte((unsigned char*)&value, 8);
		return value;
	}
	NATIVE_LIB_DLL unsigned char 		NATIVE_LIB_API 		EncryToBoolean(unsigned char value)
	{
		GetBytesEncrypteBytes((unsigned char*)&value, 1);
		return value;
	}
	NATIVE_LIB_DLL int 		NATIVE_LIB_API 		EncryToInt32(int value)
	{
		GetBytesEncrypteBytes((unsigned char*)&value, 4);
		return value;
	}
	NATIVE_LIB_DLL int64_t 	NATIVE_LIB_API 		EncryToLong(int64_t value)
	{
		GetBytesEncrypteBytes((unsigned char*)&value, 8);
		return value;
	}
	NATIVE_LIB_DLL int 	NATIVE_LIB_API 		EncryToFloat(float value)
	{
		GetBytesEncrypteBytes((unsigned char*)&value, 4);
		return *((int*)&value);
	}
	NATIVE_LIB_DLL int64_t 	NATIVE_LIB_API 		EncryToDouble(double value)
	{
		GetBytesEncrypteBytes((unsigned char*)&value, 8);
		return *((int64_t*)&value);
	}
}