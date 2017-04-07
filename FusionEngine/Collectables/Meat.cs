using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public class Meat : Health {

        public Meat() : base("MEAT") {
            AddSprite(Animation.State.STANCE, new Sprite("Sprites/Misc/Meat/STANCE"), true);
            SetFrameDelay(Animation.State.STANCE, 7);

            SetAnimationState(Animation.State.STANCE);
            AddBoundsBox(120, 50, -60, 93, 30);
            SetOnLoadScale(3.3f, 3.2f);

            SetPostion(600, 0, 400);
        }
    }
}
