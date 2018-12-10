using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Exercise_5_3_alternating_dots
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			canvas = FindName("Canvas") as Canvas;

			const double Radius = 2.5;
			const double Diam = 2.0 * Radius;
		

			for (int i = 0; i < dots.Length; ++i) {
				var dot = new Ellipse();
				dot.Width = Diam;
				dot.Height = Diam;
				dot.Fill = new SolidColorBrush(Colors.Black);
				dot.Margin = new Thickness(i * 10.0 + spacing, 0.0, 0.0, 0.0);
				dots[i] = dot;
				canvas.Children.Add(dot);
			}

			System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
			dispatcherTimer.Tick += Tick;
			dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
			dispatcherTimer.Start();

		}

		private void Tick(object sender, EventArgs e)
		{
			if (flag)
			{
				for (int i = 0; i < dots.Length; ++i)
				{
					var dot = canvas.Children[i] as Ellipse;
					dot.Margin = new Thickness(i * 10.0, 0.0, 0.0, 0.0);
				}
			}
			else
			{
				for (int i = 0; i < dots.Length; ++i)
				{
					var dot = canvas.Children[i] as Ellipse;
					dot.Margin = new Thickness(i * 10.0 + spacing, 0.0, 0.0, 0.0);
				}
			}

			flag = !flag;
		}

		private void OnSpacingChange(object sender, RoutedPropertyChangedEventArgs<double> e) {
			spacing = e.NewValue;
		}

		Ellipse[] dots = new Ellipse[3];
		double spacing = 2.5;
		static bool flag = false;
		Canvas canvas;
	}

}
