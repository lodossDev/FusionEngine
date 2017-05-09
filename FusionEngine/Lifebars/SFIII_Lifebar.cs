using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace FusionEngine {

    public class SFIII_Lifebar : LifeBar {

        public SFIII_Lifebar(int posx, int posy, int ox, int oy, float sx, float sy, SpriteEffects spriteEffect = SpriteEffects.None) : base(posx, posy, ox, oy, sx, sy, spriteEffect) {
        
        }

        public override void Load(int posx, int posy, int ox, int oy, float sx, float sy) {
            AddSprite(SpriteType.PLACEHOLDER, "Sprites/LifeBars/SFIII/LIFEBAR/PLACEHOLDER", posx, posy, 0, 0, sx, sy);
            AddSprite(SpriteType.CONTAINER, "Sprites/LifeBars/SFIII/LIFEBAR/CONTAINER", posx, posy, ox, oy, sx, sy);
            AddSprite(SpriteType.BAR, "Sprites/LifeBars/SFIII/LIFEBAR/BAR", posx, posy, ox, oy, sx, sy);
        }
    }
}
