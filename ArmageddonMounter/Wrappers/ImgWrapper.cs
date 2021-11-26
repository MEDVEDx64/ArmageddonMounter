﻿using Syroot.Worms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace ArmageddonMounter.Wrappers
{
    public class ImgWrapper : IConversionWrapper
    {
        [DllImport("PngCheatBox", EntryPoint = "am_png_read", CallingConvention = CallingConvention.Cdecl)]
        unsafe static extern bool ReadPng(PngImageData* dst, void* src, int srcLen);

        [DllImport("PngCheatBox", EntryPoint = "am_png_read_end", CallingConvention = CallingConvention.Cdecl)]
        unsafe static extern void ReadPngEnd(PngImageData* imageData);

        [DllImport("PngCheatBox", EntryPoint = "am_png_make", CallingConvention = CallingConvention.Cdecl)]
        unsafe static extern int MakePng(
            byte* dst, int dstLen,
            byte* srcPixels, int srcLen,
            void* pal, int palItems,
            int width, int height);

        unsafe public byte[] ToExternal(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                var img = new Img(stream);
                if (img.Palette == null)
                    throw new InvalidOperationException("WTF? Do these non-palettized IMGs really exist?");

                var pixels = Marshal.AllocHGlobal(img.Data.Length);
                Marshal.Copy(img.Data, 0, pixels, img.Data.Length);

                var palette = new List<byte>();

                foreach(var col in img.Palette)
                {
                    palette.Add(col.R);
                    palette.Add(col.G);
                    palette.Add(col.B);
                }

                var palettePtr = Marshal.AllocHGlobal(palette.Count);
                Marshal.Copy(palette.ToArray(), 0, palettePtr, palette.Count);

                // Assuming that our PNG will never exceed the size of an uncompressed source
                int pngLen = img.Size.Width * img.Size.Height;
                var png = Marshal.AllocHGlobal(pngLen);

                var pngBytesLen = MakePng((byte*)png, pngLen, (byte*)pixels, img.Data.Length,
                    (void*)palettePtr, palette.Count, img.Size.Width, img.Size.Height);
                if (pngBytesLen <= 0)
                    throw new InvalidOperationException("Conversion failure");

                var pngBytes = new byte[pngBytesLen];
                Marshal.Copy(png, pngBytes, 0, pngBytesLen);

                Marshal.FreeHGlobal(pixels);
                Marshal.FreeHGlobal(palettePtr);
                Marshal.FreeHGlobal(png);

                return pngBytes;
            }
        }

        unsafe public byte[] ToInternal(byte[] bytes)
        {
            byte[] dst = null;
            PngImageData imageData;
            var src = Marshal.AllocHGlobal(bytes.Length);
            Marshal.Copy(bytes, 0, src, bytes.Length);

            var success = ReadPng(&imageData, (void*)src, bytes.Length);

            if (success)
            {
                var pixels = new byte[imageData.Width * imageData.Height];
                Marshal.Copy((IntPtr)imageData.Pixels, pixels, 0, pixels.Length);

                var palette = new List<Color>();
                var paletteBytes = new byte[imageData.PaletteLength];
                Marshal.Copy((IntPtr)imageData.Palette, paletteBytes, 0, imageData.PaletteLength);

                for(int i = 0; i < paletteBytes.Length; i += 3)
                {
                    palette.Add(Color.FromArgb(
                        paletteBytes[i],
                        paletteBytes[i + 1],
                        paletteBytes[i + 2]
                    ));
                }

                var img = new Img();

                img.BitsPerPixel = 8;
                img.Data = pixels;
                img.Palette = palette;
                img.Size = new Size(imageData.Width, imageData.Height);

                using (var dstStream = new MemoryStream())
                {
                    img.Save(dstStream);
                    dst = dstStream.ToArray();
                }
            }

            ReadPngEnd(&imageData);
            Marshal.FreeHGlobal(src);

            if (!success)
                throw new InvalidOperationException("PNG reading has failed");

            return dst;
        }
    }
}
