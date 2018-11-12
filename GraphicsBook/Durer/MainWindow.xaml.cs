using System;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.Collections.Generic;

namespace GraphicsBook
{

    public struct Vector3 {

        public Vector3(double xx, double yy, double zz) {
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

        public static Geometry Cube() {

            Vector3[] vertices = new Vector3[8] {
                new Vector3(-0.5, -0.5, 2.5),
                new Vector3(-0.5, 0.5, 2.5),
                new Vector3(0.5, 0.5, 2.5),
                new Vector3(0.5, -0.5, 2.5),
                new Vector3(-0.5, -0.5, 3.5),
                new Vector3(-0.5, 0.5, 3.5),
                new Vector3(0.5, 0.5, 3.5),
                new Vector3(0.5, -0.5, 3.5)
            };

            for(int i = 0; i < vertices.Length; ++i) {
                vertices[i].y -= 1.0;
        vertices[i].z += 5.0;
      }

            FaceI[] faces = new FaceI[12] {
                new FaceI(0, 1, 2),
                new FaceI(0, 2, 3),
                new FaceI(2, 6, 7),
                new FaceI(2, 7, 3),
                new FaceI(5, 4, 7),
                new FaceI(5, 7, 6),
                new FaceI(5, 1, 0),
                new FaceI(5, 0, 4),
                new FaceI(0, 3, 7),
                new FaceI(0, 7, 4),
                new FaceI(1, 5, 6),
                new FaceI(1, 6, 2)
            };

            return new Geometry(vertices, faces);
        }

        public static Geometry TriangularPrism()
        {

            Vector3[] vertices = new Vector3[6] {
                new Vector3(-0.5, -0.5, 2.5),
                new Vector3(0.5, -0.5, 2.5),
                new Vector3(0.0, -0.5, 3.5),
                new Vector3(-0.5, 0.5, 2.5),
                new Vector3(0.5, 0.5, 2.5),
                new Vector3(0.0, 0.5, 3.5),
            };

      for (int i = 0; i < vertices.Length; ++i)
      {
        vertices[i].y -= 4.0;
        vertices[i].z += 5.0;
      }

      FaceI[] faces = new FaceI[8] {
                new FaceI(1, 2, 0),
                new FaceI(4, 3, 5),
                new FaceI(0, 2, 3),
                new FaceI(2, 5, 3),
                new FaceI(1, 4, 2),
                new FaceI(2, 4, 5),
                new FaceI(1, 0, 4),
                new FaceI(4, 0, 3)
            };

            return new Geometry(vertices, faces);
        }

    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GraphPaper gp = null; 
        bool ready = false;  // Flag for allowing sliders, etc., to influence display. 
        public MainWindow()
        {
            InitializeComponent();
            InitializeCommands();
            // Now add some graphical items in the main Canvas, whose name is "GraphPaper"
            gp = this.FindName("Paper") as GraphPaper;
            
            Vector3 eyePos = new Vector3(0.0, 0.0, 0.0);
            
            // Build a table of edges
            /*int [,] etable = new int[nMaxEdges, 2]{
                {0, 1}, {1, 2}, {2, 3}, {3,0}, // one face
                {0,4}, {1,5}, {2, 6}, {3, 7},  // joining edges
                {4, 5}, {5, 6}, {6, 7}, {7, 4}}; // opposite face*/
            var edges = new HashSet<EdgeI>();

            //var geometry = Geometry.Cube();
            var geometry = Geometry.TriangularPrism();

            foreach (var face in geometry.Faces)
            {

                var p1 = geometry.Vertices[face.i1];
                var p2 = geometry.Vertices[face.i2];
                var p3 = geometry.Vertices[face.i3];

                var v1 = p2 - p1;
                var v2 = p3 - p2;

                bool faceCullingEnabled = true;
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
                else {
                    edges.Add(new EdgeI(face.i1, face.i2));
                    edges.Add(new EdgeI(face.i2, face.i3));
                    edges.Add(new EdgeI(face.i3, face.i1));
                }
                
            }

            double xmin = -0.5;
            double xmax = 0.5;
            double ymin = -0.5;
            double ymax = 0.5;

            Point [] pictureVertices = new Point[geometry.Vertices.Length];
            double scale = 100;
            for (int i = 0; i < pictureVertices.Length; i++)
            {
                var p = geometry.Vertices[i];
                double xprime = p.x / p.z;
                double yprime = p.y / p.z;
                pictureVertices[i].X = scale * (1-(xprime - xmin) / (xmax - xmin));
                pictureVertices[i].Y = scale * (yprime - ymin) / (ymax - ymin); // x / z
                //gp.Children.Add(new Dot(pictureVertices[i].X, pictureVertices[i].Y));
            }

            foreach (var edge in edges)
            {
                gp.Children.Add(new Segment(pictureVertices[edge.i1], pictureVertices[edge.i2]));
            }
            
            ready = true; // Now we're ready to have sliders and buttons influence the display.
        }



#region Interaction handling -- sliders and buttons
        /* Vestigial handling-code from Testbed2DApp -- unused in this project. */

        /* Event handler for a click on button one */
        public void b1Click(object sender, RoutedEventArgs e)
        {
            Debug.Print("Button one clicked!\n");
            e.Handled = true; // don't propagate the click any further
        }

        void slider1change(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Debug.Print("Slider changed, ready = " + ready + ", and val = " + e.NewValue + ".\n");
            e.Handled = true;
            if (ready)
            {
            }
        }
        public void b2Click(object sender, RoutedEventArgs e)
        {
            Debug.Print("Button two clicked!\n");
            e.Handled = true; // don't propagate the click any further
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