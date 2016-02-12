using System;
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

		public string SetRectangle(Rectangle rect)
		{
			PointF CardBarTL = new PointF(0.0f, 0.3856f);
			PointF CardBarBR = new PointF(1.0f, 0.6144f);
			float TexDim = 512.0f;

			PointF shd = new PointF(rect.X / TexDim, rect.Y / TexDim);
			var scale = rect.Width / TexDim;
			var offX = shd.X / scale - CardBarTL.X;
			var offY = shd.Y / scale - CardBarTL.Y;

			StandardOffset.X = 0;
			StandardOffset.Y = 0;
			StandardScale.X = StandardScale.Y = 1;

			ShaderOffset.X = (float)Math.Round(offX, 2);
			ShaderOffset.Y = (float)Math.Round(offY, 2);
			ShaderScale.X = ShaderScale.Y = (float)Math.Round(scale, 2);

			return $"{shd} {offX} {offY} {scale}";
		}

		public override string ToString()
		{
			return $"{ShaderOffset.X} {ShaderOffset.Y} {ShaderScale.X} {ShaderScale.Y}";
		}
	}
}