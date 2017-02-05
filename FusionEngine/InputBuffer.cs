using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FusionEngine {

    public class InputBuffer {
        private List<InputHelper.KeyPress> inputBuffer;
        public readonly float bufferTimeout = 400f;
        public readonly float mergeInputTime = 120f;
        private float lastInputTime = 0f;
        private float timeSinceLast = 0f;
        public readonly int MAX_BUFFER = 120;
        private InputHelper.ButtonState stateType;
        public List<InputHelper.KeyPress> inputState;
        public int currentStateStep;

        public InputBuffer(InputHelper.ButtonState stateType, float bufferTimeout = 400f) {
            inputBuffer = new List<InputHelper.KeyPress>(MAX_BUFFER);
            inputState = new List<InputHelper.KeyPress>(MAX_BUFFER);
            currentStateStep = 0;

            this.bufferTimeout = bufferTimeout;
            this.stateType = stateType;

            InitiateState();
        }

        public void ReadInputBuffer(GameTime gameTime, InputHelper.KeyPress currentButtonState, InputHelper.KeyPress currentDirectionState) {
            InputHelper.KeyPress pressedButton = InputHelper.KeyPress.NONE;

            int bufferStep = inputBuffer.Count - 1;
            float time = (float)gameTime.TotalGameTime.TotalMilliseconds;
            timeSinceLast = time - lastInputTime;

            if (timeSinceLast > bufferTimeout) {
                inputBuffer.Clear();
            }

            pressedButton |= currentButtonState;
            pressedButton |= currentDirectionState;

            bool mergeInput = (inputBuffer.Count > 0 && timeSinceLast < mergeInputTime);

            if (pressedButton != InputHelper.KeyPress.NONE) {
                if (mergeInput) {
                    inputBuffer[bufferStep] = inputBuffer[bufferStep] | pressedButton;
                    AddInputState(inputBuffer[bufferStep]);

                } else {
                    if (inputBuffer.Count == inputBuffer.Capacity) {
                        inputBuffer.RemoveAt(0);
                    }

                    inputBuffer.Add(pressedButton);
                    AddInputState(pressedButton);

                    lastInputTime = time;
                }
            }

            currentStateStep++;

            if (currentStateStep >= MAX_BUFFER - 1) {
                ClearInputState();
            }
        }

        private void AddInputState(InputHelper.KeyPress keyPress) {
            inputState[currentStateStep] = keyPress;
        }

        private void InitiateState() {
            for (int i = 0; i < MAX_BUFFER - 1; i++) {
                inputState.Add(InputHelper.KeyPress.NONE);
            }
        }

        private void ClearInputState() {
            for (int i = 0; i < inputState.Count - 1; i++) {
                inputState[i] = InputHelper.KeyPress.NONE;
            }

            currentStateStep = 0;
        }

        public int GetCurrentStateStep() {
            return currentStateStep;
        }

        public InputHelper.KeyPress GetCurrentInputState() {
            return GetCurrentInputState(currentStateStep - 1);
        }

        public InputHelper.KeyPress GetCurrentInputState(int index) {
            if (index < 0) return InputHelper.KeyPress.NONE;
            if (index > inputState.Count - 1) return InputHelper.KeyPress.NONE;

            return inputState[index];
        }

        public List<InputHelper.KeyPress> GetBuffer() {
            return inputBuffer;
        }

        public InputHelper.KeyPress GetBufferState(int index) {
            if (inputBuffer.Count == 0) return InputHelper.KeyPress.NONE;
            if (index > inputBuffer.Count - 1) return InputHelper.KeyPress.NONE;

            if (index < 0) index = 0;
            return inputBuffer[index];
        }

        public float GetLastInputTime() {
            return lastInputTime;
        }

        public InputHelper.ButtonState GetStateType() {
            return stateType;
        }
    }
}
