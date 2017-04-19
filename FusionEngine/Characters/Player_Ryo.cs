using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FusionEngine {

    public class Player_Ryo : Player {

        public Player_Ryo() : base("RYO") {
            AddSprite(Animation.State.STANCE, new Sprite("Sprites/Actors/Ryo/Stance"), true);
            SetFrameDelay(Animation.State.STANCE, 7);

            AddSprite(Animation.State.WALK_TOWARDS, new Sprite("Sprites/Actors/Ryo/WalkTowards", Animation.Type.REPEAT));
            SetFrameDelay(Animation.State.WALK_TOWARDS, 5);
            SetSpriteOffSet(Animation.State.WALK_TOWARDS, 5, -1);

            AddSprite(Animation.State.RUN, new Sprite("Sprites/Actors/Ryo/Run", Animation.Type.REPEAT, 2));
            SetFrameDelay(Animation.State.RUN, 4);
            SetSpriteOffSet(Animation.State.RUN, -8, 5);

            AddSprite(Animation.State.JUMP_START, new Sprite("Sprites/Actors/Ryo/JumpStart", Animation.Type.ONCE));
            SetSpriteOffSet(Animation.State.JUMP_START, 5, 10);
            SetFrameDelay(Animation.State.JUMP_START, 3);

            AddSprite(Animation.State.JUMP, new Sprite("Sprites/Actors/Ryo/Jump", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.JUMP, 5);
            SetSpriteOffSet(Animation.State.JUMP, 8, -30);

            AddSprite(Animation.State.JUMP_TOWARDS, new Sprite("Sprites/Actors/Ryo/Jump", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.JUMP_TOWARDS, 5);
            SetSpriteOffSet(Animation.State.JUMP_TOWARDS, 8, -30);

            AddSprite(Animation.State.FALL1, new Sprite("Sprites/Actors/Ryo/Fall1", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.FALL1, 5);
            SetSpriteOffSet(Animation.State.FALL1, 8, -20);

            AddSprite(Animation.State.LAND1, new Sprite("Sprites/Actors/Ryo/Land1", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.LAND1, 5);
            SetSpriteOffSet(Animation.State.LAND1, 3, 1);

            AddSprite(Animation.State.PICKUP1, new Sprite("Sprites/Actors/Ryo/PICKUP1", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.PICKUP1, 5);
            SetSpriteOffSet(Animation.State.PICKUP1, 6, 10);

            AddSprite(Animation.State.RUN_STOP1, new Sprite("Sprites/Actors/Ryo/RunStop1", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.RUN_STOP1, 5);
            SetSpriteOffSet(Animation.State.RUN_STOP1, -3, 1);

            AddSprite(Animation.State.GRAB_HOLD1, new Sprite("Sprites/Actors/Ryo/Grab1", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.GRAB_HOLD1, 5);
            SetSpriteOffSet(Animation.State.GRAB_HOLD1, 13, 0);

            AddSprite(Animation.State.THROW1, new Sprite("Sprites/Actors/Ryo/Throw1", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.THROW1, 5);
            SetSpriteOffSet(Animation.State.THROW1, -20, 0);

            //----------------------------------------- ATTACKS -----------------------------------------//

            AddSprite(Animation.State.GRAB_ATTACK1, new Sprite("Sprites/Actors/Ryo/GrabAttack1", Animation.Type.ONCE));
            SetSpriteOffSet(Animation.State.GRAB_ATTACK1, 10, -23);
            SetFrameDelay(Animation.State.GRAB_ATTACK1, 4);

            AddSprite(Animation.State.ATTACK1, new Sprite("Sprites/Actors/Ryo/Attack1", Animation.Type.ONCE));
            SetSpriteOffSet(Animation.State.ATTACK1, 10, 0);
            SetFrameDelay(Animation.State.ATTACK1, 4);

            AddSprite(Animation.State.ATTACK2, new Sprite("Sprites/Actors/Ryo/Attack2", Animation.Type.ONCE));
            SetSpriteOffSet(Animation.State.ATTACK2, 12, 0);
            SetFrameDelay(Animation.State.ATTACK2, 4);

            AddSprite(Animation.State.ATTACK3, new Sprite("Sprites/Actors/Ryo/Attack3", Animation.Type.ONCE));
            SetSpriteOffSet(Animation.State.ATTACK3, 12, 0);
            SetFrameDelay(Animation.State.ATTACK3, 4);

            AddSprite(Animation.State.ATTACK4, new Sprite("Sprites/Actors/Ryo/Attack4", Animation.Type.ONCE));
            SetSpriteOffSet(Animation.State.ATTACK4, 12, 0);
            SetFrameDelay(Animation.State.ATTACK4, 4);

            AddSprite(Animation.State.SPECIAL1, new Sprite("Sprites/Actors/Ryo/Special1", Animation.Type.ONCE));
            SetSpriteOffSet(Animation.State.SPECIAL1, 20, -32);
            SetFrameDelay(Animation.State.SPECIAL1, 4);

            AddSprite(Animation.State.JUMP_ATTACK1, new Sprite("Sprites/Actors/Ryo/JumpAttack1", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.JUMP_ATTACK1, 5);
            SetSpriteOffSet(Animation.State.JUMP_ATTACK1, 8, -30);

            AddSprite(Animation.State.JUMP_TOWARD_ATTACK1, new Sprite("Sprites/Actors/Ryo/JumpAttack3", Animation.Type.ONCE));
            SetFrameDelay(Animation.State.JUMP_TOWARD_ATTACK1, 5);
            SetSpriteOffSet(Animation.State.JUMP_TOWARD_ATTACK1, 8, -30);

            AddBox(Animation.State.GRAB_ATTACK1, 6, new CLNS.AttackBox(150, 170, 50, 45));
            AddBox(Animation.State.ATTACK1, 2, new CLNS.AttackBox(150, 170, 50, 45));

            AddBox(Animation.State.ATTACK2, 3, new CLNS.AttackBox(150, 170, 50, 45));
            GetAttackBox(Animation.State.ATTACK2, 3).SetAttackType(CLNS.AttackBox.AttackType.MEDIUM);

            AddBox(Animation.State.ATTACK3, 3, new CLNS.AttackBox(150, 170, 50, 45));
            GetAttackBox(Animation.State.ATTACK3, 3).SetAttackType(CLNS.AttackBox.AttackType.MEDIUM);
            //AddBox(Animation.State.ATTACK1, 3, new CLNS.AttackBox(100, 80, 132, 45));

            AddBox(Animation.State.SPECIAL1, 2, new CLNS.AttackBox(150, 200, 50, 45));
            AddBox(Animation.State.SPECIAL1, 4, new CLNS.AttackBox(150, 200, 50, -15));
            AddBox(Animation.State.SPECIAL1, 6, new CLNS.AttackBox(150, 200, 50, 45));
            AddBox(Animation.State.SPECIAL1, 8, new CLNS.AttackBox(150, 200, 50, -15));
            AddBox(Animation.State.SPECIAL1, 10, new CLNS.AttackBox(150, 200, 50, 45));
            AddBox(Animation.State.SPECIAL1, 12, new CLNS.AttackBox(150, 200, 50, -15));
            AddBox(Animation.State.SPECIAL1, 14, new CLNS.AttackBox(150, 200, 50, 45));

            AddAnimationLink(new Animation.Link(Animation.State.JUMP_START, Animation.State.JUMP, 1));

            SetTossFrame(Animation.State.JUMP_START, 1);
            SetTossFrame(Animation.State.JUMP, 1);
            SetTossFrame(Animation.State.JUMP_TOWARDS, 1);
            SetMoveFrame(Animation.State.WALK_TOWARDS, 1);

            //AddHitSpark(new Effect("LIGHT_SPARK", "Sprites/Actors/Ryo/Hitflash1", Effect.Type.HIT_SPARK, Effect.State.LIGHT, 1.8f, 1.5f));

            SetDefaultAttackChain(new ComboAttack.Chain(new List<ComboAttack.Move>{
                new ComboAttack.Move(Animation.State.ATTACK1, 2000, 2),
                new ComboAttack.Move(Animation.State.ATTACK1, 2000, 2),
                new ComboAttack.Move(Animation.State.ATTACK1, 2000, 2),
                new ComboAttack.Move(Animation.State.ATTACK1, 2000, 2),
                new ComboAttack.Move(Animation.State.ATTACK1, 2000, 2),
                new ComboAttack.Move(Animation.State.ATTACK2, 2000, 5),
                new ComboAttack.Move(Animation.State.ATTACK3, 2000, 4),
                new ComboAttack.Move(Animation.State.ATTACK2, 2000, 5),
                new ComboAttack.Move(Animation.State.ATTACK3, 2000, 4),
                new ComboAttack.Move(Animation.State.ATTACK2, 2000, 5),
                new ComboAttack.Move(Animation.State.ATTACK3, 2000, 4)/*,
                new ComboAttack.Move(Animation.State.ATTACK4, 2000, 4)*/
            }));

            //Normal command moves..
            InputHelper.CommandMove command = new InputHelper.CommandMove("RUNNING", Animation.State.RUN, new List<InputHelper.KeyState> {
                new InputHelper.KeyState(InputHelper.KeyPress.RIGHT, InputHelper.ButtonState.Pressed),
                new InputHelper.KeyState(InputHelper.KeyPress.RIGHT, InputHelper.ButtonState.Pressed),
            }, 200f);

            AddCommandMove(command);

            command = new InputHelper.CommandMove("RUNNING", Animation.State.RUN, new List<InputHelper.KeyState> {
                new InputHelper.KeyState(InputHelper.KeyPress.LEFT, InputHelper.ButtonState.Pressed),
                new InputHelper.KeyState(InputHelper.KeyPress.LEFT, InputHelper.ButtonState.Pressed),
            }, 200f);

            AddCommandMove(command);

            //Attack command moves..

            /*InputHelper.CommandMove command = new InputHelper.CommandMove("TEST", Animation.State.ATTACK4, new List<InputHelper.KeyState> {
                new InputHelper.KeyState(InputHelper.KeyPress.RIGHT, InputHelper.ButtonState.Pressed),
                new InputHelper.KeyState(InputHelper.KeyPress.RIGHT, InputHelper.ButtonState.Pressed),
                new InputHelper.KeyState(InputHelper.KeyPress.A, InputHelper.ButtonState.Pressed)
            }, 500f);

            AddCommandMove(command);

            command = new InputHelper.CommandMove("TEST", Animation.State.ATTACK4, new List<InputHelper.KeyState> {
                new InputHelper.KeyState(InputHelper.KeyPress.RIGHT, InputHelper.ButtonState.Pressed),
                new InputHelper.KeyState(InputHelper.KeyPress.RIGHT, InputHelper.ButtonState.Pressed),
                new InputHelper.KeyState(InputHelper.KeyPress.RIGHT | InputHelper.KeyPress.A, InputHelper.ButtonState.Pressed)
            }, 500f);

            AddCommandMove(command);

            command = new InputHelper.CommandMove("TEST", Animation.State.SPECIAL1, new List<InputHelper.KeyState> {
                new InputHelper.KeyState(InputHelper.KeyPress.RIGHT, InputHelper.ButtonState.Pressed),
                new InputHelper.KeyState(InputHelper.KeyPress.RIGHT, InputHelper.ButtonState.Pressed),
                new InputHelper.KeyState(InputHelper.KeyPress.RIGHT | InputHelper.KeyPress.A | InputHelper.KeyPress.X, InputHelper.ButtonState.Pressed)
            }, 500f);

            AddCommandMove(command);

            command = new InputHelper.CommandMove("TEST", Animation.State.SPECIAL1, new List<InputHelper.KeyState> {
                new InputHelper.KeyState(InputHelper.KeyPress.RIGHT, InputHelper.ButtonState.Pressed),
                new InputHelper.KeyState(InputHelper.KeyPress.RIGHT, InputHelper.ButtonState.Pressed),
                new InputHelper.KeyState(InputHelper.KeyPress.A | InputHelper.KeyPress.X, InputHelper.ButtonState.Pressed)
            }, 500f);

            AddCommandMove(command);
            */

            command = new InputHelper.CommandMove("TEST", Animation.State.SPECIAL1, new List<InputHelper.KeyState> {
                new InputHelper.KeyState(InputHelper.KeyPress.ATTACK1, InputHelper.ButtonState.Pressed),
                new InputHelper.KeyState(InputHelper.KeyPress.ATTACK1, InputHelper.ButtonState.Pressed),
                new InputHelper.KeyState(InputHelper.KeyPress.ATTACK1, InputHelper.ButtonState.Pressed),
                new InputHelper.KeyState(InputHelper.KeyPress.ATTACK1, InputHelper.ButtonState.Pressed),
                new InputHelper.KeyState(InputHelper.KeyPress.ATTACK1, InputHelper.ButtonState.Pressed),
                new InputHelper.KeyState(InputHelper.KeyPress.ATTACK1, InputHelper.ButtonState.Pressed),
                new InputHelper.KeyState(InputHelper.KeyPress.ATTACK1, InputHelper.ButtonState.Pressed),
                new InputHelper.KeyState(InputHelper.KeyPress.ATTACK1, InputHelper.ButtonState.Pressed)
            }, 500);

            AddCommandMove(command);

            SetKeyboard(InputHelper.KeyPress.UP, Keys.Up);
            SetKeyboard(InputHelper.KeyPress.DOWN, Keys.Down);
            SetKeyboard(InputHelper.KeyPress.LEFT, Keys.Left);
            SetKeyboard(InputHelper.KeyPress.RIGHT, Keys.Right);
            SetKeyboard(InputHelper.KeyPress.JUMP, Keys.Space);
            SetKeyboard(InputHelper.KeyPress.ATTACK1, Keys.A);

            SetGamepad(InputHelper.KeyPress.JUMP, Buttons.A);
            SetGamepad(InputHelper.KeyPress.ATTACK1, Buttons.X);

            SetAnimationState(Animation.State.STANCE);
            AddBoundsBox(160, 340, -60, 15, 50);

            SetOnLoadScale(3.3f, 3.2f);
            SetPostion(400, 0, 200);
            SetBaseOffset(50f, 190f);

            //AddFrameAction(Animation.State.STANCE, 1, 1, 10);
        }
    }
}
