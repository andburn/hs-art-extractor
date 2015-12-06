using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using HearthstoneDisunity.Util;

namespace HearthstoneDisunity.Unity
{
    public class AssetBundle
    {
        public List<ObjectData> Objects { get; private set; }
        public Dictionary<long, ObjectInfo> ObjectMap { get; private set; }
        public long DataOffset { get; private set; }
        public string BundleFile { get; private set; }
        public string BundleFileName { get; private set; }

        public AssetBundleHeader Header { get; private set; }
        public AssetBundleEntry BundleEntry { get; private set; }
        public AssetHeader AssetHeader { get; private set; }
        public TypeTree TypeTree { get; private set; }
        public ObjectInfoTable InfoTable { get; private set; }

        public AssetBundle(string file)
        {
            BundleFile = file;
            BundleFileName = StringUtils.GetFilenameNoExt(file);
            Read(BundleFile);
        }

        private void Read(string file)
        {
            Logger.Log("Reading " + file);
            try
            {
                using (BinaryBlock b = new BinaryBlock(File.Open(file, FileMode.Open)))
                {
                    // Use BigEndian for bundle header
                    b.BigEndian = true;

                    // Load the Header info
                    Header = new AssetBundleHeader(b);
                    BundleEntry = GetBundleEntry(b, (int)Header.NumberOfFiles);

                    // Move to bundle file offset
                    b.Seek(Header.HeaderSize + BundleEntry.Offset);

                    AssetHeader = new AssetHeader(b);
                    Logger.Log(LogLevel.DEBUG, AssetHeader);

                    // TODO: references? UnityVersion object
                    var version = AssetHeader.AssetVersion;

                    // Should be LittleEndian for assets
                    b.BigEndian = AssetHeader.Endianness == 1 ? true : false;
                    Debug.Assert(b.BigEndian == false);

                    // For older unity versions specify header size
                    if (version < 9)
                        b.Seek(AssetHeader.FileSize - AssetHeader.MetadataSize + 1);

                    // Read the bundle metadata, classes and attributes
                    // NOTE: not actually using directly, keeping to keep binary position correct
                    TypeTree = new TypeTree(version);
                    TypeTree.Read(b);

                    // Read the asset objects info and offsets
                    InfoTable = new ObjectInfoTable(version);
                    InfoTable.Read(b);

                    // NOTE: FileIdentifierTable stuff would go here, but not using it for now

                    // Assign some properties
                    DataOffset = AssetHeader.DataOffset + Header.DataHeaderSize + Header.HeaderSize;
                    ObjectMap = InfoTable.InfoMap;
                    // Gather the asset objects plus data together
                    Objects = LoadObjects(b);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private List<ObjectData> LoadObjects(BinaryBlock b)
        {
            var objects = new List<ObjectData>();
            foreach (var pair in ObjectMap)
            {
                var id = pair.Key;
                var info = pair.Value;

                byte[] buffer = new byte[info.Length];
                b.Seek(info.Offset + DataOffset);
                // TODO: should add eof checks
                Debug.Assert(info.Length <= int.MaxValue);
                b.Read(buffer, 0, (int)info.Length);

                ObjectData data = new ObjectData(id);
                data.Info = info;
                data.Buffer = buffer;

                objects.Add(data);
            }
            return objects;
        }

        private AssetBundleEntry GetBundleEntry(BinaryBlock b, int count)
        {
            Logger.Log(LogLevel.DEBUG, "{0} bundle entries found", count);
            // NOTE: for HS the first bundle is the one required
            if (count > 0)
                return new AssetBundleEntry(b);
            else
                throw new AssetException("No bundle entries found in " + BundleFile);
        }
    }
}