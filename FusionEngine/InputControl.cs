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

/**
 * *
 * 
 *  GAME PAD BUTTON STATE AXIS
 * X= -1 - left , 1 - right
 * Y = -1 - down, 1 - up
 * 
 **/

namespace FusionEngine {

    public class InputControl {
        public enum InputDirection {NONE, UP, DOWN, LEFT, RIGHT, UP_LEFT, UP_RIGHT, DOWN_LEFT, DOWN_RIGHT}
        public bool UP, DOWN, LEFT, RIGHT, JUMP_PRESS, ATTACK_PRESS;

        private Entity player;
        private InputDirection inputDirection;
        private PlayerIndex playerIndex;

        public KeyboardState oldKeyboardState, currentKeyboardState;
        public GamePadState oldPadState, currentPadState;

        private InputBuffer pressedState;
        private InputBuffer releasedState;
        private InputBuffer heldState;

        private float walkSpeed = 5f;
        private float runSpeed = 15f;
        private float veloctiy = 5f;
        private float jumpHeight = -15f;


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

            if (!player.IsInAnimationAction(Animation.Action.RUNNING)) {
                veloctiy = walkSpeed;
            } else {
                veloctiy = runSpeed;
            }

            ProcessAttack();
            ProcessJump();
            
            if (player.IsNonActionState()) {

                if (!DOWN && (currentKeyboardState.IsKeyDown(player.GetKeyboardKey(InputHelper.KeyPress.UP)) 
                                || currentPadState.IsButtonDown(Buttons.DPadUp) 
                                || currentPadState.IsButtonDown(Buttons.LeftThumbstickUp))) {

                    if (!RIGHT && (currentKeyboardState.IsKeyDown(player.GetKeyboardKey(InputHelper.KeyPress.LEFT)) 
                                       || currentPadState.IsButtonDown(Buttons.DPadLeft) 
                                       || currentPadState.IsButtonDown(Buttons.LeftThumbstickLeft))) {

                        inputDirection = InputDirection.UP_LEFT;
                        player.MoveX(veloctiy, -1);
                        player.SetIsLeft(true);
                        LEFT = true;

                    } else if (!LEFT && (currentKeyboardState.IsKeyDown(player.GetKeyboardKey(InputHelper.KeyPress.RIGHT)) 
                                            || currentPadState.IsButtonDown(Buttons.DPadRight) 
                                            || currentPadState.IsButtonDown(Buttons.LeftThumbstickRight))) {

                        inputDirection = InputDirection.UP_RIGHT;
                        player.MoveX(veloctiy, 1);
                        player.SetIsLeft(false);
                        RIGHT = true;

                    } else {
                        inputDirection = InputDirection.UP;
                    }

                    if (!player.IsInAnimationAction(Animation.Action.RUNNING)) {
                        player.SetAnimationState(Animation.State.WALK_TOWARDS);
                    } 

                    player.MoveZ(veloctiy, -1);
                    UP = true;

                } else if (!UP && (currentKeyboardState.IsKeyDown(player.GetKeyboardKey(InputHelper.KeyPress.DOWN)) 
                                        || currentPadState.IsButtonDown(Buttons.DPadDown) 
                                        || currentPadState.IsButtonDown(Buttons.LeftThumbstickDown))) {

                    if (!RIGHT && (currentKeyboardState.IsKeyDown(player.GetKeyboardKey(InputHelper.KeyPress.LEFT)) 
                                        || currentPadState.IsButtonDown(Buttons.DPadLeft) 
                                        || currentPadState.IsButtonDown(Buttons.LeftThumbstickLeft))) {  

                        inputDirection = InputDirection.DOWN_LEFT;
                        player.MoveX(veloctiy, -1);
                        player.SetIsLeft(true);
                        LEFT = true;

                    } else if (!LEFT && (currentKeyboardState.IsKeyDown(player.GetKeyboardKey(InputHelper.KeyPress.RIGHT)) 
                                            || currentPadState.IsButtonDown(Buttons.DPadRight) 
                                            || currentPadState.IsButtonDown(Buttons.LeftThumbstickRight))) {

                        inputDirection = InputDirection.DOWN_RIGHT;
                        player.MoveX(veloctiy, 1);
                        player.SetIsLeft(false);
                        RIGHT = true;

                    } else {
                        inputDirection = InputDirection.DOWN;
                    }

                    if (!player.IsInAnimationAction(Animation.Action.RUNNING)) {
                        player.SetAnimationState(Animation.State.WALK_TOWARDS);
                    }

                    player.MoveZ(veloctiy, 1);
                    DOWN = true;
                }
            }

