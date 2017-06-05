using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FusionEngine {

    public class InputManager : Manager {
        private Dictionary<Entity, InputControl> controllMapEntity;

        public InputManager() : base() {
            controllMapEntity = new Dictionary<Entity, InputControl>();
        }

        public void AddControl(Entity entity, PlayerIndex playerIndex) {
            InputControl inputControl = new InputControl(entity, playerIndex);
            controllMapEntity.Add(entity, inputControl);

            this.AddEntity(entity);
        }

        private void ProcessInputCommands(GameTime gameTime, InputControl inputControl, Entity entity) {
            List<InputHelper.CommandMove> commandMoves = entity.GetCommandMoves();
            commandMoves.Sort();

            entity.UpdateCommandMoves(gameTime);

            foreach (InputHelper.CommandMove command in commandMoves) {

                if (Matches(inputControl, command) && command.CanExecute()) {
                    Animation.State? state;

                    if (entity.HasHit() && command.GetOnHitState() != null) { 
                        state = command.GetOnHitState();
                    } else {
                        state = command.GetAnimationState();
                    }

                    entity.SetAnimationState(state);
                    entity.OnCommandMoveComplete(command);
                    break;
                }
            }
        }

        public void Update(GameTime gameTime) {

            for (int i = 0; i < entities.Count; i++) {
                Entity entity = entities[i];

                if (controllMapEntity.ContainsKey(entity)) { 
                    InputControl inputControl = controllMapEntity[entity];
                    inputControl.SetCurrentKeyboardState(Keyboard.GetState());
                    inputControl.SetCurrentPadState(GamePad.GetState(inputControl.GetPlayerIndex()));
                    
                    ReadPressedInputBuffer(gameTime, inputControl);
                    ReadHeldInputBuffer(gameTime, inputControl);
                    ReadReleasedInputBuffer(gameTime, inputControl);

                    ProcessInputCommands(gameTime, inputControl, entity);
                    inputControl.UpdateDefaultControls(gameTime);

                    if (inputControl.IsInputDirection(InputControl.InputDirection.NONE)) {
                        entity.StopMovement();
                        entity.ResetToIdle(gameTime);
                    }
                
                    inputControl.SetOldKeyboardState(inputControl.GetCurrentKeyboardState());
                    inputControl.SetOldPadState(inputControl.GetCurrentPadState());
                }
            }
        }

        public InputControl GetInputControl(Entity entity) {
            return controllMapEntity[entity];
        }

        public void ReadPressedInputBuffer(GameTime gameTime, InputControl inputControl) {
            InputHelper.KeyPress pressedButtonState = InputHelper.KeyPress.NONE;
            InputHelper.KeyPress pressedDirectionState = InputHelper.KeyPress.NONE;

            pressedButtonState = InputHelper.GetPressedButtons(inputControl.GetPlayer().GetKeyboardButtonsOnly(), 
                                                                    inputControl.GetPlayer().GetGamepadButtonsOnly(), 
                                                                    inputControl.GetOldPadState(), inputControl.GetOldKeyboardState(), 
                                                                    inputControl.GetCurrentPadState(), inputControl.GetCurrentKeyboardState());

            pressedDirectionState = InputHelper.GetPressedDirections(inputControl.GetPlayer().GetKeyboardSettings(),  
                                                                        inputControl.GetOldPadState(), inputControl.GetOldKeyboardState(), 
                                                                        inputControl.GetCurrentPadState(), inputControl.GetCurrentKeyboardState());

            inputControl.GetPressedState().ReadInputBuffer(gameTime, pressedButtonState, pressedDirectionState);
        }

        public void ReadReleasedInputBuffer(GameTime gameTime, InputControl inputControl) {
            InputHelper.KeyPress releasedButtonState = InputHelper.KeyPress.NONE;
            InputHelper.KeyPress releasedDirectionState = InputHelper.KeyPress.NONE;

            releasedButtonState = InputHelper.GetReleasedButtons(inputControl.GetPlayer().GetKeyboardButtonsOnly(), 
                                                                      inputControl.GetPlayer().GetGamepadButtonsOnly(),  
                                                                      inputControl.GetOldPadState(), inputControl.GetOldKeyboardState(), 
                                                                      inputControl.GetCurrentPadState(), inputControl.GetCurrentKeyboardState());

            releasedDirectionState = InputHelper.GetReleasedDirections(inputControl.GetPlayer().GetKeyboardSettings(), 
                                                                            inputControl.GetOldPadState(), inputControl.GetOldKeyboardState(), 
                                                                            inputControl.GetCurrentPadState(), inputControl.GetCurrentKeyboardState());

            inputControl.GetReleasedState().ReadInputBuffer(gameTime, releasedButtonState, releasedDirectionState);
        }

        public void ReadHeldInputBuffer(GameTime gameTime, InputControl inputControl) {
            InputHelper.KeyPress heldButtonState = InputHelper.KeyPress.NONE;
            InputHelper.KeyPress heldDirectionState = InputHelper.KeyPress.NONE;

            heldButtonState = InputHelper.GetHeldButtons(inputControl.GetPlayer().GetKeyboardButtonsOnly(), inputControl.GetPlayer().GetGamepadButtonsOnly(), inputControl.GetCurrentPadState(), inputControl.GetCurrentKeyboardState());
            heldDirectionState = InputHelper.GetHeldDirections(inputControl.GetPlayer().GetKeyboardSettings(), inputControl.GetCurrentPadState(), inputControl.GetCurrentKeyboardState());

            inputControl.GetHeldState().ReadInputBuffer(gameTime, heldButtonState, heldDirectionState);
        }

        private void ResetBuffers(InputControl inputControl) {
            inputControl.GetPressedState().GetBuffer().Clear();
            inputControl.GetReleasedState().GetBuffer().Clear();
            inputControl.GetHeldState().GetBuffer().Clear();
        }

        private InputBuffer GetNextBuffer(InputControl inputControl, InputHelper.KeyState currentKeyPress) {
            InputBuffer currentBuffer = null;

            if (currentKeyPress.GetState() == InputHelper.ButtonState.Pressed) {
                currentBuffer = inputControl.GetPressedState();

            } else if (currentKeyPress.GetState() == InputHelper.ButtonState.Released) {
                currentBuffer = inputControl.GetReleasedState();

            } else if (currentKeyPress.GetState() == InputHelper.ButtonState.Held) {
                currentBuffer = inputControl.GetHeldState();
            }

            return currentBuffer;
        }

        private void checkHeld(InputControl inputControl, InputHelper.CommandMove command, InputHelper.KeyState currentKeyState) {
            int held = 0;
            //Debug.WriteLine("HELD KEY: " + currentKeyState.GetState());
            currentKeyState = command.GetCurrentMove();

            if (command.IsMaxNegativeReached() == true) {
                command.Reset();
                held = 0;
            }

            if (inputControl.GetReleasedState().GetCurrentInputState() != InputHelper.KeyPress.NONE) {

                if (inputControl.GetReleasedState().IsCurrentPress(currentKeyState.GetKey())
                        || ((inputControl.GetReleasedState().GetCurrentInputState(inputControl.GetReleasedState().GetCurrentStateStep() - 2) & currentKeyState.GetKey()) != 0
                                && !inputControl.GetReleasedState().IsKeyPressed(currentKeyState.GetKey()))) {

                    command.Reset();
                }
            }

            for (int i = 0; i < inputControl.GetHeldState().GetBuffer().Count - 1; i++) {

                bool reset = (inputControl.GetReleasedState().GetBuffer().Count >= i + 2
                                    && (inputControl.GetReleasedState().GetBuffer()[i + 1] & currentKeyState.GetKey()) != 0);

                if (reset) {
                    held = 0;
                    command.IncrementNegativeCount();
                    break;
                }

                if ((inputControl.GetHeldState().GetBuffer()[i + 1] & currentKeyState.GetKey()) != 0) {
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

        public bool Matches(InputControl inputControl, InputHelper.CommandMove command) {
            InputHelper.KeyState previousKeyState = command.GetPreviousMove();
            InputHelper.KeyState currentKeyState = command.GetCurrentMove();
            InputBuffer currentBuffer = GetNextBuffer(inputControl, currentKeyState);

            if (currentKeyState.GetState() != InputHelper.ButtonState.Held) {

                if (command.IsMaxNegativeReached() == true) {
                    command.Reset();
                    return false;
                }
                
                if (currentBuffer.IsCurrentPress(currentKeyState.GetKey())) {
                    command.Next();

                    if (command.GetCurrentMoveStep() >= command.GetMoves().Count - 1) {
                        currentKeyState = command.GetMoves()[command.GetMoves().Count - 1];

                    } else {
                        currentKeyState = command.GetCurrentMove();
                    }

                    currentBuffer = GetNextBuffer(inputControl, currentKeyState);
                    //Debug.WriteLine("NEXT BUFFER: " + currentKeyState.GetState());
                } 
            } else {
                //Debug.WriteLine("IN HELD");
                checkHeld(inputControl, command, currentKeyState);
            }

            //Debug.WriteLine("CURRENTMOVE STEP: " + command.GetCurrentMoveStep());

            if (command.IsComplete()) {
                //Debug.WriteLine("IS COMPLETE");
                command.Reset();
                ResetBuffers(inputControl);
                return true;
            }

            return false;
        }
    }
}
