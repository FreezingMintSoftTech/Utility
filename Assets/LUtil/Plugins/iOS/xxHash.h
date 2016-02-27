#ifndef INC_LUTIL_XXHASH_H__
#define INC_LUTIL_XXHASH_H__
/**
@file xxHash.h
@author t-sakai
@date 2016/02/28 create
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

EXPORT_API struct Context32_t
{
    u64 totalLength_;
    u32 seed_;
    u32 v1_;
    u32 v2_;
    u32 v3_;
    u32 v4_;
    u32 mem_[4];
    u32 memSize_;
};

EXPORT_API struct Context64_t
{
    u64 totalLength_;
    u64 seed_;
    u64 v1_;
    u64 v2_;
    u64 v3_;
    u64 v4_;
    u64 mem_[4];
    u64 memSize_;
};

typedef struct Context32_t Context32;
typedef struct Context64_t Context64;

/**
The xxHash algorithm
https://github.com/Cyan4973/xxHash
*/
EXPORT_API void xxHash32Init(struct Context32_t* context, u32 seed);
EXPORT_API void xxHash32Update(struct Context32_t* context, const u8* input, u32 length);
EXPORT_API u32 xxHash32Finalize(struct Context32_t* context);

EXPORT_API void xxHash64Init(struct Context64_t* context, u64 seed);
EXPORT_API void xxHash64Update(struct Context64_t* context, const u8* input, u32 length);
EXPORT_API u64 xxHash64Finalize(struct Context64_t* context);

#ifdef __cplusplus
}
#endif

#endif //INC_LUTIL_XXHASH_H__
