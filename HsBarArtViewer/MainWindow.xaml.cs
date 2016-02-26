using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HsArtExtractor.Hearthstone.CardArt;
using Microsoft.WindowsAPICodePack.Dialogs;
using ControlsImage = System.Windows.Controls.Image;
using WindowsPoint = System.Windows.Point;

namespace HsBarArtViewer
{
	public partial class MainWindow : Window
	{
		// Dragging and Zoom

		private ControlsImage _draggedImage;
		private BitmapImage _original;
		private static WindowsPoint _offset;
		private double _imgScale;
		private static bool _isDragging = false;

		// Bar info

		private FileList _fileList;
		private string _mapFile;
		private ArtCardBarWrapper _barContext;

		public MainWindow()
		{
			_imgScale = 1;
			_mapFile = null;
			_barContext = new ArtCardBarWrapper();
			DataContext = _barContext;
			InitializeComponent();
		}

		// Click Handlers

		private void BtnBrowse_Click(object sender, RoutedEventArgs e)
		{
			var fileExt = "*.png";
			var dirpath = OpenBrowseDialog(true);
			if (dirpath != null)
			{
				StatusWrite("Loading from " + dirpath);
				_fileList = new FileList(
					Directory.GetFiles(dirpath, fileExt));
				LoadFile(_fileList.First);
				LblFolder.Content = dirpath;
			}
		}

		private void BtnMapBrowse_Click(object sender, RoutedEventArgs e)
		{
			var filepath = OpenBrowseDialog(false, "XML Files;xml");
			if (filepath != null)
			{
				CardArtDb.Read(filepath);
				_fileList.UpdateBars();
				LoadFile(_fileList.Current);
				LblMapFile.Content = filepath;
				_mapFile = filepath;
			}
		}

		private void BtnReset_Click(object sender, RoutedEventArgs e)
		{
			ResetView(_barContext);
		}

		private void BtnPrevious_Click(object sender, RoutedEventArgs e)
		{
			SaveChanges();
			LoadFile(_fileList.Previous);
		}

		private void BtnNext_Click(object sender, RoutedEventArgs e)
		{
			SaveChanges();
			LoadFile(_fileList.Next);
		}

		private void BtnToggleMask_Click(object sender, RoutedEventArgs e)
		{
			ImgOverlayOpaque.Visibility =
				ImgOverlayOpaque.IsVisible ? Visibility.Hidden : Visibility.Visible;
			ImgOverlay.Visibility =
				ImgOverlay.IsVisible ? Visibility.Hidden : Visibility.Visible;
		}

		private void BtnUsePrev_Click(object sender, RoutedEventArgs e)
		{
			var prev = _fileList.Peek(-1);
			ResetView(prev.CardBar);
		}

		// Canvas image movement handlers

		private void Canvas_MouseLeftDown(object sender, MouseButtonEventArgs e)
		{
			FrameworkElement element = sender as FrameworkElement;
			if (element == null)
				return;

			// start dragging and get the offset of the mouse relative to the element
			_isDragging = true;
			_offset = e.GetPosition(ImgBase);

			if (ImgBase == null)
				return;

			_draggedImage = ImgBase;
		}

		private void Canvas_MouseLeftUp(object sender, MouseButtonEventArgs e)
		{
			_isDragging = false;
		}

		private void Canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			// If we're not dragging, don't bother
			if (!_isDragging)
				return;

			FrameworkElement element = sender as FrameworkElement;
			if (element == null)
				return;

			// Get the position of the mouse relative to the canvas
			WindowsPoint mousePoint = e.GetPosition(CnvMain);

			// Offset the mouse position by the original offset position
			mousePoint.Offset(-_offset.X, -_offset.Y);

			// Move the element on the canvas
			_draggedImage.SetValue(Canvas.LeftProperty, mousePoint.X);
			_draggedImage.SetValue(Canvas.TopProperty, mousePoint.Y);
		}

