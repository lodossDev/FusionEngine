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

        public void Update(GameTime gameTime) {

            for (int i = 0; i < entities.Count; i++) {
                Entity entity = entities[i];

                List<InputHelper.CommandMove> commandMoves = entity.GetCommandMoves();
                commandMoves.Sort();

                if (controllMapEntity.ContainsKey(entity)) { 
                    InputControl inputControl = controllMapEntity[entity];
                    inputControl.Update(gameTime);
                
                    foreach (InputHelper.CommandMove command in commandMoves) {
                        if (inputControl.Matches(command) && command.CanExecute(entity)) {
                            entity.OnCommandMoveComplete(command);
                            Animation.State? state;

                            if (entity.HasHit() && command.GetOnHitState() != null) { 
                                state = command.GetOnHitState();
                            } else {
                                state = command.GetAnimationState();
                            }

                            if (entity.GetCurrentAnimationAction(state) == Animation.Action.RUNNING) {
                                if (entity.CanRunAction()) {
                                    entity.SetAnimationState(state);
                                }
                            } else {
                                 entity.SetAnimationState(state);
                            }

                            break;
                        }
                    }
                }
            }
        }

        public InputControl GetInputControl(Entity entity) {
            return controllMapEntity[entity];
        }
    }
}
