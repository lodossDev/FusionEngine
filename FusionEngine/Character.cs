﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FusionEngine {

    public class Character : Entity {
        private float distanceX, distanceZ;
        private Random rnd;

        public Character(Entity.ObjectType entityType, String name) : base(entityType, name) {

            GetAiStateMachine().Add("STANCE", new AiState_Stance(this));
            GetAiStateMachine().Add("AVOID_OSBTACLE", new AiState_AvoidObstacle(this));
            GetAiStateMachine().Add("AVOID_OTHER", new AiState_AvoidOtherEnemy(this));

            GetAiStateMachine().Add("FOLLOW", new AiState_Follow(this)); 
            GetAiStateMachine().Add("FOLLOW_X", new AiState_FollowX(this));
            GetAiStateMachine().Add("FOLLOW_Z", new AiState_FollowZ(this));

            GetAiStateMachine().Add("ATTACK", new AiState_Attack(this));

            GetAiStateMachine().Change("FOLLOW");

            rnd = new Random();
            distanceX = distanceZ = 0;

            SetDrawShadow(true);
            SetIsHittable(true);
        }

        public virtual void UpdateAI(GameTime gameTime) {
            List<Player> players = GameManager.GetInstance().Players;
            List<Entity> enemies = GameManager.GetInstance().GetEntities().FindAll(item => item is Enemy).Cast<Entity>().ToList();

            Attributes.CollisionState closeObstacle = GameManager.GetInstance().CollisionManager.FindObstacle(this);
            GetCollisionInfo().SetObstacleState(closeObstacle);

            Entity otherEnemy = CollisionActions.GetCloseEntity(this, enemies, 140, 40);
            GetCollisionInfo().SetOtherEnemy(otherEnemy);
            bool avoidOthers = (otherEnemy != null && !IsEdgeZ());

            if (players != null && players.Count > 0) {
                Entity player = CollisionActions.GetNearestEntity(this, players.ToList<Entity>());
                SetCurrentTarget(player);

                if (!IsInAnimationAction(Animation.Action.ATTACKING)) {
                    distanceX = Vector2.Distance(GetProxyX(), player.GetProxyX());
                    distanceZ = Vector2.Distance(GetProxyZ(), player.GetProxyZ());

                    if (distanceX > 100 && distanceX < 160 && distanceZ < 20) {
                        GetAiStateMachine().Change("ATTACK");
                    }

                    if (rnd.Next(1, 100) < 40 && GetAiStateMachine().GetCurrentStateId() != "AVOID_OBSTACLE") {
                        if (rnd.Next(1, 100) == 30) {
                            GetAiStateMachine().Change("STANCE");
                        }
                    }

                    if (avoidOthers == false) { 
                        if (rnd.Next(1, 100) > 80 && GetAiStateMachine().GetCurrentStateId() != "AVOID_OBSTACLE") {
                            if (rnd.Next(1, 100) < 5) {
                                GetAiStateMachine().Change("FOLLOW_X");
                            }
                        }

                        if (rnd.Next(1, 100) > 10 && rnd.Next(1, 100) < 25 && GetAiStateMachine().GetCurrentStateId() != "AVOID_OBSTACLE") {
                            if (rnd.Next(1, 100) > 95) {
                                GetAiStateMachine().Change("FOLLOW_Z");
                            }
                        }

                        if (rnd.Next(1, 100) > 85 && rnd.Next(1, 100) < 100) {
                            if (rnd.Next(1, 100) > 0 && rnd.Next(1, 100) < 5) {
                                GetAiStateMachine().Change("FOLLOW");
                            }
                        }
                    } else {
                        GetAiStateMachine().Change("AVOID_OTHER");
                    }

                    if (GetCollisionInfo().GetObstacleState() != Attributes.CollisionState.NO_COLLISION) {
                        GetAiStateMachine().Change("AVOID_OSBTACLE");
                    }
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
