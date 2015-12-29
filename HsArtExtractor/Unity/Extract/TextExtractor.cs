using System.IO;
using HsArtExtractor.Unity.Objects;
using HsArtExtractor.Util;

namespace HsArtExtractor.Unity.Extract
{
    internal class TextExtractor : IExtractor
    {
        public void Extract(ObjectData data, string dir)
        {
            var unityClass = (UnityClass)data.Info.ClassId;
            if (unityClass == UnityClass.TextAsset || unityClass == UnityClass.Shader)
            {
                var path = Path.Combine(dir, unityClass.ToString());
                Directory.CreateDirectory(path);

                var block = BinaryBlock.Create(data.Buffer);
                var text = new TextAsset(block);
                text.Save(path);
            }
        }
    }
}