using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FusionEngine
{
    public class AiState_Follow : AiBehaviour {

        public AiState_Follow(Entity entity) : base(entity) {
          
        }

        public override void OnEnter() {
            base.OnEnter();
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            Entity target = entity.GetCurrentTarget();

            if (target != null) {
                CollisionActions.LookAtTarget(entity, target);

                Vector2 p1 = target.GetProxyX() - entity.GetProxyX();
                p1.Normalize();

                Vector2 p2 = target.GetProxyZ() - entity.GetProxyZ();
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

        public override void OnExit() {

        }
    }
}
