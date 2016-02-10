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

        [DllImport(PluginName)]
        private static extern void calcMD5(System.IntPtr hash, uint length, System.IntPtr data);

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

        public static string GetMD5Hash(byte[] bytes)
        {
            const string Digits = "0123456789ABCDEF";

            byte[] hash = new byte[16];

            GCHandle hBytes = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            GCHandle hHash = GCHandle.Alloc(hash, GCHandleType.Pinned);
            calcMD5(hHash.AddrOfPinnedObject(), (uint)bytes.Length, hBytes.AddrOfPinnedObject());
            hHash.Free();
            hBytes.Free();

            char[] hashChars = new char[32];
            for(int i = 0; i < 16; ++i) {
                int index = i<<1;
                hashChars[index+0] = Digits[hash[i]>>4];
                hashChars[index+1] = Digits[hash[i]&0x0F];
            }
            return new string(hashChars);
        }
    }
}
