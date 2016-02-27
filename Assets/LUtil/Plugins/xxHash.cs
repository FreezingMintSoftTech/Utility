
using System.Runtime.InteropServices;
namespace LUtil
{
    //https://github.com/Cyan4973/xxHash
    public class xxHash
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        private const string PluginName = "xxHash";

#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_IOS
        private const string PluginName = "__Internal";

#elif UNITY_ANDROID
        private const string PluginName = "xxHash";
#endif

        private const string Digits = "0123456789ABCDEF";
#if false
        private const uint PRIME32_1 = 2654435761U;
        private const uint PRIME32_2 = 2246822519U;
        private const uint PRIME32_3 = 3266489917U;
        private const uint PRIME32_4 = 668265263U;
        private const uint PRIME32_5 = 374761393U;

        public struct Context32
        {
            public ulong totalLength_;
            public uint seed_;
            public uint v1_;
            public uint v2_;
            public uint v3_;
            public uint v4_;
            public byte[] mem_;//= new byte[16];//= new uint[4];
            public uint memSize_;
        };

        private static uint rotl32(uint x, int r)
        {
            return (x<<r) | (x>>(32-r));
        }

        public static uint read32LE(byte[] bytes, uint offset)
        {
            return bytes[offset] | ((uint)bytes[offset+1]<<8) | ((uint)bytes[offset+2]<<16) | ((uint)bytes[offset+3]<<24);
        }

        public static void init(out Context32 context, uint seed)
        {
            context = new Context32();
            context.mem_ = new byte[16];
            context.seed_ = seed;
            context.v1_ = seed + PRIME32_1 + PRIME32_2;
            context.v2_ = seed + PRIME32_2;
            context.v3_ = seed + 0;
            context.v4_ = seed - PRIME32_1;
        }

        public static void update(ref Context32 context, byte[] input, uint length)
        {
            context.totalLength_ += length;
            if((context.memSize_+length)<16) {
                System.Array.Copy(input, 0, context.mem_, context.memSize_, length);
                context.memSize_ += length;
                return;
            }

            uint index = 0;
            if(0 != context.memSize_) {
                System.Array.Copy(input, 0, context.mem_, context.memSize_, 32-context.memSize_);

                uint offset = 0;
                context.v1_ += read32LE(context.mem_, offset) * PRIME32_2;
                context.v1_ = rotl32(context.v1_, 13);
                context.v1_ *= PRIME32_1;
                offset += 4;
                context.v2_ += read32LE(context.mem_, offset) * PRIME32_2;
                context.v2_ = rotl32(context.v2_, 13);
                context.v2_ *= PRIME32_1;
                offset += 4;
                context.v3_ += read32LE(context.mem_, offset) * PRIME32_2;
                context.v3_ = rotl32(context.v3_, 13);
                context.v3_ *= PRIME32_1;
                offset += 4;
                context.v4_ += read32LE(context.mem_, offset) * PRIME32_2;
                context.v4_ = rotl32(context.v4_, 13);
                context.v4_ *= PRIME32_1;

                index += 16 - context.memSize_;
                context.memSize_ = 0;
            }
            if((index+16)<=length) {
                uint limit = length-16;
                uint v1 = context.v1_;
                uint v2 = context.v2_;
                uint v3 = context.v3_;
                uint v4 = context.v4_;
                do {
                    v1 += read32LE(input, index) * PRIME32_2;
                    v1 = rotl32(v1, 13);
                    v1 *= PRIME32_1;
                    index += 4;
                    v2 += read32LE(input, index) * PRIME32_2;
                    v2 = rotl32(v2, 13);
                    v2 *= PRIME32_1;
                    index += 4;
                    v3 += read32LE(input, index) * PRIME32_2;
                    v3 = rotl32(v3, 13);
                    v3 *= PRIME32_1;
                    index += 4;
                    v4 += read32LE(input, index) * PRIME32_2;
                    v4 = rotl32(v4, 13);
                    v4 *= PRIME32_1;
                    index += 4;
                } while(index<=limit);

                context.v1_ = v1;
                context.v2_ = v2;
                context.v3_ = v3;
                context.v4_ = v4;
            }
            if(index<length) {
                context.memSize_ = length-index;
                System.Array.Copy(input, index, context.mem_, 0, context.memSize_);
            }
        }

