using System;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using System.IO;

namespace GraphicsBook
{

    public struct Vector3 {

        public Vector3(double xx = 0.0, double yy = 0.0, double zz = 0.0) {
            x = xx;
            y = yy;
            z = zz;
        }

        public static double Dot(Vector3 v1, Vector3 v2) {
            return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
        }

        public static Vector3 Cross(Vector3 v1, Vector3 v2) {
            return new Vector3(
                v1.y * v2.z - v1.z * v2.y, 
                v1.z * v2.x - v1.x * v2.z, 
                v1.x * v2.y - v1.y * v2.x);
        }

        public static Vector3 operator +(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }

        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }

        public double x;
        public double y;
        public double z;
    }

    class EdgeI {

        public EdgeI(int ii1, int ii2) { 
            i1 = ii1;
            i2 = ii2;
        }

        public override int GetHashCode() {
            return i1.GetHashCode() + i2.GetHashCode();
        }

        public override bool Equals(object obj) {
            if (this == obj) return true;

            var other = obj as EdgeI;
            if (other == null) return false;

            return (i1 == other.i1 && i2 == other.i2) || (i1 == other.i2 && i2 == other.i1);
        }

        public int i1;
        public int i2;
    }

    class FaceI
    {

        public FaceI(int ii1, int ii2, int ii3)
        {
            i1 = ii1;
            i2 = ii2;
            i3 = ii3;
        }

        public override int GetHashCode()
        {
            return i1.GetHashCode() + i2.GetHashCode() + i3.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;

            var other = obj as FaceI;
            if (other == null) return false;

            return (i1 == other.i1 && ((i2 == other.i2 && i3 == other.i3) || (i2 == other.i3 && i3 == other.i2))) ||
                (i1 == other.i2 && ((i2 == other.i1 && i3 == other.i3) || (i2 == other.i3 && i3 == other.i1))) ||
                (i1 == other.i3 && ((i2 == other.i1 && i3 == other.i2) || (i2 == other.i2 && i3 == other.i1)));
        }

        public int i1;
        public int i2;
        public int i3;
    }

    class Geometry {

        public Geometry(Vector3[] vs, FaceI[] fs) {
            Vertices = vs;
            Faces = fs;
        }

        public Vector3[] Vertices { get; set; }
        public FaceI[] Faces { get; set; }

        public static Geometry Create(string file) {

			var data = JObject.Parse(File.ReadAllText(file));

			var vertices = new List<Vector3>();
			foreach (var token in data["vertices"].Children()) {
				var vertData = token.ToObject<float[]>();
				vertices.Add(new Vector3(vertData[0], vertData[1], vertData[2]));
			}

			var faces = new List<FaceI>();
			foreach (var token in data["faces"].Children())
			{
				var faceData = token.ToObject<int[]>();
				faces.Add(new FaceI(faceData[0], faceData[1], faceData[2]));
			}

            return new Geometry(vertices.ToArray(), faces.ToArray());
        }

    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GraphPaper gp = null; 
        bool ready = false;  // Flag for allowing sliders, etc., to influence display. 
		string shapeFile;
		bool faceCullingEnabled = false;
		int geometryStartingIndex;
		Vector3 position;
		Vector3 rotation;

		public MainWindow()
        {
            InitializeComponent();
            InitializeCommands();
           
            gp = this.FindName("Paper") as GraphPaper;
			ResetTransformation();
			geometryStartingIndex = gp.Children.Count;
			shapeFile = "geometry/cube.json";

			RebuildGeometry();

			ready = true; // Now we're ready to have sliders and buttons influence the display.
        }

		void ResetTransformation() {
			position = new Vector3(0.0, 0.0, 3.0);
			rotation = new Vector3();
		}

		void RebuildGeometry() {

			// Clean the scene
			gp.Children.RemoveRange(geometryStartingIndex, gp.Children.Count - geometryStartingIndex);

			Vector3 eyePos = new Vector3(0.0, 0.0, 0.0);
			var edges = new HashSet<EdgeI>();

			//var geometry = Geometry.Create("geometry/cube.json");
			var geometry = Geometry.Create(shapeFile);

			var transformedVertices = new Vector3[geometry.Vertices.Length];
			for (int i = 0; i < geometry.Vertices.Length; ++i) {

				var vert = geometry.Vertices[i];

				// Around x
				var y = vert.y * Math.Cos(rotation.x) - vert.z * Math.Sin(rotation.x);
				var z = vert.y * Math.Sin(rotation.x) + vert.z * Math.Cos(rotation.x);
				vert.y = y;
				vert.z = z;

				// Around y
				z = vert.z * Math.Cos(rotation.y) - vert.x * Math.Sin(rotation.y);
				var x = vert.z * Math.Sin(rotation.y) + vert.x * Math.Cos(rotation.y);
				vert.x = x;
				vert.z = z;

				// around z
				x = vert.x * Math.Cos(rotation.z) - vert.y * Math.Sin(rotation.z);
				y = vert.x * Math.Sin(rotation.z) + vert.y * Math.Cos(rotation.z);
				vert.x = x;
				vert.y = y;

				transformedVertices[i] = vert + position;
			}

			foreach (var face in geometry.Faces)
			{

				var p1 = transformedVertices[face.i1];
				var p2 = transformedVertices[face.i2];
				var p3 = transformedVertices[face.i3];

				var v1 = p2 - p1;
				var v2 = p3 - p2;

				// Face culling
				if (faceCullingEnabled)
				{
					if (Vector3.Dot(Vector3.Cross(v1, v2), p1 - eyePos) < 0.0)
					{
						edges.Add(new EdgeI(face.i1, face.i2));
						edges.Add(new EdgeI(face.i2, face.i3));
						edges.Add(new EdgeI(face.i3, face.i1));
					}
				}
				else
				{
					edges.Add(new EdgeI(face.i1, face.i2));
					edges.Add(new EdgeI(face.i2, face.i3));
					edges.Add(new EdgeI(face.i3, face.i1));
				}

			}

			double xmin = -0.5;
			double xmax = 0.5;
			double ymin = -0.5;
			double ymax = 0.5;

			Point[] pictureVertices = new Point[transformedVertices.Length];
			double scale = 100;
			for (int i = 0; i < pictureVertices.Length; i++)
			{
				var p = transformedVertices[i];
				double xprime = p.x / p.z;
				double yprime = p.y / p.z;
				pictureVertices[i].X = scale * (1 - (xprime - xmin) / (xmax - xmin));
				pictureVertices[i].Y = scale * (yprime - ymin) / (ymax - ymin); // x / z
				//gp.Children.Add(new Dot(pictureVertices[i].X, pictureVertices[i].Y));
			}

			geometryStartingIndex = gp.Children.Count;
			foreach (var edge in edges)
			{
				gp.Children.Add(new Segment(pictureVertices[edge.i1], pictureVertices[edge.i2]));
			}

		}

		#region Interaction handling -- sliders and buttons
		/* Vestigial handling-code from Testbed2DApp -- unused in this project. */

		void shapeChange(object sender, RoutedEventArgs e) {
			if (!ready)
			{
				return;
			}

			var combo = sender as ComboBox;
			var item = combo.SelectedItem as ComboBoxItem;
			shapeFile = item.Tag as string;
			ResetTransformation();
			RebuildGeometry();
		}

		public void toggleFaceCulling(object sender, RoutedEventArgs e) {
			
			if (!ready) {
				return;
			}

			var checkbox = sender as CheckBox;
			faceCullingEnabled = checkbox.IsChecked.Value;
			RebuildGeometry();
		}

		public void positionXSliderChange(object sender, RoutedEventArgs e)
		{
			if (!ready)
			{
				return;
			}

			var slider = sender as Slider;
			position.x = slider.Value;
			RebuildGeometry();
		}

		public void positionYSliderChange(object sender, RoutedEventArgs e)
		{
			if (!ready)
			{
				return;
			}
			var slider = sender as Slider;
			position.y = slider.Value;
			RebuildGeometry();
		}

		public void positionZSliderChange(object sender, RoutedEventArgs e)
		{
			if (!ready)
			{
				return;
			}
			var slider = sender as Slider;
			position.z = slider.Value;
			RebuildGeometry();
		}

		public void rotationXSliderChange(object sender, RoutedEventArgs e) {
			if (!ready)
			{
				return;
			}
			var slider = sender as Slider;
			rotation.x = slider.Value * Math.PI / 180;
			RebuildGeometry();
		}

		public void rotationYSliderChange(object sender, RoutedEventArgs e)
		{
			if (!ready)
			{
				return;
			}
			var slider = sender as Slider;
			rotation.y = slider.Value * Math.PI / 180;
			RebuildGeometry();
		}

		public void rotationZSliderChange(object sender, RoutedEventArgs e)
		{
			if (!ready)
			{
				return;
			}
			var slider = sender as Slider;
			rotation.z = slider.Value * Math.PI / 180;
			RebuildGeometry();
		}

		#endregion
		#region Menu, command, and keypress handling
		protected static RoutedCommand ExitCommand;
        protected void InitializeCommands()
        {
            InputGestureCollection inp = new InputGestureCollection();
            inp.Add(new KeyGesture(Key.X, ModifierKeys.Control));
            ExitCommand = new RoutedCommand("Exit", typeof(MainWindow), inp);
            CommandBindings.Add(new CommandBinding(ExitCommand, CloseApp));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, CloseApp));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, NewCommandHandler));
        }
        void NewCommandHandler(Object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("You selected the New command",
                                Title,
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
        }
        // Announce keypresses, EXCEPT for CTRL, ALT, SHIFT, CAPS-LOCK, and "SYSTEM" (which is how Windows 
        // seems to refer to the "ALT" keys on my keyboard) modifier keys
        // Note that keypresses that represent commands (like ctrl-N for "new") get trapped and never get
        // to this handler.
        void KeyDownHandler(object sender, KeyEventArgs e)
        {
            if ((e.Key != Key.LeftCtrl) &&
                (e.Key != Key.RightCtrl) &&
                (e.Key != Key.LeftAlt) &&
                (e.Key != Key.RightAlt) &&
                (e.Key != Key.System) &&
                (e.Key != Key.Capital) &&
                (e.Key != Key.LeftShift) &&
                (e.Key != Key.RightShift))
            {
                MessageBox.Show(String.Format("[{0}]  {1} received @ {2}",
                                        e.Key,
                                        e.RoutedEvent.Name,
                                        DateTime.Now.ToLongTimeString()),
                                Title,
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
            }
        }
        void CloseApp(Object sender, ExecutedRoutedEventArgs args)
        {
            if (MessageBoxResult.Yes ==
                MessageBox.Show("Really Exit?",
                                Title,
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question)
               ) Close();
        }
#endregion //Menu, command and keypress handling        
    }
}