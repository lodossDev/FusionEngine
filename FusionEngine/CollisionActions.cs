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
                    CollisionManager.CreateHitId();
                    OnAttackHit(entity, target, attackBox);                                     
                }
            } else { 
                if (entity.GetAttackInfo().lastAttackState != entity.GetCurrentAnimationState()) {
                    OnAttackHit(entity, target, attackBox);                                           
                }

                if (entity.GetAttackInfo().lastAttackFrame != entity.GetCurrentSprite().GetCurrentFrame()) {
                    CollisionManager.CreateHitId();
                    entity.GetAttackInfo().lastAttackFrame = entity.GetCurrentSprite().GetCurrentFrame();
                }
            }    
        }

        private static void OnAttackHit(Entity entity, Entity target, CLNS.AttackBox attackBox) {
            if (!entity.IsInAnimationAction(Animation.Action.ATTACKING)) { 
                return;    
            }

            if (entity != target) {
                entity.GetAttackInfo().currentAttackTime = entity.GetAttackInfo().nextAttackTime;
                entity.GetAttackInfo().hasHit = true;

                EntityActions.IncrementAttackChain(entity, attackBox);
                
                target.GetAttackInfo().lastJuggleState = 1;
                entity.GetAttackInfo().lastHitDirection = entity.GetDirX();
                entity.GetAttackInfo().lastAttackState = entity.GetCurrentAnimationState();

                entity.OnAttackHit(target, attackBox);
            }
        }

        private static void CheckComboHitStats(Entity entity) {
            if (entity != null) {

                if (!entity.IsInAnimationAction(Animation.Action.ATTACKING)) { 
                    return;    
                }

                var target = (entity is Projectile ? entity.GetOwner() : entity);
                 
                if (target.GetAttackInfo().comboHitTime < 200) { 
                    target.GetAttackInfo().showComboHits += 1;
                }

                if (target.GetAttackInfo().showComboHits >= target.GetAttackInfo().targetComboHits) { 
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
  
                float velX = 7;
                target.TossFast(-15, velX * dir, 1, 2, true); 
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

        private static void OnTargetHit(Entity target, Entity entity, CLNS.AttackBox attackBox) {
            if (!entity.IsInAnimationAction(Animation.Action.ATTACKING)) { 
                return;    
            }

            if (target != entity) {
                target.OnTargetHit(entity, attackBox);

                int attackDir = (entity.IsLeft() ? 1 : -1);
                float lookDir = (entity.IsLeft() ? -1 : 1);

                if (!target.InvalidHitState()) {

                    if (target.InBlockAction()) {
                        GameManager.GetInstance().PlaySFX("block");
                        EntityActions.FaceTarget(target, entity);
                        target.SetPainTime((int)(attackBox.GetPainTime() / 4));
                        target.SetRumble(lookDir, 2.0f);
                        target.DecreaseBlockResistance();
                        ApplyFrameActions(entity, target, attackBox);
               
                    } else {
                        target.GetAttackInfo().isHit = true;
                        target.GetAttackInfo().lastAttackDir = attackDir;
                        target.GetAttackInfo().attacker = entity;
                        entity.GetAttackInfo().victim = target;

                        CheckComboHitStats(entity);
                        
                        EntityActions.SetPainState(entity, target, attackBox);
                        EntityActions.FaceTarget(target, entity);
                        EntityActions.CheckMaxGrabHits(entity, target);

                        target.SetPainTime((entity is Player ? (int)(attackBox.GetPainTime() / 2) : (int)attackBox.GetPainTime()));
                        target.SetRumble(lookDir, 2.0f);
                        
                        HitPauseTime(entity, target, attackBox);

                        entity.IncreaseMP((int)(attackBox.GetHitDamage() * attackBox.GetHitStrength()));
                        entity.IncreasePoints(attackBox.GetHitPoints());
                        
                        target.DecreaseHealth(attackBox.GetHitDamage() + 10);
                        target.SetLifebarPercent(target.GetHealth());
                        
                        ShowEnemyLifebar(entity, target);
                        ApplyFrameActions(entity, target, attackBox);

                        KnockIfToss(target, attackBox, lookDir);

                        if (entity is Projectile) {
                            EntityActions.SetInfront(entity, target, 10);
                            entity.DecreaseHealth(15);
                        }
                    }
                }

                if (target.InJuggleState()) {

                    if (target.GetAttackInfo().juggleHits < target.GetAttackInfo().maxJuggleHits) { 
                        GameManager.GetInstance().PlaySFX("beat2");
                        target.SetAnimationState(Animation.State.KNOCKED_DOWN1);
                        target.GetCurrentSprite().ResetAnimation();

                        float velX = ((5 - target.GetAttackInfo().juggleHits) * lookDir);
                        float sHeight = -((Math.Abs(target.GetTossInfo().tempHeight) / 2) + 2f);
                        float height = (sHeight / GameManager.GAME_VELOCITY) / 2;

                        CheckComboHitStats(entity);

                        target.TossFast(height, velX, target.GetAttackInfo().maxJuggleHits + 1, 1); 
                        target.SetTossGravity(0.6f);
                        target.SetPainTime((int)attackBox.GetPainTime());
  
                        entity.IncreaseMP((int)(attackBox.GetHitDamage() * attackBox.GetHitStrength()));
                        target.SetHitPauseTime(10);
                        entity.SetHitPauseTime(10);

                        entity.IncreasePoints(attackBox.GetHitPoints());
                        target.DecreaseHealth(attackBox.GetHitDamage() + 5);
                        target.SetLifebarPercent(target.GetHealth());

                        ShowEnemyLifebar(entity, target);

                        if (entity is Projectile) {
                            EntityActions.SetInfront(entity, target, 10);
                            entity.DecreaseHealth(15);
                        }
                    }

                    if (target.GetAttackInfo().juggleHits <= 0) {
                        target.GetAttackInfo().juggleHits = -1;
                    }

                    target.GetAttackInfo().lastJuggleState = -1;
                }  
            }
        }

        public static void AddSparkState(Entity entity, Entity target, CLNS.AttackBox attackBox) {
            if (!entity.IsInAnimationAction(Animation.Action.ATTACKING)) { 
                return;    
            }

            if (!target.InvalidHitState() || target.InJuggleState()) {
                if (!target.IsInAnimationAction(Animation.Action.BLOCKING)) { 
                    GameManager.AddSpark(entity, target, attackBox, Effect.Type.HIT_SPARK);

                    if (target.InJuggleState()) {
                        target.TakeJuggleHit();
                    }
                } else {
                    GameManager.AddSpark(entity, target, attackBox, Effect.Type.BLOCK_SPARK);
                }
            }
        }

        public static void SetTargetHit(Entity entity, Entity target, CLNS.AttackBox attackBox, ref bool targetHit, float time) {
            if (!entity.IsInAnimationAction(Animation.Action.ATTACKING)) { 
                return;    
            }

            AddSparkState(entity, target, attackBox);

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

            if (target.IsEntity(Entity.ObjectType.ENEMY)) {

                float dirX = (target.IsEdgeX() == false && GameManager.GetInstance().CollisionManager.FindObstacle(target) == false
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
    }
}
