using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FusionEngine
{
    public static class CollisionHelper {

        public class PointSort : IComparer<Point> 
        { 
            public enum Mode 
            { 
                X, 
                Y 
            } 
 
            Mode currentMode = Mode.X; 
 
            public PointSort(Mode mode) 
            { 
                currentMode = mode; 
            } 
 
 
            //Comparing function 
            //Returns one of three values - 0 (equal), 1 (greater than), 2 (less than) 
            public int Compare(Point a, Point b) 
            { 
                Point point1 = (Point)a; 
                Point point2 = (Point)b; 
 
                if (currentMode == Mode.X) //Compare X values 
                { 
                    if (point1.X > point2.X) 
                        return 1; 
                    else if (point1.X < point2.X) 
                        return -1; 
                    else 
                        return 0; 
                } 
                else 
                { 
                    if (point1.Y > point2.Y) //Compare Y Values 
                        return 1; 
                    else if (point1.Y < point2.Y) 
                        return -1; 
                    else 
                        return 0; 
                } 
            } 
        } 

        public struct Ray2D 
        { 
            private Vector2 startPos; 
            private Vector2 endPos; 
            private readonly List<Point> result; 
 
            public Ray2D(Vector2 startPos, Vector2 endPos) 
            { 
                this.startPos = startPos; 
                this.endPos = endPos; 
                result = new List<Point>(); 
            } 
 
            /// <summary>  
            /// Determine if the ray intersects the rectangle  
            /// </summary>  
            /// <param name="rectangle">Rectangle to check</param>  
            /// <returns></returns>  
            public Vector2 Intersects(Rectangle rectangle) 
            { 
                Point p0 = new Point((int)startPos.X, (int)startPos.Y); 
                Point p1 = new Point((int)endPos.X, (int)endPos.Y); 
 
                foreach (Point testPoint in BresenhamLineSorted(p0, p1)) 
                { 
                    if (rectangle.Contains(testPoint)) 
                        return new Vector2((float)testPoint.X, (float)testPoint.Y); 
                } 
 
                return Vector2.Zero; 
            } 
 
            // Swap the values of A and B  
 
            private void Swap<T>(ref T a, ref T b) 
            { 
                T c = a; 
                a = b; 
                b = c; 
            } 
 
 
            private List<Point> BresenhamLineSorted(Point startPoint, Point endPoint) 
            { 
                List<Point> points = BresenhamLine(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y); 
 
                bool steep = IsSteep(startPoint, endPoint); 
 
                if(steep) 
                { 
                    points.Sort(new PointSort(PointSort.Mode.Y)); 
 
                    if(startPoint.Y > endPoint.Y) 
                    { 
                        points.Reverse(); 
                    } 
                } 
                else 
                { 
                    points.Sort(new PointSort(PointSort.Mode.Y)); 
 
                    if(startPoint.X > endPoint.X) 
                    { 
                        points.Reverse(); 
                    } 
                } 
 
                return points; 
            } 
 
 
            private bool IsSteep(Point startPoint, Point endPoint) 
            { 
                return IsSteep(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y); 
            } 
 
 
            private bool IsSteep(int x0, int y0, int x1, int y1) 
            { 
                return Math.Abs(y1 - y0) > Math.Abs(x1 - x0); 
            } 
 
 
            // Returns the list of points from (x0, y0) to (x1, y1)  
            private List<Point> BresenhamLine(int x0, int y0, int x1, int y1) 
            { 
                // Optimization: it would be preferable to calculate in  
                // advance the size of "result" and to use a fixed-size array  
                // instead of a list.  
 
                result.Clear(); 
 
                bool steep = IsSteep(x0, y0, x1, y1); 
                if (steep) 
                { 
                    Swap(ref x0, ref y0); 
                    Swap(ref x1, ref y1); 
                } 
                if (x0 > x1) 
                { 
                    Swap(ref x0, ref x1); 
                    Swap(ref y0, ref y1); 
                } 
 
                int deltax = x1 - x0; 
                int deltay = Math.Abs(y1 - y0); 
                int error = 0; 
                int ystep; 
                int y = y0; 
                if (y0 < y1) ystep = 1; else ystep = -1; 
                for (int x = x0; x <= x1; x++) 
                { 
                    if (steep) result.Add(new Point(y, x)); 
                    else result.Add(new Point(x, y)); 
                    error += deltay; 
                    if (2 * error >= deltax) 
                    { 
                        y += ystep; 
                        error -= deltax; 
                    } 
                } 
 
                return result; 
            } 
        }  

        /// <summary>
        /// Calculates the signed depth of intersection between two rectangles.
        /// </summary>
        /// <returns>
        /// The amount of overlap between two intersecting rectangles. These
        /// depth values can be negative depending on which sides the rectangles
        /// intersect. This allows callers to determine the correct direction
        /// to push objects in order to resolve collisions.
        /// If the rectangles are not intersecting, Vector2.Zero is returned.
        /// </returns>
        public static Vector2 GetIntersectionDepth(this Rectangle rectA, Rectangle rectB) {
            // Calculate half sizes.
            float halfWidthA = rectA.Width / 2.0f;
            float halfHeightA = rectA.Height / 2.0f;
            float halfWidthB = rectB.Width / 2.0f;
            float halfHeightB = rectB.Height / 2.0f;

            // Calculate centers.
            Vector2 centerA = new Vector2(rectA.Left + halfWidthA, rectA.Top + halfHeightA);
            Vector2 centerB = new Vector2(rectB.Left + halfWidthB, rectB.Top + halfHeightB);

            // Calculate current and minimum-non-intersecting distances between centers.
            float distanceX = centerA.X - centerB.X;
            float distanceY = centerA.Y - centerB.Y;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;

            // If we are not intersecting at all, return (0, 0).
            if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
                return Vector2.Zero;

            // Calculate and return intersection depths.
            float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
            return new Vector2(depthX, depthY);
        }

        /// <summary>
        /// Gets the position of the center of the bottom edge of the rectangle.
        /// </summary>
        public static Vector2 GetBottomCenter(this Rectangle rect) {
            return new Vector2(rect.X + rect.Width / 2.0f, rect.Bottom);
        }

        public static float GetHorizontalIntersectionDepth(this Rectangle rectA, Rectangle rectB) {
            // Calculate half sizes.
            float halfWidthA = rectA.Width / 2.0f;
            float halfWidthB = rectB.Width / 2.0f;

            // Calculate centers.
            float centerA = rectA.Left + halfWidthA;
            float centerB = rectB.Left + halfWidthB;

            // Calculate current and minimum-non-intersecting distances between centers.
            float distanceX = centerA - centerB;
            float minDistanceX = halfWidthA + halfWidthB;

            // If we are not intersecting at all, return (0, 0).
            if (Math.Abs(distanceX) >= minDistanceX)
                return 0f;

            // Calculate and return intersection depths.
            return distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
        }

        public static float GetVerticalIntersectionDepth(this Rectangle rectA, Rectangle rectB) {
            // Calculate half sizes.
            float halfHeightA = rectA.Height / 2.0f;
            float halfHeightB = rectB.Height / 2.0f;

            // Calculate centers.
            float centerA = rectA.Top + halfHeightA;
            float centerB = rectB.Top + halfHeightB;

            // Calculate current and minimum-non-intersecting distances between centers.
            float distanceY = centerA - centerB;
            float minDistanceY = halfHeightA + halfHeightB;

            // If we are not intersecting at all, return (0, 0).
            if (Math.Abs(distanceY) >= minDistanceY)
                return 0f;

            // Calculate and return intersection depths.
            return distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
        }

        public static float GetDiff(float e1, float e2) {
            return Math.Abs(e1 - e2);
        }

        public static bool DepthCollision(this Entity e1, Entity e2) {
            int y1 = e1.GetDepthBox().GetRect().Top;
            int y2 = e1.GetDepthBox().GetRect().Bottom;

            int h1 = e2.GetDepthBox().GetRect().Top;
            int h2 = e2.GetDepthBox().GetRect().Bottom;

            // check if the first y value of the hero is in between both
            // of the villain's y values
            if( y1 >= h1 && y1 <= h2) { 
                return true;
            }

            // check if the second y value of the hero is in between both
            // of the villain's y values
            if(y2 >= h1 && y2 <= h2) { 
                return true;
            }

            return false;
        }

        public static bool DepthCollision(this Entity e1, Entity e2, float vel) {
            int y1 = e1.GetDepthBox().GetRect().Top;
            int y2 = e1.GetDepthBox().GetRect().Bottom;

            int h1 = e2.GetDepthBox().GetRect().Top;
            int h2 = e2.GetDepthBox().GetRect().Bottom;

            // check if the first y value of the hero is in between both
            // of the villain's y values
            if( y1 + vel >= h1 && y1 - vel <= h2) { 
                return true;
            }

            // check if the second y value of the hero is in between both
            // of the villain's y values
            if(y2 + vel >= h1 && y2 - vel <= h2) { 
                return true;
            }

            return false;
        }

        public static bool DepthCollisionBottom(this Entity e1, Entity e2) {
            int y1 = e1.GetDepthBox().GetRect().Top;
            int y2 = e1.GetDepthBox().GetRect().Bottom;

            int h1 = e2.GetDepthBox().GetRect().Top;
            int h2 = e2.GetDepthBox().GetRect().Bottom;

            // check if the first y value of the hero is in between both
            // of the villain's y values
            if( y1 >= h1 && y1 <= h2) { 
                return true;
            }
            
            return false;
        }

        public static bool DepthCollisionTop(this Entity e1, Entity e2) {
            int y1 = e1.GetDepthBox().GetRect().Top;
            int y2 = e1.GetDepthBox().GetRect().Bottom;

            int h1 = e2.GetDepthBox().GetRect().Top;
            int h2 = e2.GetDepthBox().GetRect().Bottom;

            // check if the second y value of the hero is in between both
            // of the villain's y values
            if(y2 >= h1 && y2 <= h2) { 
                return true;
            }

            return false;
        }

        public static bool HorizontalCollisionLeft(this Entity e1, Entity e2, float offset = 0) {
            int x1 = e1.GetDepthBox().GetRect().Left;
            int x2 = e2.GetDepthBox().GetRect().Right;

            return (x1 + Math.Round((double)offset) < x2);
        }

        public static bool HorizontalCollisionRight(this Entity e1, Entity e2, float offset = 0) {
            int x1 = e1.GetDepthBox().GetRect().Right;
            int x2 = e2.GetDepthBox().GetRect().Left;

            return (x1 - Math.Round((double)offset) > x2);
        }

        public static bool VerticleCollisionTop(this Entity e1, Entity e2, float offset = 0) {
            int z1 = e1.GetDepthBox().GetRect().Bottom;
            int z2 = e2.GetDepthBox().GetRect().Top;

            return (z1 - Math.Round((double)offset) > z2);
        }

        public static bool VerticleCollisionBottom(this Entity e1, Entity e2, float offset = 0) {
            int z1 = e1.GetDepthBox().GetRect().Top;
            int z2 = e2.GetDepthBox().GetRect().Bottom;

            return (z1 + Math.Round((double)offset) < z2);
        }

        public static bool IsWithinBoundsX(this Entity e1, Entity e2, float offset = 5) {
            return (e1.HorizontalCollisionLeft(e2, offset) == true && e1.HorizontalCollisionRight(e2, offset) == true);
        }

        public static bool IsWithinBoundsZ(this Entity e1, Entity e2, float offset = 5) {
            return (e1.VerticleCollisionTop(e2, offset) == true && e1.VerticleCollisionBottom(e2, offset) == true);
        }
    }
}
