using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public class Enemy_Bred : Enemy {

        public Enemy_Bred() : base("BRED") {
            AddSprite(Animation.State.STANCE, new Sprite("Sprites/Actors/Bred/STANCE"), true);
            SetFrameDelay(Animation.State.STANCE, 7);

            AddSprite(Animation.State.WALK_TOWARDS, new Sprite("Sprites/Actors/Bred/WALK_TOWARDS", Animation.Type.REPEAT));
            SetFrameDelay(Animation.State.WALK_TOWARDS, 10);
            SetSpriteOffSet(Animation.State.WALK_TOWARDS, 0, 0);

            AddSprite(Animation.State.JUMP, new Sprite("Sprites/Actors/Bred/JUMP", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.JUMP, 5);
            SetSpriteOffSet(Animation.State.JUMP, 8, 0);

            AddSprite(Animation.State.FALL1, new Sprite("Sprites/Actors/Bred/FALL1", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.FALL1, 4);
            SetSpriteOffSet(Animation.State.FALL1, -10, -27);

            AddSprite(Animation.State.PAIN1, new Sprite("Sprites/Actors/Bred/PAIN1", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.PAIN1, 4);
            SetSpriteOffSet(Animation.State.PAIN1, 10, 0);

            AddSprite(Animation.State.PAIN2, new Sprite("Sprites/Actors/Bred/PAIN2", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.PAIN2, 4);
            SetSpriteOffSet(Animation.State.PAIN2, 0, 0);

            AddSprite(Animation.State.RISE1, new Sprite("Sprites/Actors/Bred/RISE1", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.RISE1, 4);
            SetSpriteOffSet(Animation.State.RISE1, 0, 0);

            AddSprite(Animation.State.RISE2, new Sprite("Sprites/Actors/Bred/RISE2", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.RISE2, 4);
            SetSpriteOffSet(Animation.State.RISE2, 0, 0);

            AddSprite(Animation.State.KO1, new Sprite("Sprites/Actors/Bred/KO1", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.KO1, 4);
            SetSpriteOffSet(Animation.State.KO1, 0, 0);

            AddSprite(Animation.State.ATTACK1, new Sprite("Sprites/Actors/Bred/ATTACK1", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.ATTACK1, 4);
            SetSpriteOffSet(Animation.State.ATTACK1, -20, 0);

            AddSprite(Animation.State.ATTACK2, new Sprite("Sprites/Actors/Bred/ATTACK2", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.ATTACK2, 4);
            SetSpriteOffSet(Animation.State.ATTACK2, 30, 0);

            AddSprite(Animation.State.ATTACK3, new Sprite("Sprites/Actors/Bred/ATTACK3", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.ATTACK3, 4);
            SetSpriteOffSet(Animation.State.ATTACK3, 30, 0);

            AddSprite(Animation.State.JUMP_ATTACK1, new Sprite("Sprites/Actors/Bred/JUMP_ATTACK1", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.JUMP_ATTACK1, 5);
            SetSpriteOffSet(Animation.State.JUMP_ATTACK1, 8, -30);

            SetAnimationState(Animation.State.WALK_TOWARDS);
            SetOnLoadScale(3.2f, 3.2f);
            AddBoundsBox(160, 240, -60, 240, 50);
            SetBaseOffset(0, -25);

            SetPostion(400, 0, 100);
        }
    }
}
