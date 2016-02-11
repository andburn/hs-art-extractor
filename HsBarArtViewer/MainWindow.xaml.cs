﻿using System;
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

		// Data context object
		private BarTransform _barContext;

		public MainWindow()
		{
			_zoomPercent = 1;
			_fileIndex = 0;
			_barContext = new BarTransform();
			DataContext = _barContext;
			InitializeComponent();
		}

		// Click Handlers

		private void BtnBrowse_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new FolderBrowserDialog();
			//dialog.RootFolder = Environment.SpecialFolder.MyDocuments;
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
			StatusWrite(_barContext.GetRectangle().ToString());
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
			// defuault: 120 forward scroll, -120 backward scroll
			int movement = e.Delta;
			StatusWrite("Delta:" + movement);

			if (movement != 0)
			{
				var tick = movement > 0 ? 0.05 : -0.05;
				_zoomPercent = Math.Round(_zoomPercent + tick, 2);
				ZoomImage(_zoomPercent);
			}

			StatusWrite("Zoom level after: " + _zoomPercent);
		}

		// Utility Methods

		private void StatusWrite(string text)
		{
			tbStatus.Text += $"{text}\n";
			svStatus.ScrollToBottom();
		}

		private void ZoomImage(double amount)
		{
			if (_original == null)
				_original = (BitmapImage)ImgBase.Source; // TODO cast can be bad here

			StatusWrite("Zoom level: " + amount);

			var bitmap = new TransformedBitmap(_original,
				new ScaleTransform(amount, amount)
			);

			ImgBase.Source = bitmap;
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
				Rectangle rect = new Rectangle();
				if (CardArtDb.All.ContainsKey(cardId))
				{
					StatusWrite(CardArtDb.All[cardId].Texture.Path);
					_barContext = new BarTransform(CardArtDb.All[cardId]);
					DataContext = _barContext;
					// Rect stuff
					rect = _barContext.GetRectangle();
				}
				ResetView();
				// unset view!
				if (rect.Width != 0 && rect.Height != 0)
				{
					var scale = 512.0 / rect.Width;
					StatusWrite("bar scale = " + scale);
					ZoomImage(scale);

					var yFlip = 512.0 - rect.Y - rect.Height; // TODO need to double check this, seems wrong
					var wDash = rect.Width * scale;
					var hDash = rect.Height * scale;
					var xDash = ((double)rect.X * scale) * -1;
					var yDash = (yFlip * scale) * -1 + 197.5;

					ImgBase.SetValue(Canvas.LeftProperty, xDash);
					ImgBase.SetValue(Canvas.TopProperty, yDash);
					//var shape = new System.Windows.Shapes.Rectangle();
					//shape.Width = rect.Width;
					//shape.Height = rect.Height;
					//shape.SetValue(Canvas.LeftProperty, (double)rect.X);
					//shape.SetValue(Canvas.TopProperty, yFlip);
					//shape.Stroke = new SolidColorBrush() { Color = System.Windows.Media.Colors.Azure };
					//CnvMain.Children.Add(shape);
				}
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