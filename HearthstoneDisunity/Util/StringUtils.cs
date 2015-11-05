using System;
using System.IO;
using System.Text;

namespace HearthstoneDisunity.Util
{
    public static class StringUtils
    {
        public static int MakeID(String id)
        {
            if (id.Length != 4)
            {
                throw new ArgumentException("String must be exactly 4 characters long");
            }
            // Assuming ASCII
            byte[] bytes = Encoding.ASCII.GetBytes(id);
            return (bytes[3] << 24) | (bytes[2] << 16) | (bytes[1] << 8) | bytes[0];
        }

        // To get the filename without the extension, mainly for texture asset path
        public static string GetFilenameNoExt(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                return null;

            FileInfo fi;
            try
            {
                fi = new FileInfo(filename);
            }
            catch (Exception)
            {
                fi = new FileInfo("C:/" + filename);
            }
            return fi.Name.Replace(fi.Extension, "");
        }

        // To get the filename without the extension, mainly for texture asset path
        public static string GetFilePathParentDir(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                return null;

            FileInfo fi;
            try
            {
                fi = new FileInfo(filename);
            }
            catch (Exception)
            {
                fi = new FileInfo("C:/" + filename);
            }
            var fid = fi.Directory;
            return fid.Name;
        }
    }
}

