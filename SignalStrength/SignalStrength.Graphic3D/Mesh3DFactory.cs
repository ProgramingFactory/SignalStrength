using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using SignalStrength.Graphic3D.Extensions;

using DoubleOrSingle = System.Double;

namespace SignalStrength.Graphic3D
{
    public class Mesh3DFactory
    {
        /// <summary>
        /// The positions.
        /// </summary>
        private Point3DCollection positions;
        /// <summary>
        /// Gets the positions collection of the mesh.
        /// </summary>
        /// <value> The positions. </value>
        public Point3DCollection Positions { get { return this.positions; } }
        /// <summary>
        /// The triangle indices.
        /// </summary>
        private Int32Collection triangleIndices;
        /// <summary>
        /// Gets the triangle indices.
        /// </summary>
        /// <value>The triangle indices.</value>
        public Int32Collection TriangleIndices { get { return this.triangleIndices; } }
        /// <summary>
        /// The normal vectors.
        /// </summary>
        private Vector3DCollection normals;
        /// <summary>
        /// Gets the normal vectors of the mesh.
        /// </summary>
        /// <value>The normal vectors.</value>
        public Vector3DCollection Normals { get { return this.normals; } set { this.normals = value; } }
        /// <summary>
        /// The texture coordinates.
        /// </summary>
        private PointCollection textureCoordinates;
        /// <summary>
        /// Gets the texture coordinates of the mesh.
        /// </summary>
        /// <value>The texture coordinates.</value>
        public PointCollection TextureCoordinates { get { return this.textureCoordinates; } set { this.textureCoordinates = value; } }

        /// <summary>
        /// The circle cache.
        /// </summary>
        private static readonly ThreadLocal<Dictionary<int, IList<Point>>> CircleCache 
                          = new ThreadLocal<Dictionary<int, IList<Point>>>(() => new Dictionary<int, IList<Point>>());
        /// <summary>
        /// The closed circle cache.
        /// </summary>
        private static readonly ThreadLocal<Dictionary<int, IList<Point>>> ClosedCircleCache 
                          = new ThreadLocal<Dictionary<int, IList<Point>>>(() => new Dictionary<int, IList<Point>>());



        public Mesh3DFactory(bool generateNormals = true, bool generateTexCoords = true)
        {
            this.positions = new Point3DCollection();
            this.triangleIndices = new Int32Collection();
            if (generateNormals)
            {
                this.normals = new Vector3DCollection();
            }
            if (generateTexCoords)
            {
                this.textureCoordinates = new PointCollection();
            }
        }


        /// <summary>
        /// Adds a box with the specified leftRightFrontBack face,top face and bottom face,for coloring top and bottom custom material, aligned with the specified axes 
        /// </summary>
        /// <param name="center">The center point of the box.</param>
        /// <param name="x">The x axis.</param>
        /// <param name="y">The y axis.</param>
        /// <param name="xlength">The length of the box along the X axis.</param>
        /// <param name="ylength">The length of the box along the Y axis.</param>
        /// <param name="zlength">The length of the box along the Z axis.</param>
        /// <param name="leftRightMat">Left,right,front and back material</param>
        /// <param name="bottomMat">bottom material</param>
        /// <param name="topMat">top material</param>
        /// <returns></returns>
        public Model3DGroup AddCustomBox_B_LRFB_T_Faces(Point3D center, Vector3D x, Vector3D y, double xlength, double ylength, double zlength,
                                    Material leftRightMat, Material bottomMat, Material topMat)
        {
            Model3DGroup model3DGroup = new Model3DGroup();

            var leftRightFrontBack = this.AddBoxLeftRightFrontBackFace(center, x, y, xlength, ylength, zlength);
            var bottom = this.AddBoxBottomFace(center, x, y, xlength, ylength, zlength);
            var top = this.AddBoxTopFace(center, x, y, xlength, ylength, zlength);

            var Top = new GeometryModel3D(top, topMat);
            var Bottom = new GeometryModel3D(bottom, bottomMat);
            var LeftRight = new GeometryModel3D(leftRightFrontBack, leftRightMat);

            Top.Freeze();
            Bottom.Freeze();
            LeftRight.Freeze();

            model3DGroup.Children.Add(Top);
            model3DGroup.Children.Add(Bottom);
            model3DGroup.Children.Add(LeftRight);

            return model3DGroup;

        }


