﻿using ArmageddonMounter.Wrappers;
using Syroot.Worms;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ArmageddonMounter
{
    // External (virtual) archive - .img replaced with .png and so on
    public class WrappedArchive : Dictionary<string, byte[]>
    {
        public WrappedArchive(string path)
        {
            Load(new Archive(path));
        }

        void Isolate(Mutex mutex, Action act)
        {
            mutex.WaitOne();
            act();
            mutex.ReleaseMutex();
        }

        void Iterate(Mutex mutex, IDictionary<string, byte[]> data, Action<string> act)
        {
            int counter = data.Count;

            foreach (var k in data.Keys)
            {
                new Thread(() =>
                {
                    act(k);
                    Isolate(mutex, () => counter--);
                }).Start();
            }

            bool exit = false;

            do
            {
                Thread.Sleep(3);

                Isolate(mutex, () =>
                {
                    exit = counter == 0;
                });
            } while (!exit);
        }

        void Load(Archive arc)
        {
            using (var m = new Mutex())
            {
                Iterate(m, arc, (k) =>
                {
                    var kLow = k.ToLower();

                    if (kLow.EndsWith(".img"))
                    {
                        var wrap = new ImgWrapper().ToExternal(arc[k]);
                        Isolate(m, () => this[k + ".png"] = wrap);
                    }

                    else
                        Isolate(m, () => this[k] = arc[k]);
                });
            }
        }

        public void Save(string path)
        {
            var arc = new Archive();

            using (var m = new Mutex())
            {
                Iterate(m, this, (k) =>
                {
                    if (k.EndsWith(".img.png"))
                    {
                        var wrap = new ImgWrapper().ToInternal(this[k]);
                        Isolate(m, () => arc[k.Substring(0, k.Length - 4)] = wrap);
                    }

                    else
                        Isolate(m, () => arc[k] = this[k]);
                });
            }

            arc.Save(path);
        }
    }
}
