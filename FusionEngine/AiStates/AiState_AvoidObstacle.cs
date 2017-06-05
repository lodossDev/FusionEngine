using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace FusionEngine {

    public class AiState_AvoidObstacle : AiState.IState {
        private AiState.StateMachine stateMachine;
        private Entity entity;
        private Vector2 velocity;
        private Vector2 direction;
        private Random rnd;


        public AiState_AvoidObstacle(Entity entity) {
            this.entity = entity;
            stateMachine = this.entity.GetAiStateMachine();

            velocity = new Vector2(2.5f, 2.5f);
            direction = new Vector2(1, -1);

            rnd = new Random();
        }

        public void OnEnter() {

        }

        public void Update(GameTime gameTime) {

            if (entity.CanProcessAiState()) { 
                Entity obstacle = entity.GetCollisionInfo().GetCloseObstacle();

                if (obstacle == null) {
                    stateMachine.Change("FOLLOW");
                    return;
                }

                if (obstacle != null) { 
                    if (CollisionHelper.GetDiff(entity.GetPosX(), obstacle.GetPosX()) > 160
                            && CollisionHelper.GetDiff(entity.GetPosZ(), obstacle.GetPosZ()) > 70) {

                        entity.GetCollisionInfo().SetCloseObstacle(null);
                    }

                    velocity.Y = 2.5f;
                    velocity.X = 2.5f;

                    if (entity.GetCollisionInfo().IsCollideX(Attributes.CollisionState.LEFT)) {
                        direction.X = -1;
                    } else if (entity.GetCollisionInfo().IsCollideX(Attributes.CollisionState.RIGHT)) {
                        direction.X = 1;
                    } else {
                        if (entity.GetPosX() > obstacle.GetPosX() && direction.X != -1) { 
                            direction.X = 1;
                        } else if (entity.GetPosX() < obstacle.GetPosX() && direction.X != 1) { 
                            direction.X = -1;
                        } 
                    }

                    if (entity.GetCollisionInfo().IsCollideZ(Attributes.CollisionState.TOP)) {
                        direction.Y = 1;
                    } else if (entity.GetCollisionInfo().IsCollideZ(Attributes.CollisionState.BOTTOM)) {
                        direction.Y = -1;
                    }

                    entity.SetAnimationState(Animation.State.WALK_TOWARDS);

                } else {
                     velocity.X = direction.X = 0;
                     velocity.Y = direction.Y = 0;
                }

                entity.MoveX(velocity.X, direction.X);
                entity.MoveZ(velocity.Y, direction.Y);

                Debug.WriteLine("## VELX: " + velocity.X);
                Debug.WriteLine("## VELZ: " + velocity.Y);
            }
        }

        public void OnExit() {

        }
    }
}