        private MeshGeometry3D AddBoxLeftRightFrontBackFace(Point3D center, Vector3D x, Vector3D y, double xlength, double ylength, double zlength)
        {
            Vector3D vector3D = Text3DFactoryExtensions.CrossProduct(ref x, ref y);

            //Front
            this.AddCubeFace(center, x, vector3D, xlength, ylength, zlength);

            //Back
            this.AddCubeFace(center, -x, vector3D, xlength, ylength, zlength);

            //Lef
            this.AddCubeFace(center, -y, vector3D, ylength, xlength, zlength);

            //Right
            this.AddCubeFace(center, y, vector3D, ylength, xlength, zlength);


            return this.ToMesh();
        }



        private MeshGeometry3D AddBoxBottomFace(Point3D center, Vector3D x, Vector3D y, double xlength, double ylength, double zlength)
        {
            Vector3D vector3D = Text3DFactoryExtensions.CrossProduct(ref x, ref y);
            //Bottom
            this.AddCubeFace(center, -vector3D, y, zlength, xlength, ylength);
            return ToMesh();
        }


        private MeshGeometry3D AddBoxTopFace(Point3D center, Vector3D x, Vector3D y, double xlength, double ylength, double zlength)
        {
            Vector3D vector3D = Text3DFactoryExtensions.CrossProduct(ref x, ref y);
            //Top
            this.AddCubeFace(center, vector3D, y, zlength, xlength, ylength);
            return ToMesh();
        }


        /// <summary>
        /// Adds a cube face.
        /// </summary>
        /// <param name="center">
        /// The center of the cube.
        /// </param>
        /// <param name="normal">
        /// The normal vector for the face.
        /// </param>
        /// <param name="up">
        /// The up vector for the face.
        /// </param>
        /// <param name="dist">
        /// The distance from the center of the cube to the face.
        /// </param>
        /// <param name="width">
        /// The width of the face.
        /// </param>
        /// <param name="height">
        /// The height of the face.
        /// </param>(center, -x, y, zlength, xlength, ylength)
        private MeshGeometry3D AddCubeFace(Point3D center, Vector3D normal, Vector3D up, double dist, double width, double height)
        {
            Vector3D right = Vector3D.CrossProduct(normal, up);
            Vector3D vector2 = normal * dist / 2.0;

            up *= height / 2.0;
            right *= width / 2.0;
            Point3D value = center + vector2 - up - right;
            Point3D value2 = center + vector2 - up + right;
            Point3D value3 = center + vector2 + up + right;
            Point3D value4 = center + vector2 + up - right;
            int count = positions.Count;
            positions.Add(value);
            positions.Add(value2);
            positions.Add(value3);
            positions.Add(value4);
            if (normals != null)
            {
                normals.Add(normal);
                normals.Add(normal);
                normals.Add(normal);
                normals.Add(normal);
            }
            if (textureCoordinates != null)
            {
                textureCoordinates.Add(new Point(1.0, 1.0));
                textureCoordinates.Add(new Point(0.0, 1.0));
                textureCoordinates.Add(new Point(0.0, 0.0));
                textureCoordinates.Add(new Point(1.0, 0.0));
            }
            triangleIndices.Add(count + 2);
            triangleIndices.Add(count + 1);
            triangleIndices.Add(count);
            triangleIndices.Add(count);
            triangleIndices.Add(count + 3);
            triangleIndices.Add(count + 2);

            return this.ToMesh();
        }


