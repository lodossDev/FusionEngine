using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FusionEngine {

    public class AiState {

        public interface IState {
            void OnEnter();
            void Update(GameTime gameTime);
            void OnExit();
        }

        public class StateMachine {
            Dictionary<string, IState> states;
            private IState currentState;
            private string stateId;


            public StateMachine() {
                states = new Dictionary<string, IState>();
            }

            public void Add(string id, IState state) {
                states.Add(id, state);
            }

            public void Remove(string id) {
                states.Remove(id);
            }

            public void Clear() {
                states.Clear();
            }

            public void Change(string id) {
                if (currentState != null) {
                    currentState.OnExit();
                }

                if (states.ContainsKey(id)) {
                    IState next = states[id];
                    next.OnEnter();
                    currentState = next;
                    stateId = id;
                }
            }

            public void Update(GameTime gameTime) {
                if (currentState != null) {
                    currentState.Update(gameTime);
                }
            }

            public String GetCurrentStateId() {
                return stateId;
            }
        }
    }
}
