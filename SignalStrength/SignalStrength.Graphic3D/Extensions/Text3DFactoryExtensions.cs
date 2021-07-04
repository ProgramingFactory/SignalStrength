using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using LineSegment = System.Windows.Media.LineSegment;
using Point = System.Windows.Point;

namespace SignalStrength.Graphic3D.Extensions
{
    internal static class Text3DFactoryExtensions
    {

        /// <summary>
        /// Find a <see cref="Vector3D"/> that is perpendicular to the given <see cref="Vector3D"/>.
        /// </summary>
        /// <param name="n">
        /// The input vector.
        /// </param>
        /// <returns>
        /// A perpendicular vector.
        /// </returns>
        public static Vector3D FindAnyPerpendicular(this Vector3D n)
        {
            n.Normalize();
            Vector3D u = Vector3D.CrossProduct(new Vector3D(0, 1, 0), n);
            if (u.LengthSquared < 1e-3)
            {
                u = Vector3D.CrossProduct(new Vector3D(1, 0, 0), n);
            }

            return u;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3D CrossProduct(ref Vector3D first, ref Vector3D second)
        {
            return Vector3D.CrossProduct(first, second);
        }
       
         
        public static IEnumerable<Point> ToSegments(this IList<Point> input)
        {
            bool first = true;
            var previous = default(Point);
            foreach (var point in input)
            {
                if (!first)
                {
                    yield return previous;
                    yield return point;
                }
                else
                {
                    first = false;
                }

                previous = point;
            }
        }




        public static bool IsPointInPolygon(IList<Point> polygon, Point testPoint)
        {
            bool result = false;
            int j = polygon.Count - 1;
            for (int i = 0; i < polygon.Count; i++)
            {
                if ((polygon[i].Y < testPoint.Y && polygon[j].Y >= testPoint.Y) || (polygon[j].Y < testPoint.Y && polygon[i].Y >= testPoint.Y))
                {
                    if (polygon[i].X + ((testPoint.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X)) < testPoint.X)
                    {
                        result = !result;
                    }
                }

                j = i;
            }

            return result;
        }




        public static Point[] ToPolyLine(this PathFigure figure)
        {
            var outline = new List<Point> { figure.StartPoint };
            var previousPoint = figure.StartPoint;
            foreach (var segment in figure.Segments)
            {
                var polyline = segment as PolyLineSegment;
                if (polyline != null)
                {
                    outline.AddRange(polyline.Points);
                    previousPoint = polyline.Points.Last();
                    continue;
                }

                var polybezier = segment as PolyBezierSegment;
                if (polybezier != null)
                {
                    for (int i = -1; i + 3 < polybezier.Points.Count; i += 3)
                    {
                        var p1 = i == -1 ? previousPoint : polybezier.Points[i];
                        outline.AddRange(FlattenBezier(p1, polybezier.Points[i + 1], polybezier.Points[i + 2], polybezier.Points[i + 3], 10));
                    }

                    previousPoint = polybezier.Points.Last();
                    continue;
                }

                var lineSegment = segment as LineSegment;
                if (lineSegment != null)
                {
                    outline.Add(lineSegment.Point);
                    previousPoint = lineSegment.Point;
                    continue;
                }

                var bezierSegment = segment as BezierSegment;
                if (bezierSegment != null)
                {
                    outline.AddRange(FlattenBezier(previousPoint, bezierSegment.Point1, bezierSegment.Point2, bezierSegment.Point3, 10));
                    previousPoint = bezierSegment.Point3;
                    continue;
                }

                throw new NotImplementedException();
            }

            return outline.ToArray();
        }
        public static IEnumerable<Point> FlattenBezier(Point p1, Point p2, Point p3, Point p4, int n)
        {
            // http://tsunami.cis.usouthal.edu/~hain/general/Publications/Bezier/bezier%20cccg04%20paper.pdf
            // http://en.wikipedia.org/wiki/De_Casteljau's_algorithm
            for (int i = 1; i <= n; i++)
            {
                var t = (double)i / n;
                var u = 1 - t;
                yield return new Point(
                    (u * u * u * p1.X) + (3 * t * u * u * p2.X) + (3 * t * t * u * p3.X) + (t * t * t * p4.X),
                    (u * u * u * p1.Y) + (3 * t * u * u * p2.Y) + (3 * t * t * u * p3.Y) + (t * t * t * p4.Y));
            }
        }



        public static double AreaOfSegment(this Point[] segment)
        {
            return Math.Abs(segment.Take(segment.Length - 1)
                           .Select((p, i) => (segment[i + 1].X - p.X) * (segment[i + 1].Y + p.Y))
                           .Sum() / 2);
        }



        // project triangle.net vertex
        public static Point3D Project(this TriangleNet.Geometry.Vertex v, Point3D p0, Vector3D x, Vector3D y, Vector3D z, double h)
        {
            return p0 + (x * v.X) - (y * v.Y) + (z * (h / 2));
        }

      
    }
}


