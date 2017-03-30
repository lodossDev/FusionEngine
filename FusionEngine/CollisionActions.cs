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

        public static bool IsInAttackRange(Entity entity, Entity target, int targetBodyBoxSize) {
            return (Math.Abs(entity.GetDepthBox().GetRect().Bottom - target.GetDepthBox().GetRect().Bottom) < target.GetDepthBox().GetZdepth() + 10 
                        && entity.IsInAnimationAction(Animation.Action.ATTACKING) 
                        && targetBodyBoxSize > 0);
        }

        public static void QueueHitFrames(Entity entity, Entity target, CLNS.AttackBox attackBox) {
            if (attackBox.GetHitType() == CLNS.AttackBox.HitType.ONCE) { 
                if (entity.GetAttackInfo().lastAttackState != entity.GetCurrentAnimationState()) {
                    CollisionManager.current_hit_id++;
                    OnAttackHit(entity, target, attackBox);                                     
                }
            } else { 
                if (entity.GetAttackInfo().lastAttackState != entity.GetCurrentAnimationState()) {
                    OnAttackHit(entity, target, attackBox);                                           
                }

                if (entity.GetAttackInfo().lastAttackFrame != entity.GetCurrentSprite().GetCurrentFrame()) {
                    CollisionManager.current_hit_id++;
                    entity.GetAttackInfo().lastAttackFrame = entity.GetCurrentSprite().GetCurrentFrame();
                }
            }    
        }

        public static void SetTargetHit(Entity entity, Entity target, CLNS.AttackBox attackBox, ref bool targetHit) {
            if (!targetHit) {
                OnTargetHit(target, entity, attackBox);
                targetHit = true;
            }

            if (!target.IsInAnimationAction(Animation.Action.BLOCKING)) { 
                AddSpark(entity, target, attackBox, Effect.Type.HIT_SPARK);
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

                CollisionManager.renderManager.AddEntity(spark);
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
                if (!target.IsInAnimationAction(Animation.Action.BLOCKING)) {
                    CollisionManager.hitCount++;
                    CollisionManager.hiteffect1.CreateInstance().Play();
                    //target.Toss(-5.2f, 0, 200000000);
                    float dir = (entity.IsLeft() ? -1 : 1);

                    EntityActions.SetPainState(entity, target, attackBox);
                    target.SetPainTime(80);
                    target.SetRumble(dir, 2.8f);
                    EntityActions.FaceTarget(target, entity);
                    EntityActions.CheckMaxGrabHits(entity, target);
                    target.DecreaseHealth(attackBox.GetHitDamage());
                    //target.SetHitPauseTime(50);
                    //target.MoveY(-125 * attackBox.GetHitStrength());
                } else {
                    //play blocking sound.
                    AddSpark(entity, target, attackBox, Effect.Type.BLOCK_SPARK);
                }
            }
        }
    }
}
