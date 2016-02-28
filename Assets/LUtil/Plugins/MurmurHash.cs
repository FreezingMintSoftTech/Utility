//-----------------------------------------------------------------------------
// MurmurHash3 was written by Austin Appleby, and is placed in the public
// domain. The author hereby disclaims copyright to this source code.
/**
@file MurmurHash.cs
@author t-sakai
@date 2016/02/29
*/
using System.Runtime.InteropServices;

namespace LUtil
{
    public class MurmurHash
    {
        private const string Digits = "0123456789ABCDEF";
        private const uint c1_32 = 0xcc9e2d51U;
        private const uint c2_32 = 0x1b873593U;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        private const string PluginName = "MurmurHash";

#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_IOS
        private const string PluginName = "__Internal";

#elif UNITY_ANDROID
        private const string PluginName = "MurmurHash";
#endif

        public struct Context32
        {
            public uint totalLength_;
            public uint h1_;
            public uint memLength_;
            public byte[] mem_;
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct Context
        {
            public uint totalLength_;
            public uint h1_;
            public uint memLength_;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=4)]
            public byte[] mem_;
        };

        [DllImport(PluginName)]
        private static extern void MurmurHash32Init(System.IntPtr context, uint seed);

        [DllImport(PluginName)]
        private static extern void MurmurHash32Update4(System.IntPtr context, System.IntPtr input, uint offset, uint length);

        [DllImport(PluginName)]
        private static extern void MurmurHash32Tail(System.IntPtr context, System.IntPtr input, uint offset, uint length);

        [DllImport(PluginName)]
        private static extern uint MurmurHash32Finalize(System.IntPtr context);


        private static uint read32LE(byte[] bytes, int offset)
        {
            return bytes[offset] | ((uint)bytes[offset+1]<<8) | ((uint)bytes[offset+2]<<16) | ((uint)bytes[offset+3]<<24);
        }

        private static uint rotl32(uint x, int r)
        {
            return (x<<r) | (x>>(32-r));
        }

        private static uint fmix32(uint h)
        {
            h ^= h >> 16;
            h *= 0x85ebca6bU;
            h ^= h >> 13;
            h *= 0xc2b2ae35U;
            h ^= h >> 16;
            return h;
        }

        public static void init(out Context32 context, uint seed)
        {
            context = new Context32();
            context.h1_ = seed;
            context.mem_ = new byte[4];
        }

        public static void update4(ref Context32 context, byte[] data, int length)
        {
            int nblocks = length>>2;
            int index = nblocks<<2;

            for(int i=-index; i<0; i+=4){
                uint k1 = read32LE(data, index+i);
                k1 *= c1_32;
                k1 = rotl32(k1,15);
                k1 *= c2_32;

                context.h1_ ^= k1;
                context.h1_ = rotl32(context.h1_,13);
                context.h1_ = context.h1_*5 + 0xe6546b64U;
            }
            context.totalLength_ += (uint)length;
        }

        public static void updateTail(ref Context32 context, byte[] data, int length)
        {
            int nblocks = length>>2;
            int index = nblocks<<2;

            for(int i=-index; i<0; i+=4){
                uint k1 = read32LE(data, index+i);
                k1 *= c1_32;
                k1 = rotl32(k1,15);
                k1 *= c2_32;

                context.h1_ ^= k1;
                context.h1_ = rotl32(context.h1_,13);
                context.h1_ = context.h1_*5 + 0xe6546b64U;
            }

            int t = length & 3;
            uint l1 = 0;
            switch(length&3)
            {
            case 3:
                l1 ^= (uint)data[index+2] << 16;
                l1 ^= (uint)data[index+1] << 8;
                l1 ^= (uint)data[index];
                l1 *= c1_32;
                l1 = rotl32(l1,15);
                l1 *= c2_32;
                context.h1_ ^= l1;
                break;
            case 2:
                l1 ^= (uint)data[index+1] << 8;
                l1 ^= (uint)data[index];
                l1 *= c1_32;
                l1 = rotl32(l1,15);
                l1 *= c2_32;
                context.h1_ ^= l1;
                break;
            case 1:
                l1 ^= (uint)data[index];
                l1 *= c1_32;
                l1 = rotl32(l1,15);
                l1 *= c2_32;
                context.h1_ ^= l1;
                break;
            }
            context.totalLength_ += (uint)length;
        }

        public static uint finalize(ref Context32 context)
        {
            context.h1_ ^= context.totalLength_;
            return fmix32(context.h1_);
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

        public static string GetHashCS(byte[] bytes)
        {
            Context32 context;
            init(out context, 0);
            int count = bytes.Length>>10;
            byte[] tmp = new byte[1024];
            for(int i=0; i<count; ++i){
                int index= i<<10;
                tmp[0] = bytes[index+0];
                tmp[1] = bytes[index+1];
                tmp[2] = bytes[index+2];
                tmp[3] = bytes[index+3];
                update4(ref context, tmp, tmp.Length);
            }

            int tailLen = bytes.Length & 1023;
            int offset = count<<10;
            for(int i=0; i<tailLen; ++i){
                tmp[i] = bytes[offset+i];
            }
            updateTail(ref context, tmp, tailLen);
            uint hash32 = finalize(ref context);
            return toHex(hash32);
        }

        public static string GetHash(byte[] bytes)
        {
            System.IntPtr pContext = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Context)));
            MurmurHash32Init(pContext, 0);

            GCHandle hBytes = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            System.IntPtr ptr = hBytes.AddrOfPinnedObject();
            MurmurHash32Tail(pContext, ptr, 0, (uint)bytes.Length);

            hBytes.Free();
            uint hash = MurmurHash32Finalize(pContext);
            Marshal.FreeHGlobal(pContext);
            return toHex(hash);
        }

        public static string GetHash(System.IO.BinaryReader reader)
        {
            System.IntPtr pContext = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Context)));
            MurmurHash32Init(pContext, 0);

            byte[] bytes = new byte[1024];
            GCHandle hBytes = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            System.IntPtr ptr = hBytes.AddrOfPinnedObject();
            int size = 0;
            while(0 < (size = reader.Read(bytes, 0, 1024))) {
                if(size<1024){
                    MurmurHash32Tail(pContext, ptr, 0U, (uint)size);
                }else{
                    MurmurHash32Update4(pContext, ptr, 0U, (uint)size);
                }
            }
            hBytes.Free();
            uint hash = MurmurHash32Finalize(pContext);
            Marshal.FreeHGlobal(pContext);
            return toHex(hash);
        }

        public static string GetHash(string filepath)
        {
            try {
                using(System.IO.FileStream file = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read)) {
                    System.IntPtr pContext = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Context)));
                    MurmurHash32Init(pContext, 0);

                    byte[] bytes = new byte[1024];
                    GCHandle hBytes = GCHandle.Alloc(bytes, GCHandleType.Pinned);
                    System.IntPtr ptr = hBytes.AddrOfPinnedObject();
                    int size = 0;
                    while(0 < (size = file.Read(bytes, 0, 1024))) {
                        if(size<1024) {
                            MurmurHash32Tail(pContext, ptr, 0U, (uint)size);
                        } else {
                            MurmurHash32Update4(pContext, ptr, 0U, (uint)size);
                        }
                    }
                    file.Close();
                    hBytes.Free();

                    uint hash = MurmurHash32Finalize(pContext);
                    Marshal.FreeHGlobal(pContext);
                    return toHex(hash);
                }
            } catch {

            }
            return string.Empty;
        }
	};
}
