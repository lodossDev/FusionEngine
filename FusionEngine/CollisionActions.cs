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

                    OnAttack(entity, target, attackBox);
                    entity.OnAttack(target, attackBox);

                    entity.GetAttackInfo().lastHitDirection = entity.GetDirX();
                    entity.GetAttackInfo().lastAttackState = entity.GetCurrentAnimationState();                                               
                }
            } else { 
                if (entity.GetAttackInfo().lastAttackState != entity.GetCurrentAnimationState()) {
                    OnAttack(entity, target, attackBox);
                    entity.OnAttack(target, attackBox);

                    entity.GetAttackInfo().lastHitDirection = entity.GetDirX();
                    entity.GetAttackInfo().lastAttackState = entity.GetCurrentAnimationState();                                               
                }

                if (entity.GetAttackInfo().lastAttackFrame != entity.GetCurrentSprite().GetCurrentFrame()) {
                    CollisionManager.current_hit_id++;
                    entity.GetAttackInfo().lastAttackFrame = entity.GetCurrentSprite().GetCurrentFrame();
                }
            }    
        }

        public static void SetTargetHit(Entity entity, Entity target, CLNS.AttackBox attackBox, ref bool targetHit) {
            if (!targetHit) {
                OnHit(target, entity, attackBox);
                target.OnHit(entity, attackBox);
                targetHit = true;
            }

            if (!target.IsInAnimationAction(Animation.Action.BLOCKING)) { 
                AddHitSpark(entity, target, attackBox);
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

        public static void AddHitSpark(Entity entity, Entity target, CLNS.AttackBox attackBox) {
            Effect sparkInfo = GetSpark(entity, target, attackBox, Effect.Type.HIT_SPARK);

            if (sparkInfo != null) { 
                float x1 = TargetBodyX(target, entity, attackBox);
                float y1 = TargetBodyY(target, entity, attackBox);

                Entity hitSpark1 = new Entity(Entity.ObjectType.HIT_FLASH, sparkInfo.GetName());
                hitSpark1.AddSprite(Animation.State.STANCE, new Sprite(sparkInfo.GetAsset(), Animation.Type.ONCE));
                hitSpark1.SetAnimationState(Animation.State.STANCE);
                hitSpark1.SetFrameDelay(Animation.State.STANCE, (int)sparkInfo.GetDelay());
                hitSpark1.SetScale(sparkInfo.GetScale().X, sparkInfo.GetScale().Y);

                hitSpark1.SetPostion(x1 , y1, entity.GetPosZ() + 5);
                hitSpark1.SetLayerPos(target.GetDepthBox().GetRect().Bottom + 15);
                hitSpark1.SetFade(sparkInfo.GetAlpha());

                CollisionManager.renderManager.AddEntity(hitSpark1);
            }
        }

        private static float TargetBodyX(Entity target, Entity entity, CLNS.AttackBox attackBox) {
            float v1 = ((target.GetPosX() + entity.GetPosX()) / 2);

            if (entity.IsLeft()) {
                v1 -= attackBox.GetOffset().X;
            } else {
                v1 += attackBox.GetOffset().X;
            }

            return v1;
        }

        private static float TargetBodyY(Entity target, Entity entity, CLNS.AttackBox attackBox) {
            return (int)-attackBox.GetRect().Height + (int)Math.Round(attackBox.GetOffset().Y + entity.GetPosY());
        }

        private static void OnAttack(Entity entity, Entity target, CLNS.AttackBox attackBox) {
            if (entity != target) {
                EntityActions.IncrementAttackChain(entity, attackBox);
            }
        }

        private static void OnHit(Entity target, Entity entity, CLNS.AttackBox attackBox) {
            if (target != entity) {
                CollisionManager.hitCount++;
                CollisionManager.hiteffect1.CreateInstance().Play();
                //target.Toss(-5.2f, 0, 200000000);
                float dir = (entity.IsLeft() ? -1 : 1);

                EntityActions.SetPainState(entity, target, attackBox);
                target.GetCurrentSprite().ResetAnimation();
                target.SetPainTime(80);
                target.SetRumble(dir, 2.8f);
                EntityActions.FaceTarget(target, entity);

                EntityActions.CheckMaxGrabHits(entity, target);
                target.DecreaseHealth(attackBox.GetHitDamage());
                //target.SetHitPauseTime(50);
                //target.MoveY(-125 * attackBox.GetHitStrength());
            }
        }
    }
}
