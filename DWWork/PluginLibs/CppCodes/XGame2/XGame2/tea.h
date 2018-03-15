#pragma once
#include "PreInclude.h"
#include <stddef.h> /* for size_t & NULL declarations */

#if defined(_MSC_VER)

typedef unsigned __int32 xxtea_long;

#else

#if defined(__FreeBSD__) && __FreeBSD__ < 5
/* FreeBSD 4 doesn't have stdint.h file */
#include <inttypes.h>
#else
#include <stdint.h>
#endif

typedef uint32_t xxtea_long;

#endif /* end of if defined(_MSC_VER) */

#define XXTEA_MX (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z)
#define XXTEA_DELTA 0x9e3779b9

extern "C"
{
	extern void xxtea_long_encrypt(xxtea_long *v, xxtea_long len, xxtea_long *k);
	extern void xxtea_long_decrypt(xxtea_long *v, xxtea_long len, xxtea_long *k);
}