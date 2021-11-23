using MabiClock.Entities;
using System;
using System.IO;
using System.Text.Json;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MabiClock.Windows
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private double TimeGap = 0;
		private readonly Timer Timer;
		private const string SettingsFileName = "settings.json";

		public MainWindow()
		{
			InitializeComponent();
			this.Timer = new Timer();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				var settings = ReadSettings();
				this.Top = settings.TopPosition;
				this.Left = settings.LeftPosition;
				this.Topmost = settings.TopMost;
				this.TimeGap = settings.TimeGap;
				this.Timer.Interval = 100;
				this.Timer.Elapsed += Timer_Elapsed;
				this.Timer.Start();
			}
			catch (Exception)
			{
				//
			}
		}

		private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
		{
			try
			{
				//現実時間取得
				var realTime = DateTime.Now;

				//現実時間を秒数に変換
				var realTimeSecond = ((double)realTime.Hour * 60 + realTime.Minute) * 60 + realTime.Second + ((double)realTime.Millisecond / 1000) + TimeGap;

				//ET経過日数
				//var elapsedDays = realTimeSecond / 2160;

				//ET00:00からの経過RT秒数
				var etFullMinute = realTimeSecond % 2160;

				//ET時
				var etHour = (int)(etFullMinute / 90);

				//ET分
				var etMinute = (int)(etFullMinute % 90 * 0.6666);

				//画面更新
				this.TimeTextBlock.Dispatcher.Invoke(() =>
				{
					this.TimeTextBlock.Text = $"{etHour}:{string.Format("{0:D2}", etMinute)}";
				});
			}
			catch (Exception)
			{
				//
			}
		}

		private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			try
			{
				var settingsWindow = new SettingsWindow
				{
					Owner = this,
				};
				var settings = ReadSettings();
				settingsWindow.TopMostCheckBox.IsChecked = settings.TopMost;
				settingsWindow.TimeGapTextBox.Text = settings.TimeGap.ToString("F1");
				settingsWindow.ShowDialog();
				settings.TopMost = (bool)settingsWindow.TopMostCheckBox.IsChecked;
				settings.TimeGap = double.Parse(settingsWindow.TimeGapTextBox.Text);
				WriteSettings(settings);
				this.Topmost = settings.TopMost;
				this.TimeGap = settings.TimeGap;
				e.Handled = true;
			}
			catch (Exception)
			{
				//
			}
		}

		private void Window_MouseEnter(object sender, MouseEventArgs e)
		{
			try
			{
				this.Background = (LinearGradientBrush)this.Resources["BackgroundBrush"];
			}
			catch (Exception)
			{
				//
			}
		}

		private void Window_MouseLeave(object sender, MouseEventArgs e)
		{
			try
			{
				this.Background = Brushes.Transparent;
			}
			catch (Exception)
			{
				//
			}
		}

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			try
			{
				this.DragMove();
				var settings = ReadSettings();
				settings.TopPosition = this.Top;
				settings.LeftPosition = this.Left;
				WriteSettings(settings);
			}
			catch (Exception)
			{
				//
			}
		}

		private Settings ReadSettings()
		{
			Settings settings;
			if (File.Exists(SettingsFileName))
			{
				using var stream = new FileStream(SettingsFileName, FileMode.Open, FileAccess.Read, FileShare.None);
				settings = JsonSerializer.Deserialize<Settings>(stream);
			}
			else
			{
				settings = new Settings
				{
					TopMost = true,
					TimeGap = -4.1,
				};
			}
			return settings;
		}

		private void WriteSettings(Settings settings)
		{
			using var stream = new FileStream(SettingsFileName, FileMode.Create, FileAccess.Write, FileShare.None);
			JsonSerializer.Serialize(stream, settings);
		}
	}
}
