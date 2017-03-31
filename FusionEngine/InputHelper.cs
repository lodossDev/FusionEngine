using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FusionEngine {
    public static class InputHelper { 

        [Flags]
        public enum KeyPress  {
            NONE = 0,
            UP = 1,
            DOWN = 2,
            LEFT = 4,
            RIGHT = 8,
            UP_LEFT = UP | LEFT,
            UP_RIGHT = UP | RIGHT,
            DOWN_LEFT = DOWN | LEFT,
            DOWN_RIGHT = DOWN | RIGHT,

            ATTACK1 = 16,
            ATTACK2 = 32,
            ATTACK3 = 64,
            ATTACK4 = 128,
            SPECIAL = 256,
            JUMP = 512,

            START = 2048,
            PAUSE = 4096,

            ANY_DIRECTION = UP | DOWN | LEFT | RIGHT,
        }

        [Flags]
        public enum ButtonState {
            Pressed = 1,
            Released = 2,
            Held = 4
        }

        internal static readonly int NEGATIVE_EDGE_PRESS = 15;

        public class KeyState
        {
            private InputHelper.KeyPress key;
            private InputHelper.ButtonState state;
            private int negativeEdge = NEGATIVE_EDGE_PRESS;
            private float keyHeldTime = 0f;

            public KeyState(InputHelper.KeyPress key, InputHelper.ButtonState state, float keyHeldTime = 5, int negativeEdge = 15) {
                this.key = key;
                this.state = state;
                this.negativeEdge = negativeEdge;
                this.keyHeldTime = keyHeldTime;
            }

            public KeyState(InputHelper.KeyPress key, InputHelper.ButtonState state, int negativeEdge = 15) {
                this.key = key;
                this.state = state;
                this.negativeEdge = negativeEdge;
                this.keyHeldTime = 0f;
            }

            public KeyState(InputHelper.KeyPress key, InputHelper.ButtonState state) {
                this.key = key;
                this.state = state;
                this.keyHeldTime = 0f;
            }

            public InputHelper.KeyPress GetKey() {
                return key;
            } 

            public InputHelper.ButtonState GetState() {
                return state;
            }

            public int GetNegativeEdge() {
                return negativeEdge;
            }

            public float GetKeyHeldTime() {
                return keyHeldTime;
            }
        }

        public class CommandMove : IComparable<CommandMove> {
            private List<InputHelper.KeyState> moves;
            private string name;
            private double priority;
            private int currentMoveStep = 0;
            private float currentMoveTime = 0f;
            private float maxMoveTime = 500f;
            private int currentNegativeEdge = 0;
            private Animation.State animationState;
            private Animation.State? onHitState;


            public CommandMove(string name, Animation.State animationState, Animation.State? onHitState, 
                                        List<InputHelper.KeyState> moves, float maxMoveTime = 1000f, double priority = 1) {
                this.name = name;
                this.onHitState = onHitState;
                this.animationState = animationState;
                this.moves = moves;
                this.maxMoveTime = maxMoveTime;
                this.priority = priority;
            }

            public CommandMove(string name, Animation.State animationState, List<InputHelper.KeyState> moves, 
                                    float maxMoveTime = 1000f, double priority = 1) : this(name, animationState, null, moves, maxMoveTime, priority) {
               
            }

            public string GetName() {
                return name;
            }

            public List<InputHelper.KeyState> GetMoves() {
                return moves;
            }

            public double GetPriority() {
                return priority;
            }

            public Animation.State? GetOnHitState() {
                return onHitState;
            }

            public Animation.State GetAnimationState() {
                return animationState;
            }

            public InputHelper.KeyState GetPreviousMove() {
                int temp = currentMoveStep - 1;
                if (temp < 0) return null;

                return moves[temp];
            }

            public InputHelper.KeyState GetCurrentMove() {
                return moves[currentMoveStep];
            }

            public int GetCurrentMoveStep() {
                return currentMoveStep;
            }

            public int GetCurrentNegativeEdgeExpire() {
                return GetCurrentMove().GetNegativeEdge();
            }

            public int GetNegativeCount() {
                return currentNegativeEdge;
            }

            public bool IsMaxNegativeReached() {
                return (GetNegativeCount() >= GetCurrentNegativeEdgeExpire());
            }

            public void ResetNegativeEdge() {
                currentNegativeEdge = 0;
            }

            public void IncrementNegativeCount() {
                currentNegativeEdge++;
            }

            public void Next() {
                currentNegativeEdge = 0;
                currentMoveStep++;
            }

            public void Reset() {
                currentNegativeEdge = 0;
                currentMoveTime = 0f;
                currentMoveStep = 0;
            }

            public bool IsComplete() {
                return (currentMoveStep > moves.Count - 1);
            }
            
            public void Update(GameTime gameTime)  {
                if (currentMoveStep > 0) {
                    if (currentMoveStep > moves.Count - 1) {
                        currentMoveStep = moves.Count - 1;
                    }

                    currentMoveTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (currentMoveTime >= maxMoveTime) {
                        Reset();
                    }
                }
            }

            public int CompareTo(CommandMove other) {
                if (other == null) {
                    return 0;
                }

                return GetPriority().CompareTo(other.GetPriority());
            }
        }

        public static InputHelper.KeyPress GetPressedDirections(Dictionary<InputHelper.KeyPress, Keys> keyboardSettings, 
                                                                GamePadState oldPadState, KeyboardState oldKeyboardState,
                                                                GamePadState newPadState, KeyboardState newKeyboardState)
        {
            InputHelper.KeyPress directions = InputHelper.KeyPress.NONE;

            // Get vertical direction.
            if (oldPadState.IsButtonUp(Buttons.DPadUp) && newPadState.IsButtonDown(Buttons.DPadUp) 
                    || oldPadState.IsButtonUp(Buttons.LeftThumbstickUp) && newPadState.IsButtonDown(Buttons.LeftThumbstickUp) 
                    || oldKeyboardState.IsKeyUp(keyboardSettings[KeyPress.UP]) && newKeyboardState.IsKeyDown(keyboardSettings[KeyPress.UP]))
            {
                directions |= InputHelper.KeyPress.UP;
            }
            else if (oldPadState.IsButtonUp(Buttons.DPadDown) && newPadState.IsButtonDown(Buttons.DPadDown)
                        || oldPadState.IsButtonUp(Buttons.LeftThumbstickDown) && newPadState.IsButtonDown(Buttons.LeftThumbstickDown)
                        || oldKeyboardState.IsKeyUp(keyboardSettings[KeyPress.DOWN]) && newKeyboardState.IsKeyDown(keyboardSettings[KeyPress.DOWN]))
            {
                directions |= InputHelper.KeyPress.DOWN;
            }

            // Comebine with horizontal direction.
            if (oldPadState.IsButtonUp(Buttons.DPadLeft) && newPadState.IsButtonDown(Buttons.DPadLeft)
                    || oldPadState.IsButtonUp(Buttons.LeftThumbstickLeft) && newPadState.IsButtonDown(Buttons.LeftThumbstickLeft)
                    || oldKeyboardState.IsKeyUp(keyboardSettings[KeyPress.LEFT]) && newKeyboardState.IsKeyDown(keyboardSettings[KeyPress.LEFT]))
            {
                directions |= InputHelper.KeyPress.LEFT;
            }
            else if (oldPadState.IsButtonUp(Buttons.DPadRight) && newPadState.IsButtonDown(Buttons.DPadRight)
                        || oldPadState.IsButtonUp(Buttons.LeftThumbstickRight) && newPadState.IsButtonDown(Buttons.LeftThumbstickRight)
                        || oldKeyboardState.IsKeyUp(keyboardSettings[KeyPress.RIGHT]) && newKeyboardState.IsKeyDown(keyboardSettings[KeyPress.RIGHT]))
            {
                directions |= InputHelper.KeyPress.RIGHT;
            }

            return directions;
        }

        public static InputHelper.KeyPress GetReleasedDirections(Dictionary<InputHelper.KeyPress, Keys> keyboardSettings,
                                                                 GamePadState oldPadState, KeyboardState oldKeyboardState,
                                                                 GamePadState newPadState, KeyboardState newKeyboardState)
        {
            InputHelper.KeyPress directions = InputHelper.KeyPress.NONE;

            // Get vertical direction.
            if (oldPadState.IsButtonDown(Buttons.DPadUp) && newPadState.IsButtonUp(Buttons.DPadUp)
                    || oldPadState.IsButtonDown(Buttons.LeftThumbstickUp) && newPadState.IsButtonUp(Buttons.LeftThumbstickUp)
                    || oldKeyboardState.IsKeyDown(keyboardSettings[KeyPress.UP]) && newKeyboardState.IsKeyUp(keyboardSettings[KeyPress.UP]))
            {
                directions |= InputHelper.KeyPress.UP;
            }
            else if (oldPadState.IsButtonDown(Buttons.DPadDown) && newPadState.IsButtonUp(Buttons.DPadDown)
                        || oldPadState.IsButtonDown(Buttons.LeftThumbstickDown) && newPadState.IsButtonUp(Buttons.LeftThumbstickDown)
                        || oldKeyboardState.IsKeyDown(keyboardSettings[KeyPress.DOWN]) && newKeyboardState.IsKeyUp(keyboardSettings[KeyPress.DOWN]))
            {
                directions |= InputHelper.KeyPress.DOWN;
            }

            // Comebine with horizontal direction.
            if (oldPadState.IsButtonDown(Buttons.DPadLeft) && newPadState.IsButtonUp(Buttons.DPadLeft)
                    || oldPadState.IsButtonDown(Buttons.LeftThumbstickLeft) && newPadState.IsButtonUp(Buttons.LeftThumbstickLeft)
                    || oldKeyboardState.IsKeyDown(keyboardSettings[KeyPress.LEFT]) && newKeyboardState.IsKeyUp(keyboardSettings[KeyPress.LEFT]))
            {
                directions |= InputHelper.KeyPress.LEFT;
            }
            else if (oldPadState.IsButtonDown(Buttons.DPadRight) && newPadState.IsButtonUp(Buttons.DPadRight)
                        || oldPadState.IsButtonDown(Buttons.LeftThumbstickRight) && newPadState.IsButtonUp(Buttons.LeftThumbstickRight)
                        || oldKeyboardState.IsKeyDown(keyboardSettings[KeyPress.RIGHT]) && newKeyboardState.IsKeyUp(keyboardSettings[KeyPress.RIGHT]))
            {
                directions |= InputHelper.KeyPress.RIGHT;
            }

            return directions;
        }

        public static InputHelper.KeyPress GetHeldDirections(Dictionary<InputHelper.KeyPress, Keys> keyboardSettings,
                                                             GamePadState newPadState, KeyboardState newKeyboardState)
        {
            InputHelper.KeyPress directions = InputHelper.KeyPress.NONE;

            // Get vertical direction.
            if (newPadState.IsButtonDown(Buttons.DPadUp)
                    || newPadState.IsButtonDown(Buttons.LeftThumbstickUp)
                    || newKeyboardState.IsKeyDown(keyboardSettings[KeyPress.UP]))
            {
                directions |= InputHelper.KeyPress.UP;
            }
            else if (newPadState.IsButtonDown(Buttons.DPadDown)
                        || newPadState.IsButtonDown(Buttons.LeftThumbstickDown)
                        || newKeyboardState.IsKeyDown(keyboardSettings[KeyPress.DOWN]))
            {
                directions |= InputHelper.KeyPress.DOWN;
            }

            // Comebine with horizontal direction.
            if (newPadState.IsButtonDown(Buttons.DPadLeft)
                    || newPadState.IsButtonDown(Buttons.LeftThumbstickLeft)
                    || newKeyboardState.IsKeyDown(keyboardSettings[KeyPress.LEFT]))
            {
                directions |= InputHelper.KeyPress.LEFT;
            }
            else if (newPadState.IsButtonDown(Buttons.DPadRight)
                        || newPadState.IsButtonDown(Buttons.LeftThumbstickRight)
                        || newKeyboardState.IsKeyDown(keyboardSettings[KeyPress.RIGHT]))
            {
                directions |= InputHelper.KeyPress.RIGHT;
            }

            return directions;
        }

        public static InputHelper.KeyPress GetPressedButtons(Dictionary<InputHelper.KeyPress, Keys> keyboardBtnsOnly,
                                                             Dictionary<InputHelper.KeyPress, Buttons> gamepadBtnsOnly,
                                                             GamePadState oldPadState, KeyboardState oldKeyboardState, 
                                                             GamePadState newPadState, KeyboardState newKeyboardState)
        {
            InputHelper.KeyPress buttons = InputHelper.KeyPress.NONE;

            foreach (var btn in keyboardBtnsOnly) {
                KeyPress press = btn.Key;
                Keys key = btn.Value;

                // Check the keyboard for presses.
                if (oldKeyboardState.IsKeyUp(key) && newKeyboardState.IsKeyDown(key)) {
                    // Use a bitwise-or to accumulate button presses.
                    buttons |= press;
                }
            }

            foreach (var btn in gamepadBtnsOnly) {
                KeyPress press = btn.Key;
                Buttons key = btn.Value;

                // Check the game pad for presses.
                if (oldPadState.IsButtonUp(key) && newPadState.IsButtonDown(key)) {
                    // Use a bitwise-or to accumulate button presses.
                    buttons |= press;
                }
            }

            return buttons;
        }

        public static InputHelper.KeyPress GetReleasedButtons(Dictionary<InputHelper.KeyPress, Keys> keyboardBtnsOnly,
                                                              Dictionary<InputHelper.KeyPress, Buttons> gamepadBtnsOnly,
                                                              GamePadState oldPadState, KeyboardState oldKeyboardState,
                                                              GamePadState newPadState, KeyboardState newKeyboardState)
        {
            InputHelper.KeyPress buttons = InputHelper.KeyPress.NONE;

            foreach (var btn in keyboardBtnsOnly) {
                KeyPress press = btn.Key;
                Keys key = btn.Value;

                // Check the keyboard for presses.
                if (oldKeyboardState.IsKeyDown(key) && newKeyboardState.IsKeyUp(key)) {
                    // Use a bitwise-or to accumulate button presses.
                    buttons |= press;
                }
            }

            foreach (var btn in gamepadBtnsOnly) {
                KeyPress press = btn.Key;
                Buttons key = btn.Value;

                // Check the game pad for presses.
                if (oldPadState.IsButtonDown(key) && newPadState.IsButtonUp(key)) {
                    // Use a bitwise-or to accumulate button presses.
                    buttons |= press;
                }
            }
            
            return buttons;
        }

        public static InputHelper.KeyPress GetHeldButtons(Dictionary<InputHelper.KeyPress, Keys> keyboardBtnsOnly,
                                                          Dictionary<InputHelper.KeyPress, Buttons> gamepadBtnsOnly,
                                                          GamePadState newPadState, KeyboardState newKeyboardState)
        {
            InputHelper.KeyPress buttons = InputHelper.KeyPress.NONE;

            foreach (var btn in keyboardBtnsOnly) {
                KeyPress press = btn.Key;
                Keys key = btn.Value;

                // Check the keyboard for presses.
                if (newKeyboardState.IsKeyDown(key)) {
                    // Use a bitwise-or to accumulate button presses.
                    buttons |= press;
                }
            }

            foreach (var btn in gamepadBtnsOnly) {
                KeyPress press = btn.Key;
                Buttons key = btn.Value;

                // Check the game pad for presses.
                if (newPadState.IsButtonDown(key)) {
                    // Use a bitwise-or to accumulate button presses.
                    buttons |= press;
                }
            }

            return buttons;
        }
    }
}
