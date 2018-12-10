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
using System.Collections.Generic;
using System.Collections.ObjectModel;



namespace PlainVanilla
{
  public class XYZUV
  {
    public string _X;
    public string _Y;
    public string _Z;
    public string _U;
    public string _V;
      public int _idx;

      public string IDX
      {
          get { return _idx.ToString(); }
          set { _idx = int.Parse(value); }
      }

    public string X
    {
      get { return _X; }
      set { _X = value; }
    }
    public string Y
    {
      get { return _Y; }
      set { _Y = value; }
    }
    public string Z
    {
      get { return _Z; }
      set { _Z = value; }
    }
    public string U
    {
      get { return _U; }
      set { _U = value; }
    }
    public string V
    {
      get { return _V; }
      set { _V = value; }
    }

  }
      


  public class XYZUVCollection : ObservableCollection<XYZUV>
  {
    public XYZUVCollection()
      {
      }

    

    public void Add(Point3D point3D, Point uv)
    {
      // THIS ALSO CHANGES XY GROUNDPLANE TO XZ GROUNDPLANE!
      XYZUV baby = new XYZUV();
      string fmt = "{0:0.00}";

      baby.X = String.Format(fmt, point3D.X);
      baby.Y = String.Format(fmt, point3D.Z);
      baby.Z = String.Format(fmt, (0 - point3D.Y));
      baby.U = String.Format(fmt, uv.X);
      baby.V = String.Format(fmt, uv.Y);
      baby.IDX = this.Count.ToString();

      this.Add(baby);
    }
  }

  public partial class ChapterTexture
  {
    public ChapterTexture()
    {
      this.InitializeComponent();

      LoadFlowDocumentPageViewerWithXAMLFile("etextbook.xaml");

      tb = new _3DTools.Trackball(VP3D.Camera);
      tb.EventSource = VP3D;

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



    void LoadFlowDocumentPageViewerWithXAMLFile(string fileName)
    {


      // Open the file that contains the FlowDocument...
      //FileStream xamlFile = new FileStream(fileName, FileMode.Open, FileAccess.Read);
      // and parse the file with the XamlReader.Load method.
      //FlowDocument content = System.Windows.Markup.XamlReader.Load(xamlFile) as FlowDocument;



      FlowDocument content =
        System.Windows.Markup.XamlReader.Load(
                                              new XmlTextReader(new StringReader(RuntimeData.FLOWDOCchapterTexture))) as FlowDocument;





      // Finally, set the Document property to the FlowDocument object that was
      // parsed from the input file.
      VIEWtextbook.Document = content;

      //xamlFile.Close();
    }





    private void ACTdirectionalLiteEnable(object sender, System.Windows.RoutedEventArgs e)
    {
      LITEdirectional.Color = Color.FromRgb(255, 255, 255);
    }
    private void ACTdirectionalLiteDisable(object sender, System.Windows.RoutedEventArgs e)
    {
      LITEdirectional.Color = Color.FromRgb(0, 0, 0);
    }





    private void ACTshowaxesEnable(object sender, System.Windows.RoutedEventArgs e)
    {
      ConstructAnnotationArrow(new Point3D(0, 0, 0), new Vector3D(0, 0, 1), CONTAINERaxes);
      ConstructAnnotationArrow(new Point3D(0, 0, 0), new Vector3D(0, 1, 0), CONTAINERaxes);
      ConstructAnnotationArrow(new Point3D(0, 0, 0), new Vector3D(1, 0, 0), CONTAINERaxes);

      ConstructAnnotationText(new Point3D(20, 0, 0), 4, 4, "X", CONTAINERaxes);
      ConstructAnnotationText(new Point3D(0, 0, 20), 4, 4, "Z", CONTAINERaxes);
      //            ConstructAnnotationText(new Point3D(0, 30, 0), 2, 2, "Y", CONTAINERaxes);
    }


    private void ConstructAnnotationText
    (Point3D originLowerleft, int width, int height, string text, Model3DGroup parent)
    {
      MeshGeometry3D _mesh = new MeshGeometry3D();
      _mesh.Positions.Add(originLowerleft);
      _mesh.Positions.Add(originLowerleft + new Vector3D(width, 0, 0));
      _mesh.Positions.Add(originLowerleft + new Vector3D(width, 0, height));
      _mesh.Positions.Add(originLowerleft + new Vector3D(0, 0, height));

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
      _model.Material = new EmissiveMaterial(vb);

      parent.Children.Add(_model);
    }




    private void ACTshowaxesDisable(object sender, System.Windows.RoutedEventArgs e)
    {
      CONTAINERaxes.Children.Clear();
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





    private void ACTtextureSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      if (SELECTstretchmode == null)
        return;

      TextBlock selblk = (TextBlock)(SELECTstretchmode.SelectedItem);
      switch ((String)(selblk.Tag))
        {
        case "NONE":
          TheImageBrush.Stretch = Stretch.None;
          break;
        case "UNI":
          TheImageBrush.Stretch = Stretch.Uniform;
          break;
        case "UNIFILL":
          TheImageBrush.Stretch = Stretch.UniformToFill;
          break;
        case "FILL":
          TheImageBrush.Stretch = Stretch.Fill;
          break;
        }



      if (SELECTtilemode == null)
        return;

      selblk = (TextBlock)(SELECTtilemode.SelectedItem);
      switch ((String)(selblk.Tag))
        {
        case "NONE":
          TheImageBrush.TileMode = TileMode.None;
          break;
        case "TILE":
          TheImageBrush.TileMode = TileMode.Tile;
          break;
        case "FLIPX":
          TheImageBrush.TileMode = TileMode.FlipX;
          break;
        case "FLIPY":
          TheImageBrush.TileMode = TileMode.FlipY;
          break;
        case "FLIPXY":
          TheImageBrush.TileMode = TileMode.FlipXY;
          break;
        }



      /*
        if (SELECTviewportunits == null)
        return;

        selblk = (TextBlock)(SELECTviewportunits.SelectedItem);
        switch ((String)(selblk.Tag))
        {
        case "RELBB":
        TheImageBrush.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
        break;
        case "ABS":
        TheImageBrush.ViewportUnits = BrushMappingMode.Absolute;
        break;
        }
      */



      if (SELECTimagesource == null)
        return;

      switch (SELECTimagesource.SelectedIndex)
        {
        case 0:
          TheImageBrush.ImageSource =
            (ImageSource)FindResource("IMGgrid");
          break;
        case 1:
          TheImageBrush.ImageSource =
            (ImageSource)FindResource("IMGbrick");
          break;
        case 2:
          TheImageBrush.ImageSource =
            (ImageSource)FindResource("IMGbrickmarked");
          break;
        case 3:
          TheImageBrush.ImageSource =
            (ImageSource)FindResource("IMGcrackle");
          break;
        }
    }



    private void ACTmodelSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      GenerateWalledCompound_UNCOMPR(4);
    }




