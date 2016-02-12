/**
@file md5.c
@author t-sakai
@date 2016/02/10 create
*/
#include "md5.h"
#include <assert.h>
#include <string.h>

#ifdef __cplusplus
extern "C" {
#endif

//----------------------------------------------------
//---
//--- MD5
//---
//----------------------------------------------------
#ifndef LASSERT
#ifdef _DEBUG
#define LASSERT(exp) assert((exp))
#else
#define LASSERT(exp)
#endif
#endif

static const s8 md5_S[16] =
{
    7, 12, 17, 22,
    5, 9, 14, 20,
    4, 11, 16, 23,
    6, 10, 15, 21,
};

static const u32 md5_K[64] =
{
    0xd76aa478, 0xe8c7b756, 0x242070db, 0xc1bdceee,
    0xf57c0faf, 0x4787c62a, 0xa8304613, 0xfd469501,
    0x698098d8, 0x8b44f7af, 0xffff5bb1, 0x895cd7be,
    0x6b901122, 0xfd987193, 0xa679438e, 0x49b40821,
    0xf61e2562, 0xc040b340, 0x265e5a51, 0xe9b6c7aa,
    0xd62f105d, 0x02441453, 0xd8a1e681, 0xe7d3fbc8,
    0x21e1cde6, 0xc33707d6, 0xf4d50d87, 0x455a14ed,
    0xa9e3e905, 0xfcefa3f8, 0x676f02d9, 0x8d2a4c8a,
    0xfffa3942, 0x8771f681, 0x6d9d6122, 0xfde5380c,
    0xa4beea44, 0x4bdecfa9, 0xf6bb4b60, 0xbebfbc70,
    0x289b7ec6, 0xeaa127fa, 0xd4ef3085, 0x04881d05,
    0xd9d4d039, 0xe6db99e5, 0x1fa27cf8, 0xc4ac5665,
    0xf4292244, 0x432aff97, 0xab9423a7, 0xfc93a039,
    0x655b59c3, 0x8f0ccc92, 0xffeff47d, 0x85845dd1,
    0x6fa87e4f, 0xfe2ce6e0, 0xa3014314, 0x4e0811a1,
    0xf7537e82, 0xbd3af235, 0x2ad7d2bb, 0xeb86d391,
};

static LUTIL_INLINE void initMD5Context(struct MD5Context_t* context)
{
    context->hash_.u32_[0] = 0x67452301; //A
    context->hash_.u32_[1] = 0xefcdab89; //B
    context->hash_.u32_[2] = 0x98badcfe; //C
    context->hash_.u32_[3] = 0x10325476; //D

    context->length_[0] = 0;
    context->length_[1] = 0;
}

static LUTIL_INLINE u32 F(u32 x, u32 y, u32 z)
{
    return (x & y) | (~x & z);
}

static LUTIL_INLINE u32 G(u32 x, u32 y, u32 z)
{
    return (x & z) | (y & ~z);
}

static LUTIL_INLINE u32 H(u32 x, u32 y, u32 z)
{
    return x ^ y ^ z;
}

static LUTIL_INLINE u32 I(u32 x, u32 y, u32 z)
{
    return y ^ (x | ~z);
}

static LUTIL_INLINE u32 RotateLeft(u32 x, u32 n)
{
    return (x << n) | (x>>(32-n));
}

static LUTIL_INLINE u32 FF(u32 a, u32 b, u32 c, u32 d, u32 x, u32 s, u32 ac)
{
    a += F(b, c, d) + x + ac;
    a = RotateLeft(a, s);
    a += b;
    return a;
}

static LUTIL_INLINE u32 GG(u32 a, u32 b, u32 c, u32 d, u32 x, u32 s, u32 ac)
{
    a += G(b, c, d) + x + ac;
    a = RotateLeft(a, s);
    a += b;
    return a;
}

static LUTIL_INLINE u32 HH(u32 a, u32 b, u32 c, u32 d, u32 x, u32 s, u32 ac)
{
    a += H(b, c, d) + x + ac;
    a = RotateLeft(a, s);
    a += b;
    return a;
}

static LUTIL_INLINE u32 II(u32 a, u32 b, u32 c, u32 d, u32 x, u32 s, u32 ac)
{
    a += I(b, c, d) + x + ac;
    a = RotateLeft(a, s);
    a += b;
    return a;
}

static LUTIL_INLINE u32 getU32FromLittleEndian(const u8* bytes, u32 index)
{
    return (bytes[index+0]<<0) | (bytes[index+1]<<8) | (bytes[index+2]<<16) | (bytes[index+3]<<24);
}

static LUTIL_INLINE void setU32ToLittleEndian(u8* bytes, u32 value, u32 index)
{
    bytes[index + 0] = (u8)((value>> 0) & 0xFFU);
    bytes[index + 1] = (u8)((value>> 8) & 0xFFU);
    bytes[index + 2] = (u8)((value>>16) & 0xFFU);
    bytes[index + 3] = (u8)((value>>24) & 0xFFU);
}

static const u8 md5_Padding[64] =
{
    0x80, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
};


static void transformMD5(u32* hash, const u8* data)
{
    u32 a = hash[0];
    u32 b = hash[1];
    u32 c = hash[2];
    u32 d = hash[3];

    u32 x[16];
    x[0] = getU32FromLittleEndian(data, 0);
    x[1] = getU32FromLittleEndian(data, 4);
    x[2] = getU32FromLittleEndian(data, 8);
    x[3] = getU32FromLittleEndian(data, 12);
    x[4] = getU32FromLittleEndian(data, 16);
    x[5] = getU32FromLittleEndian(data, 20);
    x[6] = getU32FromLittleEndian(data, 24);
    x[7] = getU32FromLittleEndian(data, 28);

    x[8] = getU32FromLittleEndian(data, 32);
    x[9] = getU32FromLittleEndian(data, 36);
    x[10] = getU32FromLittleEndian(data, 40);
    x[11] = getU32FromLittleEndian(data, 44);
    x[12] = getU32FromLittleEndian(data, 48);
    x[13] = getU32FromLittleEndian(data, 52);
    x[14] = getU32FromLittleEndian(data, 56);
    x[15] = getU32FromLittleEndian(data, 60);

    //Round 1
    a = FF(a, b, c, d, x[0], md5_S[0], md5_K[0]);
    d = FF(d, a, b, c, x[1], md5_S[1], md5_K[1]);
    c = FF(c, d, a, b, x[2], md5_S[2], md5_K[2]);
    b = FF(b, c, d, a, x[3], md5_S[3], md5_K[3]);
    a = FF(a, b, c, d, x[4], md5_S[0], md5_K[4]);
    d = FF(d, a, b, c, x[5], md5_S[1], md5_K[5]);
    c = FF(c, d, a, b, x[6], md5_S[2], md5_K[6]);
    b = FF(b, c, d, a, x[7], md5_S[3], md5_K[7]);
    a = FF(a, b, c, d, x[8], md5_S[0], md5_K[8]);
    d = FF(d, a, b, c, x[9], md5_S[1], md5_K[9]);
    c = FF(c, d, a, b, x[10], md5_S[2], md5_K[10]);
    b = FF(b, c, d, a, x[11], md5_S[3], md5_K[11]);
    a = FF(a, b, c, d, x[12], md5_S[0], md5_K[12]);
    d = FF(d, a, b, c, x[13], md5_S[1], md5_K[13]);
    c = FF(c, d, a, b, x[14], md5_S[2], md5_K[14]);
    b = FF(b, c, d, a, x[15], md5_S[3], md5_K[15]);

    //Round 2
    a = GG(a, b, c, d, x[1], md5_S[4], md5_K[16]);
    d = GG(d, a, b, c, x[6], md5_S[5], md5_K[17]);
    c = GG(c, d, a, b, x[11], md5_S[6], md5_K[18]);
    b = GG(b, c, d, a, x[0], md5_S[7], md5_K[19]);
    a = GG(a, b, c, d, x[5], md5_S[4], md5_K[20]);
    d = GG(d, a, b, c, x[10], md5_S[5], md5_K[21]);
    c = GG(c, d, a, b, x[15], md5_S[6], md5_K[22]);
    b = GG(b, c, d, a, x[4], md5_S[7], md5_K[23]);
    a = GG(a, b, c, d, x[9], md5_S[4], md5_K[24]);
    d = GG(d, a, b, c, x[14], md5_S[5], md5_K[25]);
    c = GG(c, d, a, b, x[3], md5_S[6], md5_K[26]);
    b = GG(b, c, d, a, x[8], md5_S[7], md5_K[27]);
    a = GG(a, b, c, d, x[13], md5_S[4], md5_K[28]);
    d = GG(d, a, b, c, x[2], md5_S[5], md5_K[29]);
    c = GG(c, d, a, b, x[7], md5_S[6], md5_K[30]);
    b = GG(b, c, d, a, x[12], md5_S[7], md5_K[31]);

    //Round 3
    a = HH(a, b, c, d, x[5], md5_S[8], md5_K[32]);
    d = HH(d, a, b, c, x[8], md5_S[9], md5_K[33]);
    c = HH(c, d, a, b, x[11], md5_S[10], md5_K[34]);
    b = HH(b, c, d, a, x[14], md5_S[11], md5_K[35]);
    a = HH(a, b, c, d, x[1], md5_S[8], md5_K[36]);
    d = HH(d, a, b, c, x[4], md5_S[9], md5_K[37]);
    c = HH(c, d, a, b, x[7], md5_S[10], md5_K[38]);
    b = HH(b, c, d, a, x[10], md5_S[11], md5_K[39]);
    a = HH(a, b, c, d, x[13], md5_S[8], md5_K[40]);
    d = HH(d, a, b, c, x[0], md5_S[9], md5_K[41]);
    c = HH(c, d, a, b, x[3], md5_S[10], md5_K[42]);
    b = HH(b, c, d, a, x[6], md5_S[11], md5_K[43]);
    a = HH(a, b, c, d, x[9], md5_S[8], md5_K[44]);
    d = HH(d, a, b, c, x[12], md5_S[9], md5_K[45]);
    c = HH(c, d, a, b, x[15], md5_S[10], md5_K[46]);
    b = HH(b, c, d, a, x[2], md5_S[11], md5_K[47]);

    //Round 4
    a = II(a, b, c, d, x[0], md5_S[12], md5_K[48]);
    d = II(d, a, b, c, x[7], md5_S[13], md5_K[49]);
    c = II(c, d, a, b, x[14], md5_S[14], md5_K[50]);
    b = II(b, c, d, a, x[5], md5_S[15], md5_K[51]);
    a = II(a, b, c, d, x[12], md5_S[12], md5_K[52]);
    d = II(d, a, b, c, x[3], md5_S[13], md5_K[53]);
    c = II(c, d, a, b, x[10], md5_S[14], md5_K[54]);
    b = II(b, c, d, a, x[1], md5_S[15], md5_K[55]);
    a = II(a, b, c, d, x[8], md5_S[12], md5_K[56]);
    d = II(d, a, b, c, x[15], md5_S[13], md5_K[57]);
    c = II(c, d, a, b, x[6], md5_S[14], md5_K[58]);
    b = II(b, c, d, a, x[13], md5_S[15], md5_K[59]);
    a = II(a, b, c, d, x[4], md5_S[12], md5_K[60]);
    d = II(d, a, b, c, x[11], md5_S[13], md5_K[61]);
    c = II(c, d, a, b, x[2], md5_S[14], md5_K[62]);
    b = II(b, c, d, a, x[9], md5_S[15], md5_K[63]);

    hash[0] += a;
    hash[1] += b;
    hash[2] += c;
    hash[3] += d;
}


static void updateMD5(MD5Context* context, u32 length, const u8* data)
{
    u32 index = context->length_[0] & 0x3FU;
    u32 part = 64 - index;

    context->length_[0] += length;
    if(context->length_[0] < length){
        ++context->length_[1];
    }

    u32 rest = 0;
    if(part<=length){
        memcpy(&context->buffer_[index], data, part);
        transformMD5(context->hash_.u32_, context->buffer_);

        u32 i;
        for(i=part; (i+63)<length; i+=64){
            transformMD5(context->hash_.u32_, &data[i]);
        }
        index = 0;
        rest = i;
    }
    memcpy(&context->buffer_[index], &data[rest], length-rest);
}


/**
The MD5 algorithm
RFC document: http://www.ietf.org/rfc/rfc1321.txt
*/
EXPORT_API void calcMD5(u8* hash, u32 length, const u8* data)
{
    MD5Context context;
    initMD5Context(&context);

    updateMD5(&context, length, data);

    termMD5(hash, &context);
}

void initMD5(struct MD5Context_t* context)
{
    LASSERT(NULL != context);
    initMD5Context(context);
}

void processMD5(struct MD5Context_t* context, u32 offset, u32 length, const u8* data)
{
    LASSERT(NULL != context);
    updateMD5(context, length, data+offset);
}

void termMD5(u8* hash, struct MD5Context_t* context)
{
    LASSERT(NULL != context);

    u32 high = (context->length_[0]>>29) | (context->length_[1]<<3);
    u32 low  = (context->length_[0]<<3);
    u8 bytes[8];
    setU32ToLittleEndian(bytes, low, 0);
    setU32ToLittleEndian(bytes, high, 4);

    u32 index = context->length_[0] & 0x3FU;
    u32 padLen = (index<56)? (56-index) : (120-index);
    updateMD5(context, padLen, md5_Padding);
    updateMD5(context, 8, bytes);

    for(index=0; index<16; ++index){
        hash[index] = context->hash_.u8_[index];
    }
}

#ifdef __cplusplus
}
#endif
