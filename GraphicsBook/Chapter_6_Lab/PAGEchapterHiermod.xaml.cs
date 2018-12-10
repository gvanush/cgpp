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
using _3DTools;
using System.ComponentModel;
using System.Windows.Input;


namespace PlainVanilla
{





  public partial class ChapterHiermod
  {
    FrameworkElement fwl;

        
    public ChapterHiermod()
    {
      this.InitializeComponent();

      LoadFlowDocumentPageViewerWithXAMLFile();

      tb = new _3DTools.Trackball(VP3D.Camera);
      tb.EventSource = VP3D;

      sky.Geometry = Sky.GenerateSky();

    }



    private void OnKeyDownInViewport(object sender, KeyEventArgs e)
    {

      if (e.Key == Key.Space)
	{
	  if (RobotLaunchStoryboard.GetIsPaused(this))
	    {
	      RobotLaunchStoryboard.Resume(this);
	    }
	  else
	    {
	      RobotLaunchStoryboard.Pause(this);
	    }
	  e.Handled = true;
	}
      else
	{
	}
    }
 



    private _3DTools.Trackball tb = null;




    protected void EVENTkeydownInViewport3d(object sender, KeyEventArgs keyevt)
    {
      int x = 3;
    }



    void LoadFlowDocumentPageViewerWithXAMLFile()
    {


      // Open the file that contains the FlowDocument...
      //FileStream xamlFile = new FileStream(fileName, FileMode.Open, FileAccess.Read);
      // and parse the file with the XamlReader.Load method.
      //FlowDocument content = System.Windows.Markup.XamlReader.Load(xamlFile) as FlowDocument;


      StringReader sr = new StringReader(RuntimeData.FLOWDOCchapterHiermod);
      XmlTextReader xtr = new XmlTextReader(sr);
      FlowDocument content =
	System.Windows.Markup.XamlReader.Load(xtr) as FlowDocument;





      // Finally, set the Document property to the FlowDocument object that was
      // parsed from the input file.
      VIEWtextbook.Document = content;

      //xamlFile.Close();
    }






    private void ACTtreeviewSelectedItemChanged
    (object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      if (e.NewValue is RotateTransform3D)
	{
	  RotationAxisDisplay.Start(StuffOnTurntable, (RotateTransform3D)(e.NewValue));
	  return;
	}else{
	RotationAxisDisplay.Stop(StuffOnTurntable);            
      }



      if (e.NewValue is TRVU_GeometryMesh)
	{
	  RemoveObjectHighlights();
	  AddObjectHighlights((e.NewValue as TRVU_GeometryMesh).parent);
	  return;
	}

      if (e.NewValue is TRVU_GeometryModel3D)
	{
	  RemoveObjectHighlights();
	  AddObjectHighlights((e.NewValue as TRVU_GeometryModel3D));
	  return;
	}
            
      if ( (e.NewValue is BindingList<object>) ) {
	RemoveObjectHighlights();
	AddObjectHighlights(e.NewValue as BindingList<object>);
	return;
      }

      e.Handled = true;
    }


    private void AddObjectHighlights(BindingList<object> blst)
    {
      for (int i = 0; i<blst.Count; i++) {
	object curobj = blst[i];
	if (curobj is TRVU_GeometryModel3D)
	  {
	    AddObjectHighlights(curobj as TRVU_GeometryModel3D);
	  }
	else if ((curobj is BindingList<object>))
	  {
	    AddObjectHighlights(curobj as BindingList<object>);
	  }
      }
    }

    private void AddObjectHighlights(TRVU_GeometryModel3D trvugm3d)
    {
      trvugm3d.actual.Material = FindResource("BRUSHyellow") as DiffuseMaterial;
    }

    private void RemoveObjectHighlights(string objname)
    {
      GeometryModel3D gm3d = FindName(objname) as GeometryModel3D;
      if (gm3d != null) 
	gm3d.Material = FindResource("BRUSHgrey") as DiffuseMaterial;
    }

