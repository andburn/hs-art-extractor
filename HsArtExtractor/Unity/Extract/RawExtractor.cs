using System.IO;
using HsArtExtractor.Util;

namespace HsArtExtractor.Unity.Extract
{
    internal class RawExtractor : IExtractor
    {
        public void Extract(ObjectData data, string dir)
        {
            var unityClass = (UnityClass)data.Info.ClassId;
            var path = Path.Combine(dir, unityClass.ToString());
            var file = Path.Combine(path, data.Id.ToString() + ".bin");
            Directory.CreateDirectory(path);
            File.WriteAllBytes(file, data.Buffer);
        }
    }
}