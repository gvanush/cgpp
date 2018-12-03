using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.Diagnostics;
using System.ComponentModel;

namespace GraphicsBook
{
    /// <summary>
    /// Display and interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        GraphPaper gp = null;

		Dot[] myDots = null;
		Circle myCircle = null;
		Random autoRand = new System.Random();

		// Are we ready for interactions like slider-changes to alter the 
		// parts of our display (like polygons or images or arrows)? Probably not until those things 
		// have been constructed!
		bool ready = false;

        // Code to create and display objects goes here.
        public Window1()
        {
            InitializeComponent();
            InitializeCommands();

            // Now add some graphical items in the main drawing area, whose name is "Paper"
            gp = this.FindName("Paper") as GraphPaper;
            

            // Track mouse activity in this window
            MouseLeftButtonDown += MyMouseButtonDown;
            MouseLeftButtonUp += MyMouseButtonUp;
            MouseMove += MyMouseMove;

			#region Triangles, segments, dots

			myDots = new Dot[3];
			myDots[0] = new Dot(new Point(-40, 60));
			myDots[1] = new Dot(new Point(40, 60));
			myDots[2] = new Dot(new Point(40, -60));
			for (int i = 0; i < 3; i++)
			{
				myDots[i].MakeDraggable(gp);
				gp.Children.Add(myDots[i]);
			}

			myCircle = new Circle(new Point(0, 0), 0.0);
			UpdateCircle(myDots[0].Position, myDots[1].Position, myDots[2].Position);
			gp.Children.Add(myCircle);

			DependencyPropertyDescriptor dotDescr = DependencyPropertyDescriptor.FromProperty(Dot.PositionProperty, typeof(Dot));
			if (dotDescr != null)
			{
				dotDescr.AddValueChanged(myDots[0], OnDotMove);
				dotDescr.AddValueChanged(myDots[1], OnDotMove);
				dotDescr.AddValueChanged(myDots[2], OnDotMove);
			}

			#endregion

			ready = true; // Now we're ready to have sliders and buttons influence the display.
        }

		protected void OnDotMove(object sender, EventArgs args)
		{
			UpdateCircle(myDots[0].Position, myDots[1].Position, myDots[2].Position);
		}

		protected void UpdateCircle(Point P, Point Q, Point R)
		{
			if ((P.X == Q.X) & (P.Y == Q.Y))
			{
				P.X += .001 * (2 * autoRand.NextDouble() - 1);
				P.Y += .001 * (2 * autoRand.NextDouble() - 1);
			}
			if ((Q.X == R.X) & (Q.Y == R.Y))
			{
				R.X += .001 * (2 * autoRand.NextDouble() - 1);
				R.Y += .001 * (2 * autoRand.NextDouble() - 1);
			}

			Point A = P + (Q - P) / 2.0;
			Point B = Q + (R - Q) / 2.0;
			Vector v = Q - P;
			double tmp = v.X;
			v.X = v.Y;
			v.Y = -tmp;
			v.Normalize();
			Vector w = R - Q;
			w.Normalize();
			tmp = w.X;
			w.X = w.Y;
			w.Y = -tmp;

			double a = v.X;
			double b = w.X;
			double c = v.Y;
			double d = w.Y;
			double det = a * d - b * c;
			// If the det is too small (or zero), we can fix it by moving the x-coordinates of v and w by 1 / 1000,
            // which is too small to have any visual impact.
            if (Math.Abs(det) < 1e-9)
			{
				a += .001;
				c += .001;
			}

			Vector target = (P - R) / 2.0;
			double t = -(d * target.X - b * target.Y);
			double s = (-c * target.X + a * target.Y);
			t /= det;

			Point C = A + t * v;
			double r = (P - C).Length;
			myCircle.Position = C;
			myCircle.Radius = r;

		}

		#region Interaction handling -- sliders and buttons

		/* Click-handling in the main graph-paper window */
		public void MyMouseButtonUp(object sender, RoutedEventArgs e)
        {
            if (sender != this) return;
            System.Windows.Input.MouseButtonEventArgs ee =
              (System.Windows.Input.MouseButtonEventArgs)e;
            Debug.Print("MouseUp at " + ee.GetPosition(this));
            e.Handled = true;
        }

        public void MyMouseButtonDown(object sender, RoutedEventArgs e)
        {
            if (sender != this) return;
            System.Windows.Input.MouseButtonEventArgs ee =
              (System.Windows.Input.MouseButtonEventArgs)e;
            Debug.Print("MouseDown at " + ee.GetPosition(this));
            e.Handled = true;
        }


        public void MyMouseMove(object sender, MouseEventArgs e)
        {
            if (sender != this) return;
            System.Windows.Input.MouseEventArgs ee =
              (System.Windows.Input.MouseEventArgs)e;
            // Uncommment following line to get a flood of mouse-moved messages. 
            // Debug.Print("MouseMove at " + ee.GetPosition(this));
            e.Handled = true;
        }

        #endregion

        #region Menu, command, and keypress handling

        protected static RoutedCommand ExitCommand;

        protected void InitializeCommands()
        {
            InputGestureCollection inp = new InputGestureCollection();
            inp.Add(new KeyGesture(Key.X, ModifierKeys.Control));
            ExitCommand = new RoutedCommand("Exit", typeof(Window1), inp);
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