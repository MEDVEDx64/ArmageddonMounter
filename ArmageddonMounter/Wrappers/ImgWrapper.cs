using Syroot.Worms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace ArmageddonMounter.Wrappers
{
    public class ImgWrapper : IConversionWrapper
    {
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

        public byte[] ToInternal(byte[] bytes)
        {
            using(var srcStream = new MemoryStream(bytes))
            {
                using (var png = Image.FromStream(srcStream))
                {
                    var pixels = new byte[png.Width * png.Height];

                    using (var bitmap = new Bitmap(png))
                    {
                        var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                            ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
                        Marshal.Copy(data.Scan0, pixels, 0, pixels.Length);
                        bitmap.UnlockBits(data);
                    }

                    var img = new Img();

                    img.BitsPerPixel = 8;
                    img.Data = pixels;
                    img.Palette = png.Palette.Entries.ToList();
                    img.Size = png.Size;

                    using (var dstStream = new MemoryStream())
                    {
                        img.Save(dstStream);
                        return dstStream.ToArray();
                    }
                }
            }
        }
    }
}