    private void ACTviewportcoordschanged
    (object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {

      TextBlock selblk = (TextBlock)(SELECTviewportcoords.SelectedItem);
      string commasep = (String)(selblk.Tag);
      // commasep is a comma-sep list like 0,0,1,1
      // Tokenize and use it to set the viewport for TheImageBrush
      TheImageBrush.Viewport = Rect.Parse(commasep);
    }




    private void ACTstamptypeSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      GenerateWalledCompound_UNCOMPR(4);
    }


    /*
      private void ACTmodelSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
      {
      // SELECTgranularity.SelectedIndex
      // SELECTxxxx.SelectedItem.Text

      int granularity;
      String targetsurface;
      String vertspectype;

      try
      {
      granularity = int.Parse(((TextBlock)SELECTgranularity.SelectedItem).Text);
      targetsurface = "Cone";
      vertspectype = ((TextBlock)SELECTvertspec.SelectedItem).Tag.ToString();
      }
      catch (Exception) { return; }


      switch (targetsurface)
      {
      case "Cone":
      GenerateCone(granularity, vertspectype);
      break;
      }

      ComputeXAML();

      }
    */




    private double r /*radius*/ = 60;


    private void GenerateCone(int nSegments, String vertspectype)
    {
      switch (vertspectype)
        {
        case "SHARED":
          GenerateCone_SHARED(nSegments);
          break;
        case "UNCOMPRESSED":
          GenerateCone_UNCOMPR(nSegments);
          break;
        }
    }




    private void GenerateCone_SHARED(int nSegments)
    {
      MeshGeometry3D S = new MeshGeometry3D();

      double fDeltaSegAngle = (2 * Math.PI / nSegments);

      double octant = 2 * Math.PI / 8;

      S.Positions.Add(new Point3D(0, 0, pyrheight));
      S.TextureCoordinates.Add(new Point(0.5,0));

      for (int seg = 0; seg <= nSegments; seg++)
        {
          double x0 = r * Math.Sin(octant + seg * fDeltaSegAngle);
          double y0 = r * Math.Cos(octant + seg * fDeltaSegAngle);

          S.Positions.Add(new Point3D(x0, y0, 0));
          if (seg == 0)
            S.TextureCoordinates.Add(new Point(0, 1));
          else
            S.TextureCoordinates.Add(new Point((seg * 1.0) / nSegments, 1));

          if (seg > 0)
            {
              S.TriangleIndices.Add(0);
              S.TriangleIndices.Add(seg + 1);
              S.TriangleIndices.Add(seg);
            }
        }

      GM3D_pyramid.Geometry = S;
    }



