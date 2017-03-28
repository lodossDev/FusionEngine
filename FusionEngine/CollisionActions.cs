using System;
using System.Collections.Generic;
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
