#pragma once

//在代码中删除所有字符串常量
#define ENABLE_DEBUG_LOG

#include <stdint.h>
#include <stdarg.h>
#include <memory.h>
#include <string>
#include <vector>
#include <map>
#include <stdlib.h>
using namespace std;

#define MACRO_CAT( a, b )				MACRO_PRIMITIVE_CAT( a, b )
#define MACRO_PRIMITIVE_CAT( a, b )		a ## b
#define MACRO_COMMA						,
#define MACRO_EMPTY						

// DLL 导出函数定义宏
#ifdef _MSC_VER
#	define NATIVE_LIB_DLL __declspec(dllexport)
#else
#	define NATIVE_LIB_DLL
#endif

//调用约定定义宏
#ifdef _WINDLL
#	ifdef _MSC_VER
#		define NATIVE_LIB_API __stdcall
#	else
#		define NATIVE_LIB_API __attribute__((stdcall))
#	endif
#else
#	ifdef _MSC_VER
#		define NATIVE_LIB_API __cdecl
#	else
#		define NATIVE_LIB_API
#	endif
#endif

#include "Main.h"
#include "Utilities.h"
