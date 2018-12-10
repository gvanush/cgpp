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




    public partial class ChapterPointlight
    {
        public ChapterPointlight()
        {
            this.InitializeComponent();

            LoadFlowDocumentPageViewerWithXAMLFile();

            tb = new _3DTools.Trackball(VP3D.Camera);
            tb.EventSource = VP3D;

            sky.Geometry = Sky.GenerateSky();

            SphereGeometry3D lightAvatar = new SphereGeometry3D();
            lightAvatar.Separators = 4;
            lightAvatar.Radius = 3;
            MODELlightAvatar.Geometry = lightAvatar.Geometry();
        }


        private _3DTools.Trackball tb = null;




        void LoadFlowDocumentPageViewerWithXAMLFile()
        {
            StringReader sr = new StringReader(RuntimeData.FLOWDOCchapterPointlight);
            XmlTextReader xtr = new XmlTextReader(sr);
            FlowDocument content =
                System.Windows.Markup.XamlReader.Load(xtr) as FlowDocument;

            // Finally, set the Document property to the FlowDocument object that was
            // parsed from the input file.
            VIEWtextbook.Document = content;
        }











        private void ACTshowaxesEnable(object sender, System.Windows.RoutedEventArgs e)
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




        private void ACTambientLiteEnable(object sender, System.Windows.RoutedEventArgs e)
        {
            LITEambient.Color = Color.FromRgb(255, 255, 255);
        }
        private void ACTambientLiteDisable(object sender, System.Windows.RoutedEventArgs e)
        {
            LITEambient.Color = Color.FromRgb(0, 0, 0);
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
    









        // Here we create a rectangle via tesselation.
        // We place each triangle in its own Model so we can control the appearance
        // of each one individually.  This allows for showing the tesselation to the
        // user in a kind of "wireframe" mode, versus allowing the rectangle to be
        // unified with a single texture in "render" mode.


        private Model3D GenerateSquare(int gran, double length, bool modeShowTess)
        {
            if (!modeShowTess)
            {
                return GenerateSquareTextured(gran, length);
            }
            else
            {
                GenerateSquareHarlequin(gran, length);
                return null;
            }
        }





        void NOTUSED(int gran, double length, bool modeShowTess)
        {

            double zero = 1;

            // The granularity specifies the number of minisquares the
            // full square will be divided into.
            //
            // The square is built on the XY plane, so Z is locked to zero.
            // The square is built in the 1st quadrant of the XY plane, so
            // all coordinates are non-negative.

            Model3DGroup G = new Model3DGroup();

            Material matodd = (Material)FindResource("MATtriOdd");
            Material mateven = (Material)FindResource("MATtriEven");

            if (!modeShowTess)
            {
                matodd = (Material)FindResource("MATbrickwall");
                mateven= matodd;
            }

            double minisquarelen = length / gran;

            for (int i = 0; i < gran; i++)
            {
                for (int j = 0; j < gran; j++)
                {
                    // CREATE THE ODD TRIANGLE FOR THIS PAIR

                    MeshGeometry3D meshOdd = new MeshGeometry3D();

                    meshOdd.Positions.Add(new Point3D(i * minisquarelen, j * minisquarelen, zero));
                    meshOdd.Positions.Add(new Point3D(i * minisquarelen, (j + 1) * minisquarelen, zero));
                    meshOdd.Positions.Add(new Point3D((i + 1) * minisquarelen, (j + 1) * minisquarelen, zero));
                    meshOdd.Positions.Add(new Point3D((i + 1) * minisquarelen, (j + 0) * minisquarelen, zero));

                    meshOdd.TextureCoordinates.Add(new Point((i + 0.0) / gran, (j + 0.0) / gran));
                    meshOdd.TextureCoordinates.Add(new Point((i + 0.0) / gran, (j + 1.0) / gran));
                    meshOdd.TextureCoordinates.Add(new Point((i + 1.0) / gran, (j + 1.0) / gran));
                    meshOdd.TextureCoordinates.Add(new Point((i + 1.0) / gran, (j + 0.0) / gran));

                    meshOdd.TriangleIndices.Add(0);
                    meshOdd.TriangleIndices.Add(2);
                    meshOdd.TriangleIndices.Add(1);

                    MeshGeometry3D meshEven = meshOdd.Clone();

                    meshEven.TriangleIndices.Clear();
                    meshEven.TriangleIndices.Add(0);
                    meshEven.TriangleIndices.Add(3);
                    meshEven.TriangleIndices.Add(2);

                    GeometryModel3D GM3Dodd = new GeometryModel3D(meshOdd, matodd);
                    GeometryModel3D GM3Deven = new GeometryModel3D(meshEven, mateven);

                    G.Children.Add(GM3Dodd);
                    G.Children.Add(GM3Deven);
                }
            }

   
        }


        private Model3D GenerateSquareTextured(int gran, double length)
        {
            double zero = 0;

            // The granularity specifies the number of minisquares the
            // full square will be divided into.
            //
            // The square is built on the XY plane, so Z is locked to zero.
            // The square is built in the 1st quadrant of the XY plane, so
            // all coordinates are non-negative.


            
            Material matl = (Material)FindResource("MATbrickwall");

            MeshGeometry3D mesh = new MeshGeometry3D();
            GeometryModel3D gm3d = new GeometryModel3D(mesh, matl);

            double minisquarelen = length / gran;

            int posidx = 0;

            for (int i = 0; i < gran; i++)
            {
                for (int j = 0; j < gran; j++)
                {
                    mesh.Positions.Add(new Point3D(i * minisquarelen, j * minisquarelen, zero));
                    mesh.Positions.Add(new Point3D(i * minisquarelen, (j + 1) * minisquarelen, zero));
                    mesh.Positions.Add(new Point3D((i + 1) * minisquarelen, (j + 1) * minisquarelen, zero));
                    mesh.Positions.Add(new Point3D((i + 1) * minisquarelen, (j + 0) * minisquarelen, zero));

                    mesh.TextureCoordinates.Add(new Point((i + 0.0) / gran, (j + 0.0) / gran));
                    mesh.TextureCoordinates.Add(new Point((i + 0.0) / gran, (j + 1.0) / gran));
                    mesh.TextureCoordinates.Add(new Point((i + 1.0) / gran, (j + 1.0) / gran));
                    mesh.TextureCoordinates.Add(new Point((i + 1.0) / gran, (j + 0.0) / gran));

                    mesh.TriangleIndices.Add(posidx+0);
                    mesh.TriangleIndices.Add(posidx + 2);
                    mesh.TriangleIndices.Add(posidx + 1);

                    mesh.TriangleIndices.Add(posidx + 0);
                    mesh.TriangleIndices.Add(posidx + 3);
                    mesh.TriangleIndices.Add(posidx + 2);

                    posidx += 4;
                }
            }

            return gm3d;
        }






        private void GenerateSquareHarlequin(int gran, double length)
        {
            double zero = 0;

            // The granularity specifies the number of minisquares the
            // full square will be divided into.
            //
            // The square is built on the XY plane, so Z is locked to zero.
            // The square is built in the 1st quadrant of the XY plane, so
            // all coordinates are non-negative.


            MeshGeometry3D meshodd = new MeshGeometry3D();
            MeshGeometry3D mesheven = new MeshGeometry3D();

            double minisquarelen = length / gran;

            int posidx = 0;

            for (int i = 0; i < gran; i++)
            {
                for (int j = 0; j < gran; j++)
                {
                    meshodd.Positions.Add(new Point3D(i * minisquarelen, j * minisquarelen, zero));
                    meshodd.Positions.Add(new Point3D(i * minisquarelen, (j + 1) * minisquarelen, zero));
                    meshodd.Positions.Add(new Point3D((i + 1) * minisquarelen, (j + 1) * minisquarelen, zero));
                    meshodd.Positions.Add(new Point3D((i + 1) * minisquarelen, (j + 0) * minisquarelen, zero));

                    meshodd.TriangleIndices.Add(posidx + 0);
                    meshodd.TriangleIndices.Add(posidx + 2);
                    meshodd.TriangleIndices.Add(posidx + 1);

                    mesheven.TriangleIndices.Add(posidx + 0);
                    mesheven.TriangleIndices.Add(posidx + 3);
                    mesheven.TriangleIndices.Add(posidx + 2);

                    posidx += 4;
                }
            }

            mesheven.Positions = meshodd.Positions.Clone();

            Material matodd = (Material)FindResource("MATtriOdd");
            Material mateven = (Material)FindResource("MATtriEven");



            /*
            HarlequinOdd.Geometry = meshodd;
            HarlequinOdd.Material = matodd;

            HarlequinEven.Geometry = mesheven;
            HarlequinEven.Material = mateven;
             * */


            
            GeometryModel3D GM3Dodd = new GeometryModel3D(meshodd, matodd);
            UnregisterName("HarlequinOdd");
            RegisterName("HarlequinOdd", GM3Dodd);

            GeometryModel3D GM3Deven = new GeometryModel3D(mesheven, mateven);
            UnregisterName("HarlequinEven");
            RegisterName("HarlequinEven", GM3Deven);

            MODELGRPtesselationdemo.Children.Clear();
            MODELGRPtesselationdemo.Children.Add(GM3Dodd);
            MODELGRPtesselationdemo.Children.Add(GM3Deven);
            

              
            
        }






        private void ACTmodelSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            String selectedGran = ((TextBlock)(e.AddedItems[0])).Tag.ToString();

    

            Model3D newsquare = GenerateSquareTextured(int.Parse(selectedGran), 100);
            StuffOnTurntable.Children.Clear();
            StuffOnTurntable.Children.Add(newsquare);

            MODELGRPtesselationdemo.Children.Clear();
            GenerateSquareHarlequin(int.Parse(selectedGran), 100);
  
            STORYBDfadeTesselation.Begin(this, true);

        }






        private void ComputeXAML()
        {
            if (TEXTBOXshowxaml != null)
                TEXTBOXshowxaml.Text = "NOT YET SUPPORTED FOR THIS CHAPTER";

        }





        private void ACTlightAttentypeChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        { }



        private void ACTlightingSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            String s = ((TextBlock)(e.AddedItems[0])).Tag.ToString();
            /*
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
             */
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
    }
}
