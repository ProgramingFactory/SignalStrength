namespace SignalStrength.Graphic3D
{

    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    using MaterialHelper = HelixToolkit.Wpf.MaterialHelper;

    public class TextMesh3D : ModelVisual3D, IEditableObject
    {
        // /////////////////////////////////////////////////  TEXT3D REGION   \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        /// <summary>
        /// Identifies the <see cref="Text3DType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty Text3DTypeProperty = DependencyProperty.Register(
            "Text3DType", typeof(Text3DType), typeof(TextMesh3D), new UIPropertyMetadata(Text3DType.Extruded, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="Text"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(TextMesh3D), new UIPropertyMetadata("3D-Text", OnPropertyChanged));



        // /////////////////////////////////////////////////  FONT REGION   \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

        /// <summary>
        ///  Identifies the FontSize dependency property
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
            "FontSize", typeof(double), typeof(TextMesh3D), new UIPropertyMetadata(5.0D, OnPropertyChanged));


        /// <summary>
        ///     Identifies the Font dependency property.
        /// </summary>
        public static readonly DependencyProperty FontFamilyNameProperty = DependencyProperty.Register(
            "FontFamilyName", typeof(FontFamily), typeof(TextMesh3D), new UIPropertyMetadata(new FontFamily("Segoe UI"), OnPropertyChanged));


        /// <summary>
        ///     Identifies the Font Style dependency property.
        /// </summary>
        public static readonly DependencyProperty FontStyleProperty = DependencyProperty.Register(
            "FontStyle", typeof(FontStyle), typeof(TextMesh3D), new UIPropertyMetadata(FontStyles.Normal , OnPropertyChanged));


        /// <summary>
        ///     Identifies the Font Stretch dependency property.
        /// </summary>
        public static readonly DependencyProperty FontStretchProperty = DependencyProperty.Register(
            "FontStretch", typeof(FontStretch), typeof(TextMesh3D), new UIPropertyMetadata(FontStretches.Normal, OnPropertyChanged));


        /// <summary>
        ///     Identifies the Font Weight dependency property.
        /// </summary>
        public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register(
            "FontWeight", typeof(FontWeight), typeof(TextMesh3D), new UIPropertyMetadata(FontWeights.Normal, OnPropertyChanged));


        // ///////////////////////////////////////////////////  POSITION REGION   \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        /// <summary>
        ///   Identifies the Origin dependency property.
        /// </summary>
        public static readonly DependencyProperty TextOrginProperty = DependencyProperty.Register(
            "TextOrgin", typeof(Point3D), typeof(TextMesh3D), new UIPropertyMetadata(new Point3D(0, 0, 0), GeometryChanged));

        /// <summary>
        ///     Identifies the Text Direction dependency property.
        /// </summary>
        public static readonly DependencyProperty TextDirectionProperty = DependencyProperty.Register(
            "TextDirection", typeof(Vector3D), typeof(TextMesh3D), new UIPropertyMetadata(new Vector3D(-1, 0, 0), GeometryChanged));

        /// <summary>
        ///     Identifies the Up Direction dependency property.
        /// </summary>
        public static readonly DependencyProperty UpDirectionProperty = DependencyProperty.Register(
            "UpDirection", typeof(Vector3D), typeof(TextMesh3D), new UIPropertyMetadata(new Vector3D(0, 0, 1), GeometryChanged));




        // ///////////////////////////////////////////////////  GEOMETRY REGION   \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        /// <summary>
        /// Identifies the <see cref="TextOutlineMesh"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextOutlineMeshProperty = DependencyProperty.Register(
            "TextOutlineMesh", typeof(MeshGeometry3D), typeof(TextMesh3D), new UIPropertyMetadata(null));

        /// <summary>
        ///     Identifies the <see cref="FontExtrudedHowMuch"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontExtrudedHowMuchProperty = DependencyProperty.Register(
            "FontExtrudedHowMuch", typeof(double), typeof(TextMesh3D), new UIPropertyMetadata(0.5D, GeometryChanged));


        /// <summary>
        ///     Identifies the <see cref="FontOutlineThicknes"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontOutlineThicknesProperty = DependencyProperty.Register(
            "FontOutlineThicknes", typeof(double), typeof(TextMesh3D), new UIPropertyMetadata(0.5D, GeometryChanged));

        // ///////////////////////////////////////////////////  MATERIAL & COLOR REGION   \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


        /// <summary>
        /// Identifies the <see cref="FontBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontBrushProperty = DependencyProperty.Register(
            "FontBrush", typeof(Brush), typeof(TextMesh3D), new UIPropertyMetadata(null, FillChanged));


        /// <summary>
        /// Identifies the <see cref="BackMaterial"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BackMaterialProperty = DependencyProperty.Register(
            "BackMaterial", typeof(Material), typeof(TextMesh3D),
            new UIPropertyMetadata(MaterialHelper.CreateMaterial(Brushes.BlueViolet), MaterialChanged));


        /// <summary>
        /// Identifies the <see cref="Material"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaterialProperty = DependencyProperty.Register(
            "Material", typeof(Material), typeof(TextMesh3D),
            new UIPropertyMetadata(MaterialHelper.CreateMaterial(Brushes.Yellow), MaterialChanged));



        // ///////////////////////////////////////////////////  VISIBILITY REGION   \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

        /// <summary>
        ///   The visibility property.
        /// </summary>
        public static readonly DependencyProperty VisibleProperty = DependencyProperty.Register(
            "Visible", typeof(bool), typeof(TextMesh3D), new UIPropertyMetadata(true, VisibleChanged));




        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        /// <summary>
        ///  Gets the  3D Text type
        /// </summary>
        public Text3DType Text3DType
        {
            get { return (Text3DType)GetValue(Text3DTypeProperty); }
            set { SetValue(Text3DTypeProperty, value); }
        }



        /// <summary>
        ///  Gets the  Text Mesh
        /// </summary>
        public MeshGeometry3D TextOutlineMesh
        {
            get { return (MeshGeometry3D)GetValue(TextOutlineMeshProperty); }
            set { SetValue(TextOutlineMeshProperty, value); }
        }

        /// <summary>
        /// FontFamily
        /// </summary>
        [TypeConverter(typeof(FontFamilyConverter))]
        public FontFamily FontFamilyName
        {
            get { return (FontFamily)GetValue(FontFamilyNameProperty); }
            set { SetValue(FontFamilyNameProperty, value); }
        }

        /// <summary>
        /// FontSize
        /// </summary>
        [TypeConverter(typeof(FontSizeConverter))]
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }


        /// <summary>
        /// Orgin of text
        /// </summary>
        [TypeConverter(typeof(Point3DConverter))]
        public Point3D TextOrgin
        {
            get { return (Point3D)GetValue(TextOrginProperty); }
            set { SetValue(TextOrginProperty, value); }
        }


        /// <summary>
        /// TextDirection
        /// </summary>
        [TypeConverter(typeof(Vector3DConverter))]
        public Vector3D TextDirection
        {
            get { return (Vector3D)GetValue(TextDirectionProperty); }
            set { SetValue(TextDirectionProperty, value); }
        }



        /// <summary>
        /// UpDirection
        /// </summary>
        [TypeConverter(typeof(Vector3DConverter))]
        public Vector3D UpDirection
        {
            get { return (Vector3D)GetValue(UpDirectionProperty); }
            set { SetValue(UpDirectionProperty, value); }
        }

        /// <summary>
        /// FontStyle
        /// </summary>
        [TypeConverter(typeof(FontStyleConverter))]
        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }


        /// <summary>
        /// FontStretch
        /// </summary>
        [TypeConverter(typeof(FontStretchConverter))]
        public FontStretch FontStretch
        {
            get { return (FontStretch)GetValue(FontStretchProperty); }
            set { SetValue(FontStretchProperty, value); }
        }

        /// <summary>
        /// FontWeight
        /// </summary>
        [TypeConverter(typeof(FontWeightConverter))]
        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }


        /// <summary>
        /// Font outline thicknes. This is only when <see cref="Text3DType.Outlined"/> is set!
        /// </summary>
        [TypeConverter(typeof(DoubleConverter))]
        public double FontOutlineThicknes
        {
            get { return (double)GetValue(FontOutlineThicknesProperty); }
            set { SetValue(FontOutlineThicknesProperty, value); }
        }

        /// <summary>
        /// Font how much extruded. This is only when <see cref="Text3DType.Extruded"/> is set!
        /// </summary>
        [TypeConverter(typeof(DoubleConverter))]
        public double FontExtrudedHowMuch
        {
            get { return (double)GetValue(FontExtrudedHowMuchProperty); }
            set { SetValue(FontExtrudedHowMuchProperty, value); }
        }



        /// <summary>
        /// Gets or sets the fill brush. This brush will be used for both the Material and BackMaterial.
        /// </summary>
        /// <value>The fill brush.</value>
        //[TypeConverter(typeof(BrushConverter))]
        public Brush FontBrush
        {
            get { return (Brush)GetValue(FontBrushProperty); }
            set { this.SetValue(FontBrushProperty, value); }
        }


        /// <summary>
        /// 
        /// </summary>
        [TypeConverter(typeof(StringConverter))]
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }


        /// <summary>
        /// Gets or sets the back material.
        /// </summary>
        /// <value>The back material.</value>
        public Material BackMaterial
        {
            get { return (Material)this.GetValue(BackMaterialProperty); }
            set { this.SetValue(BackMaterialProperty, value); }
        }
        /// <summary>
        /// Gets or sets the material.
        /// </summary>
        /// <value>The material.</value>
        public Material Material
        {
            get { return (Material)this.GetValue(MaterialProperty); }
            set { this.SetValue(MaterialProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MeshElement3D"/> is visible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the element is visible; otherwise, <c>false</c>.
        /// </value>
        public bool Visible
        {
            get { return (bool)this.GetValue(VisibleProperty); }
            set { this.SetValue(VisibleProperty, value); }
        }

        /// <summary>
        ///   Gets the geometry model.
        /// </summary>
        /// <value>The geometry model.</value>
        public GeometryModel3D Model
        {
            get
            {
                return this.Content as GeometryModel3D;
            }
        }

      

        private Mesh3DFactory Text3DFactory;



        /// <summary>
        /// A flag that is set when the element is in editing mode (<see cref="IEditableObject"/>, 
        /// <see cref="M:System.ComponentModel.IEditableObject.BeginEdit"/> and <see cref="M:System.ComponentModel.IEditableObject.EndEdit"/>).
        /// </summary>
        private bool isEditing;

        /// <summary>
        /// A flag that is set when the geometry is changed.
        /// </summary>
        private bool isGeometryChanged;

        /// <summary>
        /// A flag that is set when the material is changed.
        /// </summary>
        private bool isMaterialChanged;



        /// <summary>
        /// Begins an edit on the object.
        /// </summary>
        public void BeginEdit()
        {
            this.isEditing = true;
            this.isGeometryChanged = false;
            this.isMaterialChanged = false;
        }

        /// <summary>
        /// Discards changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit"/> call.
        /// </summary>
        public void CancelEdit()
        {
            this.isEditing = false;
        }

        /// <summary>
        /// Pushes changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit"/> or <see cref="M:System.ComponentModel.IBindingList.AddNew"/> call into the underlying object.
        /// </summary>
        public void EndEdit()
        {
            this.isEditing = false;
            if (this.isGeometryChanged)
            {
                this.OnGeometryChanged();
            }

            if (this.isMaterialChanged)
            {
                this.OnMaterialChanged();
            }
        }


 

        /// <summary>
        /// Called when Fill is changed.
        /// </summary>
        /// <param name="d">
        /// The mesh element.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        internal static void FillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextMesh3D)d).OnFillChanged();
        }
        
        /// <summary>
        /// The visible flag changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected static void VisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextMesh3D)d).OnGeometryChanged();
        }


        /// <summary>
        /// The geometry was changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected static void GeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextMesh3D)d).OnGeometryChanged();
        }


        /// <summary>
        /// The Material or BackMaterial property was changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected static void MaterialChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextMesh3D)d).OnMaterialChanged();
        }


       
        /// <summary>
        /// Forces an update of the geometry and materials.
        /// </summary>
        public void UpdateModel()
        {
            this.OnGeometryChanged();
            this.OnMaterialChanged();
        }


        /// <summary>
        /// The Property changed.
        /// </summary>
        /// <param name="d">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            ((TextMesh3D)d).OnGeometryChanged();
        }


        /// <summary>
        /// The Fill property was changed.
        /// </summary>
        protected virtual void OnFillChanged()
        {
            if (FontBrush.Opacity == 1) {
                this.Material = MaterialHelper.CreateMaterial(FontBrush);
                this.BackMaterial = this.Material; 
            }
            else
            {
                this.Material = SignalStrength.Graphic3D.Helpers.MaterialHelper.CreateMaterialWithOpacity(FontBrush,FontBrush.Opacity,255D);
                this.BackMaterial = this.Material;
            }
        }

        /// <summary>
        /// Handles changes in geometry or visible state.
        /// </summary>
        protected virtual void OnGeometryChanged()
        {
            if (!this.isEditing)
            {
                this.Model.Geometry = this.Visible ? this.Tessellate() : null;
            }
            else
            {
                // flag the geometry as changed, the geometry will be updated when the <see cref="M:System.ComponentModel.IEditableObject.EndEdit"/> is called.
                this.isGeometryChanged = true;
            }
        }

        /// <summary>
        /// Handles changes in material/back material.
        /// </summary>
        protected virtual void OnMaterialChanged()
        {
            if (!this.isEditing)
            {
                this.Model.Material = this.Material;
                this.Model.BackMaterial = this.BackMaterial;
            }
            else
            {
                this.isMaterialChanged = true;
            }
        }


        /// <summary>
        /// The Pixels Per Density Independent Pixel value, which is the equivalent of the scale factor. 
        /// For example, if the DPI of a screen is 120 (or 1.25 because 120/96 = 1.25) , 1.25 pixel per density independent pixel is drawn
        /// </summary>
        private const int PIXEL_PER_DIP = 1;

        /// <summary>
        /// ctor
        /// </summary>
        public TextMesh3D()
        {
       
            Text3DFactory = new Mesh3DFactory(false,false);
            TextOutlineMesh = new MeshGeometry3D();

            this.Content = new GeometryModel3D();
            this.FontBrush = Brushes.OrangeRed;//TO UPDATE VALUE
            this.UpdateModel();

        }



        protected MeshGeometry3D Tessellate()
        {
            if (Text3DFactory == null || TextOutlineMesh == null)
            {
                return null;
            }

            Text3DFactory = new Mesh3DFactory(false, false);


            this.BeginEdit();
            if (Text3DType == Text3DType.Extruded) {
                this.Model.Geometry = Text3DFactory.ConstructTextExtrudedType(this.TextDirection, this.UpDirection, this.TextOrgin, this.Text, this.FontFamilyName.Source,
                                                                     this.FontStyle, this.FontWeight, this.FontSize, PIXEL_PER_DIP, this.FontExtrudedHowMuch);
            }
         
            else
                throw new NotImplementedException("NotImplementedException TextMesh3D.Text3DType.Outlined");

            this.EndEdit();

            this.Content = this.Model;

            return this.Model.Geometry as MeshGeometry3D;
        }


    }
    public enum Text3DType
    {
        Outlined = 1,
        Extruded = 2
    }
}
