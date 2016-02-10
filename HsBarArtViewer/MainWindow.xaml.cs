using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HsArtExtractor.Hearthstone.CardArt;
using HsArtExtractor.Util;
using ControlsImage = System.Windows.Controls.Image;
using FormsDialogResult = System.Windows.Forms.DialogResult;
using WindowsPoint = System.Windows.Point;

namespace HsBarArtViewer
{
	public partial class MainWindow : Window
	{
		private static bool _isDragging = false;

		// The offset from the top, left of the item being dragged
		// and the original mouse down
		private static WindowsPoint _offset;

		private ControlsImage _draggedImage;
		private BitmapImage _original;

		// For zoom in/out
		private double _zoomPercent;

		// File list
		private string[] _fileList;

		// Current file index in the list
		private int _fileIndex;

		public MainWindow()
		{
			_zoomPercent = 1;
			_fileIndex = 0;
			InitializeComponent();
		}

		// Click Handlers

		private void BtnBrowse_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new FolderBrowserDialog();
			var result = dialog.ShowDialog();
			if (result == FormsDialogResult.OK)
			{
				StatusWrite("Loading from " + dialog.SelectedPath);
				_fileList = Directory.GetFiles(dialog.SelectedPath, "*.png");
				LoadFile();
			}
		}

		private void BtnMapBrowse_Click(object sender, RoutedEventArgs e)
		{
			StatusWrite("MapBrowse Clicked");
			var dialog = new OpenFileDialog();
			dialog.DefaultExt = ".xml";
			dialog.Filter = "XML Files (*.xml)|*.xml";

			var result = dialog.ShowDialog();
			if (result == FormsDialogResult.OK)
			{
				string filename = dialog.FileName;
				StatusWrite("MapBrowse: " + filename);
				// read db info from file
				// TODO: handle exceptions?
				CardArtDb.Read(filename);
				LoadFile();
			}
		}

		private void BtnCalculate_Click(object sender, RoutedEventArgs e)
		{
			StatusWrite("Calculate Clicked");
		}

		private void BtnReset_Click(object sender, RoutedEventArgs e)
		{
			StatusWrite("Reset Clicked");
			ResetView();
		}

		private void BtnPrevious_Click(object sender, RoutedEventArgs e)
		{
			StatusWrite("Previous Clicked");
			if (_fileList != null && _fileIndex - 1 >= 0)
				_fileIndex -= 1;
			LoadFile();
		}

		private void BtnNext_Click(object sender, RoutedEventArgs e)
		{
			StatusWrite("Next Clicked");
			if (_fileList != null && _fileIndex + 1 < _fileList.Length)
				_fileIndex += 1;
			LoadFile();
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
			if (_original == null)
				_original = (BitmapImage)ImgBase.Source; // TODO cast can be bad here

			// defuault: 120 forward scroll, -120 backward scroll
			int movement = e.Delta;

			StatusWrite("Zoom level: " + _zoomPercent);
			StatusWrite("Delta:" + movement);

			if (movement != 0)
			{
				var tick = movement > 0 ? 0.05 : -0.05;
				_zoomPercent = Math.Round(_zoomPercent + tick, 2);
			}

			StatusWrite("Zoom level after: " + _zoomPercent);
			if (movement != 0)
			{
				var bitmap = new TransformedBitmap(_original,
					new ScaleTransform(_zoomPercent, _zoomPercent)
				);

				ImgBase.Source = bitmap;
			}
		}

		// Utility Methods

		private void StatusWrite(string text)
		{
			tbStatus.Text += $"{text}\n";
		}

		private void ResetView()
		{
			_zoomPercent = 1;
			_offset = new WindowsPoint(0, 0);
			if (_original != null)
				ImgBase.Source = _original;
			ImgBase.SetValue(Canvas.LeftProperty, _offset.X);
			ImgBase.SetValue(Canvas.TopProperty, _offset.Y);
		}

		private void LoadFile()
		{
			if (_fileList != null && _fileList.Length > 0)
			{
				var filename = _fileList[_fileIndex];
				var cardId = StringUtils.GetFilenameNoExt(filename);
				ImgBase.Source = BitmapToImageSource(new Bitmap(filename));
				_original = (BitmapImage)ImgBase.Source; // TODO same problem as above
				if (CardArtDb.All.ContainsKey(cardId))
				{
					StatusWrite(CardArtDb.All[cardId].Texture.Path);
				}
				ResetView();
			}
		}

		private BitmapImage BitmapToImageSource(Bitmap bitmap)
		{
			using (MemoryStream memory = new MemoryStream())
			{
				bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
				memory.Position = 0;
				BitmapImage bitmapimage = new BitmapImage();
				bitmapimage.BeginInit();
				bitmapimage.StreamSource = memory;
				bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapimage.EndInit();

				return bitmapimage;
			}
		}
	}
}