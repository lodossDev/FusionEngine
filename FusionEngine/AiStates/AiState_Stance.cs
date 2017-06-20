using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace FusionEngine {

    public class AiState_Stance : AiState.IState {
        private AiState.StateMachine stateMachine;
        private Entity entity;
        private float stanceTime;

        public AiState_Stance(Entity entity) {
            this.entity = entity;
            stateMachine = this.entity.GetAiStateMachine();
            stanceTime = 0;
        }

        public void OnEnter() {
            stanceTime = 0;
        }

        public void OnExit() {
            
        }

        public void Update(GameTime gameTime) {
            Entity target = entity.GetCurrentTarget();
            
            if (target != null) {
                CollisionActions.LookAtTarget(entity, target);
                
                entity.SetAnimationState(Animation.State.STANCE);
                entity.ResetMovement();
            }

            stanceTime ++;

            if (stanceTime >= 50) {
                stateMachine.Change("FOLLOW");
                stanceTime = 0;
            }
        }
    }
}
