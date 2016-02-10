using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneMD5 : MonoBehaviour
{
    public const int MinSize = 512*1024;
    public const int MaxSize = 512*1024;
    public const int NumSamples = 100;

    public enum State
    {
        Check =0,
        CSharp,
        Native,
        None,
    };

    private static readonly string Chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    private State state_ = State.Check;
    private int count_;
    private List<byte[]> samples_ = new List<byte[]>();
    private string[] results_ = new string[NumSamples];

    private System.Diagnostics.Stopwatch stopwatch_ = new System.Diagnostics.Stopwatch();

    void Start()
    {
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        for(int i = 0; i < NumSamples; ++i) {
            stringBuilder.Length = 0;
            int size = Random.Range(MinSize, MaxSize);
            for(int j = 0; j < size; ++j) {
                stringBuilder.Append(Chars[Random.Range(0, Chars.Length)]);
            }
            samples_.Add( System.Text.UTF8Encoding.UTF8.GetBytes(stringBuilder.ToString()));
        }
        System.GC.Collect();
    }

    public static string GetMD5Hash(System.Security.Cryptography.MD5 md5, byte[] bytes)
    {
#if false
        byte[] hash = md5.ComputeHash(bytes);

        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        for(int i = 0; i < hash.Length; ++i) {
            stringBuilder.AppendFormat("{0:X2}", hash[i]);
        }
        return stringBuilder.ToString();

#else
        const string Digits = "0123456789ABCDEF";
        byte[] hash = md5.ComputeHash(bytes);
        char[] hashChars = new char[32];
        for(int i = 0; i < 16; ++i) {
            int index = i << 1;
            hashChars[index + 0] = Digits[hash[i] >> 4];
            hashChars[index + 1] = Digits[hash[i] & 0x0F];
        }
        return new string(hashChars);
#endif
    }

	void Update()
    {
        switch(state_)
        {
        case State.Check:
            {
                System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
                for(int i = 0; i < NumSamples; ++i) {
                    string n = LUtil.MD5.GetMD5Hash(samples_[i]);
                    string c = GetMD5Hash(md5, samples_[i]);
                    if(n != c) {
                        Debug.LogFormat("Error: native {0} != csharp {1}", n, c);
                    }
                }
                md5.Clear();
                state_ = State.CSharp;
            }
            break;

        case State.CSharp:
            {
                System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
                stopwatch_.Reset();
                stopwatch_.Start();

                for(int i = 0; i < NumSamples; ++i) {
                    results_[i] = GetMD5Hash(md5, samples_[i]);
                }
                stopwatch_.Stop();
                md5.Clear();
                Debug.LogFormat("charp:{0} samples {1} sec", NumSamples, stopwatch_.Elapsed.TotalSeconds);
                state_ = State.Native;
            }
            break;

        case State.Native:
            {
                stopwatch_.Reset();
                stopwatch_.Start();

                for(int i = 0; i < NumSamples; ++i) {
                    results_[i] = LUtil.MD5.GetMD5Hash(samples_[i]);
                }
                stopwatch_.Stop();
                Debug.LogFormat("native:{0} samples {1} sec", NumSamples, stopwatch_.Elapsed.TotalSeconds);
                state_ = State.None;
            }
            break;

        case State.None:
            System.GC.Collect();
            if(count_ < 10) {
                ++count_;
                state_ = State.CSharp;
            }
            break;
        }
	}
}
