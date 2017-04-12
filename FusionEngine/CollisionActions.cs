using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public static class CollisionActions {

        public static bool IsHitFrameProcessed(Entity entity, Entity target, CLNS.AttackBox attackBox, int currentAttackHits) {
            return (currentAttackHits > 0 && (attackBox.GetHitType() == CLNS.AttackBox.HitType.FRAME
                        || attackBox.GetHitType() == CLNS.AttackBox.HitType.ONCE));
        }
        
        public static void CheckAttack(Entity entity, Entity target, CLNS.AttackBox attackBox) {
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

        public static void SetTargetHit(Entity entity, Entity target, CLNS.AttackBox attackBox, ref bool targetHit) {
            if (!target.InvalidHitState()) { 
                if (!target.IsInAnimationAction(Animation.Action.BLOCKING)) { 
                    AddSpark(entity, target, attackBox, Effect.Type.HIT_SPARK);
                } else {
                    AddSpark(entity, target, attackBox, Effect.Type.BLOCK_SPARK);
                }
            }

            if (!targetHit) {
                OnTargetHit(target, entity, attackBox);
                targetHit = true;
            }
        }

        public static Effect GetSpark(Entity entity, Entity target, CLNS.AttackBox attackBox, Effect.Type effectType) {
            Effect spark = null;

            switch(attackBox.GetAttackType()) {
                case CLNS.AttackBox.AttackType.LIGHT: 
                    if (effectType == Effect.Type.HIT_SPARK) { 
                        spark = entity.GetHitSpark(Effect.State.LIGHT);

                    } else if (effectType == Effect.Type.BLOCK_SPARK) {
                        spark = entity.GetBlockSpark(Effect.State.LIGHT);
                    }
                break;

                case CLNS.AttackBox.AttackType.MEDIUM: 
                    if (effectType == Effect.Type.HIT_SPARK) { 
                        spark = entity.GetHitSpark(Effect.State.MEDIUM);

                    } else if (effectType == Effect.Type.BLOCK_SPARK) {
                        spark = entity.GetBlockSpark(Effect.State.MEDIUM);
                    }
                break;

                case CLNS.AttackBox.AttackType.HEAVY: 
                    if (effectType == Effect.Type.HIT_SPARK) { 
                        spark = entity.GetHitSpark(Effect.State.HEAVY);

                    } else if (effectType == Effect.Type.BLOCK_SPARK) {
                        spark = entity.GetBlockSpark(Effect.State.HEAVY);
                    }
                break;

                default:
                    if (effectType == Effect.Type.HIT_SPARK) { 
                        spark = entity.GetHitSpark(Effect.State.LIGHT);

                    } else if (effectType == Effect.Type.BLOCK_SPARK) {
                        spark = entity.GetBlockSpark(Effect.State.LIGHT);
                    }
                break;
            }

            if (spark == null) {
                if (effectType == Effect.Type.HIT_SPARK) { 
                    spark = entity.GetHitSpark(Effect.State.LIGHT);

                } else if (effectType == Effect.Type.BLOCK_SPARK) {
                    spark = entity.GetBlockSpark(Effect.State.LIGHT);
                }
            }

            return spark;
        }

        public static void AddSpark(Entity entity, Entity target, CLNS.AttackBox attackBox, Effect.Type effectType) {
            Effect sparkInfo = GetSpark(entity, target, attackBox, effectType);

            if (sparkInfo != null) { 
                float x1 = HitBodyX(target, entity, attackBox);
                float y1 = HitBodyY(target, entity, attackBox);

                Entity spark = new Entity(Entity.ObjectType.HIT_FLASH, sparkInfo.GetName());
                spark.AddSprite(Animation.State.STANCE, new Sprite(sparkInfo.GetAsset(), Animation.Type.ONCE));
                spark.SetAnimationState(Animation.State.STANCE);
                spark.SetFrameDelay(Animation.State.STANCE, sparkInfo.GetDelay());
                spark.SetScale(sparkInfo.GetScale().X, sparkInfo.GetScale().Y);

                spark.SetPostion(x1 , y1, entity.GetPosZ());
                spark.SetLayerPos(target.GetDepthBox().GetRect().Bottom + 15);
                spark.SetFade(sparkInfo.GetAlpha());

                GameManager.GetInstance().AddSpark(spark);
            }
        }

        private static float HitBodyX(Entity target, Entity entity, CLNS.AttackBox attackBox) {
            float x1 = ((target.GetPosX() + entity.GetPosX()) / 2);

            if (entity.IsLeft()) {
                x1 -= attackBox.GetOffset().X;
            } else {
                x1 += attackBox.GetOffset().X;
            }

            return x1;
        }

        private static float HitBodyY(Entity target, Entity entity, CLNS.AttackBox attackBox) {
            return (int)-attackBox.GetRect().Height + (int)Math.Round(attackBox.GetOffset().Y + entity.GetPosY());
        }

        private static void OnAttackHit(Entity entity, Entity target, CLNS.AttackBox attackBox) {
            entity.OnAttackHit(target, attackBox);

            if (entity != target) {
                entity.GetAttackInfo().hasHit = true;
                EntityActions.IncrementAttackChain(entity, attackBox);

                entity.GetAttackInfo().lastHitDirection = entity.GetDirX();
                entity.GetAttackInfo().lastAttackState = entity.GetCurrentAnimationState();
            }
        }

        private static void OnTargetHit(Entity target, Entity entity, CLNS.AttackBox attackBox) {
            target.OnTargetHit(entity, attackBox);

            if (target != entity) {
                target.GetAttackInfo().isHit = true;
                target.GetAttackInfo().lastAttackDir = (entity.GetPosX() > target.GetPosX() ? 1 : -1);
                target.GetAttackInfo().attacker = entity;

                //Put this in inner if after invalidHitState
                if (!target.InvalidHitState()) {
                    float dir = (entity.IsLeft() ? -1 : 1);

                    if (target.InBlockAction()) {
                        GameManager.GetInstance().PlaySFX("block");
                        EntityActions.FaceTarget(target, entity);
                        target.SetPainTime(25);
                        target.SetRumble(dir, 1.8f);
                        target.DecreaseBlockResistance();
                        ApplyFrameActions(entity, target, attackBox);
               
                    } else {      
                        EntityActions.SetPainState(entity, target, attackBox);
                        EntityActions.FaceTarget(target, entity);
                        EntityActions.CheckMaxGrabHits(entity, target);
                        target.SetPainTime(80);
                        target.SetRumble(dir, 1.8f);
                        target.DecreaseHealth(attackBox.GetHitDamage());
                        ApplyFrameActions(entity, target, attackBox);
                    }
                }
            }
        }

        public static void ApplyFrameActions(Entity entity, Entity target, CLNS.AttackBox attackBox) {
            if (target.IsEntity(Entity.ObjectType.ENEMY) 
                    && !target.GetGrabInfo().isGrabbed) {

                for (int i = 0; i < 2; i++) { 
                    if (attackBox.GetMoveX() != 0.0) {
                        if (entity.GetPosX() < target.GetPosX()) {
                            target.MoveX(attackBox.GetMoveX());
                        } else {
                            target.MoveX(-attackBox.GetMoveX());
                        }
                    }
                }

                if (attackBox.GetTossHeight() != 0.0) {
                    target.Toss(attackBox.GetTossHeight());
                }
            }
        }

        public static void ProcessGrabItem(Entity entity, Collectable collectable, ref bool isCollected) {
            entity.GetCollisionInfo().SetItem(collectable);

            if (entity.InGrabItemFrameState()) {

                if (!isCollected) { 

                    if (collectable is Health) {
                        entity.IncreaseHealth(collectable.GetPoints());

                    } else if (collectable is Money) {
                        entity.IncreasePoints(collectable.GetPoints());

                    } else if (collectable is Life) {
                        entity.IncreaseLives(collectable.GetPoints());

                    } else if (collectable is MP) {
                        entity.IncreaseMP(collectable.GetPoints());
                    }

                    GameManager.GetInstance().PlaySFX("1up");
                    isCollected = true;
                }

                GameManager.GetInstance().RemoveEntity(collectable);
            }
        }
    }
}
