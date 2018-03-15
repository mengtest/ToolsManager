#include "PreInclude.h"

extern "C"
{
#ifdef __GNUC__

	int _vscprintf(const char* format, va_list pargs)
	{
		int retval;
		va_list argcopy;
		va_copy(argcopy, pargs);
		retval = vsnprintf(NULL, 0, format, argcopy);
		va_end(argcopy);
		return retval;
	}

	int vsprintf_s(char* pszBuffer, size_t nBufferSize, const char* format, va_list pargs)
	{
		int retval;
		va_list argcopy;
		va_copy(argcopy, pargs);
		retval = vsnprintf(pszBuffer, nBufferSize, format, argcopy);
		va_end(argcopy);
		return retval;
	}

#endif // #ifdef __GNUC__

	//格式化字符串（pszBuffer 为输出缓存；nBufferSize 为缓存大小；pszFormat 为格式化字符串；返回值为包含零结尾的输出字符串长度，负数表示错误。）
	int NATIVE_LIB_API FormatString(char* pszBuffer, int nBufferSize, const char* pszFormat, ...)
	{
		//获取参数列表
		va_list args;
		va_start(args, pszFormat);

		//获取格式化所需的字符串长度
		int nLength = FormatStringArgs(pszBuffer, nBufferSize, pszFormat, args);

		va_end(args);

		return nLength;
	}

	//格式化字符串（pszBuffer 为输出缓存；nBufferSize 为缓存大小；pszFormat 为格式化字符串；返回值为包含零结尾的输出字符串长度，负数表示错误。）
	int NATIVE_LIB_API FormatStringArgs(char* pszBuffer, int nBufferSize, const char* pszFormat, va_list args)
	{
		//获取格式化所需的字符串长度
		int nLength = _vscprintf(pszFormat, args);
		if (nLength < 0)
		{
			return nLength;
		}

		++nLength;
		if (nLength > nBufferSize)
		{
			return -1;
		}

		//格式化字符串
		vsprintf_s(pszBuffer, nLength, pszFormat, args);

		return nLength;
	}

	//输出信息
	void NATIVE_LIB_API DebugInfo(const char* pszMessage)
	{
		g_ManagedCallbacks.DebugLog(pszMessage, LOG_LEVEL_INFO);
	}

	//输出格式化字符串信息
	void NATIVE_LIB_API DebugInfoFormat(char* pszMessageBuffer, int nBufferSize, const char* pszFormat, ...)
	{
		va_list args;
		va_start(args, pszFormat);

		int result = FormatStringArgs(pszMessageBuffer, nBufferSize, pszFormat, args);

		va_end(args);

		if (result <= 0)
		{
			return;
		}

		g_ManagedCallbacks.DebugLog(pszMessageBuffer, LOG_LEVEL_INFO);
	}

	//输出警告
	void NATIVE_LIB_API DebugWarning(const char* pszMessage)
	{
		g_ManagedCallbacks.DebugLog(pszMessage, LOG_LEVEL_WARNING);
	}

	//输出格式化字符串警告
	void NATIVE_LIB_API DebugWarningFormat(char* pszMessageBuffer, int nBufferSize, const char* pszFormat, ...)
	{
		va_list args;
		va_start(args, pszFormat);

		int result = FormatStringArgs(pszMessageBuffer, nBufferSize, pszFormat, args);

		va_end(args);

		if (result <= 0)
		{
			return;
		}

		g_ManagedCallbacks.DebugLog(pszMessageBuffer, LOG_LEVEL_WARNING);
	}

	//输出错误
	void NATIVE_LIB_API DebugError(const char* pszMessage)
	{
		g_ManagedCallbacks.DebugLog(pszMessage, LOG_LEVEL_ERROR);
	}

	//输出格式化字符串错误
	void NATIVE_LIB_API DebugErrorFormat(char* pszMessageBuffer, int nBufferSize, const char* pszFormat, ...)
	{
		va_list args;
		va_start(args, pszFormat);

		int result = FormatStringArgs(pszMessageBuffer, nBufferSize, pszFormat, args);

		va_end(args);

		if (result <= 0)
		{
			return;
		}

		g_ManagedCallbacks.DebugLog(pszMessageBuffer, LOG_LEVEL_ERROR);
	}

	float NATIVE_LIB_API CalculateSquared(float diffX, float DiffZ)
	{
		return diffX * diffX + DiffZ * DiffZ;
	}

	//计算宽字符串长度
	/*int wcslen_custom(const wchar_t* pszString, int iMaxLengthInWords)
	{
		if (pszString == NULL)
		{
			DebugError("_______String Atrribute is NULL!!!");
			return -1;
		}

		int iMaxLength = iMaxLengthInWords / 2 * sizeof(wchar_t);
		int iLength = 0;
		for (; iLength < iMaxLength && pszString[iLength] != 0; ++iLength)
		{
		}

		if (iLength == maxLen)
		{
			DebugError("_______String Atrribute Length is not available!!!");
			iLength = -1;
		}

		return iLength;
	}*/
};
