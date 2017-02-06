using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FusionEngine
{
    public class AiState_Follow : System.IState
    {
        private System.StateMachine stateMachine;
        private Entity entity;
        private Vector2 velocity;
        private Vector2 direction;
        private Vector2 sPx,sPy;
        private Vector2 tPx, tPy;
        private float maxDistanceX;
        private float maxDistanceZ;
        private Random rnd;


        public AiState_Follow(Entity entity)
        {
            this.entity = entity;
            stateMachine = this.entity.GetAiStateMachine();

            sPx = sPy = tPx = tPy = Vector2.Zero;

            maxDistanceX = 200f;
            maxDistanceZ = 20f;

            velocity = new Vector2(2.5f, 2.5f);
            direction = new Vector2(1, -1);

            rnd = new Random();
        }

        public void OnEnter()
        {

        }

        public void Update(GameTime gameTime)
        {
            Entity target = entity.GetCurrentTarget();

            if (target != null)
            {
                sPx.X = entity.GetPosX();
                sPy.Y = entity.GetPosZ();

                tPx.X = target.GetPosX();
                tPy.Y = target.GetPosZ() + 50f;

                float distanceX = Vector2.Distance(sPx, tPx);
                float distanceZ = Vector2.Distance(sPy, tPy);

                if (!entity.IsInAnimationAction(Animation.Action.ATTACKING))
                {
                    if (distanceX > maxDistanceX)
                    {
                        Vector2 p1 = tPx - sPx;
                        p1.Normalize();

                        direction.X = p1.X;
                        velocity.X = 2.5f;

                        if (((entity.IsLeft() == false && velocity.X < 0.0f) || (entity.IsLeft() == true && velocity.X > 0.0f)))
                        {
                            entity.SetAnimationState(Animation.State.WALK_BACKWARDS);
                        }
                        else
                        {
                            entity.SetAnimationState(Animation.State.WALK_TOWARDS);
                        }
                    }

                    if (distanceZ > maxDistanceZ)
                    {
                        Vector2 p1 = tPy - sPy;
                        p1.Normalize();

                        direction.Y = p1.Y;
                        velocity.Y = 2.5f;

                        if (((entity.IsLeft() == false && velocity.X < 0.0f) || (entity.IsLeft() == true && velocity.X > 0.0f)))
                        {
                            entity.SetAnimationState(Animation.State.WALK_BACKWARDS);
                        }
                        else
                        {
                            entity.SetAnimationState(Animation.State.WALK_TOWARDS);
                        }
                    }
                }

                if (distanceX < maxDistanceX && distanceZ < maxDistanceZ && !entity.IsInAnimationAction(Animation.Action.ATTACKING))
                {
                    if (distanceX < (maxDistanceX - 20))
                    {
                        Vector2 p1 = tPx - sPx;
                        p1.Normalize();

                        direction.X = -p1.X;
                        velocity.X = 2.5f;
                        entity.SetAnimationState(Animation.State.WALK_BACKWARDS);
                    }
                    else if (distanceX < maxDistanceX)
                    {
                        int mode = rnd.Next(1, 100);
                        //Debug.WriteLine("mode: " + mode);

                        if (mode > 80)
                        {
                            int agg = rnd.Next(1, 100);

                            if (agg > 95)
                            {
                                entity.SetAnimationState(Animation.State.ATTACK3);
                            }
                            else
                            {
                                entity.SetAnimationState(Animation.State.STANCE);
                            }
                        }
                        else
                        {
                            entity.SetAnimationState(Animation.State.STANCE);
                        }

                        velocity.X = 0f;
                        velocity.Y = 0f;
                    }
                    else
                    {
                        entity.SetAnimationState(Animation.State.STANCE);
                        velocity.X = 0f;
                        velocity.Y = 0f;
                    }
                }

                if (float.IsNaN(direction.X)) direction.X = 0f;
                if (float.IsNaN(direction.Y)) direction.Y = 0f;

                if (float.IsNaN(velocity.X)) velocity.X = 0f;
                if (float.IsNaN(velocity.Y)) velocity.Y = 0f;

                entity.MoveX(velocity.X, direction.X);
                entity.MoveZ(velocity.Y, direction.Y);
            }
        }

        public void OnExit()
        {

        }
    }
}