            if (player.IsNonActionState() && !IsDirectionalPress()) {

                if (!RIGHT && (currentKeyboardState.IsKeyDown(player.GetKeyboardKey(InputHelper.KeyPress.LEFT)) 
                                  || currentPadState.IsButtonDown(Buttons.DPadLeft)
                                  || currentPadState.IsButtonDown(Buttons.LeftThumbstickLeft))) {

                    inputDirection = InputDirection.LEFT;
                    
                    if (!player.IsInAnimationAction(Animation.Action.RUNNING)) {
                        player.SetAnimationState(Animation.State.WALK_TOWARDS);
                    }

                    player.MoveX(veloctiy, -1);
                    player.SetIsLeft(true);
                    LEFT = true;

                } else if (!LEFT && (currentKeyboardState.IsKeyDown(player.GetKeyboardKey(InputHelper.KeyPress.RIGHT)) 
                                         || currentPadState.IsButtonDown(Buttons.DPadRight)
                                         || currentPadState.IsButtonDown(Buttons.LeftThumbstickRight))) {

                    inputDirection = InputDirection.RIGHT; 
                    
                    if (!player.IsInAnimationAction(Animation.Action.RUNNING)) {
                        player.SetAnimationState(Animation.State.WALK_TOWARDS);
                    }
                       
                    player.MoveX(veloctiy, 1);
                    player.SetIsLeft(false);
                    RIGHT = true;
                }
            }
        }

