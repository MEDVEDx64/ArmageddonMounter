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
            foreach(var k in arc.Keys)
            {
                var kLow = k.ToLower();

                if (kLow.EndsWith(".img"))
                    this[k + ".png"] = new ImgWrapper().ToExternal(arc[k]);

                else
                    this[k] = arc[k];
            }
        }

        /*string ReplaceExt(string path, string ext)
        {
            var pathCut = path.Split('.');
            pathCut[pathCut.Length - 1] = ext;
            return string.Join(".", pathCut);
        }*/

        public void Save(string path)
        {
            var arc = new Archive();

            foreach (var k in Keys)
            {
                if (k.EndsWith(".img.png"))
                    arc[k.Substring(0, k.Length - 4)] = new ImgWrapper().ToInternal(this[k]);

                else
                    arc[k] = this[k];
            }

            arc.Save(path);
        }
    }
}