    private void RemoveObjectHighlights()
    {
      RemoveObjectHighlights("LowerLeg");
      RemoveObjectHighlights("Leg");
      RemoveObjectHighlights("GM3D_Camel_SHIN");
      RemoveObjectHighlights("GM3D_Camel_FOOT");
      RemoveObjectHighlights("GM3D_Camel_HEAD");
      RemoveObjectHighlights("GM3D_Camel_TRUNK");
      RemoveObjectHighlights("GM3D_Camel_THIGH");
    }


    static class RotationAxisDisplay {

      static ScreenSpaceLines3D ssl = null;
      static RotateTransform3D rotxform = null;

            
        
      internal static void Start(ModelVisual3D VP3D,RotateTransform3D _rotxform)
      {
	ssl = new ScreenSpaceLines3D();
	rotxform = _rotxform;
	UpdateLocation();

	ssl.Thickness = 2;
	ssl.Color = Color.FromRgb(255, 0, 0);

	VP3D.Children.Add(ssl);
      }


      internal static void UpdateLocation()
      {
	if (ssl == null)
	  return;

	Point3DCollection ray = new Point3DCollection(2);
	Point3D cntrpoint = new Point3D(
					rotxform.CenterX,
					rotxform.CenterY,
					rotxform.CenterZ);

	Vector3D v = new Vector3D();
	v = (((AxisAngleRotation3D)(rotxform.Rotation)).Axis * 500);
	ray.Add(cntrpoint - v);
	ray.Add(cntrpoint + v);

	ssl.Points = ray;
      }



      internal static void Stop(ModelVisual3D VP3D)
      {
	if (ssl == null)
	  return;

	VP3D.Children.Remove(ssl);
	ssl.ShutDown();
	ssl = null;
      }
    }






    private void ACTsetRotateCenterpoint(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
      TextBox tb = (TextBox)sender;
      RotateTransform3D rot = (RotateTransform3D)(((TextBox)sender).Tag);
      Point3D newpt = new Point3D();
      try
	{
	  newpt = Point3D.Parse(tb.Text);
	  rot.CenterX = newpt.X;
	  rot.CenterY = newpt.Y;
	  rot.CenterZ = newpt.Z;
	}
      catch (Exception)
	{
	  return;
	}

      RotationAxisDisplay.UpdateLocation();
    }


    private void ACTsetRotationAxis(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      if ( ! (e.AddedItems[0] is TextBlock)) 
	return;

      String s = (String)(((TextBlock)(e.AddedItems[0])).Tag);
      ComboBox cb = (ComboBox)(((TextBlock)(e.AddedItems[0])).Parent);
      RotateTransform3D rt = (RotateTransform3D)(cb.Tag);
      AxisAngleRotation3D aar = (AxisAngleRotation3D)(rt.Rotation);
      switch (s)
	{
	case "X":
	  aar.Axis = new Vector3D(1, 0, 0);
	  break;
	case "Y":
	  aar.Axis = new Vector3D(0, 1, 0);
	  break;
	case "Z":
	  aar.Axis = new Vector3D(0, 0, 1);
	  break;
	}
      RotationAxisDisplay.UpdateLocation();
    }

        

    private void ACTsetTheSexOfBabyTransform(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      String s = (String)(((TextBlock)(e.AddedItems[0])).Tag);
      if (s == "-")
	return;

      ComboBox cb = (ComboBox)(((TextBlock)(e.AddedItems[0])).Parent);
      TRVU_TransformSexless ts = (TRVU_TransformSexless)(cb.Tag);
      ts.SetSex(s);
    }





    private void ACTdirectionalLiteEnable(object sender, System.Windows.RoutedEventArgs e)
    {
      LITEdirectional.Color = Color.FromRgb(255, 255, 255);
      ComputeXAML();
    }
    private void ACTdirectionalLiteDisable(object sender, System.Windows.RoutedEventArgs e)
    {
      LITEdirectional.Color = Color.FromRgb(0, 0, 0);
      ComputeXAML();
    }





