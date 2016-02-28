//-----------------------------------------------------------------------------
// MurmurHash3 was written by Austin Appleby, and is placed in the public
// domain. The author hereby disclaims copyright to this source code.
#ifndef INC_LUTIL_MURMURHASH_H__
#define INC_LUTIL_MURMURHASH_H__
/**
@file MurmurHash.h
@author t-sakai
@date 2016/02/29
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
//--- MurmurHash
//---
//----------------------------------------------------
#ifdef __cplusplus
extern "C" {
#endif

EXPORT_API struct Context32_t
{
    u32 totalLength_;
    u32 h1_;
    u32 memLength_;
    u8 mem_[4];
};

typedef struct Context32_t Context32;

EXPORT_API void MurmurHash32Init(struct Context32_t* context, u32 seed);
EXPORT_API void MurmurHash32Update4(struct Context32_t* context, const u8* data, u32 offset, u32 length);
EXPORT_API void MurmurHash32Tail(struct Context32_t* context, const u8* data, u32 offset, u32 length);
EXPORT_API u32 MurmurHash32Finalize(struct Context32_t* context);

#ifdef __cplusplus
}
#endif

#endif //INC_LUTIL_MURMURHASH_H__
