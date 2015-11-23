using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using HearthstoneDisunity.Unity;
using HearthstoneDisunity.Unity.Objects;
using HearthstoneDisunity.Util;

namespace HearthstoneDisunity.Hearthstone.Bundle
{
    public class TexturesBundle
    {
        private Dictionary<string, List<CardArt>> _cardArt;
        private AssetBundle _bundle;
        private Dictionary<long, ObjectInfo> _bundleObjects;

        public TexturesBundle(AssetBundle bundle, Dictionary<string, List<CardArt>> map)
        {
            _bundle = bundle;
            _bundleObjects = bundle.ObjectMap;
            _cardArt = map;
        }

        public void Extract(string dir)
        {
            try
            {
                using (BinaryFileReader b = new BinaryFileReader(System.IO.File.Open(_bundle.File, FileMode.Open)))
                {
                    foreach (var pair in _bundleObjects)
                    {
                        var info = pair.Value;
                        //var subdir = Path.Combine(dir, UnityClasses.Get(info.ClassId));
                        // TODO: don't create dir if not supported type
                        //Directory.CreateDirectory(subdir);

                        b.Seek(info.Offset + _bundle.DataOffset);
                        byte[] data = new byte[info.Length];
                        // TODO: can there be loss of precision here, long to int?
                        Debug.Assert(info.Length <= int.MaxValue);
                        b.Read(data, 0, (int)info.Length);
                        var block = BinaryFileReader.CreateFromByteArray(data);
                        if (info.ClassId == 28)
                        {
                            var tex = new Texture2D(block);
                            if (_cardArt.ContainsKey(tex.Name))
                            {
                                //Console.WriteLine("Tex: " + tex.Name);
                                var list = _cardArt[tex.Name];
                                //Console.WriteLine(list.Count);
                                foreach (var c in list)
                                {
                                    //Console.WriteLine(c.Name);
                                    tex.Save(dir, c.Name);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}