        /// <summary>
        /// Create 3D text
        /// </summary>
        /// <param name="textDirection">Vector3D for text direction</param>
        /// <param name="upDirection">Vector3D for text up direction</param>
        /// <param name="centar">Text centar</param>
        /// <param name="text">Text</param>
        /// <param name="fontFamily">Font family</param>
        /// <param name="fontStyle">Font style</param>
        /// <param name="fontWeight">Font weight</param>
        /// <param name="fontSize">Font size</param>
        /// <param name="pixelPerDip">  /// The Pixels Per Density Independent Pixel value, which is the equivalent of the scale factor. 
        /// For example, if the DPI of a screen is 120 (or 1.25 because 120/96 = 1.25) , 1.25 pixel per density independent pixel is drawn </param>
        /// <param name="fontExtruded">How much to extrud text</param>
        /// <returns>Constrocted 3D text mesh (<see cref="MeshGeometry3D"/>)</returns>
        public MeshGeometry3D ConstructTextExtrudedType(Vector3D textDirection, Vector3D upDirection, Point3D centar, string text, string fontFamily,
                                                        FontStyle fontStyle, FontWeight fontWeight, double fontSize, double pixelPerDip, double fontExtruded)
        {
            var fontList = GetTextOutlines(text, fontFamily, fontStyle, fontWeight, fontSize, pixelPerDip);



            var extrudeDistance = fontExtruded / 4;

            // Build the polygon to mesh (using Triangle.NET to triangulate)
            var polygon = new TriangleNet.Geometry.Polygon();
            int marker = 0;

            var p0 = new Point3D(centar.X, centar.Y - extrudeDistance, centar.Z);
            var p1 = new Point3D(centar.X, centar.Y + extrudeDistance, centar.Z);


            foreach (var outlines in fontList)
            {
                var outerOutline = outlines.OrderBy(x => x.AreaOfSegment()).Last();/*.ToSegments();*/

                for (int i = 0; i < outlines.Count; i++)
                {

                    var outline = outlines[i];
                    var isHole = i != outlines.Count - 1 && Text3DFactoryExtensions.IsPointInPolygon(outerOutline.ToList(), outline[0]);
                    polygon.AddContour(outline.Select(p => new TriangleNet.Geometry.Vertex(p.X, p.Y)), marker++, isHole);


                    this.AddExtrudedSegments(outline.ToSegments().ToList(), textDirection, p0, p1);
                }


            }

            var mesher = new TriangleNet.Meshing.GenericMesher();
            var options = new TriangleNet.Meshing.ConstraintOptions();

            var mesh = mesher.Triangulate(polygon, options);



            var u = textDirection;
            u.Normalize();
            var z = p1 - p0;
            z.Normalize();
            var v = Vector3D.CrossProduct(z, u);
            var dist = extrudeDistance * 4 + 0.0001;

            // Convert the triangles
            foreach (var t in mesh.Triangles)
            {
                var v0 = t.GetVertex(0);
                var v1 = t.GetVertex(1);
                var v2 = t.GetVertex(2);


                // Project the X/Y vertices onto a plane defined by textdirection, p0 and p1.                               
                this.AddTriangle(v0.Project(p0, u, v, z, dist), v1.Project(p0, u, v, z, dist), v2.Project(p0, u, v, z, dist));

                this.AddTriangle(v2.Project(p0, u, v, z, 0), v1.Project(p0, u, v, z, 0), v0.Project(p0, u, v, z, 0));
            }

            var textMesh = this.ToMesh();


            return this.ToMesh();
        }

     
 