		private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			// defuault: 120 forward scroll, -120 backward scroll
			int movement = e.Delta;
			if (movement != 0)
			{
				// amount to scale per delta
				var tick = movement > 0 ? 0.05 : -0.05;
				_imgScale = Math.Round(_imgScale + tick, 2);
				ScaleImage(_imgScale);
			}
		}

		// Utility Methods

		private void StatusWrite(object obj)
		{
			StatusWrite(obj.ToString());
		}

		private void StatusWrite(string text)
		{
			tbStatus.Text += $"{text}\n";
			svStatus.ScrollToBottom();
		}

		private void ScaleImage(double amount)
		{
			_imgScale = amount;

			if (_original == null)
				_original = _fileList.Current.Image;

			var bitmap = new TransformedBitmap(_original,
				new ScaleTransform(amount, amount)
			);

			ImgBase.Source = bitmap;
		}

		private void ResetView()
		{
			_imgScale = 1;
			_offset = new WindowsPoint(0, 0);
			if (_original != null)
				ImgBase.Source = _original;
			ImgBase.SetValue(Canvas.LeftProperty, _offset.X);
			ImgBase.SetValue(Canvas.TopProperty, _offset.Y);
		}

		private void ResetView(ArtCardBarWrapper bar)
		{
			Rectangle rect = bar.GetRectangle();
			if (rect.Width != 0 && rect.Height != 0)
			{
				var calc = CalculateFromBar(rect);
				ScaleImage(calc.Item1);
				ImgBase.SetValue(Canvas.LeftProperty, calc.Item2);
				ImgBase.SetValue(Canvas.TopProperty, calc.Item3);
			}
			else
			{
				ResetView();
			}
		}

		private Tuple<double, double, double> CalculateFromBar(Rectangle rect)
		{
			var scale = 512.0 / rect.Width;
			var yFlip = 512.0 - rect.Y - rect.Height;
			var wDash = rect.Width * scale;
			var hDash = rect.Height * scale;
			var xDash = ((double)rect.X * scale) * -1;
			var yDash = (yFlip * scale) * -1 + 197.5;

			return new Tuple<double, double, double>(scale, xDash, yDash);
		}

		private string OpenBrowseDialog(bool folderSelect = false, string filter = null)
		{
			var dialog = new CommonOpenFileDialog();
			dialog.IsFolderPicker = folderSelect;

			if (!string.IsNullOrEmpty(filter))
			{
				var fs = filter.Split(new char[] { ';' }, 2);
				if (fs.Length >= 2)
					dialog.Filters.Add(new CommonFileDialogFilter(fs[0], fs[1]));
			}

			CommonFileDialogResult result = dialog.ShowDialog();

			if (result == CommonFileDialogResult.Ok)
				return dialog.FileName;
			else
				return null;
		}

		private void SetImageTitle()
		{
			LblImageTitle.Text =
				$"{_barContext.CardId} ({_fileList.Index + 1}/{_fileList.Size})";
		}

		private void LoadFile(FileObject file)
		{
			if (file != null)
			{
				StatusWrite(file.CardBar.CardId);
				StatusWrite(file.CardBar.TexturePath);
				ImgBase.Source = _original = file.Image;
				_barContext = file.CardBar;
				DataContext = _barContext;
				ResetView(_barContext);
				SetImageTitle();
			}
		}

		private void SaveChanges()
		{
			var imgX = Canvas.GetLeft(ImgBase);
			var imgY = Canvas.GetTop(ImgBase);

			var rect = new Rectangle();
			rect.Width = (int)Math.Round(512 / _imgScale);
			rect.Height = (int)Math.Round(117 / _imgScale);
			rect.X = (int)Math.Round((imgX * -1) / _imgScale);
			var yFlip = (int)Math.Round((197.5 - imgY) / _imgScale);
			rect.Y = (int)Math.Round(512.0 - rect.Height - yFlip);

			var original = _barContext.GetRectangle();
			if (original != rect)
			{
				// rectangles are different save changes
				_barContext.SetRectangle(rect);
				_barContext.Save();
				StatusWrite($"Changes saved {_barContext.CardId}");
				// write the changes to the mapfile
				CardArtDb.Write(_mapFile, CardArtDb.Defs);
			}
		}
	}
}