    private void ACTshowaxesEnable(object sender, System.Windows.RoutedEventArgs e)
    {
      int lenAxis = 1;
      ConstructAnnotationArrow(new Point3D(0, 0, 0), new Vector3D(lenAxis, 0, 0), CONTAINERaxes);
      ConstructAnnotationArrow(new Point3D(0, 0, 0), new Vector3D(0, lenAxis, 0), CONTAINERaxes);
      ConstructAnnotationArrow(new Point3D(0, 0, 0), new Vector3D(0, 0, lenAxis), CONTAINERaxes);

      lenAxis = 40;
      ConstructAnnotationText(new Point3D(lenAxis, 0, 0), 5, 5, "X", CONTAINERaxes);
      ConstructAnnotationText(new Point3D(0, lenAxis, 0), 5, 5, "Y", CONTAINERaxes);
      ConstructAnnotationText(new Point3D(-8, 0, lenAxis), 5, 5, "Z", CONTAINERaxes);
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
	      Brush backBrush = new SolidColorBrush(Colors.AntiqueWhite);
	      drawingContext.DrawRectangle(backBrush, new Pen(backBrush, 1), new Rect(0, 0, ft.Width, ft.Height));
	    }
	  drawingContext.DrawText(ft, new Point(0, 0));
	}
      drawingContext.Close();

