using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public class PhoneBooth1 : Obstacle {

        public PhoneBooth1() : base("PhoneBooth1") {
            AddSprite(Animation.State.STANCE, new Sprite("Sprites/Misc/Phone/Stance"), true);
            AddSprite(Animation.State.DIE1, new Sprite("Sprites/Misc/Phone/Die1"));

            AddBoundsBox(141, 380, -70, 110, 60);
            SetScale(2.5f, 3.0f);
            SetPostion(200, 0, 100);
        }
    }
}
