namespace SignalStrength.Graphic3D
{

    using System;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    using HelixToolkit.Wpf;

    using SignalStrength.Graphic3D.Helpers;


    public class ChannalAnalyzer3D : ModelVisual3D
    {
        #region DependencyProperty


        public static readonly DependencyProperty ChannalColumnsProperty =
            DependencyProperty.Register("ChannalColumns", typeof(int), typeof(ChannalAnalyzer3D),
                                        new UIPropertyMetadata(11));//channals in 2.4 ghz

        public static readonly DependencyProperty TimeColumnsProperty =
            DependencyProperty.Register("TimeColumns", typeof(int), typeof(ChannalAnalyzer3D),
                                         new UIPropertyMetadata(1)); 


        public static readonly DependencyProperty ShowIntensityProperty =
            DependencyProperty.Register("ShowIntensity", typeof(bool), typeof(ChannalAnalyzer3D),
                                        new UIPropertyMetadata(false));


        public static readonly DependencyProperty ModelScanValueProperty =
          DependencyProperty.Register("ModelScanValue", typeof(ScanValues[]), typeof(ChannalAnalyzer3D),
                                      new UIPropertyMetadata(default(ScanValues[]), ScanValuesChanged));
        #endregion

        #region Properties
        public ScanValues[] ModelScanValue
        {
            get { return (ScanValues[])GetValue(ModelScanValueProperty); }
            set { SetValue(ModelScanValueProperty, value); }
        }
        public int TimeColumns
        {
            get { return (int)GetValue(TimeColumnsProperty); }
            set { SetValue(TimeColumnsProperty, value); }
        }

        public int ChannalColumns
        {
            get { return (int)GetValue(ChannalColumnsProperty); }
            set { SetValue(ChannalColumnsProperty, value); }
        }

        /// <summary>
        /// Show  crowd channals stretched
        /// </summary>
        public bool ShowIntensity
        {
            get { return (bool)GetValue(ShowIntensityProperty); }
            set { SetValue(ShowIntensityProperty, value); }
        }

        #endregion

        #region Private fields
        private const string FRAME_FILE = "fontsFrame.obj";
        private const string FRAME_HR_FILE = "fontsFrame_hr.obj";
        private const string POINTER4TIMECOLL_FILE = "pointer.obj";
        private const string SCEEN_3D_FOLDER = "Models3D";

        private static string basePath4Models = $"{Environment.CurrentDirectory}{System.IO.Path.DirectorySeparatorChar}{SCEEN_3D_FOLDER}";
        
        private Model3DGroup modelGroup;
        private GeometryModel3D pointerModel;

        private  double DEFAULT_DISTANCE = 0.5;
        private double Distance = 1;
        private  double CUBE_SIZE = 0.5;

        private double defaultGridWeight = 11;//channals
        private double gridLenth = 1;//count grid

        private static double timeCount = 1;
        #endregion

        public ChannalAnalyzer3D()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                basePath4Models = $"{System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{System.IO.Path.DirectorySeparatorChar}Models3D";
            }

            modelGroup = new Model3DGroup();
            pointerModel = new GeometryModel3D();

            Content = modelGroup;