        public static uint finalize(ref Context32 context)
        {
            byte[] p = context.mem_;
            uint h32;

            if(16<=context.totalLength_) {
                h32 = rotl32(context.v1_, 1) + rotl32(context.v2_, 7) + rotl32(context.v3_, 12) + rotl32(context.v4_, 18);
            } else {
                h32 = context.seed_ + PRIME32_5;
            }
            h32 += (uint)context.totalLength_;

            uint index = 0;
            while((index+4)<=context.memSize_) {
                h32 += read32LE(p, index) * PRIME32_3;
                h32 = rotl32(h32, 17) * PRIME32_4;
                index += 4;
            }
            while(index<context.memSize_) {
                h32 += PRIME32_5*p[index];
                h32 = rotl32(h32, 11) * PRIME32_1;
                ++index;
            }

            h32 ^= h32>>15;
            h32 *= PRIME32_2;
            h32 ^= h32>>13;
            h32 *= PRIME32_3;
            h32 ^= h32>>16;
            return h32;
        }

        private static string toHex(uint hash)
        {
            char[] hashChars = new char[8];
            for(int i = 0; i < 8; ++i) {
                hashChars[i] = Digits[(int)(hash&0x0F)];
                hash>>=4;
            }
            return new string(hashChars);
        }
#else
        private const ulong PRIME64_1 = 11400714785074694791UL;
        private const ulong PRIME64_2 = 14029467366897019727UL;
        private const ulong PRIME64_3 = 1609587929392839161UL;
        private const ulong PRIME64_4 = 9650029242287828579UL;
        private const ulong PRIME64_5 = 2870177450012600261UL;

        public struct Context64
        {
            public ulong totalLength_;
            public ulong seed_;
            public ulong v1_;
            public ulong v2_;
            public ulong v3_;
            public ulong v4_;
            public byte[] mem_;//= new byte[32];//= new ulong[4];
            public uint memSize_;
        };

        private static ulong rotl64(ulong x, int r)
        {
            return (x<<r) | (x>>(64-r));
        }

        public static uint read32LE(byte[] bytes, uint offset)
        {
            return bytes[offset] | ((uint)bytes[offset+1]<<8) | ((uint)bytes[offset+2]<<16) | ((uint)bytes[offset+3]<<24);
        }

        public static ulong read64LE(byte[] bytes, uint offset)
        {
            return bytes[offset]
                | ((ulong)bytes[offset+1]<<8)
                | ((ulong)bytes[offset+2]<<16)
                | ((ulong)bytes[offset+3]<<24)
                | ((ulong)bytes[offset+4]<<32)
                | ((ulong)bytes[offset+5]<<40)
                | ((ulong)bytes[offset+6]<<48)
                | ((ulong)bytes[offset+7]<<56);
        }

        public static void init(out Context64 context, ulong seed)
        {
            context = new Context64();
            context.mem_ = new byte[32];
            context.seed_ = seed;
            context.v1_ = seed + PRIME64_1 + PRIME64_2;
            context.v2_ = seed + PRIME64_2;
            context.v3_ = seed + 0;
            context.v4_ = seed - PRIME64_1;
        }

