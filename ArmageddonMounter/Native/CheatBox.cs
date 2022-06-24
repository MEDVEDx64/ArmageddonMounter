using System.Runtime.InteropServices;

namespace ArmageddonMounter.Native
{
    public static unsafe class CheatBox
    {
        const string dllName = "PngCheatBox";

        // PNG

        [DllImport(dllName, EntryPoint = "am_png_read", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool ReadPng(PngImageData* dst, void* src, int srcLen);

        [DllImport(dllName, EntryPoint = "am_png_read_end", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ReadPngEnd(PngImageData* imageData);

        [DllImport(dllName, EntryPoint = "am_png_make", CallingConvention = CallingConvention.Cdecl)]
        public static extern int MakePng(
            byte* dst, int dstLen,
            byte* srcPixels, int srcLen,
            void* pal, int palItems,
            int width, int height);

        // Archive

        [DllImport(dllName, EntryPoint = "am_dir_begin", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ArchiveBegin([MarshalAs(UnmanagedType.LPWStr)] string path);

        [DllImport(dllName, EntryPoint = "am_dir_add", CallingConvention = CallingConvention.Cdecl)]
        public static extern void AddFileToArchive(void* data, int len, [MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport(dllName, EntryPoint = "am_dir_end", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ArchiveEnd();
    }
}
