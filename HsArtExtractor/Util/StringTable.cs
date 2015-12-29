using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HsArtExtractor.Util
{
	public class StringTable
	{
		private const int FLAG_INTERNAL = 1 << 31;
   
		private readonly Dictionary<int, string> strings = new Dictionary<int, string>();
    
		public StringTable()
		{
			byte[] data;
			
			try
			{
				data = File.ReadAllBytes("./Data/strings.dat");
				LoadStrings(data, true);
			}
			catch
			{
				Console.WriteLine("ERRROR:::");
				// TODO: do something
			}
		}
    
		public StringTable(byte[] data)
		{
			LoadStrings(data, false);
		}

		private void LoadStrings(byte[] data, bool isIternal) {
			for (int i = 0, n = 0; i < data.Length; i++) {
				if (data[i] == 0) {
					string str = Encoding.ASCII.GetString(data, n, i - n);					
                
					if (isIternal)
						n |= FLAG_INTERNAL;
                
					strings.Add(n, str);

					n = i + 1;
				}
			}
		}
    
		public void LoadStrings(byte[] data) {
			LoadStrings(data, false);
		}
    
		public string GetString(int offset) {
			return strings[offset];
		}

	}
}
