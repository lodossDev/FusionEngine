using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public class AiState_Attack : AiBehaviour {
        
        public AiState_Attack(Entity entity) : base(entity) {
            
        }

        public override void OnEnter() {
            base.OnEnter();
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            Entity target = entity.GetCurrentTarget();

            if (target != null) {

                if (!entity.IsInAnimationAction(Animation.Action.ATTACKING)) {
                    CollisionActions.LookAtTarget(entity, target);

                    entity.SetDirectionX(0);
                    entity.SetVelocityX(0);

                    entity.SetDirectionZ(0);
                    entity.SetVelocityZ(0);
                    entity.SetAnimationState(Animation.State.STANCE);
                }

                if (distanceX > 100 && distanceX < 160 && distanceZ < 20) {
                    //entity.ApplyDefaultAttackState();
                } else {
                    if (!entity.IsInAnimationAction(Animation.Action.ATTACKING)) {
                        stateMachine.Change("FOLLOW");
                        return;
                    }
                }
            }    
        }

        public override void OnExit() {

        }
    }
}
