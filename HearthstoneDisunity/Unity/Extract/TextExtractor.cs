using System.IO;
using HearthstoneDisunity.Unity.Objects;
using HearthstoneDisunity.Util;

namespace HearthstoneDisunity.Unity.Extract
{
    internal class TextExtractor : IExtractor
    {
        public void Extract(ObjectData data, string dir)
        {
            var unityClass = (UnityClass)data.Info.ClassId;
            if (unityClass == UnityClass.TextAsset)
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