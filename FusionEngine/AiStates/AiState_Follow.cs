﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FusionEngine
{
    public class AiState_Follow : AiState.IState
    {
        private AiState.StateMachine stateMachine;
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
                tPx.X = target.GetPosX();

                sPy.Y = entity.GetBoundsBox().GetRect().Bottom;
                tPy.Y = target.GetBoundsBox().GetRect().Bottom;

                Vector2 ss1 = new Vector2(0, entity.GetBoundsBox().GetRect().Bottom);
                Vector2 ss2 = new Vector2(0, target.GetBoundsBox().GetRect().Bottom);

                float distanceX = Vector2.Distance(sPx, tPx);
                float distanceZ = Vector2.Distance(ss1, ss2);

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
                            if (entity.HasSprite(Animation.State.WALK_BACKWARDS)) {
                                entity.SetAnimationState(Animation.State.WALK_BACKWARDS);
                            } else {
                                entity.SetAnimationState(Animation.State.WALK_TOWARDS);
                            }
                        }
                        else
                        {
                            entity.SetAnimationState(Animation.State.WALK_TOWARDS);
                        }
                    }

                    if (distanceZ > maxDistanceZ + 10)
                    {
                        Vector2 p1 = ss2 - ss1;
                        p1.Normalize();

                        direction.Y = p1.Y;
                        velocity.Y = 2.5f;

                        if (((entity.IsLeft() == false && velocity.X < 0.0f) || (entity.IsLeft() == true && velocity.X > 0.0f)))
                        {
                            if (entity.HasSprite(Animation.State.WALK_BACKWARDS)) {
                                entity.SetAnimationState(Animation.State.WALK_BACKWARDS);
                            } else {
                                entity.SetAnimationState(Animation.State.WALK_TOWARDS);
                            }
                        }
                        else
                        {
                            entity.SetAnimationState(Animation.State.WALK_TOWARDS);
                        }
                    }
                }

                if (distanceX < maxDistanceX && distanceZ < maxDistanceZ + 10 && !entity.IsInAnimationAction(Animation.Action.ATTACKING))
                {
                    if (distanceX < (maxDistanceX - 20))
                    {
                        Vector2 p1 = tPx - sPx;
                        p1.Normalize();

                        direction.X = -p1.X;
                        velocity.X = 2.5f;

                        if (entity.HasSprite(Animation.State.WALK_BACKWARDS)) {
                            entity.SetAnimationState(Animation.State.WALK_BACKWARDS);
                        } else {
                            entity.SetAnimationState(Animation.State.WALK_TOWARDS);
                        }
                    }
                    else if (distanceX < maxDistanceX)
                    {
                        int mode = rnd.Next(1, 100);
                        //Debug.WriteLine("mode: " + mode);

                        if (mode > 80)
                        {
                            int agg = rnd.Next(1, 100);

                            if (agg > 75 && !entity.IsInAnimationAction(Animation.Action.ATTACKING))
                            {
                                int atk = rnd.Next(1, 4);
                                if (atk == 1)
                                    entity.SetAnimationState(Animation.State.ATTACK1);
                                else if (atk == 2)
                                    entity.SetAnimationState(Animation.State.ATTACK2);
                                else
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
