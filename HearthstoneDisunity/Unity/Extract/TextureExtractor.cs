using System.IO;
using HearthstoneDisunity.Unity.Objects;
using HearthstoneDisunity.Util;

namespace HearthstoneDisunity.Unity.Extract
{
    internal class TextureExtractor : IExtractor
    {
        public void Extract(ObjectData data, string dir)
        {
            var unityClass = (UnityClass)data.Info.ClassId;
            if (unityClass == UnityClass.Texture2D)
            {
                var path = Path.Combine(dir, unityClass.ToString());
                Directory.CreateDirectory(path);

                var block = BinaryBlock.Create(data.Buffer);
                var texture = new Texture2D(block);
                texture.Save(path);
            }
        }
    }
}