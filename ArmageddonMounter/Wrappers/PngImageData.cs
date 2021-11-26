namespace ArmageddonMounter.Wrappers
{
    public unsafe struct PngImageData
    {
        public void* Pixels;
        public void* Palette;
        public int PaletteLength;
        public short Width;
        public short Height;
    }
}
