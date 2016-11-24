using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

namespace HsArtExtractor.Util
{
	public class Crunchy
	{
		private static readonly string _binaryUrl =
			@"https://github.com/BinomialLLC/crunch/blob/master/bin/crunch.exe?raw=true";

		private static readonly string _binary = "crunch.exe";
		private const int _timeout = 30;

		public static byte[] Decode(BinaryBlock data)
		{
			var decoded = new byte[0];
			var rawFile = "crunchy.crn";
			var ddsFile = "crunchy.dds";

			if (!File.Exists(_binary))
				DownloadBinary();

			try
			{
				data.WriteAllBytes(rawFile);

				var elapsed = 0;
				var status = Process.Start(_binary, $"-file {rawFile} -out {ddsFile}");
				while (!status.HasExited && elapsed < _timeout)
				{
					Thread.Sleep(1000);
					elapsed++;
				}
				if (elapsed >= _timeout)
					throw new Exception("Crunch process timed out");
				else if (status.ExitCode != 0)
					throw new Exception("Crunch process failed");

				decoded = File.ReadAllBytes(ddsFile);
			}
			catch (Exception e)
			{
				Logger.Log(LogLevel.ERROR, $"Failed to decode file ({e.Message})");
			}
			finally
			{
				if (File.Exists(rawFile))
					File.Delete(rawFile);
				if (File.Exists(ddsFile))
					File.Delete(ddsFile);
			}

			return decoded;
		}

		private static void DownloadBinary()
		{
			using (var wc = new WebClient())
			{
				try
				{
					wc.DownloadFile(_binaryUrl, _binary);
				}
				catch (Exception e)
				{
					Logger.Log(LogLevel.ERROR, $"Failed to download cruncher ({e.Message})");
				}
			}
		}
	}
}