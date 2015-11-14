using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HearthstoneDisunity
{
    public static class Extract
    {
        public static void Raw(string outDir, params string[] files)
        {
            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }

            foreach (var f in files)
            {
                if (File.Exists(f))
                {

                }
            }
        }

        public static void All(string outDir, params string[] files)
        {

        }

        public static void Text(string outDir, params string[] files)
        {

        }

        public static void Textures(string outDir, params string[] files)
        {

        }

        public static void CardArt(string outDir, string hsDir, params string[] cardIds)
        {

        }

    }
}
