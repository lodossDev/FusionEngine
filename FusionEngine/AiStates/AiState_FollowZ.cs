using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine
{
    public class AiState_FollowZ : AiState.IState
    {
        private AiState.StateMachine stateMachine;
        private Entity entity;
        private Vector2 velocity;
        private Vector2 direction;
        private Vector2 sPy, tPy;
        private float distanceZ;
        private Random rnd;
        private int activityTime;


        public AiState_FollowZ(Entity entity)
        {
            this.entity = entity;
            stateMachine = this.entity.GetAiStateMachine();

            sPy = tPy = Vector2.Zero;
            distanceZ = 0;

            velocity = new Vector2(2.5f, 0);
            direction = new Vector2(1, 0);

            activityTime = 0;

            rnd = new Random();
        }

        public void OnEnter()
        {
            activityTime = 0;
        }

        public void Update(GameTime gameTime)
        {
            Entity target = entity.GetCurrentTarget();

            if (target != null)
            {
                CollisionActions.LookAtTarget(entity, target);

                sPy.Y = entity.GetDepthBox().GetRect().Bottom;
                tPy.Y = target.GetDepthBox().GetRect().Bottom - 10;

                distanceZ = Vector2.Distance(sPy, tPy);

                Vector2 p1 = tPy - sPy;
                p1.Normalize();

                direction.Y = p1.Y * 1;
                velocity.Y = 2.0f;

                if (distanceZ < 10)
                {
                    velocity.Y = direction.Y = 0;
                }

                if (velocity.Y != 0)
                {
                    entity.SetAnimationState(Animation.State.WALK_TOWARDS);
                }
                else
                {
                    entity.SetAnimationState(Animation.State.STANCE);
                }

                if (float.IsNaN(direction.Y)) direction.Y = 0f;
                if (float.IsNaN(velocity.Y)) velocity.Y = 0f;
            }
            else
            {
                velocity.Y = direction.Y = 0;
            }

            entity.MoveZ(velocity.Y, direction.Y);
            entity.SetDirectionX(0);
            entity.SetVelocityX(0);
        }

        public void OnExit()
        {

        }
    }
}
