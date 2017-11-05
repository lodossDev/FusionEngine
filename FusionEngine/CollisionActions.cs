using FusionEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public static class CollisionActions {

        public static void CheckAttack(Entity entity, Entity target, CLNS.AttackBox attackBox) {
            if (!entity.IsInAnimationAction(Animation.Action.ATTACKING)) { 
                return;    
            }

            if (attackBox.GetHitType() == CLNS.AttackBox.HitType.ONCE) { 
                if (entity.GetAttackInfo().lastAttackState != entity.GetCurrentAnimationState()) {
                    entity.CreateAttackId();

                    EntityHitPause(entity, target, attackBox);
                    OnAttackHit(entity, target, attackBox); 
                    entity.GetAttackInfo().lastAttackState = entity.GetCurrentAnimationState();                                    
                }
            } else { 
                if (entity.GetAttackInfo().lastAttackFrame != entity.GetCurrentSprite().GetCurrentFrame()) {
                    entity.CreateAttackId();

                    EntityHitPause(entity, target, attackBox);
                    entity.GetAttackInfo().lastAttackFrame = entity.GetCurrentSprite().GetCurrentFrame();
                }

                if (entity.GetAttackInfo().lastAttackState != entity.GetCurrentAnimationState()) {
                    OnAttackHit(entity, target, attackBox);
                    entity.GetAttackInfo().lastAttackState = entity.GetCurrentAnimationState();                                            
                }
            }    
        }

        private static void OnAttackHit(Entity entity, Entity target, CLNS.AttackBox attackBox) {
            if (!entity.IsInAnimationAction(Animation.Action.ATTACKING)) { 
                return;    
            }

            if (entity != target) {
                entity.GetAttackInfo().hasHit = true;
                entity.GetAttackInfo().currentAttackTime = entity.GetAttackInfo().nextAttackTime;
                EntityActions.IncrementAttackChain(entity, attackBox);
                
                target.GetAttackInfo().lastJuggleState = 1;

                entity.GetAttackInfo().lastHitDirection = entity.GetDirX();
                entity.OnAttackHit(target, attackBox);
            }
        }

        private static void CheckComboHitStats(Entity entity) {
            if (entity != null) {

                if (!entity.IsInAnimationAction(Animation.Action.ATTACKING)) { 
                    return;    
                }

                var target = (entity is Projectile ? entity.GetOwner() : entity);
                 
                if (entity.GetAttackInfo().comboHitTime < 200) {
                    entity.GetAttackInfo().showComboHits += 1;
                }

                if (entity.GetAttackInfo().showComboHits >= target.GetAttackInfo().targetComboHits) {
                    target.GetAttackInfo().comboHits += 1;

                    if (target is Player) {
                        target.ExpandComboFont();
                    }
                }
            }
        }

        public static void ShowEnemyLifebar(Entity entity, Entity target) {
            if (entity is Player) {
                ((Player)entity).SetCurrentHitLifeBar(target.GetLifeBar());
                ((Player)entity).SetLifebarHitTime(90);

            } else if (entity is Projectile && entity.GetOwner() is Player) {
                ((Player)entity.GetOwner()).SetCurrentHitLifeBar(target.GetLifeBar());
                ((Player)entity.GetOwner()).SetLifebarHitTime(90);
            }
        }

        private static void KnockIfToss(Entity target, CLNS.AttackBox attackBox, float dir) {
            if (target.IsToss() && !target.IsKnocked() 
                    && !target.IsGrabbed()
                    && !target.GetGrabInfo().inGrabHeight) {

                target.SetAnimationState(Animation.State.KNOCKED_DOWN1);
  
                float velX = 5;
                target.Toss(-12, velX * dir, 1, 2, true); 
            }
        }

        private static void HitPauseTime(Entity entity, Entity target, CLNS.AttackBox attackBox) {
            if (attackBox.GetHitPauseTime() != 0.0) {
                target.SetHitPauseTime((int)attackBox.GetHitPauseTime());
                entity.SetHitPauseTime((int)attackBox.GetHitPauseTime());
            }
        }

        private static void HitPauseTime(Entity entity, Entity target, int time) {        
            target.SetHitPauseTime(time);
            entity.SetHitPauseTime(time);
        }

        private static void EntityHitPause(Entity entity, Entity target, CLNS.AttackBox attackBox) {
            if (target != entity && !target.InvalidHitState() && !target.InJuggleState()) {
               entity.SetHitPauseTime((int)attackBox.GetHitPauseTime());
            }
        }

        private static void OnTargetHit(Entity target, Entity entity, CLNS.AttackBox attackBox) {
            if (!entity.IsInAnimationAction(Animation.Action.ATTACKING)) { 
                return;    
            }

            if (target != entity) {
        
                int attackDir = (entity.IsLeft() ? 1 : -1);
                float lookDir = (entity.IsLeft() ? -1 : 1);

                if (!target.InvalidHitState() 
                        && !target.InJuggleState()) {

                    target.OnTargetHit(entity, attackBox);

                    if (target.InBlockAction()) {
                        GameManager.AddSpark(entity, target, attackBox, Effect.Type.BLOCK_SPARK);
                        GameManager.GetInstance().PlaySFX("block");
                        EntityActions.FaceTarget(target, entity);
                        target.SetPainTime((int)(attackBox.GetPainTime() / 4));
                        target.SetRumble(lookDir, 2.0f);
                        target.DecreaseBlockResistance();
                        ApplyFrameActions(entity, target, attackBox);
               
                    } else {
                        GameManager.AddSpark(entity, target, attackBox, Effect.Type.HIT_SPARK);
                        target.GetAttackInfo().isHit = true;
                        target.GetAttackInfo().lastAttackDir = attackDir;
                        target.GetAttackInfo().attacker = entity;
                        target.GetAttackInfo().lastJuggleState = 1;

                        entity.GetAttackInfo().victim = target;
     
                        CheckComboHitStats(entity);
                        
                        EntityActions.SetPainState(entity, target, attackBox);
                        EntityActions.FaceTarget(target, entity);
                        EntityActions.CheckMaxGrabHits(entity, target);

                        target.SetPainTime((entity is Player ? (int)(attackBox.GetPainTime() / 2) : (int)attackBox.GetPainTime()));
                        target.SetRumble(lookDir, 2.0f);
                        target.SetHitPauseTime((int)attackBox.GetHitPauseTime());
                        //entity.SetHitPauseTime((int)attackBox.GetHitPauseTime());
                        
                        entity.IncreaseMP((int)(attackBox.GetHitDamage() * attackBox.GetHitStrength()));
                        entity.IncreasePoints(attackBox.GetHitPoints());
                        
                        //target.DecreaseHealth(attackBox.GetHitDamage() + 10);
                        target.SetLifebarPercent(target.GetHealth());
                        ShowEnemyLifebar(entity, target);
                        
                        if (entity is Projectile) {
                            EntityActions.SetInfront(entity, entity, 10);
                            entity.DecreaseHealth(15);
                        }

                        ApplyFrameActions(entity, target, attackBox);
                        KnockIfToss(target, attackBox, lookDir);

                        if (attackBox.GetLinkState() != null) {
                            entity.SetAnimationState(attackBox.GetLinkState());
                        }
                    }
                }

                else if (target.InJuggleState()) {
                    target.TakeJuggleHit();
                    target.OnTargetHit(entity, attackBox);

                    if (target.GetAttackInfo().juggleHits < target.GetAttackInfo().maxJuggleHits) { 
                        GameManager.AddSpark(entity, target, attackBox, Effect.Type.HIT_SPARK);
                        target.OnTargetHit(entity, attackBox);

                        GameManager.GetInstance().PlaySFX("beat2");
                        target.SetAnimationState(Animation.State.KNOCKED_DOWN1);
                        target.GetCurrentSprite().ResetAnimation();

                        float velX = ((5 - target.GetAttackInfo().juggleHits) * lookDir);
                        float sHeight = -((Math.Abs(target.GetTossInfo().tempHeight) / 2) + 5f);
                        float height = (sHeight / GameManager.GAME_VELOCITY) / 2;

                        CheckComboHitStats(entity);

                        target.TossFast(height, velX, target.GetAttackInfo().maxJuggleHits + 1, 2); 
                        target.SetTossGravity(0.6f);
                        target.SetPainTime((int)attackBox.GetPainTime());
  
                        entity.IncreaseMP((int)(attackBox.GetHitDamage() * attackBox.GetHitStrength()));
                        target.SetHitPauseTime(7);
                        entity.SetHitPauseTime(7);

                        entity.IncreasePoints(attackBox.GetHitPoints());
                        //target.DecreaseHealth(attackBox.GetHitDamage() + 5);
                        target.SetLifebarPercent(target.GetHealth());

                        ShowEnemyLifebar(entity, target);

                        if (entity is Projectile) {
                            EntityActions.SetInfront(entity, entity, 10);
                            entity.DecreaseHealth(15);
                        }

                        if (attackBox.GetLinkState() != null) {
                            entity.SetAnimationState(attackBox.GetLinkState());
                        }
                    }

                    if (target.GetAttackInfo().juggleHits <= 0) {
                        target.GetAttackInfo().lastJuggleState = -1;
                        target.GetAttackInfo().juggleHits = -1;
                    }
                }
            }
        }

        public static void SetTargetHit(Entity entity, Entity target, CLNS.AttackBox attackBox, ref bool targetHit, float time) {
            if (!entity.IsInAnimationAction(Animation.Action.ATTACKING)) { 
                return;    
            }

            if (!targetHit) {
                OnTargetHit(target, entity, attackBox);
                entity.GetAttackInfo().lastComboHitTime = time;
                targetHit = true;
            }
        }

        public static void ApplyFrameActions(Entity entity, Entity target, CLNS.AttackBox attackBox) {
            if (!entity.IsInAnimationAction(Animation.Action.ATTACKING)) { 
                return;    
            }

            if (target.IsEntity(Entity.ObjectType.ENEMY) || target is Enemy) {

                Attributes.CollisionState obstacleState = GameManager.GetInstance().CollisionManager.FindObstacle(target);

                float dirX = (target.IsEdgeX() == false && obstacleState == Attributes.CollisionState.NO_COLLISION
                                    && target.GetCollisionInfo().GetCollideX() == Attributes.CollisionState.NO_COLLISION 
                                            ? (entity.IsLeft() ? -1 : 1) : 0);
 
                if (!attackBox.IsKnock()) {

                    if (!target.InBlockAction() || (target.InBlockAction() 
                            && (target.GetAttackInfo().blockMode == 2 
                                    || target.GetAttackInfo().blockMode == 3))) { 
                        
                        if (attackBox.GetMoveX() != 0.0) {
                            target.MoveX((attackBox.GetMoveX() * dirX));
                        }

                        if (!target.InBlockAction()) {
                            if (attackBox.GetTossHeight() != 0.0) {
                                target.TossFast(attackBox.GetTossHeight());
                            }
                        }

                        if (attackBox.ApplyToAttacker()) {
                            if (entity.IsToss()) {
                                entity.TossFast(-5);
                                entity.SetTossGravity(1.83f);
                                entity.GetTossInfo().velocity.X = attackBox.GetMoveX() * -dirX;
                            } else {
                                entity.MoveX((Math.Abs(entity.GetTossInfo().velocity.X) + attackBox.GetMoveX()) * -dirX);
                            }
                        }
                    }
                } else {
                    if (!target.InBlockAction()) {
                        target.SetAnimationState(Animation.State.KNOCKED_DOWN1);
                        target.SetCurrentKnockedState(Attributes.KnockedState.KNOCKED_DOWN);

                        if (attackBox.GetTossHeight() != 0.0) {
                            target.TossFast(attackBox.GetTossHeight(), (attackBox.GetMoveX() * dirX), 1, 2, true);
                        }

                        if (attackBox.ApplyToAttacker()) {
                            if (entity.IsToss()) {
                                entity.TossFast(-5);
                                entity.SetTossGravity(1.83f);
                                entity.GetTossInfo().velocity.X = attackBox.GetMoveX() * -dirX;
                            } else {
                                entity.MoveX((Math.Abs(entity.GetTossInfo().velocity.X) + attackBox.GetMoveX()) * -dirX);
                            }
                        }

                        HitPauseTime(entity, target, attackBox);
                    }

                    if (target.InBlockAction() && target.GetAttackInfo().blockMode == 3) {
                        if (attackBox.GetMoveX() != 0.0) {
                            target.MoveX((attackBox.GetMoveX() * dirX));
                        }
                    }

                    if (entity.GetGrabInfo().grabbed != null) { 
                        EntityActions.Ungrab(entity, entity.GetGrabInfo().grabbed);
                    }
                }
            }
        }

        public static void ProcessGrabItem(Entity entity, Collectable collectable, ref bool isCollected) {
            entity.GetCollisionInfo().SetItem(collectable);

            if (entity.InGrabItemFrameState()) {

                if (!isCollected) { 

                    if (collectable is Health) {
                        GameManager.GetInstance().PlaySFX("1up");
                        entity.IncreaseHealth((int)collectable.GetPoints());

                    } else if (collectable is Money) {
                        GameManager.GetInstance().PlaySFX("1up");
                        entity.IncreasePoints((int)collectable.GetPoints());

                    } else if (collectable is Life) {
                        GameManager.GetInstance().PlaySFX("1up");
                        entity.IncreaseLives((int)collectable.GetPoints());

                    } else if (collectable is MP) {
                        GameManager.GetInstance().PlaySFX("1up");
                        entity.IncreaseMP((int)collectable.GetPoints());
                    }

                    isCollected = true;
                }

                GameManager.GetInstance().RemoveEntity(collectable);
            }
        }

        public static Entity GetNearestEntity(Entity entity, List<Entity> entities) {
            Entity target = null;
            float maxDistance = 340f;

            if (entities != null && entities.Count > 0) {
                if (entities.Count == 1) {
                    target = entities.First();
                } else {
                    foreach (Entity other in entities) {
                        if (entity != other) { 
                            float distance = Vector2.Distance(entity.GetProxy(), other.GetProxy());

                            if (distance < maxDistance) {
                                target = other;
                                break;
                            }
                        }
                    }
                }
            }

            return target;
        }

        public static Entity GetCloseEntity(Entity entity, List<Entity> entities, int distX, int distZ){
            Entity target = null;

            if (entities != null && entities.Count > 0) {
                if (entities.Count == 1) {
                    target = entities.First();
                } else {
                    foreach (Entity other in entities) {
                        if (entity != other) {
                            float distanceX = Vector2.Distance(entity.GetProxyX(), other.GetProxyX());
                            float distanceZ = Vector2.Distance(entity.GetProxyZ(), other.GetProxyZ());

                            if (distanceX < distX && distanceZ < distZ) {
                                target = other;
                                break;
                            }
                        }
                    }
                }
            }

            return target;
        }

        public static void LookAtTarget(Entity entity, Entity target) {
            if (!entity.IsKnocked() 
                    && !entity.IsRise()
                    && !entity.InHitPauseTime()
                    && !entity.InPainTime()) {

                if (entity.GetPosX() > target.GetPosX()) {
                    entity.SetIsLeft(true);

                } else if (entity.GetPosX() < target.GetPosX()) {
                    entity.SetIsLeft(false);
                }
            }
        }
    }
}
