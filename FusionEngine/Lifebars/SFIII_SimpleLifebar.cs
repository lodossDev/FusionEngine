using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public class SFIII_SimpleLifebar: LifeBar {

        public SFIII_SimpleLifebar(int posx, int posy, int ox, int oy, float sx, float sy, SpriteEffects spriteEffect = SpriteEffects.None) : base(posx, posy, ox, oy, sx, sy, spriteEffect) {
        
        }

        public override void Load(int posx, int posy, int ox, int oy, float sx, float sy) {
            AddSprite(SpriteType.CONTAINER, "Sprites/LifeBars/SFIII/LIFEBAR/CONTAINER", posx, posy, ox, oy, sx, sy);
            AddSprite(SpriteType.BAR, "Sprites/LifeBars/SFIII/LIFEBAR/BAR", posx, posy, ox, oy, sx, sy);

            SetPercent(100);
        }
    }
}