        public static void update(ref Context64 context, byte[] input, uint length)
        {
            context.totalLength_ += length;
            if((context.memSize_+length)<32) {
                System.Array.Copy(input, 0, context.mem_, context.memSize_, length);
                context.memSize_ += length;
                return;
            }

            uint index = 0;
            if(0 != context.memSize_) {
                System.Array.Copy(input, 0, context.mem_, context.memSize_, 32-context.memSize_);

                uint offset = 0;
                context.v1_ += read64LE(context.mem_, offset) * PRIME64_2;
                context.v1_ = rotl64(context.v1_, 31);
                context.v1_ *= PRIME64_1;
                offset += 8;
                context.v2_ += read64LE(context.mem_, offset) * PRIME64_2;
                context.v2_ = rotl64(context.v2_, 31);
                context.v2_ *= PRIME64_1;
                offset += 8;
                context.v3_ += read64LE(context.mem_, offset) * PRIME64_2;
                context.v3_ = rotl64(context.v3_, 31);
                context.v3_ *= PRIME64_1;
                offset += 8;
                context.v4_ += read64LE(context.mem_, offset) * PRIME64_2;
                context.v4_ = rotl64(context.v4_, 31);
                context.v4_ *= PRIME64_1;

                index += 32 - context.memSize_;
                context.memSize_ = 0;
            }
            if((index+32)<=length) {
                uint limit = length-32;
                ulong v1 = context.v1_;
                ulong v2 = context.v2_;
                ulong v3 = context.v3_;
                ulong v4 = context.v4_;
                do {
                    v1 += read64LE(input, index) * PRIME64_2;
                    v1 = rotl64(v1, 31);
                    v1 *= PRIME64_1;
                    index += 8;
                    v2 += read64LE(input, index) * PRIME64_2;
                    v2 = rotl64(v2, 31);
                    v2 *= PRIME64_1;
                    index += 8;
                    v3 += read64LE(input, index) * PRIME64_2;
                    v3 = rotl64(v3, 31);
                    v3 *= PRIME64_1;
                    index += 8;
                    v4 += read64LE(input, index) * PRIME64_2;
                    v4 = rotl64(v4, 31);
                    v4 *= PRIME64_1;
                    index += 8;
                } while(index<=limit);

                context.v1_ = v1;
                context.v2_ = v2;
                context.v3_ = v3;
                context.v4_ = v4;
            }
            if(index<length) {
                context.memSize_ = length-index;
                System.Array.Copy(input, index, context.mem_, 0, context.memSize_);
            }
        }

        public static ulong finalize(ref Context64 context)
        {
            byte[] p = context.mem_;
            ulong h64;

            if(32<=context.totalLength_) {
                ulong v1 = context.v1_;
                ulong v2 = context.v2_;
                ulong v3 = context.v3_;
                ulong v4 = context.v4_;

                h64 = rotl64(v1, 1) + rotl64(v2, 7) + rotl64(v3, 12) + rotl64(v4, 18);

                v1 *= PRIME64_2;
                v1 = rotl64(v1, 31);
                v1 *= PRIME64_1;
                h64 ^= v1;
                h64 = h64*PRIME64_1 + PRIME64_4;

                v2 *= PRIME64_2;
                v2 = rotl64(v2, 31);
                v2 *= PRIME64_1;
                h64 ^= v2;
                h64 = h64*PRIME64_1 + PRIME64_4;

                v3 *= PRIME64_2;
                v3 = rotl64(v3, 31);
                v3 *= PRIME64_1;
                h64 ^= v3;
                h64 = h64*PRIME64_1 + PRIME64_4;

                v4 *= PRIME64_2;
                v4 = rotl64(v4, 31);
                v4 *= PRIME64_1;
                h64 ^= v4;
                h64 = h64*PRIME64_1 + PRIME64_4;
            } else {
                h64 = context.seed_ + PRIME64_5;
            }

            h64 += context.totalLength_;
            uint index = 0;
            while((index+8)<=context.memSize_) {
                ulong k1 = read64LE(p, index);
                k1 *= PRIME64_2;
                k1 = rotl64(k1, 31);
                k1 *= PRIME64_1;
                h64 ^= k1;
                h64 = rotl64(h64, 27) * PRIME64_1 + PRIME64_4;
                index += 8;
            }
            if((index+4)<=context.memSize_) {
                h64 ^= PRIME64_1 * read32LE(p, index);
                h64 = rotl64(h64, 23) * PRIME64_2 + PRIME64_3;
                index += 4;
            }
            while(index<context.memSize_) {
                h64 ^= PRIME64_5 * p[index];
                h64 = rotl64(h64, 11) * PRIME64_1;
                ++index;
            }

            h64 ^= h64>>33;
            h64 *= PRIME64_2;
            h64 ^= h64>>29;
            h64 *= PRIME64_3;
            h64 ^= h64>>32;
            return h64;
        }

