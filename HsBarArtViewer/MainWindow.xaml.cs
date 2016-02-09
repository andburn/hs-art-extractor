using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using FormsDialogResult = System.Windows.Forms.DialogResult;

namespace HsBarArtViewer
{
	public partial class MainWindow : Window
	{
		// Determine if we're presently dragging
		private static bool _isDragging = false;

		// The offset from the top, left of the item being dragged
		// and the original mouse down
		private static Point _offset;

		private Image draggedImage;

		// For zoom in/out
		private double _zoomPercent;

		private BitmapImage _original;

		public MainWindow()
		{
			_zoomPercent = 1;
			InitializeComponent();
		}

		// Click Handlers

		private void BtnBrowse_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new FolderBrowserDialog();
			var result = dialog.ShowDialog();
			if (result == FormsDialogResult.OK)
				StatusWrite("Browse: " + dialog.SelectedPath);
		}

		private void BtnCalculate_Click(object sender, RoutedEventArgs e)
		{
			StatusWrite("Calculate Clicked");
		}

		private void BtnReset_Click(object sender, RoutedEventArgs e)
		{
			StatusWrite("Reset Clicked");
			_zoomPercent = 1;
			_offset = new Point(0, 0);
			ImgBase.SetValue(Canvas.LeftProperty, _offset.X);
			ImgBase.SetValue(Canvas.TopProperty, _offset.Y);
		}

		private void BtnPrevious_Click(object sender, RoutedEventArgs e)
		{
			StatusWrite("Previous Clicked");
		}

		private void BtnNext_Click(object sender, RoutedEventArgs e)
		{
			StatusWrite("Next Clicked");
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

			draggedImage = ImgBase;
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
			Point mousePoint = e.GetPosition(CnvMain);

			// Offset the mouse position by the original offset position
			mousePoint.Offset(-_offset.X, -_offset.Y);

			// Move the element on the canvas
			draggedImage.SetValue(Canvas.LeftProperty, mousePoint.X);
			draggedImage.SetValue(Canvas.TopProperty, mousePoint.Y);
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
	}
}