using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public class Drum : Obstacle {

        public Drum() : base("DRUM1") {
            AddSprite(Animation.State.STANCE, new Sprite("Sprites/Misc/Drum/Stance"), true);
            AddSprite(Animation.State.PAIN1, new Sprite("Sprites/Misc/Drum/Pain1"));
            AddSprite(Animation.State.PAIN2, new Sprite("Sprites/Misc/Drum/Pain2"));
            AddSprite(Animation.State.DIE1, new Sprite("Sprites/Misc/Drum/Die1"));

            AddBoundsBox(125, 210, -63, -15, 40);
            SetOnLoadScale(2.4f, 2.9f);
            SetPostion(0, 0, 0);

            SetMaxHealth(1);
            SetMaxLives(1);
            SetIsPlatform(true);
        }
    }
}
