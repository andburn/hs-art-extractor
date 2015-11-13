using HearthstoneDisunity.Unity.Objects;
using System.Text;

namespace HearthstoneDisunity.Util
{
    public class CardObject
	{
		public string Name { get; set; }
		public string PortraitName
		{
			get
			{
				if (string.IsNullOrWhiteSpace(PortraitPath))
				{
					return null;
				}
				else
				{
					return StringUtils.GetFilenameNoExt(PortraitPath);
				}
			}
		}
		public string PortraitFileDir
		{
			get
			{
				if(string.IsNullOrWhiteSpace(PortraitPath))
				{
					return null;
				}
				else
				{
					return StringUtils.GetFilePathParentDir(PortraitPath);
				}
			}
		}
		public string PortraitPath { get; set; }
		public CardMaterial CardBarPortrait { get; set; }
		public CardMaterial EnchantmentPortrait { get; set; }

		public override string ToString()
		{
			StringBuilder str = new StringBuilder();

			str.AppendLine("---- " + Name);

			str.Append("PortraitName: ");
			if(!string.IsNullOrWhiteSpace(PortraitPath))
				str.AppendLine(PortraitName);
			else
				str.AppendLine();

			str.Append("PortraitDir: ");
			if(!string.IsNullOrWhiteSpace(PortraitPath))
				str.AppendLine(PortraitFileDir);
			else
				str.AppendLine();

			str.Append("Portrait: ");
			if(!string.IsNullOrWhiteSpace(PortraitPath))
				str.AppendLine(PortraitPath);
			else
				str.AppendLine();
			
			str.Append("CardBar: ");
			if(CardBarPortrait != null && !CardBarPortrait.IsEmpty())
				str.AppendLine(CardBarPortrait.ToString());
			else
				str.AppendLine();

			str.Append("Enchantment: ");
			if(EnchantmentPortrait != null && !EnchantmentPortrait.IsEmpty())
				str.AppendLine(EnchantmentPortrait.ToString());
			else
				str.AppendLine();

			return str.ToString();
		}

		public void SetEnchantment(object p)
		{
			if(p.GetType() == typeof(Material))
			{
				var m = (Material)p;
				var cm = new CardMaterial();

				cm.Name = m.Name; //TODO: is empty
				//Console.WriteLine("Material name = " + m.Name);
				
				var fts = m.Floats;
				foreach(var f in fts.Keys)
				{
					switch(f.ToLowerInvariant())
					{
					case "_offsetx":
						cm.OffsetX = fts[f];
						break;
					case "_offsety":
						cm.OffsetY = fts[f];
						break;
					case "_scale":
						cm.Scale = fts[f];
						break;
					case "_transition":
						cm.Transition = fts[f];
						break;
					case "_valuerange":
						cm.ValueRange = fts[f];
						break;
					default:
						break;
					}
				}
				EnchantmentPortrait = cm;
			}
		}

		public void SetCardBar(object p)
		{
			if(p.GetType() == typeof(Material))
			{
				var m = (Material)p;
				var cm = new CardMaterial();
				cm.Name = m.Name; //TODO: is empty
				var fts = m.Floats;
				foreach(var f in fts.Keys)
				{
					switch(f.ToLowerInvariant())
					{
					case "_offsetx":
						cm.OffsetX = fts[f];
						break;
					case "_offsety":
						cm.OffsetY = fts[f];
						break;
					case "_scale":
						cm.Scale = fts[f];
						break;
					case "_transition":
						cm.Transition = fts[f];
						break;
					case "_valuerange":
						cm.ValueRange = fts[f];
						break;
					default:
						break;
					}
				}
				CardBarPortrait = cm;
			}
		}
	}

	public class CardMaterial
	{
		public string Name { get; set; }
		public float OffsetX { get; set; }
		public float OffsetY { get; set; }
		public float Scale { get; set; }
		public float Transition { get; set; }
		public float ValueRange { get; set; }

		public override string ToString()
		{
			return string.Format("({0}, {1}) {2}", OffsetX, OffsetY, Scale);
		}

		public bool IsEmpty()
		{
			// TODO: all zero not really "empty"
			return OffsetX == 0.0f && OffsetY == 0.0f && Scale == 0.0f && Transition == 0.0f && ValueRange == 0.0f;
		}
	}

}
