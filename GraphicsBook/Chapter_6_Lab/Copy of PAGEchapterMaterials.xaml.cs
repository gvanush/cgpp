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




    public partial class ChapterMaterialsJuly2009
    {
        public ChapterMaterialsJuly2009()
        {
            this.InitializeComponent();


/*
 * int renderingTier = (RenderCapability.Tier >> 16);
            if (renderingTier < 2)
            {
                throw new Exception("Graphics hardware/opsys does not offer Tier-2 support for WPF.");
            }
*/


            LoadFlowDocumentPageViewerWithXAMLFile();

            /*
            _3DTools.Trackball tb = new _3DTools.Trackball(VP3D.Camera);
            tb.EventSource = VP3D;
            VP3D.Camera.Transform = tb.Transform;
            */
   
            MODELlightAvatar.Geometry = 
	      GenerateSphere(
			     new Point3D(0,0,0),
			     79.5,124,124);
        }




        private _3DTools.Trackball tb = null;



      
      // Courtesy of Petzold:
        MeshGeometry3D GenerateSphere(Point3D center, double radius,
                                      int slices, int stacks)
        {
            // Create the MeshGeometry3D.
            MeshGeometry3D mesh = new MeshGeometry3D();

            // Fill the Position, Normals, and TextureCoordinates collections.
            for (int stack = 0; stack <= stacks; stack++)
            {
                double phi = Math.PI / 2 - stack * Math.PI / stacks;
                double y = radius * Math.Sin(phi);
                double scale = -radius * Math.Cos(phi);

                for (int slice = 0; slice <= slices; slice++)
                {
                    double theta = slice * 2 * Math.PI / slices;
                    double x = scale * Math.Sin(theta);
                    double z = scale * Math.Cos(theta);

                    Vector3D normal = new Vector3D(x, y, z);
                    mesh.Normals.Add(normal);
                    mesh.Positions.Add(normal + center);
                    mesh.TextureCoordinates.Add(
                            new Point((double)slice / slices,
                                      (double)stack / stacks));
                }
            }

            // Fill the TriangleIndices collection.
            for (int stack = 0; stack < stacks; stack++)
                for (int slice = 0; slice < slices; slice++)
                {
                    int n = slices + 1; // Keep the line length down.

                    if (stack != 0)
                    {
                        mesh.TriangleIndices.Add((stack + 0) * n + slice);
                        mesh.TriangleIndices.Add((stack + 1) * n + slice);
                        mesh.TriangleIndices.Add((stack + 0) * n + slice + 1);
                    }
                    if (stack != stacks - 1)
                    {
                        mesh.TriangleIndices.Add((stack + 0) * n + slice + 1);
                        mesh.TriangleIndices.Add((stack + 1) * n + slice);
                        mesh.TriangleIndices.Add((stack + 1) * n + slice + 1);
                    }
                }
            return mesh;
        }

   


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
            //VIEWtextbook.Document = content;

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
                    case "RGB100":
                        LITEred.Color = Color.FromArgb(255, 255, 0, 0);
                        LITEgreen.Color = Color.FromArgb(255, 0, 255, 0);
                        LITEblue.Color = Color.FromArgb(255, 0, 0, 255);
                        break;
                    case "RGB50":
                        LITEred.Color = Color.FromArgb(255, 128, 0, 0);
                        LITEgreen.Color = Color.FromArgb(255, 0, 128, 0);
                        LITEblue.Color = Color.FromArgb(255, 0, 0, 128);
                        break;
                    case "3White100":
                        LITEred.Color = Color.FromArgb(255, 255, 255, 255);
                        LITEgreen.Color = Color.FromArgb(255, 255, 255, 255);
                        LITEblue.Color = Color.FromArgb(255, 255, 255, 255);
                        break;
                    case "3White50":
                        LITEred.Color = Color.FromArgb(255, 128, 128, 128);
                        LITEgreen.Color = Color.FromArgb(255, 128, 128, 128);
                        LITEblue.Color = Color.FromArgb(255, 128, 128, 128);
                        break;
                    case "1White100":
		      LITEred.Color = Color.FromArgb(255, 0,0,0);
                        LITEgreen.Color = Color.FromArgb(255, 255, 255, 255);
			LITEblue.Color = Color.FromArgb(255, 0,0,0);
                        break;
                    case "1White50":
		      LITEred.Color = Color.FromArgb(255, 0,0,0);
		      LITEgreen.Color = Color.FromArgb(255, 128,128,128);
		      LITEblue.Color = Color.FromArgb(255, 0,0,0);
		      break;
                }
			 }




        





        private void ClearXAML()
        {
           

            String xaml = @"

There is no XAML to show here at this time.

You are not currently viewing a model with any content.

";

            //TEXTBOXshowxaml.Text = xaml;
        }







    



      }
}
