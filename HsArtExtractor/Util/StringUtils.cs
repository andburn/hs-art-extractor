using System;
using System.IO;
using System.Text;

namespace HsArtExtractor.Util
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

		public static string GetFilenameNoOverwrite(string filename)
		{
			if (File.Exists(filename))
			{
				var count = 1;
				try
				{
					var fi = new FileInfo(filename);
					var name = fi.Name.Replace(fi.Extension, "");
					var renamed = filename;
					do
					{
						renamed = Path.Combine(fi.DirectoryName, name + "_" + count + fi.Extension);
						count++;
					} while (File.Exists(renamed));
					return renamed;
				}
				catch
				{
					Logger.Log("Failed to create new filename for existing file: {0}", filename);
				}
			}
			return filename;
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
			catch
			{
				fi = new FileInfo("C:/" + filename);
			}
			var extIdx = fi.Name.LastIndexOf(fi.Extension);
			return fi.Name.Substring(0, extIdx);
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

		// Remove any file system invalid chars from the string,
		// and spaces with underscores.
		// A good enough solution, doesn't cover all possilbe invalid cases.
		public static string SafeString(string str)
		{
			var invalidChars = Path.GetInvalidFileNameChars();
			var invalidSplit = str.Split(invalidChars,
				StringSplitOptions.RemoveEmptyEntries);
			return string.Join("_", invalidSplit).TrimEnd('.').Replace(' ', '_');
		}
	}
}