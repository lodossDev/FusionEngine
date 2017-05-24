using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using FusionEngine;

namespace FusionEngine {

    public class InputControl {
        public enum InputDirection {NONE, UP, DOWN, LEFT, RIGHT, UP_LEFT, UP_RIGHT, DOWN_LEFT, DOWN_RIGHT}
        public bool UP, DOWN, LEFT, RIGHT, JUMP_PRESS, ATTACK_PRESS;

        private Entity player;
        private InputDirection inputDirection;
        private PlayerIndex playerIndex;

        private KeyboardState oldKeyboardState, currentKeyboardState;
        private GamePadState oldPadState, currentPadState;

        private InputBuffer pressedState;
        private InputBuffer releasedState;
        private InputBuffer heldState;

        private float walkSpeed = 4;
        private float runSpeed = 10;
        private float veloctiy = 4;
        private float jumpHeight = -15;


        public InputControl(Entity player, PlayerIndex index) {
            this.player = player;
            this.playerIndex = index;
            inputDirection = InputDirection.NONE;

            oldKeyboardState = new KeyboardState();
            oldPadState = new GamePadState();

            pressedState = new InputBuffer(InputHelper.ButtonState.Pressed);
            releasedState = new InputBuffer(InputHelper.ButtonState.Released);
            heldState = new InputBuffer(InputHelper.ButtonState.Held, 200);

            UP = false;
            DOWN = false;
            LEFT = false;
            RIGHT = false;
            JUMP_PRESS = false;
            ATTACK_PRESS = false;
        }

        private void Reset() {
            inputDirection = InputDirection.NONE;
            JUMP_PRESS = false;
            ATTACK_PRESS = false;

            if (UP && (!currentKeyboardState.IsKeyUp(player.GetKeyboardKey(InputHelper.KeyPress.UP)) 
                    && (currentPadState.IsButtonUp(Buttons.DPadUp) 
                    && currentPadState.IsButtonUp(Buttons.LeftThumbstickUp)))) {

                player.ResetZ();
                UP = false;
            }

            if (UP && (currentKeyboardState.IsKeyUp(player.GetKeyboardKey(InputHelper.KeyPress.UP)) 
                    && !currentPadState.IsButtonDown(Buttons.DPadUp) 
                    && !currentPadState.IsButtonDown(Buttons.LeftThumbstickUp))) {

                player.ResetZ();
                UP = false;
            }

            if (DOWN && (!currentKeyboardState.IsKeyDown(player.GetKeyboardKey(InputHelper.KeyPress.DOWN)) 
                      && (currentPadState.IsButtonUp(Buttons.DPadDown) 
                      && currentPadState.IsButtonUp(Buttons.LeftThumbstickDown)))) {

                player.ResetZ();
                DOWN = false;
            }

            if (DOWN && (currentKeyboardState.IsKeyUp(player.GetKeyboardKey(InputHelper.KeyPress.DOWN)) 
                      && !currentPadState.IsButtonDown(Buttons.DPadDown) 
                      && !currentPadState.IsButtonDown(Buttons.LeftThumbstickDown))) {

                player.ResetZ();
                DOWN = false;
            }

            if (RIGHT && (!currentKeyboardState.IsKeyDown(player.GetKeyboardKey(InputHelper.KeyPress.RIGHT)) 
                        && (currentPadState.IsButtonUp(Buttons.DPadRight) 
                        && currentPadState.IsButtonUp(Buttons.LeftThumbstickRight)))) {

                player.ResetX();
                RIGHT = false;
            }
                player.ResetX();

            if (RIGHT && (currentKeyboardState.IsKeyUp(player.GetKeyboardKey(InputHelper.KeyPress.RIGHT)) 
                        && !currentPadState.IsButtonDown(Buttons.DPadRight) 
                        && !currentPadState.IsButtonDown(Buttons.LeftThumbstickRight))) {

                player.ResetX();
                RIGHT = false;
            }

            if (LEFT && (!currentKeyboardState.IsKeyDown(player.GetKeyboardKey(InputHelper.KeyPress.LEFT)) 
                        && (currentPadState.IsButtonUp(Buttons.LeftThumbstickLeft) 
                        && currentPadState.IsButtonUp(Buttons.DPadLeft)))) {

                player.ResetX();
                LEFT = false;
            }

            if (LEFT && currentKeyboardState.IsKeyUp(player.GetKeyboardKey(InputHelper.KeyPress.LEFT)) 
                    && !currentPadState.IsButtonDown(Buttons.LeftThumbstickLeft) 
                    && !currentPadState.IsButtonDown(Buttons.DPadLeft)) {

                player.ResetX();
                LEFT = false;
            }

            if (JUMP_PRESS && (currentKeyboardState.IsKeyUp(player.GetKeyboardKey(InputHelper.KeyPress.JUMP)) 
                                    || currentPadState.IsButtonUp(player.GetGamepadKey(InputHelper.KeyPress.JUMP)))) {

                player.ResetX();
                player.ResetZ();
                JUMP_PRESS = false;
            }

            if (ATTACK_PRESS && (currentKeyboardState.IsKeyUp(player.GetKeyboardKey(InputHelper.KeyPress.ATTACK1)) 
                                    || currentPadState.IsButtonUp(player.GetGamepadKey(InputHelper.KeyPress.ATTACK1)))) {
                player.ResetX();
                player.ResetZ();
                ATTACK_PRESS = false;
            }
        }

