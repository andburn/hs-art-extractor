using System;
using System.Collections.Generic;
using System.Drawing;
using HsArtExtractor.Hearthstone.CardArt;

namespace HsBarArtViewer
{
	public class ArtCardBarWrapper
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

		public ArtCardBarWrapper()
		{
			_artCard = new ArtCard();
			var mat = Export.GetDefaultMaterial();
			_tStandard = mat.GetTransform(TransformType.Standard);
			_tShader = mat.GetTransform(TransformType.Shader);
			CardId = "NA";
			TextureName = "NA";
			TexturePath = "NA";
			StandardScale = _tStandard.Scale;
			StandardOffset = _tStandard.Offset;
			ShaderScale = _tShader.Scale;
			ShaderOffset = _tShader.Offset;
		}

		public ArtCardBarWrapper(ArtCard card) : this()
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

		public void SetRectangle(Rectangle rect)
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
		}

		public void Save()
		{
			if (_artCard.GetMaterial(MaterialType.CardBar) == null)
			{
				var barmat = new Material() {
					Type = MaterialType.CardBar,
					Transforms = new List<Transform>() {
						new Transform() { Type = TransformType.Standard },
						new Transform() { Type = TransformType.Shader }
					}
				};
				_artCard.Materials.Add(barmat);
			}
			var barMaterial = _artCard.GetMaterial(MaterialType.CardBar);
			var shdr = barMaterial.GetTransform(TransformType.Shader);
			var std = barMaterial.GetTransform(TransformType.Standard);
			std.Offset = new CoordinateTransform(StandardOffset.X, StandardOffset.Y);
			std.Scale = new CoordinateTransform(StandardScale.X, StandardScale.Y);
			shdr.Offset = new CoordinateTransform(ShaderOffset.X, ShaderOffset.Y);
			shdr.Scale = new CoordinateTransform(ShaderScale.X, ShaderScale.Y);
		}

		public override string ToString()
		{
			return $"{ShaderOffset.X} {ShaderOffset.Y} {ShaderScale.X} {ShaderScale.Y}";
		}
	}
}