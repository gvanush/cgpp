using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.Diagnostics;
using System.Windows.Controls;

namespace GraphicsBook
{
    /// <summary>
    /// Display and interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
		Canvas canvas;

        GImage ball = null;
		GImage tray = null;
		Shape shadow = null;
		const double offset = 40.0;
		bool updown = false;
		readonly Point initialBallPos = new Point(90, 60);
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
			canvas = this.FindName("Canvas") as Canvas;
            
            // Track mouse activity in this window
            MouseLeftButtonDown += MyMouseButtonDown;
            MouseLeftButtonUp += MyMouseButtonUp;
            MouseMove += MyMouseMove;
			
			// And a photo from a file, then another that's 
			// created on-the-fly instead of read from a file. 

			tray = new GImage("./tray.png");
			tray.Stretch = Stretch.None;
			canvas.Children.Add(tray);

			ball = new GImage("./ball.png");
			ball.Stretch = Stretch.None;
			ball.Position = initialBallPos;
			canvas.Children.Add(ball);

			shadow = new Ellipse();
			shadow.Width = 30.0;
			shadow.Height = 10.0;
			shadow.Fill = new SolidColorBrush(Colors.Gray);
			shadow.Margin = new Thickness(ball.Position.X, ball.Position.Y - offset, 0.0, 0.0);
			canvas.Children.Add(shadow);
            
            ready = true; // Now we're ready to have sliders and buttons influence the display.
        }

		private void OnMovementChange(object sender, RoutedEventArgs e) {
			var cb = sender as CheckBox;
			updown = cb.IsChecked.Value;
			if (updown)
			{
				shadow.Margin = new Thickness(ball.Position.X, initialBallPos.Y - offset, 0.0, 0.0);
			}
			else
			{
				shadow.Margin = new Thickness(ball.Position.X, ball.Position.Y - offset, 0.0, 0.0);
			}
		}

		private void OnShadowTypeChange(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (!ready) return;

			canvas.Children.Remove(shadow);

			var cb = sender as ComboBox;
			var type = cb.SelectedValue.ToString();

			if (type == "Ellipse")
			{
				shadow = new Ellipse();
				shadow.Width = 30.0;
				shadow.Height = 10.0;
				canvas.Children.Add(shadow);
			}
			else if (type == "Disk")
			{
				shadow = new Ellipse();
				shadow.Width = 30.0;
				shadow.Height = 30.0;
				canvas.Children.Add(shadow);
			}
			else if (type == "Square")
			{
				shadow = new Rectangle();
				shadow.Width = 30.0;
				shadow.Height = 30.0;
				canvas.Children.Add(shadow);
			}

			shadow.Fill = new SolidColorBrush(Colors.Gray);
			if(updown)
			{
				shadow.Margin = new Thickness(ball.Position.X, initialBallPos.Y - offset, 0.0, 0.0);
			} else
			{
				shadow.Margin = new Thickness(ball.Position.X, ball.Position.Y - offset, 0.0, 0.0);
			}
			
			
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

        /* Event handler for a click on button one */
        void OnXPositionChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            e.Handled = true;
            // Be sure to not respond to slider-moves until all objects have been constructed. 
            if (ready)
            {
				ball.Position = new Point(e.NewValue, ball.Position.Y);
				
				if (updown)
				{
					shadow.Margin = new Thickness(ball.Position.X, initialBallPos.Y - offset, 0.0, 0.0);
				}
				else
				{
					shadow.Margin = new Thickness(ball.Position.X, ball.Position.Y - offset, 0.0, 0.0);
				}
			}
        }

		void OnYPositionChange(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			e.Handled = true;
			// Be sure to not respond to slider-moves until all objects have been constructed. 
			if (ready)
			{
				ball.Position = new Point(ball.Position.X, e.NewValue);
				if (updown)
				{
					shadow.Margin = new Thickness(ball.Position.X, initialBallPos.Y - offset, 0.0, 0.0);
				}
				else
				{
					shadow.Margin = new Thickness(ball.Position.X, ball.Position.Y - offset, 0.0, 0.0);
				}
			}
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