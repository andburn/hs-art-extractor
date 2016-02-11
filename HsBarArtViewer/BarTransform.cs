using System.Drawing;
using HsArtExtractor.Hearthstone.CardArt;

namespace HsBarArtViewer
{
	public class BarTransform
	{
		public string CardId { get; set; }
		public string TextureName { get; set; }
		public string TexturePath { get; set; }
		public CoordinateTransform StandardScale { get; set; }
		public CoordinateTransform StandardOffset { get; set; }
		public CoordinateTransform ShaderOffset { get; set; }
		public CoordinateTransform ShaderScale { get; set; }

		private ArtCard _artCard;
		private Transform _tStandard;
		private Transform _tShader;

		public BarTransform()
		{
			_artCard = new ArtCard();
			_tStandard = new Transform() {
				Type = TransformType.Shader,
				Offset = new CoordinateTransform(-0.2f, 0.25f),
				Scale = new CoordinateTransform(1, 1)
			};
			_tShader = new Transform() {
				Type = TransformType.Shader,
				Offset = new CoordinateTransform(1, 1),
				Scale = new CoordinateTransform(0, 0)
			};
			CardId = "NA";
			TextureName = "NA";
			TexturePath = "NA";
			StandardScale = _tStandard.Scale;
			StandardOffset = _tStandard.Offset;
			ShaderScale = _tShader.Scale;
			ShaderOffset = _tShader.Offset;
		}

		public BarTransform(ArtCard card) : this()
		{
			_artCard = card;
			CardId = card.Id;
			TextureName = card.Texture.Name;
			TexturePath = card.Texture.Path;

			var mat = card.GetMaterial(MaterialType.CardBar);
			if (mat != null)
			{
				foreach (var t in mat.Transforms)
				{
					if (t.Type == TransformType.Standard)
					{
						_tStandard = t;
						StandardScale = t.Scale;
						StandardOffset = t.Offset;
					}
					else if (t.Type == TransformType.Shader)
					{
						_tShader = t;
						ShaderScale = t.Scale;
						ShaderOffset = t.Offset;
					}
				}
			}
		}

		public Rectangle GetRectangle()
		{
			return Export.GetCardBarRect(_tStandard, _tShader);
		}
	}
}