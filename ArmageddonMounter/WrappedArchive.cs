using ArmageddonMounter.Wrappers;
using Syroot.Worms;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ArmageddonMounter
{
    // External (virtual) archive - .img replaced with .png and so on
    public class WrappedArchive : Dictionary<string, byte[]>
    {
        Mutex mutex = null;

        public WrappedArchive(string path)
        {
            Load(new Archive(path));
        }

        Mutex SetUpMutex()
        {
            mutex = new Mutex();
            return mutex;
        }

        void Isolate(Action act)
        {
            mutex.WaitOne();
            act();
            mutex.ReleaseMutex();
        }

        void Iterate(IDictionary<string, byte[]> data, Action<string> act)
        {
            using (SetUpMutex())
            {
                int counter = data.Count;

                foreach (var k in data.Keys)
                {
                    new Thread(() =>
                    {
                        act(k);
                        Isolate(() => counter--);
                    }).Start();
                }

                bool exit = false;

                do
                {
                    Thread.Sleep(3);

                    Isolate(() =>
                    {
                        exit = counter == 0;
                    });
                } while (!exit);
            }
        }

        void Load(Archive arc)
        {
            Iterate(arc, (k) =>
            {
                var kLow = k.ToLower();

                if (kLow.EndsWith(".img"))
                {
                    var wrap = new ImgWrapper().ToExternal(arc[k]);
                    Isolate(() => this[k + ".png"] = wrap);
                }

                else
                    Isolate(() => this[k] = arc[k]);
            });
        }

        public void Save(string path)
        {
            var arc = new Archive();

            Iterate(this, (k) =>
            {
                if (k.EndsWith(".img.png"))
                {
                    var wrap = new ImgWrapper().ToInternal(this[k]);
                    Isolate(() => arc[k.Substring(0, k.Length - 4)] = wrap);
                }

                else
                    Isolate(() => arc[k] = this[k]);
            });

            arc.Save(path);
        }
    }
}