        public void UpdateDefaultControls(GameTime gameTime) {
            Reset();

            if (((currentKeyboardState.IsKeyDown(player.GetKeyboardKey(InputHelper.KeyPress.JUMP)))
                    && (!oldKeyboardState.IsKeyDown(player.GetKeyboardKey(InputHelper.KeyPress.JUMP))))

                        || (currentPadState.IsButtonDown(player.GetGamepadKey(InputHelper.KeyPress.JUMP))
                                && !oldPadState.IsButtonDown(player.GetGamepadKey(InputHelper.KeyPress.JUMP)))) {

                JUMP_PRESS = true;
            }

            if (((currentKeyboardState.IsKeyDown(player.GetKeyboardKey(InputHelper.KeyPress.ATTACK1)))
                    && (!oldKeyboardState.IsKeyDown(player.GetKeyboardKey(InputHelper.KeyPress.ATTACK1))))
                        
                        || (currentPadState.IsButtonDown(player.GetGamepadKey(InputHelper.KeyPress.ATTACK1))
                                && !oldPadState.IsButtonDown(player.GetGamepadKey(InputHelper.KeyPress.ATTACK1)))) {

                ATTACK_PRESS = true;
            }

            Entity grabbed = player.GetGrabInfo().grabbed;
            bool isLeftObstacleOnGrab = (grabbed != null && grabbed.GetCollisionInfo().IsLeft());
            bool isRightObstacleOnGrab = (grabbed != null && grabbed.GetCollisionInfo().IsRight());

            if (!player.IsInAnimationAction(Animation.Action.RUNNING)) {
                veloctiy = walkSpeed;
            } else {
                veloctiy = runSpeed;
            }

            ProcessAttack();
            ProcessJump();
            
            if (!ATTACK_PRESS && player.IsNonActionState()) {

                if (!DOWN && (currentKeyboardState.IsKeyDown(player.GetKeyboardKey(InputHelper.KeyPress.UP)) 
                                || currentPadState.IsButtonDown(Buttons.DPadUp) 
                                || currentPadState.IsButtonDown(Buttons.LeftThumbstickUp))) {

                    if (!RIGHT && (currentKeyboardState.IsKeyDown(player.GetKeyboardKey(InputHelper.KeyPress.LEFT)) 
                                       || currentPadState.IsButtonDown(Buttons.DPadLeft) 
                                       || currentPadState.IsButtonDown(Buttons.LeftThumbstickLeft))

                               && !isLeftObstacleOnGrab) {

                        inputDirection = InputDirection.UP_LEFT;
                        player.MoveX(veloctiy, -1);
                        player.SetIsLeft(true);
                        LEFT = true;

                    } else if (!LEFT && (currentKeyboardState.IsKeyDown(player.GetKeyboardKey(InputHelper.KeyPress.RIGHT)) 
                                            || currentPadState.IsButtonDown(Buttons.DPadRight) 
                                            || currentPadState.IsButtonDown(Buttons.LeftThumbstickRight))

                                     && !isRightObstacleOnGrab) {

                        inputDirection = InputDirection.UP_RIGHT;
                        player.MoveX(veloctiy, 1);
                        player.SetIsLeft(false);
                        RIGHT = true;

                    } else {
                        inputDirection = InputDirection.UP;
                    }

                    player.SetWalkState();
                    player.MoveZ(veloctiy, -1);
                    UP = true;

                } else if (!UP && (currentKeyboardState.IsKeyDown(player.GetKeyboardKey(InputHelper.KeyPress.DOWN)) 
                                        || currentPadState.IsButtonDown(Buttons.DPadDown) 
                                        || currentPadState.IsButtonDown(Buttons.LeftThumbstickDown))) {

                    if (!RIGHT && (currentKeyboardState.IsKeyDown(player.GetKeyboardKey(InputHelper.KeyPress.LEFT)) 
                                        || currentPadState.IsButtonDown(Buttons.DPadLeft) 
                                        || currentPadState.IsButtonDown(Buttons.LeftThumbstickLeft))
                               
                               && !isLeftObstacleOnGrab) {  

                        inputDirection = InputDirection.DOWN_LEFT;
                        player.MoveX(veloctiy, -1);
                        player.SetIsLeft(true);
                        LEFT = true;

                    } else if (!LEFT && (currentKeyboardState.IsKeyDown(player.GetKeyboardKey(InputHelper.KeyPress.RIGHT)) 
                                            || currentPadState.IsButtonDown(Buttons.DPadRight) 
                                            || currentPadState.IsButtonDown(Buttons.LeftThumbstickRight))
                                      
                                     && !isRightObstacleOnGrab) {

                        inputDirection = InputDirection.DOWN_RIGHT;
                        player.MoveX(veloctiy, 1);
                        player.SetIsLeft(false);
                        RIGHT = true;

                    } else {
                        inputDirection = InputDirection.DOWN;
                    }

                    player.SetWalkState();
                    player.MoveZ(veloctiy, 1);
                    DOWN = true;
                }
            }

            if (player.IsNonActionState() && !IsDirectionalPress()) {

                if (!RIGHT && (currentKeyboardState.IsKeyDown(player.GetKeyboardKey(InputHelper.KeyPress.LEFT)) 
                                  || currentPadState.IsButtonDown(Buttons.DPadLeft)
                                  || currentPadState.IsButtonDown(Buttons.LeftThumbstickLeft))
                           
                           && !isLeftObstacleOnGrab) {

                    inputDirection = InputDirection.LEFT;
                    
                    player.SetWalkState();
                    player.MoveX(veloctiy, -1);
                    player.SetIsLeft(true);
                    LEFT = true;

                } else if (!LEFT && (currentKeyboardState.IsKeyDown(player.GetKeyboardKey(InputHelper.KeyPress.RIGHT)) 
                                         || currentPadState.IsButtonDown(Buttons.DPadRight)
                                         || currentPadState.IsButtonDown(Buttons.LeftThumbstickRight))
                                
                                 && !isRightObstacleOnGrab) {

                    inputDirection = InputDirection.RIGHT; 
                    
                    player.SetWalkState();
                    player.MoveX(veloctiy, 1);
                    player.SetIsLeft(false);
                    RIGHT = true;
                }
            }
        }

