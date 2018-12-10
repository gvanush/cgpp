using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Documents;
using System.Windows.Media.Media3D;
using System.Globalization;
using System.Xml;



namespace PlainVanilla
{




    public partial class ChapterMaterials
    {
        public ChapterMaterials()
        {
            this.InitializeComponent();

            LoadFlowDocumentPageViewerWithXAMLFile();

            _3DTools.Trackball tb = new _3DTools.Trackball(VP3D.Camera);
            tb.EventSource = VP3D;
            VP3D.Camera.Transform = tb.Transform;

            //sky.Geometry = Sky.GenerateSky();
        }


        private _3DTools.Trackball tb = null;


   


        void LoadFlowDocumentPageViewerWithXAMLFile()
        {


            // Open the file that contains the FlowDocument...
            //FileStream xamlFile = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            // and parse the file with the XamlReader.Load method.
            //FlowDocument content = System.Windows.Markup.XamlReader.Load(xamlFile) as FlowDocument;


            StringReader sr = new StringReader(RuntimeData.FLOWDOCchapterMaterials);
            XmlTextReader xtr = new XmlTextReader(sr);
            FlowDocument content =
                System.Windows.Markup.XamlReader.Load(xtr) as FlowDocument;





            // Finally, set the Document property to the FlowDocument object that was
            // parsed from the input file.
            VIEWtextbook.Document = content;

            //xamlFile.Close();
        }





 



        private void ACTturntableanglereset(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                bool ispaused = TurntableStoryboard.GetIsPaused(this);
                TurntableStoryboard.Stop(this);
                zAxisRotate.Angle = 0;
                TurntableStoryboard.Begin(this, true);
                if (ispaused)
                    TurntableStoryboard.Pause(this);
            }
            catch (Exception) { }

        }



        private void ACTcamerareset(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                CAMMY.Position = (Point3D)FindResource("PT_cameraInitPosition");
                CAMMY.LookDirection = (Vector3D)FindResource("PT_cameraInitLookDir");
                tb.reset();
           
            }
            catch (Exception) { }
        }
    



        private bool turntableHasBegun = false;

        private void ACTturntableanimEnable(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!turntableHasBegun)
            {
                TurntableStoryboard.Begin(this, true);
                turntableHasBegun = true;
            }
            TurntableStoryboard.Resume(this);
        }

        private void ACTturntableanimDisable(object sender, System.Windows.RoutedEventArgs e)
        {
            TurntableStoryboard.Pause(this);
        }











        private void ComputeXAML()
        {
        }






        private void  ACTdirlightingChanged (object sender, System.Windows.Controls.SelectionChangedEventArgs e)
			 {
				String s = ((TextBlock)(e.AddedItems[0])).Tag.ToString();
				switch (s) {
                    case "OFF":
                        LITEred.Color = Color.FromArgb(0, 0, 0, 0);
                        LITEgreen.Color = Color.FromArgb(0, 0, 0, 0);
                        LITEblue.Color = Color.FromArgb(0, 0, 0, 0);
                        break;
                    case "RGB":
                        LITEred.Color = Color.FromArgb(255, 255, 0, 0);
                        LITEgreen.Color = Color.FromArgb(255, 0, 255, 0);
                        LITEblue.Color = Color.FromArgb(255, 0, 0, 255);
                        break;
                    case "White100":
                        LITEred.Color = Color.FromArgb(255, 255, 255, 255);
                        LITEgreen.Color = Color.FromArgb(255, 255, 255, 255);
                        LITEblue.Color = Color.FromArgb(255, 255, 255, 255);
                        break;
                }
			 }




        





        private void ClearXAML()
        {
           

            String xaml = @"

There is no XAML to show here at this time.

You are not currently viewing a model with any content.

";

            TEXTBOXshowxaml.Text = xaml;
        }







    



      }
}
