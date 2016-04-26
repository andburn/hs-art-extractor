using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace HsArtExtractor.Hearthstone.CardArt
{
	public class Material
	{
		[XmlAttribute("type")]
		public MaterialType Type { get; set; }

		[XmlElement("Transform")]
		public List<Transform> Transforms { get; set; }

		public Material()
		{
			Transforms = new List<Transform>();
		}

		// Pass a float map for "Shader" transform
		public void AddTransform(Dictionary<string, float> floats)
		{
			var t = new Transform();
			t.Type = TransformType.Shader;
			t.Scale = new CoordinateTransform(1f, 1f);
			t.Offset = new CoordinateTransform(0f, 0f);

			foreach (var entry in floats)
			{
				switch (entry.Key.ToLower())
				{
					case "_offsetx":
						t.Offset.X = entry.Value;
						break;

					case "_offsety":
						t.Offset.Y = entry.Value;
						break;

					case "_scale":
						t.Scale.X = t.Scale.Y = entry.Value;
						break;

					default:
						break;
				}
			}

			Transforms.Add(t);
		}

		public bool IsEmpty()
		{
			return Transforms.Count <= 0;
		}

		// Pass a TexEnv map for the "Standard" transform
		public void AddTransform(Dictionary<string, Unity.Objects.UnityTexEnv> tex)
		{
			var t = new Transform();
			t.Type = TransformType.Standard;
			t.Scale = new CoordinateTransform();
			t.Offset = new CoordinateTransform();

			foreach (var entry in tex)
			{
				if (entry.Key.ToLower() == "_maintex")
				{
					var texEnv = entry.Value;
					t.Scale.X = texEnv.Scale.x;
					t.Scale.Y = texEnv.Scale.y;
					t.Offset.X = texEnv.Offset.x;
					t.Offset.Y = texEnv.Offset.y;
				}
			}

			Transforms.Add(t);
		}

		public void AddTransform(Transform transform)
		{
			Transforms.Add(transform);
		}

		public Transform GetTransform(TransformType type)
		{
			return Transforms.FirstOrDefault(x => x.Type == type);
		}
	}
}