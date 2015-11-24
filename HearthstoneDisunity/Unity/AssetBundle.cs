using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using HearthstoneDisunity.Unity.Objects;
using HearthstoneDisunity.Util;

namespace HearthstoneDisunity.Unity
{
    public class AssetBundle
    {
        public Dictionary<long, ObjectInfo> ObjectMap { get; set; }
        public AssetBundleHeader Header { get; private set; }
        public AssetBundleEntry BundleEntry { get; private set; }
        public AssetHeader AssetHeader { get; private set; }
        public TypeTree TypeTree { get; private set; }
        public ObjectInfoTable InfoTable { get; private set; }
        public string BundleFile { get; private set; }
        public long DataOffset { get; private set; }

        public AssetBundle(string file)
        {
            BundleFile = file;
            Read(BundleFile);
        }

        public void ExtractRaw(string dir)
        {
            try
            {
                using (BinaryBlock b = new BinaryBlock(System.IO.File.Open(BundleFile, FileMode.Open)))
                {
                    foreach (var pair in ObjectMap)
                    {
                        var info = pair.Value;
                        var subdir = UnityClasses.Get(info.ClassId);
                        Directory.CreateDirectory(Path.Combine(dir, subdir));
                        b.Seek(info.Offset + DataOffset);

                        byte[] data = new byte[info.Length];
                        // TODO: can there be loss of precision here, long to int?
                        Debug.Assert(info.Length <= int.MaxValue);
                        b.Read(data, 0, (int)info.Length);

                        var outFile = Path.Combine(dir, subdir, pair.Key + ".bin");
                        System.IO.File.WriteAllBytes(outFile, data);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void ExtractText(string dir)
        {
            try
            {
                using (BinaryBlock b = new BinaryBlock(System.IO.File.Open(BundleFile, FileMode.Open)))
                {
                    foreach (var pair in ObjectMap)
                    {
                        var info = pair.Value;
                        b.Seek(info.Offset + DataOffset);

                        byte[] data = new byte[info.Length];
                        // TODO: can there be loss of precision here, long to int?
                        Debug.Assert(info.Length <= int.MaxValue);
                        b.Read(data, 0, (int)info.Length);

                        var block = BinaryBlock.CreateFromByteArray(data);
                        // TODO: enum for class ids
                        if (info.ClassId == 49)
                        {
                            var text = new TextAsset(block);
                            text.Save(dir);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void ExtractFull(string dir)
        {
            try
            {
                using (BinaryBlock b = new BinaryBlock(System.IO.File.Open(BundleFile, FileMode.Open)))
                {
                    foreach (var pair in ObjectMap)
                    {
                        var info = pair.Value;
                        var subdir = Path.Combine(dir, UnityClasses.Get(info.ClassId));
                        // TODO: don't create dir if not supported type
                        Directory.CreateDirectory(subdir);

                        b.Seek(info.Offset + DataOffset);
                        byte[] data = new byte[info.Length];
                        // TODO: can there be loss of precision here, long to int?
                        Debug.Assert(info.Length <= int.MaxValue);
                        b.Read(data, 0, (int)info.Length);
                        var block = BinaryBlock.CreateFromByteArray(data);
                        switch (info.ClassId)
                        {
                            case 1: // GameObject
                                new GameObject(block).Save(subdir, pair.Key.ToString());
                                break;

                            case 4: // Transform
                                new Transform(block).Save(subdir, pair.Key.ToString());
                                break;

                            case 21: // Material
                                new Material(block).Save(subdir, pair.Key.ToString());
                                break;

                            case 28: // Texture2D
                                new Texture2D(block).Save(subdir);
                                break;

                            case 49: // TextAsset
                                new TextAsset(block).Save(subdir);
                                break;

                            case 114: // MonoBehaviour
                                      // TODO: not only carddef obviously
                                new CardDef(block).Save(subdir, pair.Key.ToString());
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void Read(string file)
        {
            try
            {
                using (BinaryBlock b = new BinaryBlock(System.IO.File.Open(file, FileMode.Open)))
                {
                    b.BigEndian = true;

                    Header = new AssetBundleHeader(b);
                    Console.WriteLine(Header);

                    if (Header.NumberOfFiles != 1)
                    {
                        // TODO: handle elsewhere?
                        //throw new AssetException("Should be exactly one file in HS Asset Bundle");
                        // shared now has 2 files :)
                        // probably get away with ignoring second?
                        Console.WriteLine("Warning: " + file + " has " + Header.NumberOfFiles + " bundle files");
                    }

                    BundleEntry = new AssetBundleEntry(b);
                    Console.WriteLine(BundleEntry);

                    // move to bundle file offset
                    b.Seek(Header.HeaderSize + BundleEntry.Offset);

                    AssetHeader = new AssetHeader(b);
                    Console.WriteLine(AssetHeader);

                    // TODO: references? UnityVersion object
                    var version = AssetHeader.AssetVersion;

                    // should be little endian
                    b.BigEndian = AssetHeader.Endianness == 1 ? true : false;

                    if (version < 9)
                        b.Seek(AssetHeader.FileSize - AssetHeader.MetadataSize + 1);

                    // read the bundle metadata, classes and attributes
                    TypeTree = new TypeTree(version);
                    TypeTree.Read(b);

                    // read the asset objects info and offsets
                    InfoTable = new ObjectInfoTable(version);
                    InfoTable.Read(b);
                    // assign
                    ObjectMap = InfoTable.InfoMap;

                    // Skip this not using this for now
                    //FileIdentifierTable fi = new FileIdentifierTable(version);
                    //fi.Read(b);

                    DataOffset = AssetHeader.DataOffset + Header.DataHeaderSize + Header.HeaderSize;

                    // LoadObjects(InfoTable.InfoMap, TypeTree.TypeMap, b);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}