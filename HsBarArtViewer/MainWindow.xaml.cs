using System.Windows;
using System.Windows.Forms;

using FormsDialogResult = System.Windows.Forms.DialogResult;

namespace HsBarArtViewer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

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

		private void BtnPrevious_Click(object sender, RoutedEventArgs e)
		{
			StatusWrite("Previous Clicked");
		}

		private void BtnNext_Click(object sender, RoutedEventArgs e)
		{
			StatusWrite("Next Clicked");
		}

		private void StatusWrite(string text)
		{
			tbStatus.Text += $"{text}\n";
		}
	}
}