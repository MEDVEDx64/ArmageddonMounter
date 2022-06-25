using ArmageddonMounter.Native;
using Syroot.Worms;
using System.IO;
using System.Runtime.InteropServices;

namespace ArmageddonMounter
{
    public static class ArchiveExtensions
    {
        public static unsafe void SaveNative(this Archive arc, string path)
        {
            if (!CheatBox.ArchiveBegin(path))
                throw new IOException("Can't create archive with the specified path: " + path);

            foreach(var k in arc.Keys)
            {
                var mem = Marshal.AllocHGlobal(arc[k].Length);
                Marshal.Copy(arc[k], 0, mem, arc[k].Length);
                var result = CheatBox.AddFileToArchive((void*)mem, arc[k].Length, k);
                Marshal.FreeHGlobal(mem);

                if (!result)
                    throw new IOException("File addition failed");
            }

            if (!CheatBox.ArchiveEnd())
                throw new IOException("Archive finalization error");
        }
    }
}
