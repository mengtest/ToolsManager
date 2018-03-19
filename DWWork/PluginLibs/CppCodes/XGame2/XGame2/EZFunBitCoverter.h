#include "PreInclude.h"
#include "ActorAttribute.h"
#include "tea.h"
#include <cstdio>
#include <string.h>


extern "C"
{
	extern NATIVE_LIB_DLL int 		NATIVE_LIB_API  	InitBitCoverter();

	extern NATIVE_LIB_DLL int 		NATIVE_LIB_API  	ToInt32(unsigned char* pszBuffer);
	extern NATIVE_LIB_DLL bool 		NATIVE_LIB_API 		ToBoolean(unsigned char* value);
	extern NATIVE_LIB_DLL float 	NATIVE_LIB_API  	ToFloat(unsigned char* pszBuffer);
	extern NATIVE_LIB_DLL double 	NATIVE_LIB_API  	ToDouble(unsigned char* pszBuffer);
	extern NATIVE_LIB_DLL int64_t 	NATIVE_LIB_API  	ToLong(unsigned char* pszBuffer);
	extern NATIVE_LIB_DLL void 		NATIVE_LIB_API 		GetBoolBytes(bool value, unsigned char *ret);
	extern NATIVE_LIB_DLL void 		NATIVE_LIB_API 		GetIntBytes(int value, unsigned char* ret);
	extern NATIVE_LIB_DLL void 		NATIVE_LIB_API 		GetLongBytes(int64_t value, unsigned char* ret);
	extern NATIVE_LIB_DLL void 		NATIVE_LIB_API 		GetFloatBytes(float value, unsigned char* ret);
	extern NATIVE_LIB_DLL void 		NATIVE_LIB_API 		GetDoubleBytes(double value, unsigned char * ret);

	extern NATIVE_LIB_DLL int 		NATIVE_LIB_API  	DecrypteToInt32(unsigned char* pszBuffer);
	extern NATIVE_LIB_DLL bool 		NATIVE_LIB_API 		DecrypteToBoolean(unsigned char* value);
	extern NATIVE_LIB_DLL float 	NATIVE_LIB_API  	DecrypteToFloat(unsigned char* pszBuffer);
	extern NATIVE_LIB_DLL double 	NATIVE_LIB_API  	DecrypteToDouble(unsigned char* pszBuffer);
	extern NATIVE_LIB_DLL int64_t 	NATIVE_LIB_API  	DecrypteToLong(unsigned char* pszBuffer);
	extern NATIVE_LIB_DLL void 		NATIVE_LIB_API 		EncrypteGetBoolBytes(bool value, unsigned char *ret);
	extern NATIVE_LIB_DLL void 		NATIVE_LIB_API 		EncrypteGetIntBytes(int value, unsigned char* ret);
	extern NATIVE_LIB_DLL void 		NATIVE_LIB_API 		EncrypteGetLongBytes(int64_t value, unsigned char* ret);
	extern NATIVE_LIB_DLL void 		NATIVE_LIB_API 		EncrypteGetFloatBytes(float value, unsigned char* ret);
	extern NATIVE_LIB_DLL void 		NATIVE_LIB_API 		EncrypteGetDoubleBytes(double value, unsigned char * ret);


	extern NATIVE_LIB_DLL int 		NATIVE_LIB_API  	DecryToInt32(int value);
	extern NATIVE_LIB_DLL unsigned char 		NATIVE_LIB_API 		DecryToBoolean(unsigned char value);
	extern NATIVE_LIB_DLL float 	NATIVE_LIB_API  	DecryToFloat(int value);
	extern NATIVE_LIB_DLL double 	NATIVE_LIB_API  	DecryToDouble(int64_t value);
	extern NATIVE_LIB_DLL int64_t 	NATIVE_LIB_API  	DecryToLong(int64_t value);
	extern NATIVE_LIB_DLL unsigned char 		NATIVE_LIB_API 		EncryToBoolean(unsigned char ret);
	extern NATIVE_LIB_DLL int 		NATIVE_LIB_API 		EncryToInt32(int value);
	extern NATIVE_LIB_DLL int64_t 	NATIVE_LIB_API 		EncryToLong(int64_t value);
	extern NATIVE_LIB_DLL int 	NATIVE_LIB_API 		EncryToFloat(float value);
	extern NATIVE_LIB_DLL int64_t 	NATIVE_LIB_API 		EncryToDouble(double value);
}