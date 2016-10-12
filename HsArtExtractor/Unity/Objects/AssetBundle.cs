using System.Collections.Generic;
using HsArtExtractor.Util;

namespace HsArtExtractor.Unity.Objects
{
    public class AssetBundle
    {
        private List<FilePointer> _preload;
        private Dictionary<string, FilePointer> _container;
        private List<FilePointer> _nameless; // TODO: is this needed

        public string Name { get; private set; }

        public List<FilePointer> PreloadTable
        {
            get { return _preload; }
        }

        public Dictionary<string, FilePointer> Container
        {
            get { return _container; }
        }

        public AssetBundle(BinaryBlock b)
        {
            var nameSize = b.ReadInt();
            Name = b.ReadFixedString(nameSize);
            b.Align(4);

            _nameless = new List<FilePointer>();
            _preload = new List<FilePointer>();
            var preSize = b.ReadInt();
            for (int i = 0; i < preSize; i++)
            {
                _preload.Add(new FilePointer(b.ReadInt(), b.ReadLong()));
            }

            _container = new Dictionary<string, FilePointer>();
            var containerSize = b.ReadInt();
            for (int i = 0; i < containerSize; i++)
            {
                var len = b.ReadInt();
                var str = b.ReadFixedString(len);
                b.Align(4);
                var idx = b.ReadInt();
                var size = b.ReadInt();
                var file = new FilePointer(b.ReadInt(), b.ReadLong());

                if (string.IsNullOrWhiteSpace(str))
                {
                    _nameless.Add(file);
                }
                else if (_container.ContainsKey(str))
                {
                    Logger.Log(LogLevel.ERROR,
                        "Duplicate AssetBundle entry for {0} -> {1}", str, file.PathID);
                }
                else
                {
                    _container[str] = file;
                }
            }

            // Skip MainAsset info (not used here)
        }
    }
}