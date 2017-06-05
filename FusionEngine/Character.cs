using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FusionEngine {

    public class Character : Entity {
        private Vector2 followPosition;
        private AiState.StateMachine aiStateMachine;

        public Character(Entity.ObjectType entityType, String name) : base(entityType, name) {
            followPosition = Vector2.Zero;
            aiStateMachine = GetAiStateMachine();

            aiStateMachine.Add("AVOID_OSBTACLE", new AiState_AvoidObstacle(this));
            aiStateMachine.Add("FOLLOW", new AiState_Follow(this));
            aiStateMachine.Add("FOLLOW_X", new AiState_FollowXPath(this));
            aiStateMachine.Change("FOLLOW");

            SetDrawShadow(true);
            SetIsHittable(true);
        }

        public virtual void UpdateAI(GameTime gameTime, List<Player> players) {
            if (players != null && players.Count > 0) { 
                Entity target = EntityActions.GetNearestEntity(this, players.ToList<Entity>());
                SetCurrentTarget(target);

                if (target != null) {
                    EntityActions.LookAtTarget(this, target);
                    aiStateMachine.Update(gameTime);
                    ResetToIdle(gameTime);
                }
            }
        }
    }
}
