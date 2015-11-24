using HearthstoneDisunity.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HearthstoneDisunity.Unity.Objects
{
    public class Material
    {
        public string Name { get; set; }
        public FilePointer Shader { get; set; }
        public string ShaderKeywords { get; set; }
        public long LightmapFlags { get; set; }
        public int CustomRenderQueue { get; set; }
        public Dictionary<string, string> TagMap { get; set; }
        public Dictionary<string, UnityTexEnv> TexEnvs { get; set; }
        public Dictionary<string, float> Floats { get; set; }
        public Dictionary<string, ColorRGBA> Colors { get; set; }

        public Material(BinaryBlock b)
        {
            int size = b.ReadInt();
            Name = b.ReadFixedString(size);
            b.Align(4);
            var s1 = b.ReadInt();
            //b.Align(4);
            var s2 = b.ReadLong();
            Shader = new FilePointer(s1, s2);
            size = b.ReadInt();
            ShaderKeywords = b.ReadFixedString(size);
            b.Align(4);
            LightmapFlags = b.ReadUnsignedInt();
            CustomRenderQueue = b.ReadInt();

            TagMap = new Dictionary<string, string>();
            size = b.ReadInt();
            for (int i = 0; i < size; i++)
            {
                var fsize = b.ReadInt();
                var key = b.ReadFixedString(fsize);
                b.Align(4);
                fsize = b.ReadInt();
                var value = b.ReadFixedString(fsize);
                b.Align(4);
                TagMap[key] = value;
            }
            b.Align(4);

            TexEnvs = new Dictionary<string, UnityTexEnv>();
            size = b.ReadInt();
            for (int i = 0; i < size; i++)
            {
                var ute = new UnityTexEnv();

                var fsize = b.ReadInt();
                var key = b.ReadFixedString(fsize);
                b.Align(4);

                ute.Texture = new FilePointer(b.ReadInt(), b.ReadLong());
                ute.Scale = new Vector2F(b.ReadFloat(), b.ReadFloat());
                ute.Offset = new Vector2F(b.ReadFloat(), b.ReadFloat());

                TexEnvs[key] = ute;
            }
            b.Align(4);

            Floats = new Dictionary<string, float>();
            size = b.ReadInt();
            for (int i = 0; i < size; i++)
            {
                var fsize = b.ReadInt();
                var key = b.ReadFixedString(fsize);
                b.Align(4);
                Floats[key] = b.ReadFloat();
            }
            b.Align(4);

            Colors = new Dictionary<string, ColorRGBA>();
            size = b.ReadInt();
            for (int i = 0; i < size; i++)
            {
                var fsize = b.ReadInt();
                var key = b.ReadFixedString(fsize);
                b.Align(4);
                var cr = b.ReadFloat();
                //b.Align(4);
                var cg = b.ReadFloat();
                //b.Align(4);
                var cb = b.ReadFloat();
                //b.Align(4);
                var ca = b.ReadFloat();
                //b.Align(4);
                Colors[key] = new ColorRGBA(cr, cg, cb, ca);
            }
            b.Align(4);
        }

        public void Save(string dir, string name = "default")
        {
            string outFile = Name;
            if (string.IsNullOrEmpty(Name))
            {
                outFile = name;
            }
            outFile = Path.Combine(dir, outFile + ".txt");
            // TODO: duplicate check, => rename _2
            using (StreamWriter sw = new StreamWriter(outFile, false))
            {
                sw.WriteLine("Material");
                sw.WriteLine("\tShader: " + Shader.FileID);
                sw.WriteLine("\tShaderKeywords: " + ShaderKeywords);
                sw.WriteLine("\tLightmapFlags: " + LightmapFlags);
                sw.WriteLine("\tCustomRenderQueue: " + CustomRenderQueue);
                sw.WriteLine("\tTagMap (length): " + TagMap.Count);
                sw.WriteLine("\tTexEnvs (length): " + TexEnvs.Count);
                sw.WriteLine("\tFloats (length): " + Floats.Count);
                sw.WriteLine("\tColors (length): " + Colors.Count);
            }
        }

        public override string ToString()
        {
            return string.Format("{0}", Name);
        }
    }
}
