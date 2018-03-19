#pragma once

/*

FastEvent 使用示例:

void callback(int i, float f, string str)
{
}

void main()
{
	FastEvent3<void, int, float, string> fastEvent;

	fastEvent.RegisterHandler(callback);

	int i = 1;
	float f = 2.0f;
	string str("test");

	fastEvent.Invoke(i, f, str);

	fastEvent.UnregisterHandler(callback);
}

*/


#include "PreInclude.h"
#include "VariadicMacros.h"
#include <set>


#define MACRO_FASTEVENT_CLASS_NAME_ARG( n )							MACRO_CAT( FastEvent, n )
#define MACRO_FASTEVENT_TYPE_NAME_ARG( n )							MACRO_CAT( TArg, n )
#define MACRO_FASTEVENT_PARAM_NAME_ARG( n )							MACRO_CAT( Arg, n )

#define MACRO_FASTEVENT_TYPE_LIST_ARG( n )							typename MACRO_FASTEVENT_TYPE_NAME_ARG(n)
#define MACRO_FASTEVENT_PARAM_LIST_ARG( n )							MACRO_FASTEVENT_TYPE_NAME_ARG(n)& MACRO_FASTEVENT_PARAM_NAME_ARG(n)


#define MACRO_VARIADIC_ARGS_COUNT	0
#include "FastEventImplement.h"

#define MACRO_VARIADIC_ARGS_COUNT	1
#include "FastEventImplement.h"

#define MACRO_VARIADIC_ARGS_COUNT	2
#include "FastEventImplement.h"

#define MACRO_VARIADIC_ARGS_COUNT	3
#include "FastEventImplement.h"

#define MACRO_VARIADIC_ARGS_COUNT	4
#include "FastEventImplement.h"

#define MACRO_VARIADIC_ARGS_COUNT	5
#include "FastEventImplement.h"

#define MACRO_VARIADIC_ARGS_COUNT	6
#include "FastEventImplement.h"

#define MACRO_VARIADIC_ARGS_COUNT	7
#include "FastEventImplement.h"

#define MACRO_VARIADIC_ARGS_COUNT	8
#include "FastEventImplement.h"
