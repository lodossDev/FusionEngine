using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace FusionEngine {

    public class AiState_Stance : AiBehaviour {

        public AiState_Stance(Entity entity) : base(entity) {
            
        }

        public override void OnEnter() {
            base.OnEnter();
        }

        public override void Update(GameTime gameTime) {
            Entity target = entity.GetCurrentTarget();
            
            if (target != null) {
                CollisionActions.LookAtTarget(entity, target);
                
                entity.SetAnimationState(Animation.State.STANCE);
                entity.ResetMovement();
            }

            activityTime ++;

            if (activityTime >= 50) {
                stateMachine.Change("FOLLOW");
                activityTime = 0;
            }
        }

        public override void OnExit() {

        }
    }
}