        /// <summary>
        /// Adds an extruded surface of the specified line segments.
        /// </summary>
        /// <param name="points">The 2D points describing the line segments to extrude. The number of points must be even.</param>
        /// <param name="axisX">The x-axis.</param>
        /// <param name="p0">The start origin of the extruded surface.</param>
        /// <param name="p1">The end origin of the extruded surface.</param>
        /// <remarks>The y-axis is determined by the cross product between the specified x-axis and the p1-origin vector.</remarks>
        public void AddExtrudedSegments(IList<Point> points, Vector3D axisX, /*Vector3D upDir, */Point3D p0, Point3D p1)
        {
            if (points.Count % 2 != 0)
            {
                throw new InvalidOperationException("The number of points should be even.");
            }

            var p10 = p1 - p0;
            var axisY = Text3DFactoryExtensions.CrossProduct(ref axisX, ref p10);

            axisY.Normalize();
            axisX.Normalize();


            int index0 = this.positions.Count;

            for (int i = 0; i < points.Count; i++)
            {
                var point2D = points[i];

                var distance = (axisX * point2D.X) + (axisY * point2D.Y);

                this.positions.Add(p0 + distance);
                this.positions.Add(p1 + distance);
                var r0 = (p0 + distance);
                var r1 = (p1 + distance);
                if (this.normals != null)
                {
                    distance.Normalize();
                    this.normals.Add(distance);
                    this.normals.Add(distance);
                }

                if (this.textureCoordinates != null)
                {
                    var v = (DoubleOrSingle)i / (points.Count - 1);
                    this.textureCoordinates.Add(new Point(0, v));
                    this.textureCoordinates.Add(new Point(1, v));
                }
            }

            int n = points.Count - 1;
            for (int i = 0; i < n; i++)
            {
                int i0 = index0 + (i * 2);
                int i1 = i0 + 1;
                int i2 = i0 + 3;
                int i3 = i0 + 2;

                this.triangleIndices.Add(i0);
                this.triangleIndices.Add(i1);
                this.triangleIndices.Add(i2);

                this.triangleIndices.Add(i2);
                this.triangleIndices.Add(i3);
                this.triangleIndices.Add(i0);
            }
        }
       
         

