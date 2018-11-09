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
            // Build a table of vertices:
            const int nPoints = 8;
            //const int nEdges = 12;
            
            const int nFaces = 6;
            Vector3 eyeDirection = new Vector3(0.0, 0.0, 1.0);
            double[,] vtable = new double[nPoints, 3] 
            {
            {-0.5, -0.5, 2.5},
            {-0.5, 0.5, 2.5},
            {0.5, 0.5, 2.5},
            {0.5, -0.5, 2.5},
            {-0.5, -0.5, 3.5},
            {-0.5, 0.5, 3.5},
            {0.5, 0.5, 3.5},
            {0.5, -0.5, 3.5}};
            // Build a table of edges
            /*int [,] etable = new int[nMaxEdges, 2]{
                {0, 1}, {1, 2}, {2, 3}, {3,0}, // one face
                {0,4}, {1,5}, {2, 6}, {3, 7},  // joining edges
                {4, 5}, {5, 6}, {6, 7}, {7, 4}}; // opposite face*/
            var edges = new HashSet<EdgeI>();
            int[,] ftable = new int[nFaces, 4] {
                {0, 1, 2, 3}, 
                {2, 6, 7, 3}, 
                {5, 4, 7, 6}, 
                {5, 1, 0, 4},
                {0, 3, 7, 4},
                {1, 5, 6, 2}
            };

            for (int i = 0; i < nFaces; i++)
            {

                var p0x = vtable[ftable[i, 0], 0];
                var p0y = vtable[ftable[i, 0], 1];
                var p0z = vtable[ftable[i, 0], 2];

                var p1x = vtable[ftable[i, 1], 0];
                var p1y = vtable[ftable[i, 1], 1];
                var p1z = vtable[ftable[i, 1], 2];

                var p2x = vtable[ftable[i, 2], 0];
                var p2y = vtable[ftable[i, 2], 1];
                var p2z = vtable[ftable[i, 2], 2];

                var v1 = new Vector3(p1x - p0x, p1y - p0y, p1z - p0z);
                var v2 = new Vector3(p2x - p1x, p2y - p1y, p2z - p1z);
                if (Vector3.Dot(Vector3.Cross(v1, v2), eyeDirection) < 0.0)
                {
                    edges.Add(new EdgeI(ftable[i, 0], ftable[i, 1]));
                    edges.Add(new EdgeI(ftable[i, 1], ftable[i, 2]));
                    edges.Add(new EdgeI(ftable[i, 2], ftable[i, 3]));
                    edges.Add(new EdgeI(ftable[i, 3], ftable[i, 0]));
                }

            }

            double xmin = -0.5;
            double xmax = 0.5;
            double ymin = -0.5;
            double ymax = 0.5;

            Point [] pictureVertices = new Point[nPoints];
            double scale = 100;
            for (int i = 0; i < nPoints; i++)
            {
                double x = vtable[i, 0];
                double y = vtable[i, 1];
                double z = vtable[i, 2];
                double xprime = x / z;
                double yprime = y / z;
                pictureVertices[i].X = scale * (1-(xprime - xmin) / (xmax - xmin));
                pictureVertices[i].Y = scale * (yprime - ymin) / (ymax - ymin); // x / z
                //gp.Children.Add(new Dot(pictureVertices[i].X, pictureVertices[i].Y));
            }
            foreach (var edge in edges)
            {
                gp.Children.Add(new Segment(pictureVertices[edge.i1], pictureVertices[edge.i2]));
            }
            /*for (int i = 0; i < nPoints; i++)
            {
                gp.Children.Add(new Dot(pictureVertices[i].X, pictureVertices[i].Y));
            }*/

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