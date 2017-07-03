using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine
{
    public class AiState_FollowZ : AiBehaviour {

        public AiState_FollowZ(Entity entity) : base(entity) {
        
        }

        public override void OnEnter() {
            base.OnEnter();
        }

        public override void Update(GameTime gameTime) {
            base.OnEnter();
            Entity target = entity.GetCurrentTarget();

            if (target != null) {
                CollisionActions.LookAtTarget(entity, target);

                Vector2 p1 = target.GetProxyZ() - entity.GetProxyZ();
                p1.Normalize();

                direction.Y = p1.Y * 1;
                velocity.Y = 2.0f;

                if (distanceZ < 10) {
                    velocity.Y = direction.Y = 0;
                }

                if (velocity.Y != 0) {
                    entity.SetAnimationState(Animation.State.WALK_TOWARDS);
                } else {
                    entity.SetAnimationState(Animation.State.STANCE);
                }

                if (float.IsNaN(direction.Y)) direction.Y = 0f;
                if (float.IsNaN(velocity.Y)) velocity.Y = 0f;
            } else {
                velocity.Y = direction.Y = 0;
            }

            entity.MoveZ(velocity.Y, direction.Y);
            entity.SetDirectionX(0);
            entity.SetVelocityX(0);
        }

        public override void OnExit() {

        }
    }
}
