using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {
    public class Wall : Entity {
      
        public Wall(String name, int width, int height, int x, int z, int x1, int y1, int depth) : base(ObjectType.WALL, name) {
            Texture2D pixel = new Texture2D(GameManager.GraphicsDevice, width, height);  
            Color[] colorData = new Color[width * height];
            int index = 0;

            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    colorData[index++] = Color.Blue;
                }
            }
            
            // Set the texture data with our color information.  
            pixel.SetData<Color>(colorData); 
            
            AddSprite(Animation.State.STANCE, new Sprite(pixel, Animation.Type.NONE), true);
            AddBoundsBox(width, height, -(width / 2) + x1, y1, depth);

            SetFade(120);
            SetPosX(x);
            SetPosZ(z); 

            SetIsCollidable(true);
        }
    }
}
