using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace FusionEngine {

    public class AiState_AvoidObstacle : AiBehaviour {
        private int thinkAvoidTime;


        public AiState_AvoidObstacle(Entity entity) : base(entity) {
            thinkAvoidTime = 0;
        }

        public override void OnEnter() {
            base.OnEnter();
            thinkAvoidTime = 0;
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            Entity target = entity.GetCurrentTarget();
            Attributes.CollisionState closeObstacle = entity.GetCollisionInfo().GetObstacleState();

            if(target != null) { 
                
                if (closeObstacle != Attributes.CollisionState.NO_COLLISION) {
                   
                    if (closeObstacle == Attributes.CollisionState.RIGHT) {
                        velocity.X = 0f;
                        direction.X = 0f;
                        velocity.Y = 2.0f;

                        if (rnd.Next(1, 10) > 6){
                            direction.Y = 1;
                        } else {
                            direction.Y = -1;
                        }
                    } 

                    if (closeObstacle == Attributes.CollisionState.LEFT) {
                        velocity.X = 0f;
                        direction.X = 0f;
                        velocity.Y = 2.0f;

                        if (rnd.Next(1, 10) > 6) {
                            direction.Y = 1;
                        } else {
                            direction.Y = -1;
                        }
                    } 

                    if (closeObstacle == Attributes.CollisionState.BOTTOM 
                            || closeObstacle == Attributes.CollisionState.TOP) {

                        velocity.X = 2.5f;
                        direction.Y = 0;
                        velocity.Y = 0f;

                        if (!entity.IsLeft()) {
                            direction.X = 1;
                        } else {
                            direction.X = -1;
                        }
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
            
            thinkAvoidTime ++;

            if (thinkAvoidTime >= 196) {
                stateMachine.Change("FOLLOW");
                thinkAvoidTime = 0;
            } 
                
            entity.MoveX(velocity.X, direction.X);
            entity.MoveZ(velocity.Y, direction.Y);
        }

        public override void OnExit() {

        }
    }
}