      VisualBrush vb = new VisualBrush(drawingVisual);
      _model.Material = new EmissiveMaterial(vb);
      _model.BackMaterial = _model.Material;

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
	  if (ispaused)
	    TurntableStoryboard.Pause(this);
	}
      catch (Exception) { }

    }



    private void ACTcamerareset(object sender, System.Windows.RoutedEventArgs e)
    {
      try
	{
	  if (((TextBlock)(LISTBOXmodel.SelectedItem)).Text.ToString().ToUpper().Contains("ANIM"))
              
	    {
	      CAMMY.Position = (Point3D)FindResource("PT_cameraInitPosition_ANIM");
	      CAMMY.LookDirection = (Vector3D)FindResource("PT_cameraInitLookDir_ANIM");
	    }
	  else
	    {
	      CAMMY.Position = (Point3D)FindResource("PT_cameraInitPosition");
	      CAMMY.LookDirection = (Vector3D)FindResource("PT_cameraInitLookDir");
	    }
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









    private void ACTmodelSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      RobotLaunchStoryboard.Stop(this);
      String s = ((TextBlock)(e.AddedItems[0])).Tag.ToString();
      switch (s)
	{
	case "F":
	  PlaceSoleObjOnTurntable("GM3D_Camel_FOOT");
	  break;
	case "S":
	  PlaceSoleObjOnTurntable("GM3D_Camel_SHIN");
	  break;
	case "T":
	  PlaceSoleObjOnTurntable("GM3D_Camel_THIGH");
	  break;
	case "B":
	  PlaceSoleObjOnTurntable("GM3D_Camel_TRUNK");
	  break;
	case "H":
	  PlaceSoleObjOnTurntable("GM3D_Camel_HEAD");
	  break;
	case "HB+":
	  {
	    Model3DGroup cam = CreateCamel();
	    AutoCompositeHeadTrunk(cam);

	    ModelsOnTurntable.Children.Clear();
	    ModelsOnTurntable.Children.Add(cam);
                 
	    TRVU_model.Items.Clear();
	    TRVU_model.Items.Add(new TRVU_Model3DGroup(cam));
	  }
	  break;
	case "FS":
	  {
	    Model3DGroup thelowerleg = CreateLowerLeg();
	    ModelsOnTurntable.Children.Clear();
	    ModelsOnTurntable.Children.Add(thelowerleg);
                 
	    TRVU_model.Items.Clear();
	    TRVU_model.Items.Add(new TRVU_Model3DGroup(thelowerleg));
	  }
	  break;
	case "FST":
	  {
	    Model3DGroup theleg = CreateLeg();
	    ModelsOnTurntable.Children.Clear();
	    ModelsOnTurntable.Children.Add(theleg);

	    TRVU_model.Items.Clear();
	    TRVU_model.Items.Add(new TRVU_Model3DGroup(theleg));
	  }
	  break;
	case "FST+":
	  {
	    Model3DGroup theleg = CreateLeg();
	    AutoCompositeLeg(theleg);
	    ModelsOnTurntable.Children.Clear();
	    ModelsOnTurntable.Children.Add(theleg);

	    TRVU_model.Items.Clear();
	    TRVU_model.Items.Add(new TRVU_Model3DGroup(theleg));
	  }
	  break;


	case "HBLeg_fr_left":
	  {
	    Model3DGroup cam = CreateEntireCamel(1, false);
	    TRVU_Model3DGroup xyz = new TRVU_Model3DGroup(cam);
	    TRVU_model.Items.Clear();
	    TRVU_model.Items.Add(xyz);
	  }
	  break;
	case "HBLegs_fr":
	  {
	    Model3DGroup cam = CreateEntireCamel(2, false);
	    TRVU_Model3DGroup xyz = new TRVU_Model3DGroup(cam);
	    TRVU_model.Items.Clear();
	    TRVU_model.Items.Add(xyz);
	  }
	  break;
	case "HBLegs_fr+":
	  {
	    Model3DGroup cam = CreateEntireCamel(2, true);
	    TRVU_Model3DGroup xyz = new TRVU_Model3DGroup(cam);
	    TRVU_model.Items.Clear();
	    TRVU_model.Items.Add(xyz);
	  }
	  break;

	case "WHOLE+":
	  {
	    Model3DGroup cam = CreateEntireCamel(4,true);
	    TRVU_Model3DGroup xyz = new TRVU_Model3DGroup(cam);
	    TRVU_model.Items.Clear();
	    TRVU_model.Items.Add(xyz);
	  }
	  break;

	case "WHOLE+ANIM":
	  {
	    Model3DGroup cam = CreateEntireCamel(4,true);
	    AddAnimationEnablers();
	    TRVU_Model3DGroup xyz = new TRVU_Model3DGroup(cam);
	    TRVU_model.Items.Clear();
	    TRVU_model.Items.Add(xyz);

	    RobotLaunchStoryboard.Stop(this);
	    RobotLaunchStoryboard.Seek(this, TimeSpan.Zero, TimeSeekOrigin.BeginTime);
	    RobotLaunchStoryboard.Begin(this, true);
	    VP3D.Focus();
	    ACTcamerareset(sender, e);
	  }
	  break;
	}
      ComputeXAML();
    }


    private void AddAnimationEnablers()
    {
      if (FindName("camelTranslationForAnim") != null)
	{
	  return;
	}

      Model3DGroup cam = GetCanonicalComposite("Camel");
      Transform3DGroup tg = new Transform3DGroup();
      tg.Children.Add(new TranslateTransform3D());
      cam.Transform = tg;
      RegisterName("camelTranslationForAnim",tg.Children[0]);

      AddAnimationEnablerKnee("FrontLeft");
      AddAnimationEnablerKnee("FrontRight");
      AddAnimationEnablerKnee("RearLeft");
      AddAnimationEnablerKnee("RearRight");
    }



    private void AddAnimationEnablerKnee(string nameLowerLeg)
    {
      Model3DGroup ll = GetCanonicalComposite(nameLowerLeg + "_LowerLeg");
      Transform3DGroup tg = new Transform3DGroup();
      RotateTransform3D rt3d =
	new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 0));
      rt3d.CenterY = 36.5;
      tg.Children.Add(rt3d);
      ll.Transform = tg;
      RegisterName("leg" + nameLowerLeg + "RotateForAnim", rt3d.Rotation);
    }



    private Model3DGroup CreateEntireCamel(int numlegs, bool autocompos)
    {
      Model3DGroup thelegtemplate = CreateLeg();
      AutoCompositeLeg(thelegtemplate);

      ModelsOnTurntable.Children.Clear();
      TRVU_model.Items.Clear();

      Model3DGroup cam = CreateCamel();
      if (autocompos) AutoCompositeHeadTrunk(cam);

      if (numlegs > 0)
	{
	  numlegs--;
	  cam.Children.Add(ModelCloner.Clone(this, thelegtemplate, "FrontLeft_"));
	}
      if (numlegs > 0)
	{
	  numlegs--;
	  cam.Children.Add(ModelCloner.Clone(this, thelegtemplate, "FrontRight_"));
	}
      if (numlegs > 0)
	{
	  numlegs--;
	  cam.Children.Add(ModelCloner.Clone(this, thelegtemplate, "RearLeft_"));
	}

      if (numlegs > 0)
	{
	  numlegs--;
	  cam.Children.Add(ModelCloner.Clone(this, thelegtemplate, "RearRight_"));
                          
	}

      if (autocompos)
	AutoCompositeLegs();


      ModelsOnTurntable.Children.Add(cam);
      return cam;
    }



    private Model3DGroup GetCanonicalComposite(string instname)
    {
      return (Model3DGroup)(FindName(instname));
    }



    // This creates a lower leg's components but does not place them relative to each other.
    private Model3DGroup CreateLowerLeg()
    {
      object extant = FindName("LowerLeg");
      if (extant != null)
	return (Model3DGroup)extant;


      GeometryModel3D thefoot = GetCanonicalObject("GM3D_Camel_FOOT");
      GeometryModel3D theshin = GetCanonicalObject("GM3D_Camel_SHIN");
      //						TranslateTransform3D theshin_putinplace = new TranslateTransform3D();
      //						theshin_putinplace.OffsetZ = thefoot.Bounds.SizeZ - 5;
      //						((Transform3DGroup)(theshin.Transform)).Children.Add(theshin_putinplace);
      Model3DGroup thelowerleg = new Model3DGroup();
      thelowerleg.SetValue(NameProperty, "LowerLeg");
      RegisterName("LowerLeg", thelowerleg);
      thelowerleg.Children.Add(thefoot);
      thelowerleg.Children.Add(theshin);


      return thelowerleg;
    }




    public void OnAddTransform(
			       Object sender,
			       RoutedEventArgs e
			       )
    {
      TextBlock tb =
	(TextBlock)(((Hyperlink)sender).Parent);
      TRVU_TransformGroupEmpty tge = (TRVU_TransformGroupEmpty)(tb.Tag);
      tge.CreateChildSexUnknown();

    }




    private Model3DGroup CreateLeg()
    {
      object extant = FindName("Leg");
      if (extant != null)
	return (Model3DGroup)extant;

      Model3DGroup thelowerleg = CreateLowerLeg();
      GeometryModel3D thethigh = GetCanonicalObject("GM3D_Camel_THIGH");

      Model3DGroup theleg = new Model3DGroup();
      theleg.Children.Add(thelowerleg);
      theleg.Children.Add(thethigh);
      theleg.SetValue(NameProperty, "Leg");
      RegisterName("Leg", theleg);

      TRVU_Model3DGroup xyz = new TRVU_Model3DGroup(theleg);
      TRVU_model.Items.Clear();
      TRVU_model.Items.Add(xyz);

      return theleg;
    }




    private Model3DGroup CreateCamel()
    {
      object extant = FindName("Camel");
      if (extant != null)
	return (Model3DGroup)extant;

      GeometryModel3D thehead = GetCanonicalObject("GM3D_Camel_HEAD");
      GeometryModel3D thetrunk = GetCanonicalObject("GM3D_Camel_TRUNK");

      Model3DGroup theleg = new Model3DGroup();
      theleg.Children.Add(thehead);
      theleg.Children.Add(thetrunk);
      theleg.SetValue(NameProperty, "Camel");
      RegisterName("Camel", theleg);

      TRVU_Model3DGroup xyz = new TRVU_Model3DGroup(theleg);
      TRVU_model.Items.Clear();
      TRVU_model.Items.Add(xyz);

      return theleg;
    }



    private GeometryModel3D GetCanonicalObject(String rsrcname)
    {
      return GetCanonicalObject(rsrcname, rsrcname);
    }


    private GeometryModel3D GetCanonicalObject(String rsrcname, String instname)
    {
      object extant = FindName(instname);

      if (extant != null)
	{
	  return (GeometryModel3D)extant;
	}

      GeometryModel3D gm3dCanonObj = (GeometryModel3D)FindResource(rsrcname);
      gm3dCanonObj.SetValue(NameProperty, instname);
      gm3dCanonObj = gm3dCanonObj.Clone();
      RegisterName(instname, gm3dCanonObj);
      Rect3D bounds = gm3dCanonObj.Geometry.Bounds;

      return gm3dCanonObj;

      TranslateTransform3D footxform =
	(TranslateTransform3D)(((Transform3DGroup)(gm3dCanonObj.Transform)).Children[0]);
      footxform.OffsetX = -(bounds.SizeX / 2.0) - bounds.X;
      footxform.OffsetY = 0 - bounds.Y;
      footxform.OffsetZ = -(bounds.SizeZ / 2.0) - bounds.Z;

      // Save to a file the coordinates already transformed by the canonical transforms
      // in the XAML specification.
      StreamWriter outfile = File.CreateText("c:/temp/" + rsrcname + ".pts");
      outfile.WriteLine("Number of triangles: " +
			((MeshGeometry3D)gm3dCanonObj.Geometry).TriangleIndices.Count / 3);
      System.Windows.Media.Media3D.Point3DCollection.Enumerator theEnum = 
	((MeshGeometry3D)gm3dCanonObj.Geometry).Positions.GetEnumerator();
      while (theEnum.MoveNext())
	{
	  Point3D xxx = theEnum.Current;
	  Point3D xxxtransformed = gm3dCanonObj.Transform.Value.Transform(xxx);
	  outfile.WriteLine(xxxtransformed);
	}
      outfile.Close();
      //  gm3dCanonObj.Transform.Value.Transform
      //    (();

       

    }




    void PlaceSoleObjOnTurntable(String rsrcname)
    {
      ModelsOnTurntable.Children.Clear();
      TRVU_model.Items.Clear();
      AddObjToTurntable(rsrcname);
    }


    void AddObjToTurntable(String rsrcname)
    {
      GeometryModel3D obj = GetCanonicalObject(rsrcname);
      ModelsOnTurntable.Children.Add(obj);

      try
	{
	  TRVU_GeometryModel3D xyz = new TRVU_GeometryModel3D(obj);
	  TRVU_model.Items.Add(xyz);
	}
      catch (Exception e) {
	string msg = e.Message;
	string src = e.Source;
      }

    }



  

    private void ClearXAML()
    {


      String xaml = @"

There is no XAML to show here at this time.


";

      TEXTBOXshowxaml.Text = xaml;
    }




    private void ACTlightingSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      String s = ((TextBlock)(e.AddedItems[0])).Tag.ToString();
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
    }



    public void UpdateXAML(object sender, System.Windows.RoutedEventArgs e)
    {
      ComputeXAML();
    }


    private void ComputeXAML()
    {
      //MeshGeometry3D mesh = (MeshGeometry3D)GM3D_pyramid.Geometry;

      XamlDecompiler XD = new XamlDecompiler(10);

      string xamlstring = XD.GenerateXaml(
					  this.ModelsOnTurntable);

      String xaml =
	@"<Page
xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
Title='Pyramid'>

<Page.Resources>

		<DiffuseMaterial x:Key='BRUSHgrey'
						 Brush='#999999'/>

		<MeshGeometry3D x:Key='MG3D_Camel_FOOT' 
                 ... huge amount of data not shown here ... />
		<MeshGeometry3D x:Key='MG3D_Camel_SHIN' 
                 ... huge amount of data not shown here ... />
		<MeshGeometry3D x:Key='MG3D_Camel_THIGH' 
                 ... huge amount of data not shown here ... />
		<MeshGeometry3D x:Key='MG3D_Camel_HEAD' 
                 ... huge amount of data not shown here ... />
		<MeshGeometry3D x:Key='MG3D_Camel_TRUNK' 
                 ... huge amount of data not shown here ... />


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



          <!-- NOW COMES THE XAML FOR THE ACTUAL CAMEL MODEL ON THE TURNTABLE -->
" 
	+ xamlstring + @"


      </Model3DGroup>

    </ModelVisual3D.Content>
  </ModelVisual3D>
    
</Viewport3D>

</Page>
";










      if (TEXTBOXshowxaml != null)
	TEXTBOXshowxaml.Text = xaml;
    }

        
    // This creates a full robot with full relative positioning!
    // This will be used to show the reader the "final product" and
    // it will support animation of the shins.

    private void CreateFullRobot()
    {
  
    }   }




}
