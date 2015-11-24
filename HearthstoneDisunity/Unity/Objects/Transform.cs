using HearthstoneDisunity.Util;
using System.Collections.Generic;
using System.IO;

namespace HearthstoneDisunity.Unity.Objects
{
    public class Transform
    {
        public FilePointer GameObject { get; private set; }
        public QuaternionF LocalRotation { get; set; }
        public Vector3F LocalPosition { get; set; }
        public Vector3F LocalScale { get; set; }
        public List<FilePointer> Children { get; private set; }
        public FilePointer Parent { get; private set; }

        public Transform(BinaryBlock b)
        {
            GameObject = new FilePointer(b.ReadInt(), b.ReadLong());
            LocalRotation = new QuaternionF(b.ReadFloat(), b.ReadFloat(), b.ReadFloat(), b.ReadFloat());
            LocalPosition = new Vector3F(b.ReadFloat(), b.ReadFloat(), b.ReadFloat());
            LocalScale = new Vector3F(b.ReadFloat(), b.ReadFloat(), b.ReadFloat());

            Children = new List<FilePointer>();
            var size = b.ReadInt();
            for (int i = 0; i < size; i++)
            {
                Children.Add(new FilePointer(b.ReadInt(), b.ReadLong()));
            }

            Parent = new FilePointer(b.ReadInt(), b.ReadLong());
        }

        public void Save(string dir, string name)
        {
            string outFile = name;
            if (string.IsNullOrEmpty(name))
            {
                throw new AssetException("null filename Transform export");
            }

            outFile = Path.Combine(dir, outFile + ".txt");
            // TODO: duplicate check, => rename _2
            using (StreamWriter sw = new StreamWriter(outFile, false))
            {
                sw.WriteLine("Transform");
                sw.WriteLine("\tGameObject: " + GameObject.PathID);
                sw.WriteLine("\tLocalRotation: " + LocalRotation);
                sw.WriteLine("\tLocalPosition: " + LocalPosition);
                sw.WriteLine("\tLocalScale: " + LocalScale);
                sw.WriteLine("\tChildren [" + Children.Count + "]");
                for (var i = 0; i < Children.Count; i++)
                {
                    sw.WriteLine("\t\t[" + i + "]: " + Children[i]);
                }

                sw.WriteLine("\tParent: " + Parent.PathID);
            }
        }

        public override string ToString()
        {
            return string.Format("Rot: {0} Pos: {1} Scale: {2}", LocalRotation, LocalPosition, LocalScale);
        }
    }
}
