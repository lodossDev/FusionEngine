using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public class PhoneBooth : Obstacle {

        public PhoneBooth() : base("PhoneBooth1") {
            AddSprite(Animation.State.STANCE, new Sprite("Sprites/Misc/Phone/Stance"), true);

            AddSprite(Animation.State.DIE1, new Sprite("Sprites/Misc/Phone/Die1", Animation.Type.NONE));
            SetFrameDelay(Animation.State.DIE1, 10);
            
            AddBoundsBox(141, 380, -80, 10, 60);
            SetOnLoadScale(2.5f, 3.0f);
            SetPostion(200, 0, 100);
        }
    }
}
