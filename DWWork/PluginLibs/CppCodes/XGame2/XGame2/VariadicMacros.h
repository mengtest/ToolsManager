#pragma once

#define MACRO_VARIADIC_ARGS				MACRO_CAT( MACRO_VARIADIC_ARGS_, MACRO_VARIADIC_ARGS_COUNT )

// M = Macro 传入宏参数   P = Prefix 前缀宏参数   D = Delimiter 分隔宏参数
#define MACRO_VARIADIC_ARGS_0( M, P, D ) \

#define MACRO_VARIADIC_ARGS_1( M, P, D ) \
	P M(1)
#define MACRO_VARIADIC_ARGS_2( M, P, D ) \
	P M(1) D M(2)
#define MACRO_VARIADIC_ARGS_3( M, P, D ) \
	P M(1) D M(2) D M(3)
#define MACRO_VARIADIC_ARGS_4( M, P, D ) \
	P M(1) D M(2) D M(3) D M(4)
#define MACRO_VARIADIC_ARGS_5( M, P, D ) \
	P M(1) D M(2) D M(3) D M(4) D M(5)
#define MACRO_VARIADIC_ARGS_6( M, P, D ) \
	P M(1) D M(2) D M(3) D M(4) D M(5) D M(6)
#define MACRO_VARIADIC_ARGS_7( M, P, D ) \
	P M(1) D M(2) D M(3) D M(4) D M(5) D M(6) D M(7)
#define MACRO_VARIADIC_ARGS_8( M, P, D ) \
	P M(1) D M(2) D M(3) D M(4) D M(5) D M(6) D M(7) D M(8)
#define MACRO_VARIADIC_ARGS_9( M, P, D ) \
	P M(1) D M(2) D M(3) D M(4) D M(5) D M(6) D M(7) D M(8) D M(9)
#define MACRO_VARIADIC_ARGS_10( M, P, D ) \
	P M(1) D M(2) D M(3) D M(4) D M(5) D M(6) D M(7) D M(8) D M(9) D M(10)
#define MACRO_VARIADIC_ARGS_11( M, P, D ) \
	P M(1) D M(2) D M(3) D M(4) D M(5) D M(6) D M(7) D M(8) D M(9) D M(10) D M(11)
#define MACRO_VARIADIC_ARGS_12( M, P, D ) \
	P M(1) D M(2) D M(3) D M(4) D M(5) D M(6) D M(7) D M(8) D M(9) D M(10) D M(11) D M(12)
#define MACRO_VARIADIC_ARGS_13( M, P, D ) \
	P M(1) D M(2) D M(3) D M(4) D M(5) D M(6) D M(7) D M(8) D M(9) D M(10) D M(11) D M(12) D M(13)
#define MACRO_VARIADIC_ARGS_14( M, P, D ) \
	P M(1) D M(2) D M(3) D M(4) D M(5) D M(6) D M(7) D M(8) D M(9) D M(10) D M(11) D M(12) D M(13) D M(14)
#define MACRO_VARIADIC_ARGS_15( M, P, D ) \
	P M(1) D M(2) D M(3) D M(4) D M(5) D M(6) D M(7) D M(8) D M(9) D M(10) D M(11) D M(12) D M(13) D M(14) D M(15)
#define MACRO_VARIADIC_ARGS_16( M, P, D ) \
	P M(1) D M(2) D M(3) D M(4) D M(5) D M(6) D M(7) D M(8) D M(9) D M(10) D M(11) D M(12) D M(13) D M(14) D M(15) D M(16)
