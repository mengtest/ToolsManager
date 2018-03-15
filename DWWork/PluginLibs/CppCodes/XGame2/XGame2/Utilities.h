#pragma once
#include "PreInclude.h"

extern "C"
{
	//格式化字符串（pszBuffer 为输出缓存；nBufferSize 为缓存大小；pszFormat 为格式化字符串；返回值为包含零结尾的输出字符串长度，负数表示错误。）
	extern int NATIVE_LIB_API FormatString(char* pszBuffer, int nBufferSize, const char* pszFormat, ...);

	//格式化字符串（pszBuffer 为输出缓存；nBufferSize 为缓存大小；pszFormat 为格式化字符串；返回值为包含零结尾的输出字符串长度，负数表示错误。）
	extern int NATIVE_LIB_API FormatStringArgs(char* pszBuffer, int nBufferSize, const char* pszFormat, va_list args);

	//输出信息
	extern void NATIVE_LIB_API DebugInfo(const char* pszMessage);

	//输出格式化字符串信息
	extern void NATIVE_LIB_API DebugInfoFormat(char* pszMessageBuffer, int nBufferSize, const char* pszFormat, ...);

	//输出警告
	extern void NATIVE_LIB_API DebugWarning(const char* pszMessage);

	//输出格式化字符串警告
	extern void NATIVE_LIB_API DebugWarningFormat(char* pszMessageBuffer, int nBufferSize, const char* pszFormat, ...);

	//输出错误
	extern void NATIVE_LIB_API DebugError(const char* pszMessage);

	//输出格式化字符串错误
	extern void NATIVE_LIB_API DebugErrorFormat(char* pszMessageBuffer, int nBufferSize, const char* pszFormat, ...);

	// 计算平方和
	extern NATIVE_LIB_DLL float NATIVE_LIB_API CalculateSquared(float diffX, float DiffZ);

	//计算宽字符串长度
	//extern int wcslen_custom(const wchar_t* pszString, int maxLen = 48);
};

template <typename Type> __inline void SafeFree(Type*& pMem)
{
	if (pMem != NULL)
	{
		free(pMem);
		pMem = NULL;
	}
}

//对象安全删除函数
template <typename Type> __inline void SafeDelete(Type*& pObject)
{
	if (pObject != NULL)
	{
		delete pObject;
		pObject = NULL;
	}
}

//对象数组安全删除函数
template <typename Type> __inline void SafeDeleteArray(Type*& pObjectArray)
{
	if (pObjectArray != NULL)
	{
		delete[] pObjectArray;
		pObjectArray = NULL;
	}
}
