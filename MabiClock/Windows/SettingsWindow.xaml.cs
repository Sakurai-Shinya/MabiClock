using System.Text.RegularExpressions;
using System.Windows;

namespace MabiClock.Windows
{
	/// <summary>
	/// SettingsWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class SettingsWindow : Window
	{
		public SettingsWindow()
		{
			InitializeComponent();
		}

		private void ExitButton_Click(object sender, RoutedEventArgs e)
		{
			this.Owner.Close();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!Regex.IsMatch(this.TimeGapTextBox.Text, "^-?[0-9]+[.][0-9]+$"))
			{
				MessageBox.Show("時間指定の形式が間違っています。", "MabiClock", MessageBoxButton.OK, MessageBoxImage.Information);
				e.Cancel = true;
				return;
			}
		}
	}
}
