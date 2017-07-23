using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public class AiState_AvoidOtherEnemy : AiBehaviour {
        private int thinkAvoidTime;
        private int level;


        public AiState_AvoidOtherEnemy(Entity entity) : base(entity) {
            thinkAvoidTime = 0;
            level = 0;
        }

        public override void OnEnter() {
            base.OnEnter();
            thinkAvoidTime = 0;
            level = rnd.Next(1, 2);
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);

            Entity otherEnemy = entity.GetCollisionInfo().GetOtherEnemy();
            entity.SetAnimationState(Animation.State.WALK_TOWARDS);

            if (otherEnemy != null 
                    && entity != otherEnemy.GetCollisionInfo().GetOtherEnemy()) {

                Vector3 lastDirection = otherEnemy.GetDirection();

                if (level == 1) {
                    velocity.Y = 2.0f;
                    velocity.X = 0f;

                    if (lastDirection.Z < 0) direction.Y = 1;
                    if (lastDirection.Z > 0) direction.Y = -1;
                } else {
                    velocity.X = 2.5f;
                    velocity.Y = 0f;

                    if (lastDirection.X < 0) direction.X = 1;
                    if (lastDirection.X > 0) direction.X = -1;
                }

                if (float.IsNaN(direction.X)) direction.X = 0f;
                if (float.IsNaN(direction.Y)) direction.Y = 0f;
                if (float.IsNaN(velocity.X)) velocity.X = 0f;
                if (float.IsNaN(velocity.Y)) velocity.Y = 0f;
            } 
            
            thinkAvoidTime++;

            if (thinkAvoidTime >= 80) {
                stateMachine.Change("FOLLOW");
                thinkAvoidTime = 0;
                return;
            }

            entity.MoveX(velocity.X, direction.X);
            entity.MoveZ(velocity.Y, direction.Y);
        }

        public override void OnExit() {

        }
    }
}