        private void ProcessJump() {
            if (JUMP_PRESS && !player.InNegativeState()) {
                if (LEFT) {
                    player.SetJump(jumpHeight, -Math.Abs(veloctiy));

                } else if (RIGHT) {
                    player.SetJump(jumpHeight, Math.Abs(veloctiy));

                } else {
                    player.SetJump(jumpHeight);
                }
            }
        }

        private void ProcessAttack() {
            if (player.InSpecialAttack()) return;

            if (ATTACK_PRESS && player.ValidGrabItemState()) {
                player.SetAnimationState(Animation.State.PICKUP1);
            } else { 
                if (ATTACK_PRESS) {
                    EntityActions.DefaultAttack(player);
                }
            }

            if (ATTACK_PRESS && player.InGrabAttackState()) {
                InputHelper.KeyPress throwKey = InputHelper.KeyPress.RIGHT | InputHelper.KeyPress.DOWN_RIGHT | InputHelper.KeyPress.UP_RIGHT;
                
                if (player.GetDirX() > 0) {
                     throwKey = InputHelper.KeyPress.LEFT | InputHelper.KeyPress.DOWN_LEFT | InputHelper.KeyPress.UP_LEFT;
                }
          
                EntityActions.ThrowOrGrabAttack(player, heldState.IsKeyPressed(throwKey));
            } 
        }

        public Entity GetPlayer() {
            return player;
        }

        public InputDirection GetInputDirection() {
            return inputDirection;
        }

        public PlayerIndex GetPlayerIndex() {
            return playerIndex;
        }

        public bool IsInputDirection(InputDirection input) {
            return (inputDirection == input);
        }

        public bool IsDirectionalPress() {
            return (IsInputDirection(InputDirection.UP)
                        || IsInputDirection(InputDirection.DOWN)
                        || IsInputDirection(InputDirection.LEFT)
                        || IsInputDirection(InputDirection.RIGHT)
                        || IsInputDirection(InputDirection.DOWN_LEFT)
                        || IsInputDirection(InputDirection.DOWN_RIGHT)
                        || IsInputDirection(InputDirection.UP_LEFT)
                        || IsInputDirection(InputDirection.UP_RIGHT));
        }

        public KeyboardState GetOldKeyboardState() {
            return oldKeyboardState;
        }

        public void SetOldKeyboardState(KeyboardState state) {
            this.oldKeyboardState = state;
        }

        public KeyboardState GetCurrentKeyboardState() {
            return currentKeyboardState;
        }

        public void SetCurrentKeyboardState(KeyboardState state) {
            this.currentKeyboardState = state;
        }

        public GamePadState GetOldPadState() {
            return oldPadState;
        }

        public void SetOldPadState(GamePadState state) {
            this.oldPadState = state;
        }

        public GamePadState GetCurrentPadState() {
            return currentPadState;
        }

        public void SetCurrentPadState(GamePadState state) {
            this.currentPadState = state;
        }

        public InputBuffer GetPressedState() {
            return pressedState;
        }

        public InputBuffer GetReleasedState() {
            return releasedState;
        }

        public InputBuffer GetHeldState() {
            return heldState;
        }
    }
}