        /// <summary>
        /// Gets or sets a value indicating whether to create normal vectors.
        /// </summary>
        /// <value>
        /// <c>true</c> if normal vectors should be created; otherwise, <c>false</c>.
        /// </value>
        public bool CreateNormals
        {
            get
            {
                return normals != null;
            }
            set
            {
                if (value && normals == null)
                {
                    normals = new Vector3DCollection();
                }
                if (!value)
                {
                    normals = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to create texture coordinates.
        /// </summary>
        /// <value>
        /// <c>true</c> if texture coordinates should be created; otherwise, <c>false</c>.
        /// </value>
        public bool CreateTextureCoordinates
        {
            get
            {
                return textureCoordinates != null;
            }
            set
            {
                if (value && textureCoordinates == null)
                {
                    textureCoordinates = new PointCollection();
                }
                if (!value)
                {
                    textureCoordinates = null;
                }
            }
        }

      

        /// <summary>
        /// Converts the geometry to a <see cref="MeshGeometry3D"/> .
        /// </summary>
        /// <param name="freeze">
        /// freeze the mesh if set to <c>true</c> .
        /// </param>
        /// <returns>
        /// A mesh geometry.
        /// </returns>
        public MeshGeometry3D ToMesh(bool freeze = false)
        {
            if (this.triangleIndices.Count == 0)
            {
                var emptyGeometry = new MeshGeometry3D();
                if (freeze)
                {
                    emptyGeometry.Freeze();
                }

                return emptyGeometry;
            }

            if (this.normals != null && this.positions.Count != this.normals.Count)
            {
                throw new InvalidOperationException("WrongNumberOfNormals");
            }

            if (this.textureCoordinates != null && this.positions.Count != this.textureCoordinates.Count)
            {
                throw new InvalidOperationException("WrongNumberOfTextureCoordinates");
            }

            var mg = new MeshGeometry3D
            {
                Positions = new Point3DCollection(this.positions),
                TriangleIndices = new Int32Collection(this.triangleIndices)
            };
            if (this.normals != null)
            {
                mg.Normals = new Vector3DCollection(this.normals);
            }

            if (this.textureCoordinates != null)
            {
                mg.TextureCoordinates = new PointCollection(this.textureCoordinates);
            }

            if (freeze)
            {
                mg.Freeze();
            }

            return mg;
        }

 


        /// <summary>
        ///     Get text outlined geometry 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fontName"></param>
        /// <param name="fontStyle"></param>
        /// <param name="fontWeight"></param>
        /// <param name="fontSize"></param>
        /// <param name="pixelPerDIP">The Pixels Per Density Independent Pixel value, which is the equivalent of the scale factor. For example, if the DPI of a screen is 120 (or 1.25 because 120/96 = 1.25) , 1.25 pixel per density independent pixel is drawn</param>
        /// <returns>  Geometry as IEnumerable IList of font like 2D Point array </returns>
        public IEnumerable<IList<Point[]>> GetTextOutlines(string text, string fontName, FontStyle fontStyle, FontWeight fontWeight, double fontSize,double pixelPerDIP)
        {

            var formattedText = new FormattedText(
                text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(new FontFamily(fontName), fontStyle, fontWeight, FontStretches.Normal),
                fontSize,
                Brushes.Black,pixelPerDIP);


            var textGeometry = formattedText.BuildGeometry(new Point(0, 0));
            //Rect bounds = textGeometry.Bounds;
            var FontOutline = new List<List<Point[]>>();
            AppendOutlines(textGeometry, FontOutline);
            return FontOutline;
        }



        internal static void AppendOutlines(Geometry geometry, List<List<Point[]>> outlines)
        {
            var group = geometry as GeometryGroup;
            if (group != null)
            {
                foreach (var g in group.Children)
                {
                    AppendOutlines(g, outlines);
                }

                return;
            }

            var pathGeometry = geometry as PathGeometry;
            if (pathGeometry != null)
            {
                var figures = pathGeometry.Figures.Select(figure => figure.ToPolyLine()).ToList();
                outlines.Add(figures);
                return;
            }

            throw new NotImplementedException();
        }


        /// <summary>
        /// Adds a triangle.
        /// </summary>
        /// <param name="p0">
        /// The first point.
        /// </param>
        /// <param name="p1">
        /// The second point.
        /// </param>
        /// <param name="p2">
        /// The third point.
        /// </param>
        public void AddTriangle(Point3D p0, Point3D p1, Point3D p2)
        {
            var uv0 = new Point(0, 0);
            var uv1 = new Point(1, 0);
            var uv2 = new Point(0, 1);
            this.AddTriangle(p0, p1, p2, uv0, uv1, uv2);
        }



        /// <summary>
        /// Adds a triangle.
        /// </summary>
        /// <param name="p0">
        /// The first point.
        /// </param>
        /// <param name="p1">
        /// The second point.
        /// </param>
        /// <param name="p2">
        /// The third point.
        /// </param>
        /// <param name="uv0">
        /// The first texture coordinate.
        /// </param>
        /// <param name="uv1">
        /// The second texture coordinate.
        /// </param>
        /// <param name="uv2">
        /// The third texture coordinate.
        /// </param>
        public void AddTriangle(Point3D p0, Point3D p1, Point3D p2, Point uv0, Point uv1, Point uv2)
        {
            int i0 = this.positions.Count;

            this.positions.Add(p0);
            this.positions.Add(p1);
            this.positions.Add(p2);

            if (this.textureCoordinates != null)
            {
                this.textureCoordinates.Add(uv0);
                this.textureCoordinates.Add(uv1);
                this.textureCoordinates.Add(uv2);
            }

            if (this.normals != null)
            {
                var p10 = p1 - p0;
                var p20 = p2 - p0;
                var w = Text3DFactoryExtensions.CrossProduct(ref p10, ref p20);
                w.Normalize();
                this.normals.Add(w);
                this.normals.Add(w);
                this.normals.Add(w);
            }

            this.triangleIndices.Add(i0 + 0);
            this.triangleIndices.Add(i0 + 1);
            this.triangleIndices.Add(i0 + 2);
        }
    }
}