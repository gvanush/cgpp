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




  public partial class Window1
  {
    public Window1()
    {
      this.InitializeComponent();

      /*
        Uri uri =
        new Uri("PresentationFramework.Aero;V3.0.0.0;31bf3856ad364e35;component\\themes/aero.normalcolor.xaml", UriKind.Relative);

        uri =
        new Uri("PresentationFramework.Classic;V3.0.0.0;31bf3856ad364e35;component\\themes/classic.xaml", UriKind.Relative);


        Resources.MergedDictionaries.Add(Application.LoadComponent(uri) as ResourceDictionary);
      */


      LoadFlowDocumentPageViewerWithXAMLFile();

      tb = new _3DTools.Trackball(VP3D.Camera);
      tb.EventSource = VP3D;

      sky.Geometry = Sky.GenerateSky();

      ComputeXAML();

    }


    private _3DTools.Trackball tb = null;


    private void ACTshowNormals(object sender, System.Windows.RoutedEventArgs e)
    {
      MeshGeometry3D mg3d = (MeshGeometry3D)GM3D_pyramid.Geometry;
      int numFaces = mg3d.TriangleIndices.Count / 3;
      for (int i = 0; i < numFaces; i++)
        {
          ConstructFaceNormal((MeshGeometry3D)GM3D_pyramid.Geometry, i);
        }

      if (numFaces == 2 && mg3d.Positions.Count == 4)
        {
          ConstructVertexNormalsForTwoFaceSituation((MeshGeometry3D)GM3D_pyramid.Geometry);
        }
      else
        {
          for (int i = 0; i < numFaces; i++)
            {
              ConstructVertexNormalsMatchingFaceNormal((MeshGeometry3D)GM3D_pyramid.Geometry, i);
            }
        }

    }



    void LoadFlowDocumentPageViewerWithXAMLFile()
    {


      // Open the file that contains the FlowDocument...
      //FileStream xamlFile = new FileStream(fileName, FileMode.Open, FileAccess.Read);
      // and parse the file with the XamlReader.Load method.
      //FlowDocument content = System.Windows.Markup.XamlReader.Load(xamlFile) as FlowDocument;


      StringReader sr = new StringReader(RuntimeData.FLOWDOCchapter1);
      XmlTextReader xtr = new XmlTextReader(sr);
      FlowDocument content =
        System.Windows.Markup.XamlReader.Load(xtr) as FlowDocument;





      // Finally, set the Document property to the FlowDocument object that was
      // parsed from the input file.
      VIEWtextbook.Document = content;

      //xamlFile.Close();
    }





    

    private void ACTdirectionalLiteEnable(object sender, System.Windows.RoutedEventArgs e)
    {
      LITEdirectional.Color = Color.FromRgb(255, 255, 255);
      ComputeXAML();

      if (curmodelchoice == "1F")
          LambertDisplay.Height = Double.NaN;
      else
          LambertDisplay.Height = 0;

      UPDATE_Lambertdisplay();
      ShowSunlightVector();
    }

    private void ACTdirectionalLiteDisable(object sender, System.Windows.RoutedEventArgs e)
    {
      LITEdirectional.Color = Color.FromRgb(0, 0, 0);
      ComputeXAML();
      if (LambertDisplay != null)
        LambertDisplay.Height = 0;
      HideSunlightVector();
    }




    Model3DGroup sunlightarrow = null;

    private void ShowSunlightVector()
    {
      CONTAINERsunlight.Transform = Transform3D.Identity;

      if (sunlightarrow == null)
        {
          int lenAxis = 20;
          Vector3D atdir = new Vector3D(lenAxis, -lenAxis, -lenAxis);
          atdir.Normalize();
          sunlightarrow = ConstructAnnotationArrow(new Point3D(-70, 70, 70), atdir, CONTAINERsunlight);
          ConstructAnnotationText
            (new Point3D(-60, 70, 70), 50, 12, "Light direction", CONTAINERsunlight);
        }
    }


    private void HideSunlightVector()
    {
      CONTAINERsunlight.Transform = new ScaleTransform3D(0, 0, 0);
    }


    private void ACTshowaxesEnable(object sender, System.Windows.RoutedEventArgs e)
    {
      int lenAxis = 1;
      ConstructAnnotationArrow(new Point3D(0, 0, 0), new Vector3D(lenAxis, 0, 0), CONTAINERaxes);
      ConstructAnnotationArrow(new Point3D(0, 0, 0), new Vector3D(0, lenAxis, 0), CONTAINERaxes);
      ConstructAnnotationArrow(new Point3D(0, 0, 0), new Vector3D(0, 0, lenAxis), CONTAINERaxes);

      lenAxis = 20;
      ConstructAnnotationText(new Point3D(lenAxis, 0, 0), 5, 5, "X", CONTAINERaxes);
      ConstructAnnotationText(new Point3D(0, lenAxis, 0), 5, 5, "Y", CONTAINERaxes);
      ConstructAnnotationText(new Point3D(-8, 0, lenAxis), 5, 5, "Z", CONTAINERaxes);
    }


    private void _OLDACTshowaxesEnable(object sender, System.Windows.RoutedEventArgs e)
    {
      ConstructAnnotationArrow(new Point3D(0, 0, 0), new Vector3D(0, 0, 1), CONTAINERaxes);
      ConstructAnnotationArrow(new Point3D(0, 0, 0), new Vector3D(0, 3, 0), CONTAINERaxes);
      ConstructAnnotationArrow(new Point3D(0, 0, 0), new Vector3D(1, 0, 0), CONTAINERaxes);

      ConstructAnnotationText(new Point3D(20, 0, 0), 4, 4, "X", CONTAINERaxes);
      ConstructAnnotationText(new Point3D(0, 0, 20), 4, 4, "Z", CONTAINERaxes);
      ConstructAnnotationText(new Point3D(0, 20, 0), 4, 4, "Y", CONTAINERaxes);
    }


    private void ConstructAnnotationText
    (Point3D originLowerleft, int width, int height, string text, Model3DGroup parent)
    {
      MeshGeometry3D _mesh = new MeshGeometry3D();
      _mesh.Positions.Add(originLowerleft);

      _mesh.Positions.Add(originLowerleft + new Vector3D(width,       0, 0));
      _mesh.Positions.Add(originLowerleft + new Vector3D(width,  height, 0));
      _mesh.Positions.Add(originLowerleft + new Vector3D(    0,  height, 0));

      int[] arr = new int[] { 0, 1, 3, 1, 2, 3 };
      foreach (int i in arr)
        _mesh.TriangleIndices.Add(i);
      _mesh.TextureCoordinates.Add(new Point(0, 1));
      _mesh.TextureCoordinates.Add(new Point(1, 1));
      _mesh.TextureCoordinates.Add(new Point(1, 0));
      _mesh.TextureCoordinates.Add(new Point(0, 0));

      GeometryModel3D _model = new GeometryModel3D();
      _model.Geometry = _mesh;

      FormattedText ft = new FormattedText(text,
                                           new CultureInfo("en-us"),
                                           FlowDirection.LeftToRight,
                                           new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, new FontStretch()),
                                           12D,
                                           new SolidColorBrush(Colors.White));

      double _TextWidth = ft.Width;
      double _TextHeight = ft.Height;
      DrawingVisual drawingVisual = new DrawingVisual();
      DrawingContext drawingContext = drawingVisual.RenderOpen();
      if (true)
        {
          if (false) /*drawbackground?*/
            {
              //Brush backBrush = new SolidColorBrush(Colors.AntiqueWhite);
              //drawingContext.DrawRectangle(backBrush, new Pen(backBrush, 1), new Rect(0, 0, ft.Width, ft.Height));
            }
          drawingContext.DrawText(ft, new Point(0, 0));
        }
      drawingContext.Close();

      VisualBrush vb = new VisualBrush(drawingVisual);
      _model.Material = new DiffuseMaterial(vb);  // Used to be emissive

      parent.Children.Add(_model);
    }




    private void ACTshowaxesDisable(object sender, System.Windows.RoutedEventArgs e)
    {
      CONTAINERaxes.Children.Clear();
    }




    private void ACTambientLiteEnable(object sender, System.Windows.RoutedEventArgs e)
    {
      LITEambient.Color = Color.FromRgb(255, 255, 255);
    }
    private void ACTambientLiteDisable(object sender, System.Windows.RoutedEventArgs e)
    {
      LITEambient.Color = Color.FromRgb(0, 0, 0);
    }




    private void ACTturntableanglereset(object sender, System.Windows.RoutedEventArgs e)
    {
      try
        {
          bool ispaused = TurntableStoryboard.GetIsPaused(this);
          TurntableStoryboard.Stop(this);
          yAxisRotate.Angle = 0;
          TurntableStoryboard.Begin(this, true);
          if (ispaused) {
            TurntableStoryboard.Pause(this);
          }
          UPDATE_Lambertdisplay();
        }
      catch (Exception) { }

    }


    private void UPDATE_Lambertdisplay()
    {
      if (!turntableIsTurning) {
        UPDATEshowLambertDisplay();
      }else{
        UPDATEhideLambertDisplay();
      }
    }

    private void UPDATEhideLambertDisplay()
    {
      LambertTheta.Height = 0;
      LambertCos.Height = 0;
      LambertPrompt.Height = Double.NaN;
    }

    private void UPDATEshowLambertDisplay()
    {
      if (LambertTheta == null)
        return;

      LambertTheta.Height = Double.NaN;
      LambertCos.Height = Double.NaN;
      LambertPrompt.Height = 0;

      // Because the light is coming in from 45 degrees to the "left",
      // we need to add 45 degrees to the yAxisRotate angle, which
      // is based on the Z axis.
      double theta = yAxisRotate.Angle + 45;
      if (theta > 360)
        {
          theta -= 360;
        }
      if (theta > 180)
        theta = 360 - theta;

      LambertTheta.Content = theta.ToString("F");

      LambertCos.Content = Math.Cos(theta*Math.PI/180.0).ToString("F4");
    }



    private void ACTcamerareset(object sender, System.Windows.RoutedEventArgs e)
    {
      try
        {
          CAMMY.Position = (Point3D)FindResource("PT_cameraInitPosition");
          CAMMY.LookDirection = (Vector3D)FindResource("PT_cameraInitLookDir");
          CAMMY.UpDirection = new Vector3D(0, 1000, 0);
          tb.reset();
           
        }
      catch (Exception) { }
    }



    private void ACTcameraOverheadView(object sender, System.Windows.RoutedEventArgs e)
    {
      try
        {
          CAMMY.Position = (Point3D)FindResource("PT_cameraOverheadPosition");
          CAMMY.LookDirection = (Vector3D)FindResource("PT_cameraOverheadLookDir");
          CAMMY.UpDirection = new Vector3D(0, 0, -1);
          ACTshowNormals(null,null);
          CHKBOXshownormals.IsChecked = true;
          tb.reset();
        }
      catch (Exception) { }
    }



    private bool turntableHasBegun = false;
    private bool turntableIsTurning = false;



      private void ACTturntableToggle(object sender, System.Windows.RoutedEventArgs e)
      {
          if (!turntableIsTurning)
          {
              ACTturntableanimEnable(sender, e);
              BTNturntableToggle.Content = "STOP";
          }
          else
          {
              ACTturntableanimDisable(sender, e);
              BTNturntableToggle.Content = "Start/Activate";
          }
      }


    private void ACTturntableanimEnable(object sender, System.Windows.RoutedEventArgs e)
    {
      if (!turntableHasBegun)
        {
          TurntableStoryboard.Begin(this, true);
          turntableHasBegun = true;
        }
      TurntableStoryboard.Resume(this);
      turntableIsTurning = true;
      UPDATE_Lambertdisplay();
    }

    private void ACTturntableanimDisable(object sender, System.Windows.RoutedEventArgs e)
    {
      TurntableStoryboard.Pause(this);
      turntableIsTurning = false;
      UPDATE_Lambertdisplay();
    }








    private void ACTshownormalsEnable(object sender, System.Windows.RoutedEventArgs e)
    {
      ACTshowNormals(null, null);

    }
    private void ACTshownormalsDisable(object sender, System.Windows.RoutedEventArgs e)
    {
      CONTAINERfacenormals.Children.Clear();
    }




    private void ACTbackmatEnable(object sender, System.Windows.RoutedEventArgs e)
    {
      GM3D_pyramid.BackMaterial = (Material)FindResource("BRUSHbackingMatSolid");
    }
    private void ACTbackmatDisable(object sender, System.Windows.RoutedEventArgs e)
    {
      GM3D_pyramid.BackMaterial = null;
    }





      private string curmodelchoice = "1F";

    private void ACTmodelSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      String s = ((TextBlock)(e.AddedItems[0])).Tag.ToString();
      curmodelchoice = s;
      GM3D_pyramid.Geometry = (Geometry3D)(FindResource("Pyramid_" + s));
      try
        {
          CHKBOXshownormals.IsChecked = false;
          {
            if (s == "0F")
              {
                //GRPBOXinfo.IsVisible = false;
                GRPBOXinfo.Height = 0;
                ComputeXAML();
              }
            else
              {
                //GRPBOXinfo.IsVisible = true;
                GRPBOXinfo.Height = /*GRPBOXinfo.DesiredSize.Height*/ 180;
                ComputeXAML();
              }
          }

        }
      catch (Exception) { }

      if ( curLightingModel == "DIRleftshoulder" && (s == "1F") )
        {
          if (LambertDisplay != null)
            LambertDisplay.Height = Double.NaN;
        }
      else
        {
          if (LambertDisplay != null)
            LambertDisplay.Height = 0;
        }

      UPDATE_Lambertdisplay();

    }





    // New strategy as of Sep 2009: load a XAML template and
    // replace the <<<xxx>>> tags.
    //
    private void ComputeXAML()
    {
      StringReader sr = new StringReader(RuntimeData.DEMOXAMLchapter1);
      MeshGeometry3D mesh = (MeshGeometry3D)GM3D_pyramid.Geometry;

        String xaml = 
             sr.ReadToEnd();


         xaml = 
      xaml.Replace("<<<PYRMESH>>>",
      @"<MeshGeometry3D x:Key='RSRCmeshPyramid'
     Positions='" + mesh.Positions.ToString() + @"'
     TriangleIndices='" + mesh.TriangleIndices + "'" +
        ((mesh.Normals.Count > 0) ? (@"
     Normals='" + mesh.Normals + "'") : "") +
        ((mesh.TextureCoordinates.Count > 0) ? (@"
     TextureCoordinates='" + mesh.TextureCoordinates + "'") : "") +
        @"
   />");


        xaml = 
        xaml.Replace("<<<LIGHTING>>>",
              ( (LITEdirectional.Color!=Color.FromRgb(0,0,0)) ?
          (@"
          <DirectionalLight Color='#FFFFFF'
                            Direction='1, -1, -1' />")
          :
          (@"
          <AmbientLight Color='#FFFFFF'/>") ) ) ;
     



      if (TEXTBOXshowxaml != null)
        TEXTBOXshowxaml.Text = xaml;

    }

    private void ClearXAML()
    {
      MeshGeometry3D mesh = (MeshGeometry3D)GM3D_pyramid.Geometry;

      String xaml = @"

There is no XAML to show here at this time.

You are not currently viewing a model with any content.

";

      TEXTBOXshowxaml.Text = xaml;
    }


    private string curLightingModel = "Ambient";

    private void ACTlightingSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      String s = ((TextBlock)(e.AddedItems[0])).Tag.ToString();
      curLightingModel = s;
      switch (s)
        {
        case "Ambient":
          ACTdirectionalLiteDisable(null, null);
          ACTambientLiteEnable(null, null);
          break;
        case "None":
          ACTdirectionalLiteDisable(null, null);
          ACTambientLiteDisable(null, null);
          break;
        default:
          ACTdirectionalLiteEnable(null, null);
          ACTambientLiteDisable(null, null);
          break;
        }
    }


    private void ConstructFaceNormal(MeshGeometry3D themesh, int faceindex)
    {
      Point3D centroid = Tools3D.ComputeTriangleCentroid(themesh, faceindex);
      Vector3D facenormal = Tools3D.ComputeTriangleNormal(themesh, faceindex);
      ConstructAnnotationArrow(centroid, facenormal, CONTAINERfacenormals);
    }


    private void ConstructVertexNormalsMatchingFaceNormal(MeshGeometry3D themesh, int faceindex)
    {
      Vector3D facenormal = Tools3D.ComputeTriangleNormal(themesh, faceindex);
      ConstructAnnotationArrow(themesh.Positions[themesh.TriangleIndices[faceindex * 3]], facenormal, CONTAINERfacenormals);
      ConstructAnnotationArrow(themesh.Positions[themesh.TriangleIndices[faceindex * 3 + 1]], facenormal, CONTAINERfacenormals);
      ConstructAnnotationArrow(themesh.Positions[themesh.TriangleIndices[faceindex * 3 + 2]], facenormal, CONTAINERfacenormals);
    }



    private void ConstructVertexNormalsForTwoFaceSituation(MeshGeometry3D themesh)
    {
      Vector3D facenormal0 = Tools3D.ComputeTriangleNormal(themesh, 0);
      Vector3D facenormal1 = Tools3D.ComputeTriangleNormal(themesh, 1);

      // The edge vertices (non-shared) use the face normal as-is
      ConstructAnnotationArrow(themesh.Positions[1], facenormal0, CONTAINERfacenormals);
      ConstructAnnotationArrow(themesh.Positions[3], facenormal1, CONTAINERfacenormals);

      // The shared vertices 0 and 2 use the average of the two normals.
      Vector3D normalavgd = (facenormal0 + facenormal1);
      normalavgd.Normalize();
      ConstructAnnotationArrow(themesh.Positions[0], normalavgd, CONTAINERfacenormals);
      ConstructAnnotationArrow(themesh.Positions[2], normalavgd, CONTAINERfacenormals);
    }





    private Model3DGroup ConstructAnnotationArrow(Point3D origin, Vector3D direction, Model3DGroup parent)
    {
      Model3DGroup wholearrow = ((Model3DGroup)FindResource("M3DG_ArrowAtOrigin")).Clone();
      Transform3DGroup t3dgr = new Transform3DGroup();
      wholearrow.Transform = t3dgr;
      t3dgr.Children.Add((Transform3D)FindResource("TRANSFORM_Arrow"));

      // Step 1: use the Z component of the normal to rotate around Y axis
      //         to determine the vector location in the XZ plane.
      double angleFromZaxis = 90 - ((180.0 / Math.PI) * Math.Asin(direction.Z));
      t3dgr.Children.Add(
                         new RotateTransform3D(
                                               new AxisAngleRotation3D(new Vector3D(0, 1, 0), angleFromZaxis)));

      // Step 2: use the X and Y components to rotate around the Z axis.
      double adjustment = 0;
      if (direction.X == 0)
        direction.X = 0.0000001;
      if (direction.X < 0)
        adjustment = 180;
          
      double angle = adjustment + (Math.Atan(direction.Y / direction.X) * 180.0 / Math.PI);
      t3dgr.Children.Add(
                         new RotateTransform3D(
                                               new AxisAngleRotation3D(new Vector3D(0, 0, 1), angle)));

      t3dgr.Children.Add(new TranslateTransform3D(
                                                  origin.X, origin.Y, origin.Z));
      parent.Children.Add(wholearrow);

      return wholearrow;
    }
  }
}