            UpdateGrid();
            ImportBaseSceenModels();
            CreateBlankModel();

        }

        private static void ScanValuesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var value = (ScanValues[])e.NewValue;
            ((ChannalAnalyzer3D)d).UpdateChannalsModelValue(value);
        }


        private void UpdateGrid()
        {
            gridLenth =+ timeCount;
            ConstructGrid(gridLenth, defaultGridWeight);
            
        }

        private void ImportBaseSceenModels()
        {
            Model3DGroup frame, pointerTime = default(Model3DGroup);

            var frameWithCultureFile = GetFrameForCulture(System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName);

            frame =  LoadModelHelper.LoadModel($"{basePath4Models}\\{frameWithCultureFile}");
            pointerTime = LoadModelHelper.LoadModel($"{basePath4Models}\\{POINTER4TIMECOLL_FILE}");/*{System.IO.Path.DirectorySeparatorChar}*/
            pointerModel = pointerTime.Children[0] as GeometryModel3D;
            if (!frame.IsFrozen) frame.Freeze();

            this.modelGroup.Children.Add(frame);
            this.modelGroup.Children.Add(pointerModel);
        }

        private string GetFrameForCulture(string twoLetterISOLanguageName)
        {
          return  System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName != "en"
                    ? FRAME_HR_FILE
                    : FRAME_FILE;
        }


        private GeometryModel3D ConstructGrid(double lenth = 5,double width = 11)
        {
            GridLinesVisual3D grid = new GridLinesVisual3D();
            grid.Center = new Point3D(0,- lenth /2, 0);
            grid.LengthDirection = new Vector3D(0, -1, 0);
            grid.Thickness = 0.03;
            grid.MajorDistance = 2;
            grid.MinorDistance = 0.5;
            var material = HelixToolkit.Wpf.MaterialHelper.CreateMaterial(Colors.Gray, 0.5);
            grid.Material = material;
            grid.BackMaterial = material;
            grid.Length = lenth;
            grid.Width = width;
            modelGroup.Children.Add(grid.Model);
            return grid.Model;
        }


        private void InitializeSceneWithBlankModel()
        {
            modelGroup = new Model3DGroup();
            Content = null;
            Content = modelGroup;
            timeCount = 1;
            UpdateGrid();
            ImportBaseSceenModels();
            CreateBlankModel();
        }

        private void CreateBlankModel()
        {

            Color color = new Color();

            Material materialBase = HelixToolkit.Wpf.MaterialHelper.CreateMaterial(DefaultColor(CHANNAL_CROWD.NO_VALUE, out color));

            for (int i = 0; i < ChannalColumns; i++)
            {
                var centar = new Point3D((i - (ChannalColumns - 1) * DEFAULT_DISTANCE) * Distance, (0 - DEFAULT_DISTANCE) * Distance, 0);

                var defaultColumn = new Mesh3DFactory(false, false);
                var defaultBox = defaultColumn.AddCustomBox_B_LRFB_T_Faces(centar, new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), CUBE_SIZE, CUBE_SIZE, 0.2, materialBase, materialBase, materialBase);

                modelGroup.Children.Add(defaultBox);
            }
        }


        private void UpdateEmptyColumn(int position)
        {
            Color colorForMaterial = new Color();
            Material materialBase = HelixToolkit.Wpf.MaterialHelper.CreateMaterial(DefaultColor(CHANNAL_CROWD.NO_VALUE, out colorForMaterial));

            var centar = new Point3D((position - (ChannalColumns - 1) * DEFAULT_DISTANCE) * Distance, ((-timeCount - DEFAULT_DISTANCE) * Distance) + 1, 0);

            var defaultColumn = new Mesh3DFactory(false, false);
            var defaultBox = defaultColumn.AddCustomBox_B_LRFB_T_Faces(centar, new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), CUBE_SIZE, CUBE_SIZE, 0.2, materialBase, materialBase, materialBase);

            modelGroup.Children.Add(defaultBox);

        }

        public void UpdateChannalsModelValue(ScanValues[] values)
        {
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                //If TimeColumns 
                if (timeCount > TimeColumns) {
                    InitializeSceneWithBlankModel();
                }

                timeCount += 1;
                UpdateGrid();


                double zScale = 0.5;

                Color colorForMaterial = new Color();
                Material materialBase =  HelixToolkit.Wpf.MaterialHelper.CreateMaterial(DefaultColor(CHANNAL_CROWD.NO_VALUE, out  colorForMaterial )); 

                //pointers
                var pointerCopy = pointerModel.Clone();
                var pointerBounds = pointerCopy.Bounds;
                pointerCopy.Transform = new TranslateTransform3D(new Vector3D(0, (-timeCount * Distance) + 1, 0));
                pointerCopy.Freeze();
                modelGroup.Children.Add(pointerCopy);

                var scanTimeD = values.FirstOrDefault().Time;
                if (scanTimeD is null) scanTimeD = DateTime.Now.ToString("HH:mm:ss");//cca 1 sec diffrence


                var scanTime = Prepare3DText(scanTimeD, new Point3D(7, (-timeCount + 1 - DEFAULT_DISTANCE) * Distance, 0.25), 0, 0.35);
                modelGroup.Children.Add(scanTime);

                //if have duplicate 
                var repeatingCH = values.OrderBy(obj => obj.Channal).Select(o => o.Channal).ToArray().RepeatingChannals();
                var repeatingCH_Name = values.OrderBy(obj => obj.Channal).ToArray().RepeatingChannals().Select(o => new { channal = o.Item1, repeatTime = o.Item2, names = o.Item3.ToArray() });

                for (int i = 0; i < ChannalColumns; i++)
                {
                    int chCount = (ChannalColumns - 1);
                    int zScaleChannalCrowd = 0;
                    var ch = i + 1;
                    var isCH = repeatingCH.Keys.Contains(ch) ? repeatingCH.TryGetValue(ch, out zScaleChannalCrowd) : false;
                    if (!isCH)
                    {
                        // [ch,CH_Count,name]
                        UpdateEmptyColumn(i);
                    }
                    else
                    { 
                        //To show hight diffrence
                        if (ShowIntensity)
                            zScale = zScaleChannalCrowd > 3 /*CHANNAL_CROWD.TRIPLE_VALUE*/ ? 2 : 0.5;

                        var zText = (zScale * zScaleChannalCrowd) + 0.2;//0.2 -> add little more
                       
                        Point3D centar = new Point3D((i - (ChannalColumns - 1) * DEFAULT_DISTANCE) * Distance, ((-timeCount - DEFAULT_DISTANCE) * Distance) + 1, (zScale * zScaleChannalCrowd ) / 2);

                        Color colorTop = new Color();
                        Material lefRightMaterial = GetMaterial(zScaleChannalCrowd, out colorTop);
                        Material topMaterial = HelixToolkit.Wpf.MaterialHelper.CreateMaterial(colorTop);

                        var defaultColumn = new Mesh3DFactory();
                        var defaultBox = defaultColumn.AddCustomBox_B_LRFB_T_Faces(centar, new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), CUBE_SIZE, CUBE_SIZE,
                            (zScale * zScaleChannalCrowd) + 0.3, lefRightMaterial, materialBase, topMaterial);

                        modelGroup.Children.Add(defaultBox);

                        //TODO: Dont filter names? (show double names)
                        var names = repeatingCH_Name.Where(e => e.channal == ch).Select(x => x.names);
                        foreach (var item in names)
                        {
                            for (int l = 0; l < item.Count(); l++)
                            {
                               
                                string trimedShortText = item[l].Trim();
                                //check if text is empty
                                if (String.IsNullOrEmpty(trimedShortText)) {
                                    trimedShortText = "<?>";
                                }

                                const double FONT_DISTANCE_cca = 0.1;
                                const double TEXT_WIDTH = 0.038;
                                const int MAX_TEXT_LENGHT = 13;
                                int ITEM_COUNT = item.Count();

                                double startingYPoint = centar.Y - (-(ITEM_COUNT / 2) * 0.1);

                                //trim name if longer of 10 chars
                                if (item[l].Length > 10)
                                {
                                    Point3D textCentar = new Point3D(centar.X - (-(MAX_TEXT_LENGHT * TEXT_WIDTH)), (centar.Y - (-(ITEM_COUNT / 2) * 0.1)) - (l * 0.1), centar.Z + (zText / 2 + 0.22) + (l * 0.01)); //centar.Z + (z / 2) + 0.22);
                                    var textModel3d = Prepare3DText($"{item[l].TrimText()}", textCentar, item[l].CalculateSeedFromString());

                                    modelGroup.Children.Add(textModel3d);
                                }
                                else
                                {

                                    Point3D textCentar = new Point3D(centar.X - (-(trimedShortText.Length * TEXT_WIDTH)), (centar.Y - (-(ITEM_COUNT / 2) * 0.1)) - (l * 0.1), centar.Z + (zText / 2 + 0.22) + (l * 0.01));
                                    var textModel3d = Prepare3DText($"{trimedShortText}", textCentar, trimedShortText.CalculateSeedFromString());

                                    modelGroup.Children.Add(textModel3d);
                                }
                            }

                        }
                    }
                }
            }));
        }


        /// <summary>
        /// Create 3D text,throw Exception if <see cref="text"/> is null or empty
        /// </summary>
        /// <param name="text">text to create</param>
        /// <param name="centar">text centar </param>
        /// <param name="seedRandomBrush">Make random brush if seed is 0 brush is blue with opacity</param>
        /// <param name="fontSize">font size</param>
        /// <returns></returns>
        private GeometryModel3D Prepare3DText(string text, Point3D centar,int seedRandomBrush = 0,double fontSize = 0.15)
        {
            var text3d = new TextMesh3D(); 
            text3d.Visible = false;

            text3d.TextDirection = new Vector3D(-1,  0, 0);
            text3d.UpDirection =   new Vector3D( 0,  0, 1);
            text3d.Text3DType = Text3DType.Extruded;
            text3d.FontExtrudedHowMuch = 0.05;
            text3d.Text = text;
            text3d.TextOrgin = centar;
            text3d.FontBrush = seedRandomBrush == 0 ? (SolidColorBrush)new BrushConverter().ConvertFromString("#884D67D8") : GetRandomBrush(seedRandomBrush);
            text3d.FontSize = fontSize == 0.15 ? 0.15 : fontSize;
            
            text3d.Visible = true;
            text3d.Model.Freeze();
            return text3d.Model;
        }


        //TODO: GENERETE 10 RANDOM COLORS 
        public SolidColorBrush GetRandomBrush(int seed = 0)
        {
        NEW_COLOR:
            seed += 48;//86; //48;//42;//85
            string[] brushArray = BrushArray;
            Random random = seed is 48 ? new Random() : new Random(seed);
            string randomColor = brushArray[random.Next(brushArray.Length)];
            //if random our default color do it again 
            if (randomColor.Equals("Blue") || randomColor.Equals("Black") || randomColor.Equals("Green") ||
                randomColor.Equals("Yellow") || randomColor.Equals("Red") || randomColor.Equals("Orange"))
            {
                goto NEW_COLOR;
            }
            SolidColorBrush randomBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(randomColor);

            return randomBrush;
        }

        //reflection
        private static string[] BrushArray
        {
            get
            {
                if(brushArray is null)
                    BrushArray = typeof(Brushes).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                                                .Where(p => p.Name != "Transparent")
                                                .Select(o => o.Name)
                                                .ToArray();
                return brushArray;
            }
            set
            {
                brushArray = typeof(Brushes).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                                                .Where(p => p.Name != "Transparent")
                                                .Select(o => o.Name)
                                                .ToArray();
            }
        }
        private static string[] brushArray;
        /// <summary>
        /// Default  linear gradiat,colors meen value,blue => cold(no value),red => hot (extra value)
        /// </summary>
        /// <param name="channalValue">WI-FI channals value <see cref="CHANNAL_CROWD"/></param>
        /// <param name="topColor">Top color meen view from the top [box top face]</param>
        /// <returns></returns>
        private LinearGradientBrush DefaultColor(CHANNAL_CROWD channalValue,out Color topColor)
        {
            LinearGradientBrush linearGradient = new LinearGradientBrush();
            linearGradient.SpreadMethod = GradientSpreadMethod.Pad;
            linearGradient.MappingMode = BrushMappingMode.RelativeToBoundingBox;
            linearGradient.Opacity = 1;
            linearGradient.StartPoint = new Point(0, 1);


            switch (channalValue)
            {

                case CHANNAL_CROWD.SINGLE_VALUE:
                    linearGradient.GradientStops.Add(new GradientStop(Colors.Purple, 0.50));
                    linearGradient.GradientStops.Add(new GradientStop(Colors.Blue, 1.0));
                    topColor = Colors.Blue;
                    break;
                case CHANNAL_CROWD.DOUBLE_VALUE:
                    linearGradient.GradientStops.Add(new GradientStop(Colors.Purple, 0.30));
                    linearGradient.GradientStops.Add(new GradientStop(Colors.Blue, 0.60));
                    linearGradient.GradientStops.Add(new GradientStop(Colors.Green, 0.90));
                    topColor = Colors.Green;
                    break;
                case CHANNAL_CROWD.TRIPLE_VALUE:
                    linearGradient.GradientStops.Add(new GradientStop(Colors.Purple, 0.20));
                    linearGradient.GradientStops.Add(new GradientStop(Colors.Blue, 0.40));
                    linearGradient.GradientStops.Add(new GradientStop(Colors.Green, 0.60));
                    linearGradient.GradientStops.Add(new GradientStop(Colors.Yellow, 0.80));
                    topColor = Colors.Yellow;
                    break;
                case CHANNAL_CROWD.CROWD_VALUE:
                    linearGradient.GradientStops.Add(new GradientStop(Colors.Purple, 0.18));
                    linearGradient.GradientStops.Add(new GradientStop(Colors.Blue, 0.33));
                    linearGradient.GradientStops.Add(new GradientStop(Colors.Green, 0.51));
                    linearGradient.GradientStops.Add(new GradientStop(Colors.Yellow, 0.69));
                    linearGradient.GradientStops.Add(new GradientStop(Colors.Orange, 0.85));
                    topColor = Colors.Orange;
                    break;
                case CHANNAL_CROWD.EXTRA_CRAWD:
                    linearGradient.GradientStops.Add(new GradientStop(Colors.Purple, 0.14));
                    linearGradient.GradientStops.Add(new GradientStop(Colors.Blue, 0.28));
                    linearGradient.GradientStops.Add(new GradientStop(Colors.Green, 0.42));
                    linearGradient.GradientStops.Add(new GradientStop(Colors.Yellow, 0.56));
                    linearGradient.GradientStops.Add(new GradientStop(Colors.Orange, 0.70));
                    linearGradient.GradientStops.Add(new GradientStop(Colors.Red, 0.90));
                    topColor = Colors.Red;
                    break;
                default:
                    linearGradient.GradientStops.Add(new GradientStop(Colors.Purple, 1.0));
                    topColor = Colors.Purple;
                    break;
            }
           
            linearGradient.EndPoint = new Point(0, 0);
            return linearGradient;
        }


        /// <summary>
        ///     Get material for <see cref="DefaultColor(CHANNAL_CROWD, out Color)"/>
        /// </summary>
        /// <param name="value">WI-FI channals how many user have</param>
        /// <param name="colortop">Box top face</param>
        /// <returns></returns>
        private Material GetMaterial(int value,out Color colortop)
        {
            Color color = new Color();
            switch (value)
            {
                case 0:
                    var mat = HelixToolkit.Wpf.MaterialHelper.CreateMaterial(DefaultColor(CHANNAL_CROWD.NO_VALUE,out color));
                    colortop = color;
                    return mat;
                case 1:
                    var mat1 = HelixToolkit.Wpf.MaterialHelper.CreateMaterial(DefaultColor(CHANNAL_CROWD.SINGLE_VALUE, out color));
                    colortop = color;
                    return mat1;
                case 2:
                    var mat2 = HelixToolkit.Wpf.MaterialHelper.CreateMaterial(DefaultColor(CHANNAL_CROWD.DOUBLE_VALUE, out color));
                    colortop = color;
                    return mat2;
                case 3:
                    var mat3 = HelixToolkit.Wpf.MaterialHelper.CreateMaterial(DefaultColor(CHANNAL_CROWD.TRIPLE_VALUE, out color));
                    colortop = color;
                    return mat3;
                case 4:
                    var mat4 = HelixToolkit.Wpf.MaterialHelper.CreateMaterial(DefaultColor(CHANNAL_CROWD.CROWD_VALUE, out color));
                    colortop = color;
                    return mat4;
                case 5:
                    var mat5 = HelixToolkit.Wpf.MaterialHelper.CreateMaterial(DefaultColor(CHANNAL_CROWD.EXTRA_CRAWD, out color));
                    colortop = color;
                    return mat5;
                default:
                    mat = HelixToolkit.Wpf.MaterialHelper.CreateMaterial(DefaultColor(CHANNAL_CROWD.NO_VALUE, out color));
                    colortop = color;
                    return mat;

            }
        }

    }
}

