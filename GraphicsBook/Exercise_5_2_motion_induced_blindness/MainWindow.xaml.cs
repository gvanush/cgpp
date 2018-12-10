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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace Exercise_5_2_motion_induced_blindness
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		
		public MainWindow()
		{
			InitializeComponent();

			scene = this.FindName("Scene") as Grid;
			grid = scene.FindName("Grid") as Canvas;

			var dist = CrossSize + InitlaGridSpacing;
			var gridSize = dist * GridDim;

			for (int i = 0; i < GridDim; ++i)
			{
				for (int j = 0; j < GridDim; ++j)
				{
					var cross = new Cross();

					cross.Size = CrossSize;
					cross.Stroke = new SolidColorBrush(Colors.Black);
					cross.Margin = new Thickness(i * dist - gridSize * 0.5, j * dist - gridSize * 0.5, 0.0, 0.0);
					grid.Children.Add(cross);
				}
			}

			dots[0] = scene.FindName("Dot0") as Ellipse;
			dots[1] = scene.FindName("Dot1") as Ellipse;
			dots[2] = scene.FindName("Dot2") as Ellipse;

			

			ready = true;

		}

		private void OnGridColorChange(object sender, RoutedPropertyChangedEventArgs<Color?> e)
		{
			if (!ready) return;

			Color newColor;
			if (e.NewValue.HasValue)
			{
				newColor = e.NewValue.Value;
			}
			else {
				return;
			}

			foreach(Cross cross in grid.Children)
			{
				cross.Stroke = new SolidColorBrush(newColor);
			}
			
		}

		private void OnGridSpacingChange(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (!ready) return;

			var dist = CrossSize + e.NewValue;
			var gridSize = dist * GridDim;

			for (int i = 0; i < GridDim; ++i)
			{
				for (int j = 0; j < GridDim; ++j)
				{
					var cross = grid.Children[i * GridDim + j] as Cross;
					cross.Margin = new Thickness(i * dist - gridSize * 0.5, j * dist - gridSize * 0.5, 0.0, 0.0);
				}
			}
		}

		private void OnGridRotationSpeedChange(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (!ready) return;

			

		}

		private void OnDotColorChange(object sender, RoutedPropertyChangedEventArgs<Color?> e)
		{
			if (!ready) return;

			Color newColor;
			if (e.NewValue.HasValue)
			{
				newColor = e.NewValue.Value;
			}
			else
			{
				return;
			}

			foreach (var dot in dots)
			{
				dot.Fill = new SolidColorBrush(newColor);
			}

		}

		private void OnDotSizeChange(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (!ready) return;

			foreach (var dot in dots)
			{
				dot.Width = e.NewValue;
				dot.Height = e.NewValue;
			}

		}

		Grid scene;
		Canvas grid;
		Ellipse[] dots = new Ellipse[3];
		bool ready = false;
		const int GridDim = 50;
		const double CrossSize = 25.0;
		const double InitlaGridSpacing = 75.0;
	}

	class Cross : Shape
	{

		public Cross() { 
			Size = 1.0;
		}

		protected override Geometry DefiningGeometry {
			get {

				List<PathFigure> figures = new List<PathFigure>(1);
				var halfSize = 0.5 * Size;

				List<PathSegment> horzSeg = new List<PathSegment>(1);
				horzSeg.Add(new LineSegment(new Point(halfSize, 0.0), true));
				figures.Add(new PathFigure(new Point(-halfSize, 0.0), horzSeg, false));

				List<PathSegment> vertSeg = new List<PathSegment>(1);
				vertSeg.Add(new LineSegment(new Point(0.0, -halfSize), true));
				figures.Add(new PathFigure(new Point(0.0, halfSize), vertSeg, false));

				return new PathGeometry(figures, FillRule.Nonzero, null);
			}
		}

		public double Size { get; set; }
		
	}

}
