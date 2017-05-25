﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public static class CollisionActions {

        private static Effect GetEffectState(Entity entity, Effect.Type effectType, Effect.State effectState) {
            Effect spark = null;

            if (effectType == Effect.Type.HIT_SPARK) { 
                spark = entity.GetHitSpark(effectState);

            } else if (effectType == Effect.Type.BLOCK_SPARK) {
                spark = entity.GetBlockSpark(effectState);

            } 

            if (spark == null) {
                if (effectType == Effect.Type.HIT_SPARK) { 
                    spark = GameManager.GetInstance().GetHitSpark(effectState);

                } else if (effectType == Effect.Type.BLOCK_SPARK) {
                    spark = GameManager.GetInstance().GetBlockSpark(effectState);
                }
            }

            return spark;
        }

        public static Effect GetSpark(Entity entity, CLNS.AttackBox.AttackType state, Effect.Type effectType) {
            Effect spark = null;
            Effect.State effectState;

            switch(state) {
                case CLNS.AttackBox.AttackType.LIGHT: 
                    effectState = Effect.State.LIGHT;
                break;

                case CLNS.AttackBox.AttackType.MEDIUM: 
                    effectState = Effect.State.MEDIUM;
                break;

                case CLNS.AttackBox.AttackType.HEAVY: 
                    effectState = Effect.State.HEAVY;
                break;

                default:
                    effectState = Effect.State.LIGHT;
                break;
            }

            spark = GetEffectState(entity, effectType, effectState);

            if (spark == null) {
               spark = GetEffectState(entity, effectType, Effect.State.LIGHT);
            }

            return spark;
        }

        private static Entity CreateSpark(Effect sparkInfo, Entity entity, Entity target, float x1, float y1) {
            Entity spark = new Entity(Entity.ObjectType.HIT_FLASH, sparkInfo.GetName());
            spark.AddSprite(Animation.State.STANCE, new Sprite(sparkInfo.GetAsset(), Animation.Type.ONCE));
            spark.SetAnimationState(Animation.State.STANCE);
            spark.SetFrameDelay(Animation.State.STANCE, sparkInfo.GetDelay());
            spark.SetOffset(Animation.State.STANCE, sparkInfo.GetOffset().X, sparkInfo.GetOffset().Y);
            spark.SetScale(sparkInfo.GetScale().X, sparkInfo.GetScale().Y);

            spark.SetPostion(x1, y1, entity.GetPosZ() + 5);
            spark.SetLayerPos(target.GetDepthBox().GetRect().Bottom + 15);
            spark.SetFade(sparkInfo.GetAlpha());
            spark.SetBlendState(BlendState.Additive);

            if (sparkInfo.IsLeft()) { 
                if (entity.GetDirX() > 0) {
                    spark.SetIsLeft(true);
                } else {
                    spark.SetIsLeft(false);
                }
            }

            return spark;
        }

        public static void AddSpark(Entity entity, Entity target, CLNS.BoundingBox box, CLNS.AttackBox.AttackType state, Effect.Type effectType) {
            Effect sparkInfo = GetSpark(entity, state, effectType);

            if (sparkInfo != null) { 
                float x1 = HitBodyX(target, entity, box);
                float y1 = HitBodyY(target, entity, box);

                Entity spark = CreateSpark(sparkInfo, entity, target, x1, y1);
                GameManager.GetInstance().Render(spark);
            }
        }

         public static void AddSpark(Entity entity, Entity target, CLNS.AttackBox attackBox, Effect.Type effectType) {
            Effect sparkInfo = GetSpark(entity, attackBox.GetAttackType(), effectType);

            if (sparkInfo != null) { 
                float x1 = HitBodyX(target, entity, attackBox);
                float y1 = HitBodyY(target, entity, attackBox);

                Entity spark = CreateSpark(sparkInfo, entity, target, x1, y1);
                GameManager.GetInstance().Render(spark);
            }
        }

        private static float HitBodyX(Entity target, Entity entity, CLNS.AttackBox attackBox) {
            float x1 = ((target.GetPosX() + entity.GetPosX()) / 2);

            if (entity.IsLeft()) {
                x1 -= attackBox.GetOffset().X + attackBox.GetSparkOffset().X;
            } else {
                x1 += attackBox.GetOffset().X + attackBox.GetSparkOffset().X;
            }

            return x1;
        }

        private static float HitBodyX(Entity target, Entity entity, CLNS.BoundingBox box) {
            float x1 = ((target.GetPosX() + entity.GetPosX()) / 2);
            return x1;
        }

        private static float HitBodyY(Entity target, Entity entity, CLNS.AttackBox attackBox) {
            return (int)-attackBox.GetRect().Height + (int)Math.Round(attackBox.GetOffset().Y + entity.GetPosY()) + attackBox.GetSparkOffset().Y;
        }

        private static float HitBodyY(Entity target, Entity entity, CLNS.BoundingBox box) {
            return ((int)Math.Round(box.GetOffset().Y + entity.GetPosY())) / 2;
        }

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
                entity.OnAttackHit(target, attackBox);

                entity.GetAttackInfo().currentAttackTime = entity.GetAttackInfo().nextAttackTime;
                entity.GetAttackInfo().hasHit = true;

                EntityActions.IncrementAttackChain(entity, attackBox);
                
                target.GetAttackInfo().lastJuggleState = 1;
                entity.GetAttackInfo().lastHitDirection = entity.GetDirX();
                entity.GetAttackInfo().lastAttackState = entity.GetCurrentAnimationState();
            }
        }

        private static void CheckComboHitStats(Entity entity) {
            if (entity != null) {

                if (!entity.IsInAnimationAction(Animation.Action.ATTACKING)) { 
                    return;    
                }
                 
                if (entity.GetAttackInfo().comboHitTime < 200) { 
                    entity.GetAttackInfo().showComboHits += 1;
                }

                if (entity.GetAttackInfo().showComboHits >= entity.GetAttackInfo().targetComboHits) { 
                    entity.GetAttackInfo().comboHits += 1;

                    if (entity is Player) {
                        entity.ExpandComboFont();
                    }
                }
            }
        }

        public static void ShowEnemyLifebar(Entity entity, Entity target) {
            if (entity is Player) {
                ((Player)entity).SetCurrentHitLifeBar(target.GetLifeBar());
                ((Player)entity).SetLifebarHitTime(12000);
            }
        }

        private static void OnTargetHit(Entity target, Entity entity, CLNS.AttackBox attackBox) {
            if (!entity.IsInAnimationAction(Animation.Action.ATTACKING)) { 
                return;    
            }

            if (target != entity) {
                target.OnTargetHit(entity, attackBox);

                int attackDir = (entity.GetPosX() > target.GetPosX() ? 1 : -1);
                float lookDir = (entity.IsLeft() ? -1 : 1);

                if (!target.InvalidHitState()) {
                    if (target.InBlockAction()) {
                        GameManager.GetInstance().PlaySFX("block");
                        EntityActions.FaceTarget(target, entity);
                        target.SetPainTime(25);
                        target.SetRumble(lookDir, 1.8f);
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

                        target.SetPainTime((entity is Player ? 40 : 80));
                        target.SetRumble(lookDir, 1.8f);
                        //target.SetHitPauseTime(60);

                        if (entity.InSpecialAttack()) {
                            target.SetHitPauseTime(36);
                            entity.SetHitPauseTime(36);
                        }

                        //entity.TossFast(-5);
                        //entity.SetTossGravity(1.83f);
                        entity.IncreaseMP(20);
                        entity.IncreasePoints(attackBox.GetHitPoints());
                        //target.DecreaseHealth(attackBox.GetHitDamage() + 20);
                        //target.SetLifebarPercent(target.GetHealth());
                        
                        ShowEnemyLifebar(entity, target);
                        ApplyFrameActions(entity, target, attackBox);
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

                        target.Toss(height, velX, target.GetAttackInfo().maxJuggleHits + 1, 1); 
                        target.SetTossGravity(0.6f);
                        target.SetPainTime(80);
                        //entity.TossFast(-5);
                        entity.IncreaseMP(20);

                        //if (entity.InSpecialAttack()) {
                            target.SetHitPauseTime(10);
                            entity.SetHitPauseTime(10);
                        //}

                        entity.IncreasePoints(attackBox.GetHitPoints());
                        //target.DecreaseHealth(attackBox.GetHitDamage());
                        //target.SetLifebarPercent(target.GetHealth());

                        ShowEnemyLifebar(entity, target);
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
                    AddSpark(entity, target, attackBox, Effect.Type.HIT_SPARK);

                    if (target.InJuggleState()) {
                        target.TakeJuggleHit();
                    }
                } else {
                    AddSpark(entity, target, attackBox, Effect.Type.BLOCK_SPARK);
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
                                target.Toss(attackBox.GetTossHeight());
                            }
                        }
                    }
                } else {
                    if (!target.InBlockAction()) {
                        target.SetAnimationState(Animation.State.KNOCKED_DOWN1);
                        target.SetCurrentKnockedState(Attributes.KnockedState.KNOCKED_DOWN);

                        if (attackBox.GetTossHeight() != 0.0) {
                            target.Toss(attackBox.GetTossHeight(), (attackBox.GetMoveX() * dirX), 1, 2, true);
                        }
                    }

                    if (target.InBlockAction() && target.GetAttackInfo().blockMode == 3) {
                        if (attackBox.GetMoveX() != 0.0) {
                            target.MoveX((attackBox.GetMoveX() * dirX));
                        }
                    }

                    if (entity.GetGrabInfo().grabbed != null) { 
                        EntityActions.Ungrab(entity, entity.GetGrabInfo().grabbed);
                    }

                    target.SetHitPauseTime(10);
                    entity.SetHitPauseTime(10);
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