    private void GenerateCone_SHARED_lastbasevertexreused(int nSegments)
    {
      MeshGeometry3D S = new MeshGeometry3D();

      double fDeltaSegAngle = (2 * Math.PI / nSegments);

      double octant = 2 * Math.PI / 8;

      S.Positions.Add(new Point3D(0, 0, pyrheight));
      S.TextureCoordinates.Add(new Point(0.5,0));

      for (int seg = 0; seg < nSegments; seg++)
        {
          double x0 = r * Math.Sin(octant + seg * fDeltaSegAngle);
          double y0 = r * Math.Cos(octant + seg * fDeltaSegAngle);

          S.Positions.Add(new Point3D(x0, y0, 0));
          if (seg == 0)
            S.TextureCoordinates.Add(new Point(0, 1));
          else
            S.TextureCoordinates.Add(new Point((seg * 1.0) / nSegments, 1));

          if (seg > 0)
            {
              S.TriangleIndices.Add(0);
              S.TriangleIndices.Add(seg + 1);
              S.TriangleIndices.Add(seg);
            }
        }
      // Last one: reuse the original 0th base vertex
      S.TriangleIndices.Add(0);
      S.TriangleIndices.Add(1);
      S.TriangleIndices.Add(nSegments);

      GM3D_pyramid.Geometry = S;

    }





    private double pyrheight = 45;


    private void GenerateCone_UNCOMPR(int nSegments)
    {
      MeshGeometry3D S = new MeshGeometry3D();

      double fDeltaSegAngle = (2 * Math.PI / nSegments);
      double octant = 2 * Math.PI / 8;

      Point3D prevpoint = new Point3D(0, 0, 0);
      Point3D firstpoint = prevpoint;
      int curposidx = 0;

      for (int seg = 0; seg < nSegments; seg++)
        {
          double x0 = r * Math.Sin(octant + seg * fDeltaSegAngle);
          double y0 = r * Math.Cos(octant + seg * fDeltaSegAngle);

          if (seg > 0)
            {
              S.Positions.Add(new Point3D(0, 0, pyrheight));
              S.Positions.Add(new Point3D(x0, y0, 0));
              S.Positions.Add(prevpoint);

              S.TextureCoordinates.Add(new Point(0.5, 0));
              S.TextureCoordinates.Add(new Point(0, 1));
              S.TextureCoordinates.Add(new Point(1, 1));

              S.TriangleIndices.Add(curposidx++);
              S.TriangleIndices.Add(curposidx++);
              S.TriangleIndices.Add(curposidx++);
            }

          prevpoint = new Point3D(x0, y0, 0);
          if (seg == 0)
            {
              firstpoint = prevpoint;
            }
        }

      // Last one: reuse the original 0th base vertex
      S.Positions.Add(new Point3D(0, 0, pyrheight));
      S.Positions.Add(firstpoint);
      S.Positions.Add(prevpoint);
      S.TriangleIndices.Add(curposidx++);
      S.TriangleIndices.Add(curposidx++);
      S.TriangleIndices.Add(curposidx++);
      S.TextureCoordinates.Add(new Point(0.5, 0));
      S.TextureCoordinates.Add(new Point(0, 1));
      S.TextureCoordinates.Add(new Point(1, 1));

      GM3D_pyramid.Geometry = S;

    }








    public XYZUVCollection _XYZUVdata = new XYZUVCollection();

    public ObservableCollection<XYZUV> XYZUVdata
    { get { return _XYZUVdata; } }       




