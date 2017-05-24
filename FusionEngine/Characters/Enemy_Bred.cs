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
            SetFrameDelay(Animation.State.WALK_TOWARDS, 7);
            SetSpriteOffSet(Animation.State.WALK_TOWARDS, 0, 0);

            AddSprite(Animation.State.JUMP, new Sprite("Sprites/Actors/Bred/JUMP", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.JUMP, 5);
            SetSpriteOffSet(Animation.State.JUMP, 8, 0);

            AddSprite(Animation.State.FALL1, new Sprite("Sprites/Actors/Bred/FALL1", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.FALL1, 4);
            SetSpriteOffSet(Animation.State.FALL1, -10, -27);

            AddSprite(Animation.State.THROWN1, new Sprite("Sprites/Actors/Bred/KNOCK_DOWN1", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.THROWN1, 12);
            SetSpriteOffSet(Animation.State.THROWN1, 0, -15);
            SetFrameScale(Animation.State.THROWN1, -0.3f, 0);

            AddSprite(Animation.State.KNOCKED_DOWN1, new Sprite("Sprites/Actors/Bred/KNOCK_DOWN1", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.KNOCKED_DOWN1, 12);
            SetSpriteOffSet(Animation.State.KNOCKED_DOWN1, 0, -15);
            SetFrameScale(Animation.State.KNOCKED_DOWN1, -0.3f, 0);

            AddSprite(Animation.State.PAIN1, new Sprite("Sprites/Actors/Bred/PAIN1", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.PAIN1, 6);
            SetSpriteOffSet(Animation.State.PAIN1, 10, 0);

            AddSprite(Animation.State.PAIN2, new Sprite("Sprites/Actors/Bred/PAIN2", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.PAIN2, 6);
            SetSpriteOffSet(Animation.State.PAIN2, 0, 0);

            AddSprite(Animation.State.INGRAB1, new Sprite("Sprites/Actors/Bred/INGRAB1", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.INGRAB1, 6);
            SetSpriteOffSet(Animation.State.INGRAB1, 0, 0);

            AddSprite(Animation.State.RISE1, new Sprite("Sprites/Actors/Bred/RISE1", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.RISE1, 8);
            SetSpriteOffSet(Animation.State.RISE1, 0, 0);

            AddSprite(Animation.State.RISE2, new Sprite("Sprites/Actors/Bred/RISE2", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.RISE2, 4);
            SetSpriteOffSet(Animation.State.RISE2, 0, 0);

            AddSprite(Animation.State.BLOCK1, new Sprite("Sprites/Actors/Bred/BLOCK1", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.BLOCK1, 8);

            AddSprite(Animation.State.DIE1, new Sprite("Sprites/Actors/Bred/KO1", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.DIE1, 6);
            SetFrameDelay(Animation.State.DIE1, 5, 33);
            SetFrameDelay(Animation.State.DIE1, 6, 33);
            SetFrameDelay(Animation.State.DIE1, 7, 43);
            SetSpriteOffSet(Animation.State.DIE1, 0, 0);

            AddSprite(Animation.State.ATTACK1, new Sprite("Sprites/Actors/Bred/ATTACK1", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.ATTACK1, 4);
            SetFrameDelay(Animation.State.ATTACK1, 3, 10);
            SetSpriteOffSet(Animation.State.ATTACK1, -20, 0);

            AddSprite(Animation.State.ATTACK2, new Sprite("Sprites/Actors/Bred/ATTACK2", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.ATTACK2, 4);
            SetFrameDelay(Animation.State.ATTACK2, 3, 10);
            SetSpriteOffSet(Animation.State.ATTACK2, 30, 0);

            AddSprite(Animation.State.ATTACK3, new Sprite("Sprites/Actors/Bred/ATTACK3", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.ATTACK3, 4);
            SetFrameDelay(Animation.State.ATTACK2, 3, 14);
            SetSpriteOffSet(Animation.State.ATTACK3, 30, 0);

            AddSprite(Animation.State.JUMP_ATTACK1, new Sprite("Sprites/Actors/Bred/JUMP_ATTACK1", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.JUMP_ATTACK1, 5);
            SetSpriteOffSet(Animation.State.JUMP_ATTACK1, 8, -30);

            AddBox(Animation.State.ATTACK1, 2, new CLNS.AttackBox(150, 170, 50, 145));
            AddBox(Animation.State.ATTACK2, 2, new CLNS.AttackBox(150, 170, 50, 145));
            AddBox(Animation.State.ATTACK3, 2, new CLNS.AttackBox(150, 170, 50, 145));

            SetAnimationState(Animation.State.STANCE);

            SetLowPainGrabbedState(Animation.State.PAIN2);
            SetLowPainState(Animation.State.PAIN1);
            SetMediumPainState(Animation.State.PAIN2);

            SetOnLoadScale(3.2f, 3.3f);
            AddBoundsBox(160, 300, -60, 240, 50);

            //SetBaseOffset(-150, -100);
            SetShadowOffset(0, -85);
            SetScale(3.2f, 3.2f);

            SetGrabResistance(50);
            SetGrabbable(true);

            SetPostion(400, 0, 100);
            SetOffsetZ(200);
            SetMaxHealth(100);
            SetCanHurtOthers(true);

            SFIII_SimpleLifebar lifeBar = new SFIII_SimpleLifebar(225, 110, 0 , 0, 2.0f, 4.5f);
            lifeBar.SetPortrait("Sprites/Actors/Bred/PORTRAIT", 160, 90, 0, 0, 4.08f, 3f);
            SetLifeBar(lifeBar);

            SetMaxLives(3);
            SetDeathMode(DeathType.DEFAULT | DeathType.FLASH);

            //SetBoundToLevel(true);
        }
    }
}
