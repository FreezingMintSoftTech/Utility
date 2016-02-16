/**
Copyright (c) 2016 Takuro Sakai

This software is provided 'as-is', without any express or implied
warranty. In no event will the authors be held liable for any damages
arising from the use of this software.

Permission is granted to anyone to use this software for any purpose,
including commercial applications, and to alter it and redistribute it
freely, subject to the following restrictions:

1. The origin of this software must not be misrepresented; you must not
   claim that you wrote the original software. If you use this software
   in a product, an acknowledgment in the product documentation would be
   appreciated but is not required.

2. Altered source versions must be plainly marked as such, and must not be
   misrepresented as being the original software.

3. This notice may not be removed or altered from any source
   distribution.
*/
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

EXPORT_API typedef union MD5Hash_t
{
    u32	u32_[4];
    u8  u8_[16];
} MD5Hash;

EXPORT_API struct MD5Context_t
{
    MD5Hash hash_;
    u32 length_[2];
    u8 buffer_[64];
};

typedef struct MD5Context_t MD5Context;

/**
The MD5 algorithm
RFC document: http://www.ietf.org/rfc/rfc1321.txt
*/
EXPORT_API void calcMD5(u8* hash, u32 length, const u8* data);

EXPORT_API void initMD5(struct MD5Context_t* context);
EXPORT_API void processMD5(struct MD5Context_t* context, u32 offset, u32 length, const u8* data);
EXPORT_API void termMD5(u8* hash, struct MD5Context_t* context);

#ifdef __cplusplus
}
#endif

#endif //INC_LUTIL_MD5_H__
