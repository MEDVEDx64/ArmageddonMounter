using ArmageddonMounter.Wrappers;
using Syroot.Worms;
using System;
using System.Collections.Generic;
using System.IO;

namespace ArmageddonMounter
{
    // External (virtual) archive - .img replaced with .png and so on
    public class WrappedArchive : Dictionary<string, byte[]>
    {
        Dictionary<string, byte[]> unmodified = new Dictionary<string, byte[]>();

        public WrappedArchive(string path)
        {
            Load(new Archive(path));
        }

        void Load(Archive arc)
        {
            foreach (var k in arc.Keys)
            {
                var kLow = k.ToLower();

                if (kLow.EndsWith(".img"))
                {
                    var wrap = new ImgWrapper().ToExternal(arc[k]);
                    this[k + ".png"] = wrap;
                    unmodified[k + ".png"] = arc[k];
                }

                else
                    this[k] = arc[k];
            }
        }

        public void Save(string path)
        {
            var arc = new Archive();
            Exception lastExc = null;
            string lastFaultedKey = null;

            foreach (var k in Keys)
            {
                try
                {
                    if (k.EndsWith(".img.png"))
                    {
                        var wrap = new ImgWrapper().ToInternal(this[k]);
                        arc[k.Substring(0, k.Length - 4)] = wrap;
                    }

                    else
                        arc[k] = this[k];
                }

                catch(Exception e)
                {
                    lastExc = e;
                    lastFaultedKey = k;
                    if (!unmodified.ContainsKey(k))
                        throw new InvalidOperationException("File conversion failed: " + k);

                    arc[Path.GetFileNameWithoutExtension(k)] = unmodified[k];
                }
            }

            arc.SaveNative(path);

            if (lastExc != null)
                throw new WrappedFileException(lastExc, lastFaultedKey);
        }
    }
}
