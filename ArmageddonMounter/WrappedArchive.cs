using ArmageddonMounter.Wrappers;
using Syroot.Worms;
using System.Collections.Generic;

namespace ArmageddonMounter
{
    // External (virtual) archive - .img replaced with .png and so on
    public class WrappedArchive : Dictionary<string, byte[]>
    {
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
                }

                else
                    this[k] = arc[k];
            }
        }

        public void Save(string path)
        {
            var arc = new Archive();

            foreach (var k in Keys)
            {
                if (k.EndsWith(".img.png"))
                {
                    var wrap = new ImgWrapper().ToInternal(this[k]);
                    arc[k.Substring(0, k.Length - 4)] = wrap;
                }

                else
                    arc[k] = this[k];
            }

            arc.Save(path);
        }
    }
}
