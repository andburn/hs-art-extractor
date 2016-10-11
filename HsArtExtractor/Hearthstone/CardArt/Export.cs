using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using HsArtExtractor.Util;

namespace HsArtExtractor.Hearthstone.CardArt
{
	public static class Export
	{
		private static readonly int TexDim = 512;
		private static readonly PointF CardBarTL = new PointF(0.0f, 0.3856f);
		private static readonly PointF CardBarBR = new PointF(1.0f, 0.6144f);
		private static readonly Material DefaultMaterial = GetDefaultMaterial();

		public static void CardBar(ArtCard card, Bitmap bmp, string dir, int outHeight, bool cropHidden)
		{
			Bitmap original = new Bitmap(bmp);
			if (bmp.Width != TexDim || bmp.Height != TexDim)
			{
				Logger.Log("{1} not {0}x{0} ({1}x{2})", TexDim, card.Id, bmp.Width, bmp.Height);
				// Warn when not square
				if (bmp.Width != bmp.Height)
					Logger.Log(LogLevel.WARN, $"{card.Id} is not square.");
				original = new Bitmap(bmp, TexDim, TexDim);
			}
			// Tile the original texture x2, for wrapping
			original = TileHorizontal(original);

			var mat = card.GetMaterial(MaterialType.CardBar);
			// if no card bar coords found use default
			if (mat == null)
				mat = DefaultMaterial;

			var standard = mat.GetTransform(TransformType.Standard);
			var shader = mat.GetTransform(TransformType.Shader);
			if (standard == null || shader == null)
			{
				Logger.Log("Transforms are null for {0}", card.Id);
				return;
			}
			Logger.Log(LogLevel.DEBUG, "Calculating bar coords for {0}", card.Id);
			int baseWidth = (int)Math.Round(CardBarBR.X * TexDim - CardBarTL.X * TexDim);
			int baseHeight = (int)Math.Round(CardBarBR.Y * TexDim - CardBarTL.Y * TexDim);
			var coords = GetCardBarRect(standard, shader);
			var x = coords.X;
			var y = coords.Y;
			var width = coords.Width;
			var height = coords.Height;

			try
			{
				var cropRect = new Rectangle(x, y, width, height);
				Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

				using (Graphics g = Graphics.FromImage(target))
				{
					g.DrawImage(original, new Rectangle(0, 0, target.Width, target.Height),
									 cropRect,
									 GraphicsUnit.Pixel);
				}

				// After scale and offset, flip right way up
				target.RotateFlip(RotateFlipType.RotateNoneFlipY);
				// A negative standard X scale/tile, flips on x too
				if (standard.Scale.X < 0)
					target.RotateFlip(RotateFlipType.RotateNoneFlipX);

				Bitmap output = null;
				Size final = new Size(baseWidth, baseHeight);
				if (outHeight > 0)
				{
					// calc new width base on new height
					var ratio = Math.Round((float)baseWidth / baseHeight, 2);
					var outWidth = (int)Math.Round(ratio * outHeight);
					final.Height = outHeight;
					final.Width = outWidth;
				}
				if (cropHidden)
				{
					// safe to crop 10% from left, and 3% right
					var cropAmount = (int)Math.Round(target.Width * 0.1);
					var cropWidth = (int)Math.Round(target.Width * 0.87);
					// required output size
					var outWidth = (int)Math.Round(final.Width * 0.87);
					output = new Bitmap(outWidth, final.Height);
					using (Graphics g = Graphics.FromImage(output))
					{
						g.DrawImage(target,
							new Rectangle(0, 0, outWidth, final.Height),
							new Rectangle(cropAmount, 0, cropWidth, target.Height),
							GraphicsUnit.Pixel);
					}
				}
				else
				{
					output = new Bitmap(target, final.Width, final.Height);
				}
				output.Save(Path.Combine(dir, card.Id + ".png"));
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		public static Rectangle GetCardBarRect(Transform standard, Transform shader)
		{
			// do shader first (offset then scale)
			var shaderX_TL = (CardBarTL.X + shader.Offset.X) * shader.Scale.X;
			var shaderY_TL = (CardBarTL.Y + shader.Offset.Y) * shader.Scale.Y;
			var shaderX_BR = (CardBarBR.X + shader.Offset.X) * shader.Scale.X;
			var shaderY_BR = (CardBarBR.Y + shader.Offset.Y) * shader.Scale.Y;

			// standard (scale then offset)
			var standardX_TL = shaderX_TL * standard.Scale.X + standard.Offset.X;
			var standardY_TL = shaderY_TL * standard.Scale.Y + standard.Offset.Y;
			var standardX_BR = shaderX_BR * standard.Scale.X + standard.Offset.X;
			var standardY_BR = shaderY_BR * standard.Scale.Y + standard.Offset.Y;
			Logger.Log(LogLevel.DEBUG, "Scaled+Offset = ({0}, {1}) ({2}, {3})",
				standardX_TL, standardY_TL, standardX_BR, standardY_BR);

			// check for an x flip (TL & BR have crossed)
			if (standardX_TL > standardX_BR)
			{
				if (standard.Scale.X > 0)
					Logger.Log(LogLevel.DEBUG, "Flipped? {0} < {1}, TileX: {2}", standardX_TL, standardX_BR, standard.Scale.X);
				// flip back
				var w = standardX_TL - standardX_BR;
				standardX_TL -= w;
				standardX_BR += w;
			}

			// get new size
			var widthF = standardX_BR - standardX_TL;
			var heightF = standardY_BR - standardY_TL;
			Logger.Log(LogLevel.DEBUG, "Size = {0} x {1}", widthF, heightF);

			int width = Math.Abs((int)Math.Round(widthF * TexDim));
			int height = Math.Abs((int)Math.Round(heightF * TexDim));
			Logger.Log(LogLevel.DEBUG, "WxH = ({0}, {1})", width, height);

			// get TL coords at expected size
			var x = (int)Math.Round(standardX_TL * TexDim);
			var y = (int)Math.Round(standardY_TL * TexDim);
			Logger.Log(LogLevel.DEBUG, "(x,y) = ({0}, {1})", x, y);

			// reposition the texture back to "origin view"
			var minVisible = TexDim / 4;
			while (x + width > TexDim)
				x -= TexDim;
			while (x + width < minVisible)
				x += TexDim;
			// TODO: should have minvisible for y too
			while (y + height > TexDim)
				y -= TexDim;
			while (y + height < 0)
				y += TexDim;

			if (x < 0)
				x += TexDim;

			Logger.Log(LogLevel.DEBUG, "Rect = ({0}, {1}, {2}, {3})", x, y, width, height);

			return new Rectangle(x, y, width, height);
		}

		public static Material GetDefaultMaterial()
		{
			return new Material() {
				Type = MaterialType.CardBar,
				Transforms = new List<Transform>() {
					new Transform() {
						Type = TransformType.Standard,
						Offset = new CoordinateTransform(0, 0),
						Scale = new CoordinateTransform(1, 1)
					},
					new Transform() {
						Type = TransformType.Shader,
						Offset = new CoordinateTransform(0, 0),
						Scale = new CoordinateTransform(1, 1)
					}
				}
			};
		}

		private static Bitmap TileHorizontal(Bitmap bmp, int tiles = 2)
		{
			Bitmap tiled = new Bitmap(bmp.Width * tiles, bmp.Height);
			using (Graphics g = Graphics.FromImage(tiled))
			{
				for (int i = 0; i < tiles; i++)
				{
					var offset = i * bmp.Width;
					g.DrawImage(bmp,
						new System.Drawing.Rectangle(offset, 0, bmp.Width, bmp.Height),
						new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
						GraphicsUnit.Pixel);
				}
			}
			return tiled;
		}
	}
}