/**
@file TGA.cs
@author t-sakai
@date 2016/02/15
*/
using UnityEngine;

namespace LUtil
{
    public static class TGA
    {
        const byte Type_None = 0x00;
        const byte Type_IndexColor = 0x01;
        const byte Type_FullColor = 0x02;
        const byte Type_Gray = 0x03;
        const byte Type_IndexColorRLE = 0x09;
        const byte Type_FullColorRLE = 0x0A;
        const byte Type_GrayRLE = 0x0B;

        static System.UInt16 getU16(byte low, byte high)
        {
            return (System.UInt16)((high << 8) | low);
        }

        const int Offset_IDLeng = 0;
        const int Offset_ColorMapType = 1;
        const int Offset_ImageType = 2;

        const int Offset_ColorMapOriginL = 3;
        const int Offset_ColorMapOriginH = 4;

        const int Offset_ColorMapLengL = 5;
        const int Offset_ColorMapLengH = 6;

        const int Offset_ColorMapSize = 7;

        const int Offset_OriginXL = 8;
        const int Offset_OriginXH = 9;
        const int Offset_OriginYL = 10;
        const int Offset_OriginYH = 11;

        const int Offset_WidthL = 12;
        const int Offset_WidthH = 13;
        const int Offset_HeightL = 14;
        const int Offset_HeightH = 15;
        const int Offset_BitPerPixel = 16;
        const int Offset_Discripter = 17;

        const int TGA_HEADER_SIZE = 18;

        public static Texture2D load(byte[] bytes)
        {
            if(bytes == null || bytes.Length < TGA_HEADER_SIZE) {
                return null;
            }

            int width = (int)(getU16(bytes[Offset_WidthL], bytes[Offset_WidthH]));
            int height = (int)(getU16(bytes[Offset_HeightL], bytes[Offset_HeightH]));

            int bpp = (int)(bytes[Offset_BitPerPixel]);

            byte type = bytes[Offset_ImageType];

            //Debug.Log(string.Format("TGA: {0}x{1} bpp:{2} type:{3}", width, height, bpp, type));

            // 非圧縮のみ受け入れる
            if(type != Type_FullColor
                && type != Type_Gray) {
                //Debug.Log("Color Type " + type);
                return null;
            }

            //32bit or 24bit or 8bitを受け入れる
            if(bpp != 32 && bpp != 24 && bpp != 8) {
                //Debug.Log("BPP " + bpp);
                return null;
            }

            int pos = TGA_HEADER_SIZE;

            // ID文字列があればスキップ
            if(bytes[Offset_IDLeng] > 0) {
                pos += bytes[Offset_IDLeng];
            }

            // カラーマップがあればスキップ
            if(bytes[Offset_ColorMapType] == 1) {
                int length = (int)(getU16(bytes[Offset_ColorMapLengL], bytes[Offset_ColorMapLengH]));
                length *= (bytes[Offset_ColorMapSize] >> 3);
                pos += length;
            }

            int numElems = (bpp >> 3);
            int offset = pos;

            int pixels = width * height;
            Color[] colors = new Color[pixels];
            Texture2D texture;
            const float ratio = 1.0f / 255.0f;
            if(numElems == 1) {
                int index = offset;
                for(int i = 0; i < pixels; ++i) {
                    colors[i] = new Color32(bytes[index], bytes[index], bytes[index], 255);
                    ++index;
                }
                texture = new Texture2D(width, height, TextureFormat.Alpha8, false);

            } if(numElems == 3) {
                int index = offset;
                for(int i = 0; i < pixels; ++i) {
                    colors[i].r = ratio * bytes[index + 2];
                    colors[i].g = ratio * bytes[index + 1];
                    colors[i].b = ratio * bytes[index + 0];
                    colors[i].a = 1.0f;
                    index += 3;
                }
                texture = new Texture2D(width, height, TextureFormat.RGB24, false);

            } else {
                int index = offset;
                for(int i = 0; i < pixels; ++i) {
                    colors[i].r = ratio * bytes[index + 2];
                    colors[i].g = ratio * bytes[index + 1];
                    colors[i].b = ratio * bytes[index + 0];
                    colors[i].a = ratio * bytes[index + 3];
                    index += 4;
                }
                texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            }
            texture.SetPixels(colors);
            texture.Apply();
            return texture;
        }
    }
}
