using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

   public class Line {
        public Rectangle my_from;
        public Rectangle my_destination;
        public Vector2 x_finder;

        public Line(Rectangle start) {
            my_destination = start;
        }

        public void Update() {
            x_finder = StartGameScreen.RotateVector2(x_finder, 0.04f, Vector2.Zero);
            my_destination.X = (int)x_finder.X + 200;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(StartGameScreen.sand, my_destination, my_from, /*new Color(my_destination.X*20,100,my_destination.Y/4)*/Color.White *1.0f, 0, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}
