using HearthstoneDisunity.Util;
using System.Collections.Generic;

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

        public Transform(BinaryFileReader b)
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

        public override string ToString()
        {
            return string.Format("Rot: {0} Pos: {1} Scale: {2}", LocalRotation, LocalPosition, LocalScale);
        }
    }
}
