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
/**
@file MD5.cs
@author t-sakai
@date 2016/02/15
*/
using System.Runtime.InteropServices;

namespace LUtil
{
    public static class MD5
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        private const string PluginName = "md5";

#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_IOS
        private const string PluginName = "__Internal";

#elif UNITY_ANDROID
        private const string PluginName = "md5";
#endif

        [StructLayout(LayoutKind.Sequential)]
        private struct Context
        {
            [MarshalAs(UnmanagedType.ByValArray,SizeConst=16)]
            public byte[] hash_;
            [MarshalAs(UnmanagedType.ByValArray,SizeConst=2)]
            public uint[] length_;
            [MarshalAs(UnmanagedType.ByValArray,SizeConst=64)]
            public byte[] buffer_;
        };

        [DllImport(PluginName)]
        private static extern void calcMD5(System.IntPtr hash, uint length, System.IntPtr data);

        [DllImport(PluginName)]
        private static extern void initMD5(System.IntPtr context);

        [DllImport(PluginName)]
        private static extern void processMD5(System.IntPtr context, uint offset, uint length, System.IntPtr data);

        [DllImport(PluginName)]
        private static extern void termMD5(System.IntPtr hash, System.IntPtr context);

        public static byte[] GetMD5HashBytes(byte[] bytes)
        {
            byte[] hash = new byte[16];

            GCHandle hBytes = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            GCHandle hHash = GCHandle.Alloc(hash, GCHandleType.Pinned);
            calcMD5(hHash.AddrOfPinnedObject(), (uint)bytes.Length, hBytes.AddrOfPinnedObject());
            hHash.Free();
            hBytes.Free();
            return hash;
        }

        private const string Digits = "0123456789ABCDEF";
        private static string toHex(byte[] hash)
        {
            char[] hashChars = new char[32];
            for(int i = 0; i < 16; ++i) {
                int index = i<<1;
                hashChars[index+0] = Digits[hash[i]>>4];
                hashChars[index+1] = Digits[hash[i]&0x0F];
            }
            return new string(hashChars);
        }

        public static string GetMD5Hash(byte[] bytes)
        {
            byte[] hash = new byte[16];

            GCHandle hBytes = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            GCHandle hHash = GCHandle.Alloc(hash, GCHandleType.Pinned);

            calcMD5(hHash.AddrOfPinnedObject(), (uint)bytes.Length, hBytes.AddrOfPinnedObject());

            hHash.Free();
            hBytes.Free();
            return toHex(hash);
        }

        public static string GetMD5Hash(System.IO.BinaryReader reader)
        {
            System.IntPtr pContext = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Context)));
            initMD5(pContext);

            byte[] bytes = new byte[64];
            GCHandle hBytes = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            int size = 0;
            while(0 < (size = reader.Read(bytes, 0, 64))) {
                processMD5(pContext, 0, (uint)size, hBytes.AddrOfPinnedObject());
            }
            hBytes.Free();

            byte[] hash = new byte[16];
            GCHandle hHash = GCHandle.Alloc(hash, GCHandleType.Pinned);
            termMD5(hHash.AddrOfPinnedObject(), pContext);
            hHash.Free();
            Marshal.FreeHGlobal(pContext);
            return toHex(hash);
        }

        public static string GetMD5Hash(string filepath)
        {
            try {
                using(System.IO.FileStream file = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read)) {
                    System.IntPtr pContext = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Context)));
                    initMD5(pContext);

                    byte[] bytes = new byte[64];
                    GCHandle hBytes = GCHandle.Alloc(bytes, GCHandleType.Pinned);
                    int size = 0;
                    while(0 < (size = file.Read(bytes, 0, 64))) {
                        processMD5(pContext, 0, (uint)size, hBytes.AddrOfPinnedObject());
                    }
                    file.Close();

                    hBytes.Free();

                    byte[] hash = new byte[16];
                    GCHandle hHash = GCHandle.Alloc(hash, GCHandleType.Pinned);
                    termMD5(hHash.AddrOfPinnedObject(), pContext);
                    hHash.Free();
                    Marshal.FreeHGlobal(pContext);
                    return toHex(hash);
                }
            } catch {

            }
            return string.Empty;
        }
    }
}
