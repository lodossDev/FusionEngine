using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public static class CollisionActions {

        public static Effect GetSpark(Entity entity, CLNS.AttackBox.AttackType state, Effect.Type effectType) {
            Effect spark = null;

            switch(state) {
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
                    spark = GameManager.GetInstance().GetHitSpark(Effect.State.LIGHT);

                } else if (effectType == Effect.Type.BLOCK_SPARK) {
                    spark = GameManager.GetInstance().GetBlockSpark(Effect.State.LIGHT);
                }
            }

            return spark;
        }

        public static void AddSpark(Entity entity, Entity target, CLNS.BoundingBox box, CLNS.AttackBox.AttackType state, Effect.Type effectType) {
            Effect sparkInfo = GetSpark(entity, state, effectType);

            if (sparkInfo != null) { 
                float x1 = HitBodyX(target, entity, box);
                float y1 = HitBodyY(target, entity, box);

                Entity spark = new Entity(Entity.ObjectType.HIT_FLASH, sparkInfo.GetName());
                spark.AddSprite(Animation.State.STANCE, new Sprite(sparkInfo.GetAsset(), Animation.Type.ONCE));
                spark.SetAnimationState(Animation.State.STANCE);
                spark.SetFrameDelay(Animation.State.STANCE, sparkInfo.GetDelay());
                spark.SetOffset(Animation.State.STANCE, sparkInfo.GetOffset().X, sparkInfo.GetOffset().Y);
                spark.SetScale(sparkInfo.GetScale().X, sparkInfo.GetScale().Y);

                spark.SetPostion(x1, y1, entity.GetPosZ() + 5);
                spark.SetLayerPos(target.GetDepthBox().GetRect().Bottom + 15);
                spark.SetFade(sparkInfo.GetAlpha());

                if (sparkInfo.IsLeft()) { 
                    if (entity.GetDirX() > 0) {
                        spark.SetIsLeft(true);
                    } else {
                        spark.SetIsLeft(false);
                    }
                }

                GameManager.GetInstance().AddSpark(spark);
            }
        }

         public static void AddSpark(Entity entity, Entity target, CLNS.AttackBox attackBox, Effect.Type effectType) {
            Effect sparkInfo = GetSpark(entity, attackBox.GetAttackType(), effectType);

            if (sparkInfo != null) { 
                float x1 = HitBodyX(target, entity, attackBox);
                float y1 = HitBodyY(target, entity, attackBox);

                Entity spark = new Entity(Entity.ObjectType.HIT_FLASH, sparkInfo.GetName());
                spark.AddSprite(Animation.State.STANCE, new Sprite(sparkInfo.GetAsset(), Animation.Type.ONCE));
                spark.SetAnimationState(Animation.State.STANCE);
                spark.SetFrameDelay(Animation.State.STANCE, sparkInfo.GetDelay());
                spark.SetOffset(Animation.State.STANCE, sparkInfo.GetOffset().X, sparkInfo.GetOffset().Y);
                spark.SetScale(sparkInfo.GetScale().X, sparkInfo.GetScale().Y);

                spark.SetPostion(x1, y1, entity.GetPosZ() + 5);
                spark.SetLayerPos(target.GetDepthBox().GetRect().Bottom + 15);
                spark.SetFade(sparkInfo.GetAlpha());

                if (sparkInfo.IsLeft()) { 
                    if (entity.GetDirX() > 0) {
                        spark.SetIsLeft(true);
                    } else {
                        spark.SetIsLeft(false);
                    }
                }

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

        private static float HitBodyX(Entity target, Entity entity, CLNS.BoundingBox box) {
            float x1 = ((target.GetPosX() + entity.GetPosX()) / 2);
            return x1;
        }

        private static float HitBodyY(Entity target, Entity entity, CLNS.AttackBox attackBox) {
            return (int)-attackBox.GetRect().Height + (int)Math.Round(attackBox.GetOffset().Y + entity.GetPosY());
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

                entity.GetAttackInfo().hasHit = true;
                EntityActions.IncrementAttackChain(entity, attackBox);

                target.GetAttackInfo().lastJuggleState = 1;
                entity.GetAttackInfo().lastHitDirection = entity.GetDirX();
                entity.GetAttackInfo().lastAttackState = entity.GetCurrentAnimationState();
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
                        
                        EntityActions.SetPainState(entity, target, attackBox);
                        EntityActions.FaceTarget(target, entity);
                        EntityActions.CheckMaxGrabHits(entity, target);
                        target.SetPainTime(80);
                        target.SetRumble(lookDir, 1.8f);
                        //target.SetHitPauseTime(10);
                        //entity.TossFast(-5);
                        //entity.SetTossGravity(1.83f);
                        //target.DecreaseHealth(attackBox.GetHitDamage());
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
                        target.Toss(height, velX, target.GetAttackInfo().maxJuggleHits + 1, 1); 
                        target.SetTossGravity(0.6f);
                        target.SetPainTime(80);
                        //entity.TossFast(-5);
                        //target.DecreaseHealth(attackBox.GetHitDamage());
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

        public static void SetTargetHit(Entity entity, Entity target, CLNS.AttackBox attackBox, ref bool targetHit) {
            if (!entity.IsInAnimationAction(Animation.Action.ATTACKING)) { 
                return;    
            }

            AddSparkState(entity, target, attackBox);

            if (!targetHit) {
                OnTargetHit(target, entity, attackBox);
                targetHit = true;
            }
        }

        public static void ApplyFrameActions(Entity entity, Entity target, CLNS.AttackBox attackBox) {
            if (!entity.IsInAnimationAction(Animation.Action.ATTACKING)) { 
                return;    
            }

            if (target.IsEntity(Entity.ObjectType.ENEMY) && !target.IsGrabbed()) {
 
                if (!attackBox.IsKnock()) {

                    if (!target.InBlockAction() || (target.InBlockAction() 
                            && (target.GetAttackInfo().blockMode == 2 
                                    || target.GetAttackInfo().blockMode == 3))) { 

                        target.MoveX((attackBox.GetMoveX() * entity.GetDirX()));

                        if (!target.InBlockAction()) {
                            target.Toss(attackBox.GetTossHeight());
                        }
                    }
                } else {
                    if (!target.InBlockAction()) {
                        target.SetAnimationState(Animation.State.KNOCKED_DOWN1);
                        target.SetCurrentKnockedState(Attributes.KnockedState.KNOCKED_DOWN);
                        target.Toss(attackBox.GetTossHeight(), (attackBox.GetMoveX() * entity.GetDirX()));
                    }

                    if (target.InBlockAction() && target.GetAttackInfo().blockMode == 3) {
                        target.MoveX((attackBox.GetMoveX() * entity.GetDirX()));
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
                        entity.IncreaseHealth(collectable.GetPoints());
                    } else if (collectable is Money) {
                        GameManager.GetInstance().PlaySFX("1up");
                        entity.IncreasePoints(collectable.GetPoints());
                    } else if (collectable is Life) {
                        GameManager.GetInstance().PlaySFX("1up");
                        entity.IncreaseLives(collectable.GetPoints());
                    } else if (collectable is MP) {
                        GameManager.GetInstance().PlaySFX("1up");
                        entity.IncreaseMP(collectable.GetPoints());
                    }

                    isCollected = true;
                }

                GameManager.GetInstance().RemoveEntity(collectable);
            }
        }
    }
}
