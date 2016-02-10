#ifndef INC_LUTIL_MD5_H__
#define INC_LUTIL_MD5_H__
/**
@file md5.h
@author t-sakai
@date 2016/02/10 create
*/

#if defined(_MSC_VER)
typedef char Char;
typedef __int8 s8;
typedef __int16 s16;
typedef __int32 s32;
typedef __int64 s64;

typedef unsigned __int8 u8;
typedef unsigned __int16 u16;
typedef unsigned __int32 u32;
typedef unsigned __int64 u64;

typedef float f32;
typedef double f64;

#define EXPORT_API __declspec(dllexport)
#define LUTIL_INLINE __inline

#elif defined(ANDROID) || defined(__GNUC__)
#include <stdint.h>
typedef char Char;
typedef int8_t s8;
typedef int16_t s16;
typedef int32_t s32;
typedef int64_t s64;

typedef uint8_t u8;
typedef uint16_t u16;
typedef uint32_t u32;
typedef uint64_t u64;

typedef float f32;
typedef double f64;

#define EXPORT_API
#define LUTIL_INLINE inline

#else
typedef char Char;
typedef char s8;
typedef short s16;
typedef long s32;
typedef long long s64;

typedef unsigned char u8;
typedef unsigned short u16;
typedef unsigned long u32;
typedef unsigned long long u64;

typedef float f32;
typedef double f64;

#define EXPORT_API
#define LUTIL_INLINE inline

#endif

//----------------------------------------------------
//---
//--- MD5
//---
//----------------------------------------------------
#ifdef __cplusplus
extern "C" {
#endif

/**
The MD5 algorithm
RFC document: http://www.ietf.org/rfc/rfc1321.txt
*/
EXPORT_API void calcMD5(u8* hash, u32 length, const u8* data);

#ifdef __cplusplus
}
#endif

#endif //INC_LUTIL_MD5_H__
