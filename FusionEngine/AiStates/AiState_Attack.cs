using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public class AiState_Attack : AiState.IState {
        private AiState.StateMachine stateMachine;
        private Entity entity;
        private Vector2 velocity;
        private Vector2 direction;
        private Vector2 sPx, sPy;
        private Vector2 tPx, tPy;
        private float distanceX, distanceZ;
        private Random rnd;


        public AiState_Attack(Entity entity) {
            this.entity = entity;
            stateMachine = this.entity.GetAiStateMachine();

            sPx = sPy = tPx = tPy = Vector2.Zero;

            distanceX = distanceZ = 0;

            velocity = new Vector2(2.5f, 2.0f);
            direction = new Vector2(1, -1);

            rnd = new Random();
        }

        public void OnEnter() {

        }

        public void Update(GameTime gameTime) {
            Entity target = entity.GetCurrentTarget();

            if (target != null) {
                sPx.X = entity.GetBoundsBox().GetRect().X;
                sPy.Y = entity.GetDepthBox().GetRect().Bottom;

                tPx.X = target.GetBoundsBox().GetRect().X;
                tPy.Y = target.GetDepthBox().GetRect().Bottom - 10;

                distanceX = Vector2.Distance(sPx, tPx);
                distanceZ = Vector2.Distance(sPy, tPy);

                if (!entity.IsInAnimationAction(Animation.Action.ATTACKING)) {
                    CollisionActions.LookAtTarget(entity, target);

                    entity.SetDirectionX(0);
                    entity.SetVelocityX(0);

                    entity.SetDirectionZ(0);
                    entity.SetVelocityZ(0);
                    entity.SetAnimationState(Animation.State.STANCE);
                }

                if (distanceX > 100 && distanceX < 160 && distanceZ < 20) {
                    entity.ApplyDefaultAttackState();
                } else {
                    if (!entity.IsInAnimationAction(Animation.Action.ATTACKING)) {
                        stateMachine.Change("FOLLOW");
                        return;
                    }
                }
            }    
        }

        public void OnExit() {

        }
    }
}
