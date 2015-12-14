using System.Collections.Generic;
using System.Linq;
using HearthstoneDisunity.Unity;
using HearthstoneDisunity.Unity.Objects;
using HearthstoneDisunity.Util;

namespace HearthstoneDisunity.Hearthstone.Bundle
{
    public class TexturesBundle
    {
        private AssestFile _bundle;
        private List<ObjectData> _bundleObjects;
        private Dictionary<string, FilePointer> _texPathMap;

        public TexturesBundle(AssestFile bundle, string outDir)
        {
            _bundle = bundle; // TODO: not used
            _bundleObjects = bundle.Objects;
            BuildReferences();
            ProcessObjects(outDir);
        }

        private void BuildReferences()
        {
            foreach (var obj in _bundleObjects)
            {
                var unityClass = (UnityClass)obj.Info.ClassId;
                if (unityClass == UnityClass.AssetBundle)
                {
                    var data = BinaryBlock.Create(obj.Buffer);
                    var ab = new AssetBundle(data);
                    _texPathMap = ab.Container;
                    Logger.Log("AssetBundle loaded, Container size = " + _texPathMap.Count);
                }
            }
        }

        private void ProcessObjects(string dir)
        {
            foreach (var obj in _bundleObjects)
            {
                var pathId = obj.Id;
                var matchedPath = _texPathMap.FirstOrDefault(x => x.Value.PathID == pathId);
                if (matchedPath.Equals(default(KeyValuePair<string, FilePointer>)))
                {
                    Logger.Log(LogLevel.WARN, "PathId {0} not matched.", pathId);
                }
                else
                {
                    var refPath = matchedPath.Key;

                    var unityClass = (UnityClass)obj.Info.ClassId;
                    if (unityClass == UnityClass.Texture2D)
                    {
                        var data = BinaryBlock.Create(obj.Buffer);
                        var tex = new Texture2D(data);

                        if (tex.Name.ToUpper() == StringUtils.GetFilenameNoExt(refPath).ToUpper())
                        {
                            var cardid = StringUtils.GetFilePathParentDir(refPath).ToUpper();
                            tex.Save(dir, cardid);
                        }
                    }
                }
            }

            //try
            //{
            //    using (BinaryBlock b = new BinaryBlock(System.IO.File.Open(_bundle.FilePath, FileMode.Open)))
            //    {
            //        foreach (var pair in _bundleObjects)
            //        {
            //            var info = pair.Value;
            //            //var subdir = Path.Combine(dir, (UnityClass)info.ClassId);
            //            // TODO: don't create dir if not supported type
            //            //Directory.CreateDirectory(subdir);

            //            b.Seek(info.Offset + _bundle.DataOffset);
            //            byte[] data = new byte[info.Length];
            //            // TODO: can there be loss of precision here, long to int?
            //            Debug.Assert(info.Length <= int.MaxValue);
            //            b.Read(data, 0, (int)info.Length);
            //            var block = BinaryBlock.Create(data);
            //            if (info.ClassId == 28)
            //            {
            //                var tex = new Texture2D(block);
            //                if (_CardArtOld.ContainsKey(tex.Name))
            //                {
            //                    //Console.WriteLine("Tex: " + tex.Name);
            //                    var list = _CardArtOld[tex.Name];
            //                    //Console.WriteLine(list.Count);
            //                    foreach (var c in list)
            //                    {
            //                        //Console.WriteLine(c.Name);
            //                        tex.Save(dir, c.Name);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    throw e;
            //}
        }
    }
}