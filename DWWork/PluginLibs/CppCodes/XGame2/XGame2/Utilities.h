#pragma once
#include "PreInclude.h"

extern "C"
{
	//��ʽ���ַ�����pszBuffer Ϊ������棻nBufferSize Ϊ�����С��pszFormat Ϊ��ʽ���ַ���������ֵΪ�������β������ַ������ȣ�������ʾ���󡣣�
	extern int NATIVE_LIB_API FormatString(char* pszBuffer, int nBufferSize, const char* pszFormat, ...);

	//��ʽ���ַ�����pszBuffer Ϊ������棻nBufferSize Ϊ�����С��pszFormat Ϊ��ʽ���ַ���������ֵΪ�������β������ַ������ȣ�������ʾ���󡣣�
	extern int NATIVE_LIB_API FormatStringArgs(char* pszBuffer, int nBufferSize, const char* pszFormat, va_list args);

	//�����Ϣ
	extern void NATIVE_LIB_API DebugInfo(const char* pszMessage);

	//�����ʽ���ַ�����Ϣ
	extern void NATIVE_LIB_API DebugInfoFormat(char* pszMessageBuffer, int nBufferSize, const char* pszFormat, ...);

	//�������
	extern void NATIVE_LIB_API DebugWarning(const char* pszMessage);

	//�����ʽ���ַ�������
	extern void NATIVE_LIB_API DebugWarningFormat(char* pszMessageBuffer, int nBufferSize, const char* pszFormat, ...);

	//�������
	extern void NATIVE_LIB_API DebugError(const char* pszMessage);

	//�����ʽ���ַ�������
	extern void NATIVE_LIB_API DebugErrorFormat(char* pszMessageBuffer, int nBufferSize, const char* pszFormat, ...);

	// ����ƽ����
	extern NATIVE_LIB_DLL float NATIVE_LIB_API CalculateSquared(float diffX, float DiffZ);

	//������ַ�������
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

//����ȫɾ������
template <typename Type> __inline void SafeDelete(Type*& pObject)
{
	if (pObject != NULL)
	{
		delete pObject;
		pObject = NULL;
	}
}

//�������鰲ȫɾ������
template <typename Type> __inline void SafeDeleteArray(Type*& pObjectArray)
{
	if (pObjectArray != NULL)
	{
		delete[] pObjectArray;
		pObjectArray = NULL;
	}
}