    private void GenerateWalledCompound_UNCOMPR(int nSegments)
    {
      if (SELECTstamptype == null)
        return;

      TextBlock selectedStampType = (TextBlock)(SELECTstamptype.SelectedItem);

      MeshGeometry3D S = new MeshGeometry3D();

      double fDeltaSegAngle =  0 - (2 * Math.PI / nSegments);
      double octant = 2 * Math.PI / 8;

      Point3D prevpoint = new Point3D(0, 0, 0);
      Point3D firstpoint = prevpoint;

      _XYZUVdata.Clear();

      for (int seg = 0; seg <= nSegments; seg++)
        {
          double x0;
          double y0;

          if (seg < nSegments)
            {
              x0 = (0-r) * Math.Sin(octant + seg * fDeltaSegAngle);
              y0 = (0-r) * Math.Cos(octant + seg * fDeltaSegAngle);
            }
          else
            {
              x0 = firstpoint.X;
              y0 = firstpoint.Y;
            }



          if (seg > 0)
            {
              int baseidx = S.Positions.Count;

              S.Positions.Add(prevpoint);
              S.Positions.Add(new Point3D(x0, y0, pyrheight));
              S.Positions.Add(new Point3D(prevpoint.X, prevpoint.Y, pyrheight));
              S.Positions.Add(new Point3D(x0, y0, 0));

              Point baby;
              if (selectedStampType.Tag.ToString() == "PerFace")
                {
                  S.TextureCoordinates.Add(baby=new Point(0, 1));
                  _XYZUVdata.Add(S.Positions[baseidx], baby);
                  S.TextureCoordinates.Add(baby=new Point(1, 0));
                  _XYZUVdata.Add(S.Positions[baseidx+1], baby);
                  S.TextureCoordinates.Add(baby=new Point(0, 0));
                  _XYZUVdata.Add(S.Positions[baseidx+2], baby);
                  S.TextureCoordinates.Add(baby=new Point(1, 1));
                  _XYZUVdata.Add(S.Positions[baseidx+3], baby);
                }
              else
                {
                  S.TextureCoordinates.Add(baby=new Point((seg-1)*1.0/nSegments, 1));
                  _XYZUVdata.Add(S.Positions[baseidx], baby);
                  S.TextureCoordinates.Add(baby=new Point((seg)*1.0 / nSegments, 0));
                  _XYZUVdata.Add(S.Positions[baseidx+1], baby);
                  S.TextureCoordinates.Add(baby=new Point((seg-1)*1.0 / nSegments, 0));
                  _XYZUVdata.Add(S.Positions[baseidx+2], baby);
                  S.TextureCoordinates.Add(baby=new Point((seg)*1.0 / nSegments, 1));
                  _XYZUVdata.Add(S.Positions[baseidx+3], baby);
                }

              S.TriangleIndices.Add(baseidx);
              S.TriangleIndices.Add(baseidx + 1);
              S.TriangleIndices.Add(baseidx + 2);

              S.TriangleIndices.Add(baseidx);
              S.TriangleIndices.Add(baseidx + 3);
              S.TriangleIndices.Add(baseidx + 1);

            }

          prevpoint = new Point3D(x0, y0, 0);
          if (seg == 0)
            {
              firstpoint = prevpoint;
            }
        }


      GM3D_pyramid.Geometry = S;

    }





    private void ComputeXAML()
    {
      MeshGeometry3D mesh = (MeshGeometry3D)GM3D_pyramid.Geometry;

      String xaml =
        @"<Page
xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
Title='Pyramid'>

<Page.Resources>
  <DiffuseMaterial x:Key='RSRCmaterialFront'
                   Brush='#FFFF88'/>

  <MeshGeometry3D x:Key='RSRCmeshPyramid'
     Positions='" + mesh.Positions + @"'
     TriangleIndices='" + mesh.TriangleIndices + "'" +
        ((mesh.Normals.Count > 0) ? (@"'
     Normals='" + mesh.Normals + "'") : "") +
        ((mesh.TextureCoordinates.Count > 0) ? (@"'
     Normals='" + mesh.TextureCoordinates + "'") : "") +
        @"
   />

</Page.Resources>
            

<Viewport3D x:Name='VP3D' Height='380' Width='600'>

  <Viewport3D.Camera>
   <PerspectiveCamera
         Position='" + CAMMY.Position + @"'
         LookDirection='" + CAMMY.LookDirection + @"'
         UpDirection='" + CAMMY.UpDirection + @"'
         NearPlaneDistance='0.02'
         FarPlaneDistance='1000'
         FieldOfView='" + CAMMY.FieldOfView + @"'
      />
  </Viewport3D.Camera>


  <ModelVisual3D>
    <ModelVisual3D.Content>

      <Model3DGroup>

          <DirectionalLight  Color='" + LITEdirectional.Color + @"'
                             Direction='1, 1, -1' />

               
          <GeometryModel3D
             Geometry='{StaticResource RSRCmeshPyramid}'
             Material='{StaticResource RSRCmaterialFront}'
           />

      </Model3DGroup>

    </ModelVisual3D.Content>
  </ModelVisual3D>
    
</Viewport3D>

</Page>
";










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





    private void ConstructAnnotationArrow(Point3D origin, Vector3D direction, Model3DGroup parent)
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
      if (direction.X == 0)
        direction.X = 0.0000001;
      double angle = Math.Atan(direction.Y / direction.X) * 180.0 / Math.PI;
      t3dgr.Children.Add(
                         new RotateTransform3D(
                                               new AxisAngleRotation3D(new Vector3D(0, 0, 1), angle)));

      t3dgr.Children.Add(new TranslateTransform3D(
                                                  origin.X, origin.Y, origin.Z));
      parent.Children.Add(wholearrow);
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

  }
}

