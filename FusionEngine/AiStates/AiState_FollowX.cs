using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FusionEngine {

    public class AiState_FollowX : AiState.IState {
        private AiState.StateMachine stateMachine;
        private Entity entity;
        private Vector2 velocity;
        private Vector2 direction;
        private Vector2 sPx, tPx;
        private float maxDistanceX;
        private float distanceX;
        private Random rnd;
        private int activityTime;


        public AiState_FollowX(Entity entity) {
            this.entity = entity;
            stateMachine = this.entity.GetAiStateMachine();

            sPx = tPx = Vector2.Zero;
            maxDistanceX = 200f;
            distanceX = 0;

            velocity = new Vector2(2.5f, 0);
            direction = new Vector2(1, 0);

            activityTime = 0;

            rnd = new Random();
        }

        public void OnEnter() {
            activityTime = 0;
        }

        public void Update(GameTime gameTime) {
            Entity target = entity.GetCurrentTarget();

            if (target != null) {
                CollisionActions.LookAtTarget(entity, target);

                sPx.X = entity.GetBoundsBox().GetRect().X;
                tPx.X = target.GetBoundsBox().GetRect().X;
                distanceX = Vector2.Distance(sPx, tPx);

                Vector2 p1 = tPx - sPx;
                p1.Normalize();

                direction.X = p1.X * 1;
                velocity.X = 2.5f;
                     
                if (distanceX > 100 && distanceX < 160) {
                    velocity.X = direction.X = 0;
                }

                if (distanceX < 100) {
                    velocity.X = 2.5f;
                         
                    if (!entity.IsLeft()) {
                        direction.X = -1;
                    } else{
                        direction.X = 1;
                    }
                }

                if (velocity.X != 0) {
                    entity.SetAnimationState(Animation.State.WALK_TOWARDS);
                } else {
                    entity.SetAnimationState(Animation.State.STANCE);
                }

                if (float.IsNaN(direction.X)) direction.X = 0f;
                if (float.IsNaN(velocity.X)) velocity.X = 0f;

            } else {
                velocity.X = direction.X = 0;
            } 

            activityTime++;

            if (activityTime >= 90) {
                stateMachine.Change("FOLLOW");
                activityTime = 0;
            }
                
            entity.MoveX(velocity.X, direction.X);
            entity.SetDirectionZ(0);
            entity.SetVelocityZ(0);
        }

        public void OnExit() {

        }
    }
}
