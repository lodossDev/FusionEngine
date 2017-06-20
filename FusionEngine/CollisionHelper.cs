using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FusionEngine
{
    public static class CollisionHelper {

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

        public static bool DepthCollision(this Rectangle e1, Rectangle e2) {
            int y1 = e1.Top;
            int y2 = e1.Bottom;

            int h1 = e2.Top;
            int h2 = e2.Bottom;

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

        public static bool DepthCollision(this Rectangle e1, Rectangle e2, float vel) {
            int y1 = e1.Top;
            int y2 = e1.Bottom;

            int h1 = e2.Top;
            int h2 = e2.Bottom;

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

        public static bool DepthCollisionBottom(this Rectangle e1, Rectangle e2) {
            int y1 = e1.Top;
            int y2 = e1.Bottom;

            int h1 = e2.Top;
            int h2 = e2.Bottom;

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

        public static bool DepthCollisionTop(this Rectangle e1, Rectangle e2) {
            int y1 = e1.Top;
            int y2 = e1.Bottom;

            int h1 = e2.Top;
            int h2 = e2.Bottom;

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

        public static bool HorizontalCollisionLeft(this Rectangle e1, Rectangle e2, float offset = 0) {
            int x1 = e1.Left;
            int x2 = e2.Right;

            return (x1 + Math.Round((double)offset) < x2);
        }

        public static bool HorizontalCollisionRight(this Entity e1, Entity e2, float offset = 0) {
            int x1 = e1.GetDepthBox().GetRect().Right;
            int x2 = e2.GetDepthBox().GetRect().Left;

            return (x1 - Math.Round((double)offset) > x2);
        }

        public static bool HorizontalCollisionRight(this Rectangle e1, Rectangle e2, float offset = 0) {
            int x1 = e1.Right;
            int x2 = e2.Left;

            return (x1 - Math.Round((double)offset) > x2);
        }

        public static bool VerticleCollisionTop(this Entity e1, Entity e2, float offset = 0) {
            int z1 = e1.GetDepthBox().GetRect().Bottom;
            int z2 = e2.GetDepthBox().GetRect().Top;

            return (z1 - Math.Round((double)offset) > z2);
        }

        public static bool VerticleCollisionTop(this Rectangle e1, Rectangle e2, float offset = 0) {
            int z1 = e1.Bottom;
            int z2 = e2.Top;

            return (z1 - Math.Round((double)offset) > z2);
        }

        public static bool VerticleCollisionBottom(this Entity e1, Entity e2, float offset = 0) {
            int z1 = e1.GetDepthBox().GetRect().Top;
            int z2 = e2.GetDepthBox().GetRect().Bottom;

            return (z1 + Math.Round((double)offset) < z2);
        }

        public static bool VerticleCollisionBottom(this Rectangle e1, Rectangle e2, float offset = 0) {
            int z1 = e1.Top;
            int z2 = e2.Bottom;

            return (z1 + Math.Round((double)offset) < z2);
        }

        public static bool IsWithinBoundsX1(this Entity e1, Entity e2, float offset = 5) {
            return (e1.HorizontalCollisionLeft(e2, offset) == true && e1.HorizontalCollisionRight(e2, offset) == true);
        }

        public static bool IsWithinBoundsX1(this Rectangle e1, Rectangle e2, float offset = 5) {
            return (e1.HorizontalCollisionLeft(e2, offset) == true && e1.HorizontalCollisionRight(e2, offset) == true);
        }

        public static bool IsWithinBoundsZ1(this Entity e1, Entity e2, float offset = 5) {
            return (e1.VerticleCollisionTop(e2, offset) == true && e1.VerticleCollisionBottom(e2, offset) == true);
        }

        public static bool IsWithinBoundsZ1(this Rectangle e1, Rectangle e2, float offset = 5) {
            return (e1.VerticleCollisionTop(e2, offset) == true && e1.VerticleCollisionBottom(e2, offset) == true);
        }

        public static bool IsWithinBoundsX2(this Entity e1, Entity e2, float offset = 5) {
            return (e1.HorizontalCollisionLeft(e2, offset) == true && e1.HorizontalCollisionRight(e2, offset) == false
                        || e1.HorizontalCollisionLeft(e2, offset) == false && e1.HorizontalCollisionRight(e2, offset) == true);
        }

        public static bool IsWithinBoundsX2(this Rectangle e1, Rectangle e2, float offset = 5) {
            return (e1.HorizontalCollisionLeft(e2, offset) == true && e1.HorizontalCollisionRight(e2, offset) == false
                        || e1.HorizontalCollisionLeft(e2, offset) == false && e1.HorizontalCollisionRight(e2, offset) == true);
        }

        public static bool IsWithinBoundsZ2(this Entity e1, Entity e2, float offset = 5) {
            return (e1.VerticleCollisionTop(e2, offset) == false && e1.VerticleCollisionBottom(e2, offset) == true
                       || e1.VerticleCollisionTop(e2, offset) == true && e1.VerticleCollisionBottom(e2, offset) == false);
        }

        public static bool IsWithinBoundsZ2(this Rectangle e1, Rectangle e2, float offset = 5) {
            return (e1.VerticleCollisionTop(e2, offset) == false && e1.VerticleCollisionBottom(e2, offset) == true
                       || e1.VerticleCollisionTop(e2, offset) == true && e1.VerticleCollisionBottom(e2, offset) == false);
        }
    }
}
