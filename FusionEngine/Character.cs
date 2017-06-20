using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FusionEngine {

    public class Character : Entity {
        private Vector2 followPosition;
        private Random rnd;

        public Character(Entity.ObjectType entityType, String name) : base(entityType, name) {
            followPosition = Vector2.Zero;

            GetAiStateMachine().Add("STANCE", new AiState_Stance(this));
            GetAiStateMachine().Add("AVOID_OSBTACLE", new AiState_AvoidObstacle(this));
            GetAiStateMachine().Add("FOLLOW", new AiState_Follow(this));
            GetAiStateMachine().Add("FOLLOW_X", new AiState_FollowX(this));
           
            GetAiStateMachine().Change("FOLLOW");

            rnd = new Random();

            SetDrawShadow(true);
            SetIsHittable(true);
        }

        public virtual void UpdateAI(GameTime gameTime, List<Player> players) {
            Attributes.CollisionState closeObstacle = GameManager.GetInstance().CollisionManager.FindObstacle(this);
            GetCollisionInfo().SetObstacleState(closeObstacle);

            if (players != null && players.Count > 0) {
                Entity player = CollisionActions.GetNearestEntity(this, players.ToList<Entity>());
                SetCurrentTarget(player);

                if (rnd.Next(1, 100) < 40 && GetAiStateMachine().GetCurrentStateId() != "AVOID_OBSTACLE") {
                    if (rnd.Next(1, 100) == 30 ) {
                        GetAiStateMachine().Change("STANCE");
                    }
                }

                if (rnd.Next(1, 100) > 80 ) {
                    if (rnd.Next(1, 100) < 5) {
                        GetAiStateMachine().Change("FOLLOW_X");
                    }
                }

                if (GetCollisionInfo().GetObstacleState() != Attributes.CollisionState.NO_COLLISION) {
                    GetAiStateMachine().Change("AVOID_OSBTACLE");
                }

                if (CanProcessAiState()) {
                    GetAiStateMachine().Update(gameTime);
                } else {
                    ResetMovement();
                }
            }

           ResetToIdle(gameTime);
        }
    }
}