        private static string toHex(ulong hash)
        {
            char[] hashChars = new char[16];
            for(int i = 0; i < 16; ++i) {
                hashChars[i] = Digits[(int)(hash&0x0F)];
                hash>>=4;
            }
            return new string(hashChars);
        }


        [StructLayout(LayoutKind.Sequential)]
        private struct Context
        {
            public ulong totalLength_;
            public ulong seed_;
            public ulong v1_;
            public ulong v2_;
            public ulong v3_;
            public ulong v4_;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=4)]
            public ulong[] mem_;
            public ulong memSize_;
        };

        [DllImport(PluginName)]
        private static extern void xxHash64Init(System.IntPtr context, ulong seed);

        [DllImport(PluginName)]
        private static extern void xxHash64Update(System.IntPtr context, System.IntPtr input, uint length);

        [DllImport(PluginName)]
        private static extern ulong xxHash64Finalize(System.IntPtr context);
#endif

        public static string GetHashCS(byte[] bytes)
        {
            Context64 context;
            init(out context, 0);
            update(ref context, bytes, (uint)bytes.Length);
            ulong hash64 = finalize(ref context);
            return toHex(hash64);
        }

        public static string GetHashCS(System.IO.BinaryReader reader)
        {
            Context64 context;
            init(out context, 0);
            byte[] bytes = new byte[1024];
            int size = 0;
            while(0 < (size = reader.Read(bytes, 0, 1024))) {
                update(ref context, bytes, (uint)size);
            }
            ulong hash64 = finalize(ref context);
            return toHex(hash64);
        }

        public static string GetHashCS(string filepath)
        {
            try {
                using(System.IO.FileStream file = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read)) {
                    Context64 context;
                    init(out context, 0);

                    byte[] bytes = new byte[1024];
                    int size = 0;
                    while(0 < (size = file.Read(bytes, 0, 1024))) {
                        update(ref context, bytes, (uint)size);
                    }
                    file.Close();

                    ulong hash64 = finalize(ref context);
                    return toHex(hash64);
                }
            } catch {

            }
            return string.Empty;
        }


        public static string GetHash(byte[] bytes)
        {
            GCHandle hBytes = GCHandle.Alloc(bytes, GCHandleType.Pinned);

            System.IntPtr pContext = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Context)));
            xxHash64Init(pContext, 0);
            xxHash64Update(pContext, hBytes.AddrOfPinnedObject(), (uint)bytes.Length);
            ulong hash = xxHash64Finalize(pContext);

            hBytes.Free();
            return toHex(hash);
        }

        public static string GetHash(System.IO.BinaryReader reader)
        {
            System.IntPtr pContext = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Context)));
            xxHash64Init(pContext, 0);

            byte[] bytes = new byte[1024];
            GCHandle hBytes = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            int size = 0;
            while(0 < (size = reader.Read(bytes, 0, 1024))) {
                xxHash64Update(pContext, hBytes.AddrOfPinnedObject(), (uint)size);
            }
            hBytes.Free();
            ulong hash = xxHash64Finalize(pContext);
            Marshal.FreeHGlobal(pContext);
            return toHex(hash);
        }

        public static string GetHash(string filepath)
        {
            try {
                using(System.IO.FileStream file = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read)) {
                    System.IntPtr pContext = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Context)));
                    xxHash64Init(pContext, 0);

                    byte[] bytes = new byte[1024];
                    GCHandle hBytes = GCHandle.Alloc(bytes, GCHandleType.Pinned);
                    int size = 0;
                    while(0 < (size = file.Read(bytes, 0, 1024))) {
                        xxHash64Update(pContext, hBytes.AddrOfPinnedObject(), (uint)size);
                    }
                    file.Close();
                    hBytes.Free();

                    ulong hash = xxHash64Finalize(pContext);
                    Marshal.FreeHGlobal(pContext);
                    return toHex(hash);
                }
            } catch {

            }
            return string.Empty;
        }
    };
}
