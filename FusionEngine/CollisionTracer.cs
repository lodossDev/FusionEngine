using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {
    
/*  exampleMethod()
           {
            Vector2 c = new Vector2(350, 350);  
            Vector2 cw = new Vector2(100,  0);
            Vector2 ch = new Vector2( 0, 100);
            float radrotzspeed = .0001f;
            //
            float radrotz = radrotz + radrotzspeed; 
            if (radrotz > Math.PI * 2) { radrotz = 0.0f; } // ok here well update our rotation just increment it
            //
            // MP is a collision point to check could be any point of anything a rectangle player center ect...
            //
            MP = new Vector2(100, 300); // you could use the mouse to test it here
            //
            r = new Rectangle((int)(c.X - cw.X), (int)(c.Y - ch.Y), (int)(2 * cw.X), (int)(2 * ch.Y));
            if (CollisionAlgorithimFundamentals.isPointWithinSquare(r, MP, radrotz, true))
            {
                   // this square can be rotated with the image you can make a bounding box to part of the image instead if you wished and rotate it
            }
            // or we can do individual points example #2
            //
            // well rotate our original points the lil bit of vector addition and subtraction will place the originals then well rotate them
            // note we can have as many points as we like but must have at least 3 for proper inbounds check of a object and
            // remember that you must keep them all CONVEX OR 
            // you must seperate your bounding object into more then one bounding check
            Vector2 A = CollisionAlgorithimFundamentals.rotate2dPointAboutOriginOnZaxisXna(c - cw - ch, c, radrotz); // top left
            Vector2 B = CollisionAlgorithimFundamentals.rotate2dPointAboutOriginOnZaxisXna(c + cw - ch, c, radrotz); // top right
            Vector2 C = CollisionAlgorithimFundamentals.rotate2dPointAboutOriginOnZaxisXna(c + cw + ch, c, radrotz); // bottom right
            Vector2 D = CollisionAlgorithimFundamentals.rotate2dPointAboutOriginOnZaxisXna(c - cw + ch, c, radrotz); // bottom left         
            //
            // proceed to calculate and tally up all the checks on the planes
            int numberofplanescrossed = 0;
            if (CollisionAlgorithimFundamentals.hasPointCrossedLine2d(A, B, MP)) { numberofplanescrossed++; }
            if (CollisionAlgorithimFundamentals.hasPointCrossedLine2d(B, C, MP)) { numberofplanescrossed++; }
            if (CollisionAlgorithimFundamentals.hasPointCrossedLine2d(C, D, MP)) { numberofplanescrossed++; }
            if (CollisionAlgorithimFundamentals.hasPointCrossedLine2d(D, A, MP)) { numberofplanescrossed++; }
            if (numberofplanescrossed >= 4) 
            { 
                // linecolor = Color.Red; // inside
            } 
            else { 
                // linecolor = Color.Blue; // outside
            }
           }
         */ 
    /// <summary>  
    //////////////////////////////////////////////////////////////////////////////////////// 
    /// Original Author William Motill dec 03-04 2011                                  ///// 
    //////////////////////////////////////////////////////////////////////////////////////// 
    /// written to show practical application of bounds checking                       ///// 
    /// lighting and backface via normals plane and cross normals using dot products   ///// 
    /// feel free to use this as you like                                              ///// 
    //////////////////////////////////////////////////////////////////////////////////////// 
    /// </summary>  
    public static class CollisionTracer
    { 
        public const float      PIE = (float)(Math.PI); 
        public const float      PIE2 = (float)(Math.PI * 2.0d); 
        public const float      PIEHALF = (float)(Math.PI * .5f); 
        public const float      ONEDEGREEOFRADIANS = PIE2 / 360.0f; 
        public const float      ONERADIANOFDEGREES = 360.0f / PIE2; 
        public const int        ZAXIS = 0; 
        public const int        YAXIS = 1; 
        public const int        XAXIS = 2; 
         
        // these two simple collision checks dont use plane checking just simple rectangle checks nothing fancy 
        // 
        /// <summary> 
        /// is x and y within a non rotating rectangle 
        /// </summary> 
        /// <param name="x"></param> 
        /// <param name="y"></param> 
        /// <param name="r"></param> 
        /// <returns></returns> 
        public static bool      isXyPositionWithinRectangle(int x, int y, Rectangle r) 
        { 
            if (x > r.X && x < r.X + r.Width && y > r.Y && y < r.Y + r.Height) 
            { return true; } 
            else 
            { return false; } 
        } 
        /// <summary> 
        /// simply test to see if two non rotating rectangles intersect each other 
        /// </summary> 
        /// <param name="A"></param> 
        /// <param name="C"></param> 
        /// <returns></returns> 
        public static bool      isArectangleWithinCollisionRectangle(Rectangle A, Rectangle C) 
        { 
            bool result = false; 
            bool xin = false; bool yin = false; 
            if (A.Left > C.Left && A.Left < C.Right) { xin = true; } 
            if (A.Right < C.Right && A.Right > C.Left) { xin = true; } 
            if (A.Top > C.Top && A.Top < C.Bottom) { yin = true; } 
            if (A.Bottom < C.Bottom && A.Bottom > C.Top) { yin = true; } 
            // 
            if (xin == true && yin) { result = true; } 
            return result; 
        } 
 
        // the below methods allow for a point to plane type collision check to be performed in 2d upon a rotated rectangle triangle or polygon 
        // 
        /// <summary> 
        /// this simple 2d method can be called  
        /// on a line the line Does'nt need to be vertical or horizontal it can be any direction 
        /// however the order you pass the points determines how the check will find which side of the line the point is on 
        /// useful for a triangle a square any 2d shape or even to simply do exclusion checking on screen ie a dividing line down the center 
        ///  
        /// used to find if a point is on one side of a line or the other  
        /// even when rotation is applyed to that line 
        /// this method works useing cross or plane generated  
        /// unit or cubic normals and the dot product to find the magnitude of differences 
        /// </summary> 
        /// <param name="start">Lines first point</param> 
        /// <param name="end">Lines second point</param> 
        /// <param name="collision_point">collision point to check against</param> 
        /// <returns>is in on the outer or inner side of the line yes or no</returns> 
        public static bool      hasPointCrossedLine2d(Vector2 start, Vector2 end, Vector2 collision_point) 
        { 
            // get the vector normal of the collision point to a point on the planes surface ie line point, or vertice 
            // and dot against the plane of the lines surfaces normal 
            if (  getDotProduct(crossProductVector2dAtan2Xna(start, end),atan2XnaNormalized(collision_point - start)) < 0.0f) 
            { return true; } 
            else 
            { return false; } 
        } 
        /// <summary> 
        /// is A point within a 2d triangle  
        /// the order you pass the points is significant if the bool is set to off 
        /// typically ABC will not yeild the same results as CBA ie this method is not cumulative 
        /// the order you pass should be clockwise or counter clockwise 
        /// if however you set reversewindingalsotrue it should yeild a true correctly as well for cba or abc 
        /// </summary> 
        /// <param name="A">top point</param> 
        /// <param name="B">right point</param> 
        /// <param name="C">bottom left point</param> 
        /// <param name="collision_point">point to check is within the triangle</param> 
        /// <param name="reversewindingalsotrue">allow reverse winding check</param> 
        /// <returns>returns the result of the bounding check true or false</returns> 
        public static bool      isPointWithinTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 collision_point,bool reversewindingalsotrue) 
        { 
            int numberofplanescrossed = 0; 
            if (hasPointCrossedLine2d(A, B, collision_point)) { numberofplanescrossed++; } else { numberofplanescrossed--; } 
            if (hasPointCrossedLine2d(B, C, collision_point)) { numberofplanescrossed++; } else { numberofplanescrossed--; } 
            if (hasPointCrossedLine2d(C, A, collision_point)) { numberofplanescrossed++; } else { numberofplanescrossed--; } 
            if (numberofplanescrossed >= 3 || (reversewindingalsotrue && numberofplanescrossed <= -3)) 
            {return true;} 
            else 
            {return false;} 
        }       
        /// <summary> 
        /// is a point within  
        /// a 2d rectangle that has possibly been rotated or is not normally shaped 
        /// overload allows for a rectangle to be rotated then to check on the specified collision point 
        /// additionally it automatically compensates for xna's texture offset since it knows the centerpoint 
        /// ... 
        /// the order you pass the points is significant if the bool is set to off 
        /// typically ABCD will not yeild the same results as DCBA ie this method is not cumulative 
        /// the order you pass should be clockwise or counter clockwise 
        /// if however you set reversewindingalsotrue it should yeild a true correctly as well for dcba or abcd 
        ///  ... 
        /// points must be inputed in clockwise or counter clockwise in order or circularly around 
        /// points of the polygon cant be inputed in random order or a false result will occur 
        /// .. 
        /// the polygon must be convex if you dont know what that means wiki it 
        /// as it is essential for concave objects divide your bounding object into two or more convex polygons 
        /// </summary> 
        /// <param name="A">example if not rotated this would be top left</param> 
        /// <param name="B">top right</param> 
        /// <param name="C">bottom right</param> 
        /// <param name="D">bottom left</param> 
        /// <param name="collision_point"></param> 
        /// <param name="reversewindingalsotrue"> 
        /// this allows for true to return if all point checks fall outside of the plane  
        /// ie the winding is reversed</param> 
        /// <returns>true on all similar signed planes to point</returns> 
        public static bool      isPointWithinSquare(Rectangle r, Vector2 collision_point, float rectangle_rotation,bool reversewindingalsotrue) 
        { 
            float q = rectangle_rotation; 
            Vector2 A = new Vector2(r.Left,r.Top); 
            Vector2 B = new Vector2(r.Right,r.Top); 
            Vector2 C = new Vector2(r.Right,r.Bottom); 
            Vector2 D = new Vector2(r.Left, r.Bottom); 
            Vector2 o = new Vector2(r.Center.X,r.Center.Y) ; 
            A = rotate2dPointAboutOriginOnZaxisXna(A, o, q); 
            B = rotate2dPointAboutOriginOnZaxisXna(B, o, q); 
            C = rotate2dPointAboutOriginOnZaxisXna(C, o, q); 
            D = rotate2dPointAboutOriginOnZaxisXna(D, o, q); 
            return isPointWithinSquare(A, B, C, D, collision_point, reversewindingalsotrue); 
        } 
        /// <summary> 
        /// is a point within  
        /// a 2d rectangle that has possibly been rotated or is not normally shaped 
        /// ... 
        /// the order you pass the points is significant if the bool is set to off 
        /// typically ABCD will not yeild the same results as DCBA ie this method is not cumulative 
        /// the order you pass should be clockwise or counter clockwise 
        /// if however you set reversewindingalsotrue it should yeild a true correctly as well for dcba or abcd 
        ///  ... 
        /// points must be inputed in clockwise or counter clockwise in order or circularly around 
        /// points of the polygon cant be inputed in random order or a false result will occur 
        /// .. 
        /// the polygon must be convex if you dont know what that means wiki it 
        /// as it is essential for concave objects divide your bounding object into two or more convex polygons 
        /// </summary> 
        /// <param name="A">example if not rotated this would be top left</param> 
        /// <param name="B">top right</param> 
        /// <param name="C">bottom right</param> 
        /// <param name="D">bottom left</param> 
        /// <param name="collision_point"></param> 
        /// <param name="reversewindingalsotrue"> 
        /// this allows for true to return if all point checks fall outside of the plane  
        /// ie the winding is reversed</param> 
        /// <returns>true on all similar signed planes to point</returns> 
        public static bool      isPointWithinSquare(Vector2 A, Vector2 B, Vector2 C, Vector2 D, Vector2 collision_point, bool reversewindingalsotrue) 
        { 
            int numberofplanescrossed = 0; 
            if (hasPointCrossedLine2d(A, B, collision_point)) { numberofplanescrossed++; } else { numberofplanescrossed--; } 
            if (hasPointCrossedLine2d(B, C, collision_point)) { numberofplanescrossed++; } else { numberofplanescrossed--; } 
            if (hasPointCrossedLine2d(C, D, collision_point)) { numberofplanescrossed++; } else { numberofplanescrossed--; } 
            if (hasPointCrossedLine2d(D, A, collision_point)) { numberofplanescrossed++; } else { numberofplanescrossed--; } 
            if (numberofplanescrossed >= 4 || (reversewindingalsotrue && numberofplanescrossed <= -4)) 
            { return true; } 
            else 
            { return false; } 
        } 
        /// <summary> 
        /// is point within polygon 
        /// ... 
        /// points must be inputed in clockwise or counter clockwise in order or circularly around 
        /// points of the polygon cant be inputed in random order or a false result will occur 
        /// .. 
        /// the polygon must be convex if you dont know what that means wiki it 
        /// as it is essential for concave objects divide your bounding object into two or more convex polygons 
        /// </summary> 
        /// <param name="collision_point"></param> 
        /// <param name="reversewindingalsotrue"></param> 
        /// <param name="poly"></param> 
        /// <returns></returns> 
        public static bool      isPointWithinPolygon(bool reversewindingalsotrue,Vector2 collision_point, params Vector2[] poly) 
        { 
            int numberofplanescrossed = 0; 
            for (int i = 0; i < (poly.Length - 1); i++) 
            { 
                if (hasPointCrossedLine2d(poly[i], poly[i+1], collision_point)) { numberofplanescrossed++; } else { numberofplanescrossed--; } 
            } 
            if (hasPointCrossedLine2d(poly[poly.Length - 1], poly[0], collision_point)) { numberofplanescrossed++; } else { numberofplanescrossed--; } 
            // 
            if (numberofplanescrossed >= poly.Length || (reversewindingalsotrue && numberofplanescrossed <= poly.Length)) 
            { return true; } 
            else 
            { return false; } 
        } 
 
 
        // convert your single dimention array or list to or from 2d coordinate positions to 1d or for tile maps this is good too 
        // 
        /// <summary> 
        /// gets a 1d index from a 2d x,y position in relation to a mapping  
        /// ... 
        /// you can set the bool  
        /// (true) specifies that x is the stride 
        /// (false) specifys that y is the stride 
        ///  
        /// note 
        /// this could but is not normaly be used for a quick initial collision exclusion check to sectorize objects 
        /// if you know the maximum collision area of the biggest object  
        /// multiply that by 2 and get a index to a collision sector think about it 
        ///  
        /// </summary> 
        /// <param name="x">x index position to a mapping</param> 
        /// <param name="y">y index position to a mapping</param> 
        /// <param name="things_width">mapping width</param> 
        /// <param name="things_height">mapping height</param> 
        /// <param name="iswidth_used_for_stride"> 
        /// is stride by x (which is standard)  
        /// use the same value for all your fucntions recomended 
        /// </param> 
        /// <returns>a single dimentional index for example to a single dimentional array</returns> 
        public static int       getIndexFromXY(int x, int y, int the_width, int the_height, bool iswidth_used_for_stride) 
        { 
            if (iswidth_used_for_stride == true) 
            { 
                return x + y * the_width; 
            } 
            else 
            { 
                return y + x * the_height; 
            } 
        } 
        /// <summary> 
        /// returns a vector2d representation of  
        /// a 2d x,y positional index from a 1d index  
        /// ... 
        /// example 
        /// you have a 1d Array of ints in memory like a image representing a 2d image 
        /// if you know The Width and Height of the image or a tile map 
        /// you can find with this method 
        /// by a single index the 2dimentional position of the object in image x,y 
        /// the bool allows for you to  
        /// base the your returned index off the images stride in either 
        /// width (true which is the normal way) or off height (false) 
        /// </summary> 
        /// <param name="sindex"> the index into the 1d array</param> 
        /// <param name="things_width">mapping width</param> 
        /// <param name="things_height">mapping height</param> 
        /// <param name="iswidth_used_for_stride">is stride by x left to right(which is standard) or up to down</param> 
        /// <returns>return a x and y position from a one dimentional index representing a logical mapping</returns> 
        public static Vector2   getXyFromIndex(int sindex, int the_width, int the_height, bool iswidth_used_for_stride) 
        { 
            int x = 0; int y = 0; 
            if (iswidth_used_for_stride == true) 
            { 
                if (sindex >= the_width) 
                { 
                    y = sindex / the_width; 
                    x = sindex - (y * the_width); 
                } 
                else 
                { 
                    y = 0; x = sindex; 
                } 
 
            } 
            else 
            { 
                if (sindex >= the_height) 
                { 
                    x = sindex / the_height; 
                    y = sindex - (x * the_height); 
                } 
                else 
                { 
                    x = 0; y = sindex; 
                } 
            } 
            return new Vector2(x, y); 
        } 
 
        /// <summary> 
        /// simply pythagorean distance find 
        /// Between 2 points this is both a circular 
        /// finding the distance between two points is naturally a bounding check  
        /// if distance between a point at the center of your object is very close to a checked point  
        /// its obviously inside your object as well 
        ///  
        /// or can be extendended to a spherical bounding check 
        /// the 3d sphereical check is simply 
        /// difference  x,y,z = start(x',y',z') - end(x'',y'',z'')  
        ///        _______________ 
        ///  d = \/x^2 + y^2 + z^2 
        /// </summary> 
        /// <param name="x"></param> 
        /// <param name="y"></param> 
        /// <param name="x2"></param> 
        /// <param name="y2"></param> 
        public static int       getDistance(int x, int y, int x2, int y2) 
        { 
            // a^2 + b^2 = c^2 then  squroot(c) = dist heres pythagorean amazes me.. they thought about this stuff so long ago 
            int dx = Math.Abs(x - x2); 
            int dy = Math.Abs(y - y2); 
            int asqr = dx * dx; 
            int bsqr = dy * dy; 
            int csqr = asqr + bsqr; 
 
            return (int)(Math.Sqrt(csqr)); 
        } 
        /// <summary> 
        /// simply pythagorean distance find 
        /// </summary> 
        /// <param name="x"></param> 
        /// <param name="y"></param> 
        /// <returns></returns> 
        public static int       getDistance(int x, int y) 
        { 
            // a^2 + b^2 = c^2 then  squroot(c) = dist 
            int asqr = x * x; 
            int bsqr = y * y; 
            int csqr = asqr + bsqr; 
 
            return (int)(Math.Sqrt(csqr)); 
        } 
        /// <summary> 
        /// this is just standard 
        /// </summary> 
        /// <param name="deg"></param> 
        /// <returns>degrees from rads</returns> 
        public static float     getRadiansFromDegrees(float deg) 
        { 
            if (deg > 360.0f) { deg = deg - 360.0f; } 
            if (deg < 0.0f) { deg = deg + 360.0f; } 
            return (deg * ONEDEGREEOFRADIANS); 
        } 
        /// <summary> 
        /// this is just standard 
        /// </summary> 
        /// <param name="rads"></param> 
        /// <returns>rads in degrees</returns> 
        public static float     getDegreesFromRadians(float rads) 
        { 
            if (rads > PIE2) { rads = rads - PIE2; } 
            if (rads < 0.0f) { rads = rads + PIE2; } 
            return (rads * ONERADIANOFDEGREES); 
        } 
        /// <summary> 
        ///  get the radians from degrees ie 0 degrees points north or up 
        ///  for xna radians under the spritedraws 0'degrees is down thus this adds automatically 180'degrees 
        /// </summary> 
        /// <param name="deg"></param> 
        /// <returns>Inversed direction rads or rads + pie</returns> 
        public static float     getInvertedRadiansFromDegrees(float deg) 
        { 
            float uprads = (deg * ONEDEGREEOFRADIANS) + PIE; 
            if (uprads > PIE2) 
            { 
                uprads = uprads - PIE2; 
            } 
            return uprads; 
        } 
 
        /// <summary> 
        ///  this is just Math.atan2 
        ///  extra info  
        ///  math Atan2 allows you to get a angle for the difference of two points based  
        ///  on the slope of y/x and the sign of x really you can write one your self the slope is a tangent 
        ///  to .5 * pie within the 4 signed/unsigned coordinate systems 
        /// </summary> 
        /// <param name="difx"></param> 
        /// <param name="dify"></param> 
        /// <returns></returns> 
        public static float     atan2Xna(float difx, float dify) 
        { 
            return atan2Xna(new Vector2(dify, difx)); 
        } 
        /// <summary> 
        ///  this is just Math.atan2 
        ///  ah math Atan2 allows you to get a angle for the difference of two points based on the slope of y/x and the sign of x 
        /// </summary> 
        /// <param name="difx"></param> 
        /// <param name="dify"></param> 
        /// <returns></returns> 
        public static float     atan2Xna(Vector2 dif) 
        {    
            return (float)Math.Atan2(dif.Y, dif.X) - (PIEHALF * 1.0f); 
        } 
        /// <summary> 
        /// ToDo not yet finished i gotta finish this when i got time to test it to ensure it works  
        /// in theory only now 
        /// built on atan2 with a extra dimention to make it atan3  
        /// since i guess thiers no such thing as atan3 i cant find one 
        /// then i gota make a 2 vector atan 3 which is trivial once i have the atan3 
        /// really should'nt be to hard to do just a extra dimention to the atan2 
        /// </summary> 
        /// <param name="difx"></param> 
        /// <param name="dify"></param> 
        /// <returns></returns> 
        public static float atan3Xna(Vector3 dif) 
        { 
            float q = 0.0f; 
            float z = (float)Math.Atan2(dif.Z, dif.X) - (PIEHALF * 1.0f); 
            float y = (float)Math.Atan2(dif.Y, dif.X) - (PIEHALF * 1.0f); 
 
            // i surmize that 
            // were z = 0 y = 1 x = 0 = 2pie  
            // + z must start at 2pie regardless the irrational  
            // theirfore z = 0 = pie2 *0 and z = as a infantly small number then  
            // q = (atan2(y) ) + (z * pie2 * pie2 + 2pie)// and we must adjust for z -0 or z +0 on the change from irrational to rational 0 or 1 
            // y + PIE2  
            // now i just have to code it out  urggg i hate typing i need secretary or hal 2000 
            return q; 
        } 
        /// <summary> 
        ///  this is just Math.atan2 
        ///  ah math Atan2 allows you to get a angle for the difference of two points based on the slope of y/x and the sign of x 
        ///  now here its just returning a vector of that angle's sin cosine 
        /// </summary> 
        /// <param name="difx"></param> 
        /// <param name="dify"></param> 
        /// <returns></returns> 
        public static Vector2   atan2XnaNormalized(Vector2 dif) 
        { 
            double q = Math.Atan2(dif.Y, dif.X) - (PIEHALF * 1.0f); 
            return new Vector2((float)Math.Sin(q), (float)Math.Cos(q)); 
        } 
        /// <summary> 
        ///  this is just Math.atan2 + half a pie to get a right angle 
        ///  i actually just get a radian angle instead of the cross product here its quicker 
        /// </summary> 
        /// <param name="difx"></param> 
        /// <param name="dify"></param> 
        /// <returns></returns> 
        public static float     crossProductAngle2dAtan2Xna(Vector2 s, Vector2 e) 
        { 
            return (float)Math.Atan2(e.Y - s.Y, e.X - s.X) - (PIEHALF * 2.0f); 
        } 
        /// <summary> 
        ///  this is just Math.atan2 + half a pie to get a right angle 
        ///  i actually just get a radian angle instead of the cross product here its quicker 
        /// </summary> 
        /// <param name="difx"></param> 
        /// <param name="dify"></param> 
        /// <returns></returns> 
        public static Vector2   crossProductVector2dAtan2Xna(Vector2 s, Vector2 e) 
        { 
            float npq= (float)Math.Atan2(e.Y - s.Y, e.X - s.X) - (PIEHALF * 2.0f); 
            return new Vector2((float)Math.Sin(npq), (float)Math.Cos(npq));  
        } 
        /// <summary> 
        /// dot product of two points start(x,y) and end(x,y) 
        /// you should normalize the points before passing them  
        /// </summary> 
        /// <param name="start">normal vector2</param> 
        /// <param name="end">normal vector2</param> 
        /// <returns>magnitude of the difference angular'ly of the normal directions</returns> 
        public static float     getDotProduct(Vector2 normal_A, Vector2 normal_B) 
        { 
            return (normal_A.X * normal_B.X) + (normal_A.Y * normal_B.Y); 
        } 
        /// <summary> 
        /// remember that a matrix is defined as column row not row column ie in programming we often denote 
        /// row column for x y however in mathmatics this is reversed so i like to say c r or y x instead so that  
        /// i dont confuse it cause i j typically really denotes y,x which is counter intuitive as well  
        /// as i use i j for secondary x y loops 
        /// anyways 
        /// to illustrate what is happening when you perform a element multiplication of a matrix 
        /// A[y1,x2]* B[2y,x1] when you plug these into a pair of matrixs and multiply them 
        /// within the matrix multiply is exactly the same as a dot product operation 
        /// ... 
        /// please note that the cubic normal ( i dont know if thier is a proper term for what i call the cubic normal but thier it is) 
        /// and i have used it extensivly for different linear interpolation operations in place of the full unit length normal 
        /// i suppose it can be considered a coefficient normal either way all its parts sum to 1 but dont square to 1 
        /// so what you get is really a cubic line or diamond type ploting value that is has the same ratio of x,y to the sin cos 
        /// however the cubic is in the form for direct conversion to rads by pi 
        /// ... 
        /// will also dot correctly and return a value that can be directly 
        /// multiplyed to Pi to find the radians of angular incident or reflect to the plane 
        /// </summary> 
        /// <param name="A"></param> 
        /// <param name="B"></param> 
        /// <param name="usecubicnormal">if set to true the two vectors will be normalized to a cubic normal 
        /// this intentionally has a safty  
        /// that will cause a precision error around e^-7th or however you denote that ect... i forget</param> 
        /// <returns>the dot of 2 vector2's be them normals or not </returns> 
        public static float     manualMatrixPointElementMultiply(Vector2 A, Vector2 B,bool usecubicnormal) 
        { 
            if (usecubicnormal) 
            { 
                float sum = 0.0f; float safety = .0000001317f; // just a note its possible to get a divide by 0 here 
                sum = A.X + A.Y + safety; A.X = A.X / sum; A.Y = A.Y / sum; 
                sum = B.X + B.Y + safety; B.X = B.X / sum; B.Y = B.Y / sum; 
            } 
            // in matrix form 
            //[ A.X, A.Y ] *[ B.X ], 
            //              [ B.Y ] 
            // actual simplifyed mathematical operation occuring 
            //      = A.X * B.X + A.Y * B.Y    
            // ie  this is nothing less then the dot product 
            return A.X * B.X + A.Y * B.Y; 
        } 
        // simple manual non matrix rotations about all axis combined 
        // 
        /// <summary> 
        /// 2d rotate 
        /// perform a rotation around the zAxis 
        /// the most common 2d rotation to spin 2d points around  
        /// </summary> 
        /// <param name="p">the point to rotate</param> 
        /// <param name="o">origin the  point p will orbit/rotate around</param> 
        /// <param name="q">the amount of rotation in radians</param> 
        /// <returns> the position of the newly rotated p point</returns> 
        public static Vector2   rotate2dPointAboutOriginOnZaxis(Vector2 p, Vector2 o, double q) 
        { 
            //x' = x*cos q - y*sin q  // y' = x*sin q + y*cos q  
            // 
            double x = p.X - o.X; // transform locally to the orgin 
            double y = p.Y - o.Y; 
            double rx = x * Math.Cos(q) - y * Math.Sin(q); 
            double ry = x * Math.Sin(q) + y * Math.Cos(q); 
            p.X = (float)rx + o.X; // translate back to non local 
            p.Y = (float)ry + o.Y; 
            return p; 
        } 
        /// <summary> 
        /// 2d rotate  
        /// this version makes the required change to adjust for xna's centered texture offset  
        /// when it draws a rotated texture rectangle 
        /// perform a rotation around the zAxis 
        /// the most common 2d rotation to spin 2d points around  
        /// </summary> 
        /// <param name="p">the point to rotate</param> 
        /// <param name="o">origin the  point p will orbit/rotate around typically the real point relative to a real center point</param> 
        /// <param name="q">the amount of rotation in radians</param> 
        /// <returns> the position of the newly rotated p point</returns> 
        public static Vector2   rotate2dPointAboutOriginOnZaxisXna(Vector2 p, Vector2 o, double q) 
        { 
            //x' = x*cos q - y*sin q  // y' = x*sin q + y*cos q  
            // 
            double x = p.X - o.X; // transform locally to the orgin 
            double y = p.Y - o.Y; 
            double rx = x * Math.Cos(q) - y * Math.Sin(q); 
            double ry = x * Math.Sin(q) + y * Math.Cos(q); 
            // 
            if (x < 0) { x = x * -1; } // this is point to origin width then made absolute 
            if (y < 0) { y = y * -1; } // this is point to origin height then made absolute 
            // 
            p.X = (float)(rx + o.X - x); // translate back to non local + the texture offset under xna 
            p.Y = (float)(ry + o.Y - y);  
            return p; 
        } 
 
        /* these are 3d manual methods included so you can see the actual math operations all the below arent converted for xna coordinates
         * cant remember which 3d coordinate system thier set for but its trivial to fix it these are really old
         */ 
 
        /// <summary> 
        /// this is the standard one ill use for now its not a matrix but it should work  
        /// note 
        /// the order of rotations are important xyz  typically you only use two rotations anyways 
        ///  
        /// point is the positional point to be rotated about the origin 
        /// orgin is the origin point or position the above point rotates around 
        /// anglexyz is the x y and z rotational angles in radians applyed in that order to translate the point 
        /// </summary> 
        /// <param name="point"></param> 
        /// <param name="origin"></param> 
        /// <param name="anglesxyz"></param> 
        /// <returns>returns new position x y z</returns> 
        public static Vector3   rotatePoint(Vector3 point, Vector3 origin, Vector3 anglesxyz) 
        { 
 
            float x = point.X - origin.X; // transform locally to the orgin 
            float y = point.Y - origin.Y; 
            float z = point.Z - origin.Z; 
            float q = anglesxyz.X; 
            float ry = (float)(y * Math.Cos(q)) - (float)(z * Math.Sin(q)); 
            float rz = (float)(y * Math.Sin(q) + (float)(z) * Math.Cos(q)); 
            float rx = x; 
            q = anglesxyz.Y; 
            z = (float)(rz * Math.Cos(q)) - (float)(rx * Math.Sin(q)); 
            x = (float)(rz * Math.Sin(q)) + (float)(rx * Math.Cos(q)); 
            y = ry; 
            q = anglesxyz.Z; 
            rx = (float)(x * Math.Cos(q)) - (float)(y * Math.Sin(q)); 
            ry = (float)(x * Math.Sin(q)) + (float)(y * Math.Cos(q)); 
            rz = z; 
            point.X = rx + origin.X; // rotated Translated Point with orgin transformation negated 
            point.Y = ry + origin.Y; 
            point.Z = rz + origin.Z; 
 
            return point; 
        } 
        // rotations upon each axis  
        // orgin or transforms not accounted for this is just the rotations 
        // 
        /// <summary> 
        /// perform a rotation around the zAxis 
        /// the most common 2d rotation to spin 2d points around 
        /// </summary> 
        /// <returns></returns> 
        public static Vector3   rotatePointAboutZaxis(Vector3 p, double q) 
        { 
            //x' = x*cos q - y*sin q 
            //y' = x*sin q + y*cos q  
            //z' = z 
 
            float rx = (float)(p.X * Math.Cos(q) - p.Y * Math.Sin(q)); 
            float ry = (float)(p.X * Math.Sin(q) + p.Y * Math.Cos(q)); 
            float rz = 0.0f; 
            p.X = rx; 
            p.Y = ry; 
            p.Z = rz; 
            return p; 
        } 
        /// <summary> 
        /// perform a rotation on the X axis 
        /// technically this is a 3d rotation as you are pulling things off or into the z plane 
        /// </summary> 
        /// <returns></returns> 
        public static Vector3   rotatePointAboutXaxis(Vector3 p, double q) 
        { 
            //y' = y*cos q - z*sin q 
            //z' = y*sin q + z*cos q 
            //x' = x 
            float ry = (float)(p.Y * Math.Cos(q) - p.Z * Math.Sin(q)); 
            float rz = (float)(p.Y * Math.Sin(q) + p.Z * Math.Cos(q)); 
            float rx = 0.0f; 
            p.X = rx; 
            p.Y = ry; 
            p.Z = rz; 
            return p; 
        } 
        /// <summary> 
        /// perform a rotation on the Y axis 
        /// technically this is a 3d rotation as you are pulling things off or into the z plane 
        /// p position q angle in rads 
        /// </summary> 
        /// <returns></returns> 
        public static Vector3   rotatePointAboutYaxis(Vector3 p, double q) 
        { 
            //z' = z*cos q - x*sin q 
            //x' = z*sin q + x*cos q 
            //y' = y 
            float rz = (float)(p.Z * Math.Cos(q) - p.X * Math.Sin(q)); 
            float rx = (float)(p.Z * Math.Sin(q) + p.X * Math.Cos(q)); 
            float ry = 0.0f; 
            p.X = rx; 
            p.Y = ry; 
            p.Z = rz; 
            return p; 
        } 
        // the basic stuff shown manually angles arent really useful when you bump up to 3d their to confusing and typically  
        // you change all this stuff into automated matrix operations so you never have to think about it 
        // but i think its good to see the simple mathematical fundamentals of what is happening linearly without the matrix's 
        // 
        /// <summary> 
        /// dot product of two points start(x,y,z) and end(x,y,z) 
        /// you should normalize the points before passing them  
        /// </summary> 
        /// <param name="start">normal vector3</param> 
        /// <param name="end">normal vector3</param> 
        /// <returns>magnitude of the difference angular'ly of the normal directions</returns> 
        public static float     getDotProduct(Vector3 A, Vector3 B) 
        { 
            return (A.X * B.X) + (A.Y * B.Y) + (A.Z * B.Z); 
        } 
        /// <summary> 
        /// dot product of two points start(x,y,z) and end(x,y,z) 
        /// you should normalize the points before passing them 
        /// </summary> 
        /// <param name="Ax"></param> 
        /// <param name="Ay"></param> 
        /// <param name="Az"></param> 
        /// <param name="Bx"></param> 
        /// <param name="By"></param> 
        /// <param name="Bz"></param> 
        /// <returns> 
        /// returns a magnitude of the difference of the two normals  
        /// that represent right angls to the surface planes 
        /// </returns> 
        public static float     getDotProduct(float Ax, float Ay, float Az, float Bx, float By, float Bz) 
        { 
            return (Ax * Bx) + (Ay * By) + (Az * Bz); 
        } 
        /// <summary> 
        /// quick normalize simply equalize x y z  
        /// by thier absolute sum 
        /// </summary> 
        /// <param name="v"></param> 
        /// <returns>returns a approximated cubic normal</returns> 
        public static Vector3   getCubicNormal(Vector3 v) 
        { 
            float n = Math.Abs(v.X) + Math.Abs(v.Y) + Math.Abs(v.Z); 
            v.X = v.X / n; 
            v.Y = v.Y / n; 
            v.Z = v.Z / n; 
            return v; 
        } 
        /// <summary> 
        /// the vectors a b c form the vertice positions of a triangle 
        /// this function returns a normal from them based on the winding ie 
        /// (+)n is clockwise winding via local or model position a,b,c 
        /// (-)n is found by counter clockwise winding c,b,a 
        /// </summary> 
        /// <param name="a"></param> 
        /// <param name="b"></param> 
        /// <param name="c"></param> 
        /// <returns></returns> 
        public static Vector3   findUnitNormalFromCrossProduct(Vector3 a, Vector3 b, Vector3 c) 
        { 
            // find normal and unit normal 
            double Ax = (b.X - a.X);  
            double Ay = (b.Y - a.Y);  
            double Az = (b.Z - a.Z); 
            double Bx = (c.X - b.X);  
            double By = (c.Y - b.Y);  
            double Bz = (c.Z - b.Z); 
            // 
            double nx = (Ay * Bz) - (By * Az); 
            double ny = (Az * Bx) - (Bz * Ax); 
            double nz = (Ax * By) - (Bx * Ay); 
            double dsquare = (nx * nx) + (ny * ny) + (nz * nz);  
            double sqrtnp = Math.Sqrt(dsquare); 
            if (sqrtnp == 0.0f) { sqrtnp = .0000001d; } // prevent division by zero 
            double coef = 1 / sqrtnp; 
            // unit length normal       
            double unx = (double)(nx * coef); 
            double uny = (double)(ny * coef); 
            double unz = (double)(nz * coef); 
            // 
            return new Vector3((float)unx, (float)uny, (float)unz); 
        } 
        /// <summary> 
        /// the vectors a b c form the vertice positions of a triangle 
        /// this function returns a normal from them based on the winding ie 
        /// (+)n is clockwise winding via local or model position a,b,c 
        /// (-)n is found by counter clockwise winding c,b,a 
        /// </summary> 
        /// <param name="a"></param> 
        /// <param name="b"></param> 
        /// <param name="c"></param> 
        /// <returns></returns> 
        public static Vector3 findCrossProduct(Vector3 a, Vector3 b, Vector3 c) 
        { 
            // find normal and unit normal 
            double Ax = (b.X - a.X); double Ay = (b.Y - a.Y); double Az = (b.Z - a.Z); 
            double Bx = (c.X - b.X); double By = (c.Y - b.Y); double Bz = (c.Z - b.Z); 
            double nx = (Ay * Bz) - (By * Az); 
            double ny = (Az * Bx) - (Bz * Ax); 
            double nz = (Ax * By) - (Bx * Ay); 
            // 
            return new Vector3((float)nx, (float)ny, (float)nz); 
        } 
    } 
}
