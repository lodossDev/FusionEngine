using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FusionEngine
{
    public class AiState_FollowXPath : AiState.IState
    {
        private AiState.StateMachine stateMachine;
        private Entity entity;
        private float maxDistance;
        private Vector2 velocity;
        private Vector2 direction;
        private Vector2 sourceDistance;
        private Vector2 targetDistance;


        public AiState_FollowXPath(Entity entity)
        {
            this.entity = entity;
            stateMachine = this.entity.GetAiStateMachine();

            maxDistance = 220f;
            sourceDistance = Vector2.Zero;
            targetDistance = Vector2.Zero;

            velocity = new Vector2(2, 0);
            direction =new Vector2(-1, 0);
        }

        public void OnEnter()
        {

        }

        public void Update(GameTime gameTime)
        {
            Entity target = entity.GetCurrentTarget();
            targetDistance.X = target.GetPosX();
            sourceDistance.X = entity.GetPosX();

            float distance = Vector2.Distance(targetDistance, sourceDistance);

            if (target != null)
            {
                if (entity.GetPosX() - (entity.GetCurrentSpriteWidth() / 4) - 400 >= target.GetPosX())
                {
                    direction.X = -1;
                }
                else if (entity.GetPosX() + (entity.GetCurrentSpriteWidth() / 4) + 400 <= target.GetPosX())
                {
                    direction.X = 1;
                }

                if (velocity.X > 0 && entity.IsLeft() == true || velocity.X < 0 && entity.IsLeft() == false)
                {
                    entity.SetAnimationState(Animation.State.WALK_BACKWARDS);
                }
                else
                {
                    entity.SetAnimationState(Animation.State.WALK_TOWARDS);
                }
                    
                entity.MoveX(velocity.X, direction.X);
            }
        }

        public void OnExit()
        {

        }
    }
}