        private void ProcessJump() {
            if (JUMP_PRESS) {
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
            if (ATTACK_PRESS) {

                if (!player.IsToss()) {
                    player.ProcessAttackChainStep();

                } else {
                    if (!player.IsInAnimationAction(Animation.Action.ATTACKING)
                            && !player.IsInAnimationAction(Animation.Action.RECOVERY)
                            && player.InAir()) {

                        if ((double)player.GetTossInfo().velocity.X == 0.0) {
                            player.SetAnimationState(Animation.State.JUMP_ATTACK1);
                        }
                        else {
                            player.SetAnimationState(Animation.State.JUMP_TOWARD_ATTACK1);
                        }
                    }
                }
            }
        }

        public void ReadPressedInputBuffer(GameTime gameTime) {
            InputHelper.KeyPress pressedButtonState = InputHelper.KeyPress.NONE;
            InputHelper.KeyPress pressedDirectionState = InputHelper.KeyPress.NONE;

            pressedButtonState = InputHelper.GetPressedButtons(player.GetKeyboardButtonsOnly(), player.GetGamepadButtonsOnly(), oldPadState, oldKeyboardState, currentPadState, currentKeyboardState);
            pressedDirectionState = InputHelper.GetPressedDirections(player.GetKeyboardSettings(), oldPadState, oldKeyboardState, currentPadState, currentKeyboardState);

            pressedState.ReadInputBuffer(gameTime, pressedButtonState, pressedDirectionState);
        }

        public void ReadReleasedInputBuffer(GameTime gameTime) {
            InputHelper.KeyPress releasedButtonState = InputHelper.KeyPress.NONE;
            InputHelper.KeyPress releasedDirectionState = InputHelper.KeyPress.NONE;

            releasedButtonState = InputHelper.GetReleasedButtons(player.GetKeyboardButtonsOnly(), player.GetGamepadButtonsOnly(), oldPadState, oldKeyboardState, currentPadState, currentKeyboardState);
            releasedDirectionState = InputHelper.GetReleasedDirections(player.GetKeyboardSettings(), oldPadState, oldKeyboardState, currentPadState, currentKeyboardState);

            releasedState.ReadInputBuffer(gameTime, releasedButtonState, releasedDirectionState);
        }

        public void ReadHeldInputBuffer(GameTime gameTime) {
            InputHelper.KeyPress heldButtonState = InputHelper.KeyPress.NONE;
            InputHelper.KeyPress heldDirectionState = InputHelper.KeyPress.NONE;

            heldButtonState = InputHelper.GetHeldButtons(player.GetKeyboardButtonsOnly(), player.GetGamepadButtonsOnly(), currentPadState, currentKeyboardState);
            heldDirectionState = InputHelper.GetHeldDirections(player.GetKeyboardSettings(), currentPadState, currentKeyboardState);

            heldState.ReadInputBuffer(gameTime, heldButtonState, heldDirectionState);
        }

        public void Update(GameTime gameTime) {
            currentKeyboardState = Keyboard.GetState();
            currentPadState = GamePad.GetState(playerIndex);
            
            UpdateDefaultControls(gameTime);
            
            ReadPressedInputBuffer(gameTime);
            ReadHeldInputBuffer(gameTime);
            ReadReleasedInputBuffer(gameTime);

            if (IsInputDirection(InputDirection.NONE)) {
                player.ResetX();
                player.ResetZ();

                player.ResetToIdle(gameTime);
            }

            oldKeyboardState = currentKeyboardState;
            oldPadState = currentPadState;
        }

        private void ResetBuffers() {
            pressedState.GetBuffer().Clear();
            releasedState.GetBuffer().Clear();
            heldState.GetBuffer().Clear();
        }

        private InputBuffer GetNextBuffer(InputHelper.KeyState currentKeyPress) {
            InputBuffer currentBuffer = null;

            if (currentKeyPress.GetState() == InputHelper.ButtonState.Pressed) {
                currentBuffer = pressedState;

            } else if (currentKeyPress.GetState() == InputHelper.ButtonState.Released) {
                currentBuffer = releasedState;

            } else if (currentKeyPress.GetState() == InputHelper.ButtonState.Held) {
                currentBuffer = heldState;
            }

            return currentBuffer;
        }

        private void checkHeld(InputHelper.CommandMove command, InputHelper.KeyState currentKeyState) {
            int held = 0;
            //Debug.WriteLine("HELD KEY: " + currentKeyState.GetState());
            currentKeyState = command.GetCurrentMove();

            if (command.IsMaxNegativeReached() == true) {
                command.Reset();
                held = 0;
            }

            if (releasedState.GetCurrentInputState() != InputHelper.KeyPress.NONE) {

                if (releasedState.GetCurrentInputState() == currentKeyState.GetKey()
                        || (releasedState.GetCurrentInputState(releasedState.GetCurrentStateStep() - 2) == currentKeyState.GetKey()
                                && releasedState.GetCurrentInputState() != currentKeyState.GetKey())) {

                    command.Reset();
                }
            }

            for (int i = 0; i < heldState.GetBuffer().Count - 1; i++) {

                bool reset = (releasedState.GetBuffer().Count >= i + 2
                                    && releasedState.GetBuffer()[i + 1] == currentKeyState.GetKey());

                if (reset) {
                    held = 0;
                    command.IncrementNegativeCount();
                    break;
                }

                if (heldState.GetBuffer()[i + 1] == currentKeyState.GetKey()) {
                    held++;
                    command.ResetNegativeEdge();

                } else {
                    held = 0;
                    command.Reset();
                    break;
                }
            }

            //Debug.WriteLine("HELD COUNT: " + held);
            //Debug.WriteLine("HELD TIME: " + currentKeyState.GetKeyHeldTime());

            if (held >= currentKeyState.GetKeyHeldTime()) {
                command.Next();

            } else {
                command.IncrementNegativeCount();
            }
        }

        public bool Matches(InputHelper.CommandMove command) {
            InputHelper.KeyState previousKeyState = command.GetPreviousMove();
            InputHelper.KeyState currentKeyState = command.GetCurrentMove();
            InputBuffer currentBuffer = GetNextBuffer(currentKeyState);

            if (currentKeyState.GetState() != InputHelper.ButtonState.Held) {
                if (command.IsMaxNegativeReached() == true) {
                    command.Reset();
                    return false;
                }
                
                if (currentBuffer.GetCurrentInputState() == currentKeyState.GetKey()) {
                    command.Next();

                    if (command.GetCurrentMoveStep() >= command.GetMoves().Count - 1) {
                        currentKeyState = command.GetMoves()[command.GetMoves().Count - 1];

                    } else {
                        currentKeyState = command.GetCurrentMove();
                    }

                    currentBuffer = GetNextBuffer(currentKeyState);
                    //Debug.WriteLine("NEXT BUFFER: " + currentKeyState.GetState());
                } else {
                    command.IncrementNegativeCount();
                }
            } else {
                //Debug.WriteLine("IN HELD");
                checkHeld(command, currentKeyState);
            }

            //Debug.WriteLine("CURRENTMOVE STEP: " + command.GetCurrentMoveStep());

            if (command.IsComplete()) {
                //Debug.WriteLine("IS COMPLETE");
                command.Reset();
                ResetBuffers();
                return true;
            }

            return false;
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
