using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FusionEngine
{
    public class AiState_Follow : AiState.IState {

        private AiState.StateMachine stateMachine;
        private Entity entity;
        private Vector2 velocity;
        private Vector2 direction;
        private Vector2 sPx, sPy;
        private Vector2 tPx, tPy;
        private float distanceX, distanceZ;
        private Random rnd;
       

        public AiState_Follow(Entity entity) {
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
                CollisionActions.LookAtTarget(entity, target);

                sPx.X = entity.GetBoundsBox().GetRect().X;
                sPy.Y = entity.GetDepthBox().GetRect().Bottom;

                tPx.X = target.GetBoundsBox().GetRect().X;
                tPy.Y = target.GetDepthBox().GetRect().Bottom - 10;

                distanceX = Vector2.Distance(sPx, tPx);
                distanceZ = Vector2.Distance(sPy, tPy);

                Vector2 p1 = tPx - sPx;
                p1.Normalize();

                Vector2 p2 = tPy - sPy;
                p2.Normalize();

                direction.X = p1.X * 1;
                velocity.X = 2.5f;
                     
                direction.Y = p2.Y * 1;
                velocity.Y = 2.5f;

                if (distanceX > 100 && distanceX < 160) {
                    velocity.X = direction.X = 0;
                }

                if (distanceZ < 10) {
                    velocity.Y = direction.Y = 0;
                }

                if (distanceX < 100) {
                    velocity.X = 2.5f;
                         
                    if (!entity.IsLeft()) {
                        direction.X = -1;
                    } else{
                        direction.X = 1;
                    }
                }
                
                if (velocity.X != 0 || velocity.Y != 0) {
                    entity.SetAnimationState(Animation.State.WALK_TOWARDS);
                } else {
                    entity.SetAnimationState(Animation.State.STANCE);
                }

                if (float.IsNaN(direction.X)) direction.X = 0f;
                if (float.IsNaN(direction.Y)) direction.Y = 0f;
                if (float.IsNaN(velocity.X)) velocity.X = 0f;
                if (float.IsNaN(velocity.Y)) velocity.Y = 0f;

            } else {
                velocity.X = direction.X = 0;
                velocity.Y = direction.Y = 0;
            } 
                
            entity.MoveX(velocity.X, direction.X);
            entity.MoveZ(velocity.Y, direction.Y);
        }

        public void OnExit() {

        }
    }
}
