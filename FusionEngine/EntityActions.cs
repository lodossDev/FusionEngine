using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public static class EntityActions {

        public static void SetGrabbedHitPain(Entity entity, Entity target, CLNS.AttackBox.AttackType attackType) {
            if (attackType == CLNS.AttackBox.AttackType.LIGHT) {
                target.SetAnimationState(target.GetLightPainGrabbedState());

            } else if (attackType == CLNS.AttackBox.AttackType.MEDIUM) {
                target.SetAnimationState(target.GetMediumPainGrabbedState());

            } else if (attackType == CLNS.AttackBox.AttackType.HEAVY) {
                target.SetAnimationState(target.GetHeavyPainGrabbedState());
            }
        }

        public static void SetDefaultHitPain(Entity entity, Entity target, CLNS.AttackBox.AttackType attackType) {
            if (attackType == CLNS.AttackBox.AttackType.LIGHT) {
                target.SetAnimationState(target.GetLowPainState());

            } else if (attackType == CLNS.AttackBox.AttackType.MEDIUM) {
                target.SetAnimationState(target.GetMediumPainState());

            } else if (attackType == CLNS.AttackBox.AttackType.HEAVY) {
                target.SetAnimationState(target.GetHeavyPainState());
            }
        }

        public static void SetPainState(Entity entity, Entity target, CLNS.AttackBox attackBox) {
            if (!target.IsInAnimationAction(Animation.Action.KNOCKED)) {
                if (target.GetGrabInfo().isGrabbed) {
                    SetGrabbedHitPain(entity, target, attackBox.GetAttackType());
                } else {
                    SetDefaultHitPain(entity, target, attackBox.GetAttackType());
                }
            }
        }

        public static void FaceTarget(Entity target, Entity entity) {
            if (target.IsEntity(Entity.ObjectType.ENEMY)) { 
                if (entity.GetDirX() > 0) {
                    target.SetIsLeft(true);

                } else if (entity.GetDirX() < 0){
                    target.SetIsLeft(false); 
                }
            }
        }

        public static void CheckMaxGrabHits(Entity entity, Entity target) {
            if (target.GetGrabInfo().isGrabbed) { 
                if (!target.IsToss()) { 
                    target.GetGrabInfo().grabHitCount--;
                }

                if (target.GetGrabInfo().grabHitCount < 0) {
                    if (target.HasSprite(Animation.State.KNOCKED_DOWN1)) { 
                        target.SetAnimationState(Animation.State.KNOCKED_DOWN1);
                    } else {
                        target.SetAnimationState(Animation.State.THROWN1);
                    }

                    target.Toss(entity.GetGrabInfo().throwHeight, entity.GetGrabInfo().throwVelX * target.GetGrabInfo().grabDirection, 1, 2);
                    Ungrab(entity, target);
                }
            }
        }

        public static void OnGrab(out float newx, out float x, out float targetx, Entity entity, Entity target) {
            Entity obstacle = target.GetCollisionInfo().GetObstacle();
            int oWidth = (obstacle != null ? obstacle.GetBoundsBox().GetWidth() : 0);
            int ox = 0;

            if (obstacle != null) { 
                if (target.GetCollisionInfo().IsLeft()) {
                    ox = (int)(oWidth / (int)Math.Floor(entity.GetAbsoluteVelX() * GameSystem.GAME_VELOCITY));
                } else if (target.GetCollisionInfo().IsRight()) {
                    ox = -(int)(oWidth / (int)Math.Floor(entity.GetAbsoluteVelX() * GameSystem.GAME_VELOCITY));
                }
            }

            if (entity.GetGrabInfo().grabIn == 1) {
                newx = x = entity.GetPosX() + ox;
                targetx = x + ((target.GetPosX() > entity.GetPosX()) ? (entity.GetGrabInfo().dist / 2) : -(entity.GetGrabInfo().dist / 2));
            } else {
                x = ((entity.GetPosX() + target.GetPosX()) / 2) + ox;
                newx = x + ((entity.GetPosX() >= target.GetPosX()) ? (entity.GetGrabInfo().dist / 2) : -(entity.GetGrabInfo().dist / 2));
                targetx = x + ((target.GetPosX() > entity.GetPosX()) ? (entity.GetGrabInfo().dist / 2) : -(entity.GetGrabInfo().dist / 2));
            }

            entity.SetPosX(newx);
            if(!target.GetRumble().isRumble)target.SetPosX(targetx);

            EntityActions.SetGrabHeight(entity, target);
            EntityActions.LinkGrab(entity, target);
        }

        public static void SetGrabPosition(out float newx, out float newz, out float x, out float targetx, out float targetz, Entity entity, Entity target) {
            Entity obstacle = target.GetCollisionInfo().GetObstacle();
            int oWidth = (obstacle != null ? obstacle.GetBoundsBox().GetWidth() : 0);
            int ox = 0;
            
            if (obstacle != null) { 
                if (target.GetCollisionInfo().IsLeft()) {
                    ox = (int)(oWidth / (int)Math.Floor(entity.GetAbsoluteVelX() * GameSystem.GAME_VELOCITY));
                } else if (target.GetCollisionInfo().IsRight()) {
                    ox = -(int)(oWidth / (int)Math.Floor(entity.GetAbsoluteVelX() * GameSystem.GAME_VELOCITY));
                }
            }

            if (entity.GetGrabInfo().grabIn == 1) {
                newx = x = entity.GetPosX() + ox;
                targetx = x + ((target.GetPosX() > entity.GetPosX()) ? (entity.GetGrabInfo().dist / 2) : -(entity.GetGrabInfo().dist / 2));
            } else {
                x = ((entity.GetPosX() + target.GetPosX()) / 2) + ox;
                newx = x + ((entity.GetPosX() >= target.GetPosX()) ? (entity.GetGrabInfo().dist / 2) : -(entity.GetGrabInfo().dist / 2));
                targetx = x + ((target.GetPosX() > entity.GetPosX()) ? (entity.GetGrabInfo().dist / 2) : -(entity.GetGrabInfo().dist / 2));
            }

            newz = targetz = entity.GetPosZ() - ((entity.GetPosZ() - target.GetPosZ()));

            EntityActions.SetGrabMovementLink(entity, target);
            EntityActions.SetGrabDirection(out targetx, x, entity, target);
                                            
            if (target.GetCollisionInfo().IsCollideX(Attributes.CollisionState.NO_COLLISION)) {
                if(!target.GetRumble().isRumble)target.SetPosX(targetx);
            } else {
                entity.SetPosX(newx);
            }

            int zOffset = (entity.GetDepthBox().GetRect().Bottom - target.GetDepthBox().GetRect().Bottom);
            target.SetLayerPos(10);

            if (entity.GetGrabInfo().grabPos == -1) {
                target.SetLayerPos(-10);
            }

            if (target.GetCollisionInfo().IsCollideZ(Attributes.CollisionState.NO_COLLISION)) {
                target.SetPosZ(newz + zOffset);
            }        
        }

        public static void SetGrabDirection(out float targetx, float x, Entity entity, Entity target) {
             if (entity.IsLeft()) {
                targetx = x - (entity.GetGrabInfo().dist / 2);
                target.GetGrabInfo().grabDirection = -1;
            } else {
                targetx = x + (entity.GetGrabInfo().dist / 2);
                target.GetGrabInfo().grabDirection = 1;
            }
        }

        public static void SetGrabAnimation(Entity entity, Entity target) {
            if (!entity.IsInAnimationAction(Animation.Action.ATTACKING) 
                    && !entity.IsInAnimationAction(Animation.Action.GRABBING)
                    && !entity.IsInAnimationAction(Animation.Action.THROWING)
                    && !target.IsToss()) {

                entity.SetAnimationState(Animation.State.GRAB_HOLD1);
            }

            if (!target.IsInAnimationAction(Animation.Action.INPAIN) && !target.IsToss()) { 
                target.SetAnimationState(Animation.State.INGRAB1);
            }
        }

        public static void ThrowIfNoGrab(Entity entity, Entity target) {
            if (entity.GetThrowState() != null && entity.GetGrabHoldState() == null) {
                ThrowTarget(entity, target);
            }
        }

        public static void ThrowTarget(Entity entity, Entity target) {
            entity.SetAnimationState(Animation.State.THROW1);

            float velX = entity.GetGrabInfo().throwVelX * -entity.GetDirX();
            target.Toss(entity.GetGrabInfo().throwHeight, velX, 1, 2);
            target.SetAnimationState(Animation.State.THROWN1);

            target.SetIsLeft(false);
            EntityActions.Ungrab(entity, target);
        }

        public static void SetGrabMovementLink(Entity entity, Entity target) {
            if (entity.GetDirX() > 0) {
                target.SetIsLeft(true);
            } else {
                target.SetIsLeft(false);
            }

            target.MoveX(entity.GetAccelX(), entity.GetDirX());
            target.MoveZ(entity.GetAccelZ(), entity.GetDirZ());

            target.SetAbsoluteVelX(entity.GetAbsoluteVelX());
            target.SetAbsoluteVelZ(entity.GetAbsoluteVelZ());

            target.SetDirectionX(entity.GetDirX());
            target.SetDirectionZ(entity.GetDirZ());
        }

        public static void LinkGrab(Entity entity, Entity target) {
            entity.SetAnimationState(Animation.State.GRAB_HOLD1);
            target.SetAnimationState(Animation.State.INGRAB1);

            target.GetGrabInfo().isGrabbed = true;
            target.GetGrabInfo().grabbedBy = entity;
            entity.GetGrabInfo().grabbed = target;
        }

        public static void SetGrabHeight(Entity entity, Entity target) {
            target.SetPosY((entity.GetGround() + entity.GetGrabInfo().grabHeight));
            target.SetGround((entity.GetGround() + entity.GetGrabInfo().grabHeight));
        }

        public static void SetGrabGround(Entity entity, Entity target) {
            target.SetGround((entity.GetGround() + entity.GetGrabInfo().grabHeight));
        }

        public static void Ungrab(Entity entity, Entity target) {
            entity.GetGrabInfo().Reset();
            target.GetGrabInfo().Reset();
            //target.SetLayerPos(0);

            if (target.InAir() && target.IsToss()) {
                target.Toss(8);
            }
        }

        public static void CheckGrabTime(Entity entity, Entity target) {
            if (!entity.IsInAnimationAction(Animation.Action.ATTACKING)) {
                target.GetGrabInfo().grabbedTime--;
            }

            if (target.GetGrabInfo().grabbedTime <= 0) {
                target.GetAttackInfo().hitPauseTime = 0;

                if (!entity.IsInAnimationAction(Animation.Action.ATTACKING)) {
                    target.GetAttackInfo().isHit = false;
                    target.GetGrabInfo().grabbedTime = target.GetGrabInfo().maxGrabbedTime;
                    Ungrab(entity, target);

                } else {
                    target.GetGrabInfo().Reset();
                }
            }
        }

        public static void CheckUnGrabDistance(Entity entity, Entity target, float distX, float distZ) {
            if (target.GetGrabInfo().isGrabbed && (((distX > entity.GetGrabInfo().dist + 50) 
                    || distZ > (target.GetDepthBox().GetHeight() / 1.2) + 5))
                    || target.GetGrabInfo().grabbedTime <= 0) {

                Ungrab(entity, target);
            }

            if (entity.GetGrabInfo().grabbed == null && entity.IsInAnimationAction(Animation.Action.GRABBING)) {
                entity.SetAnimationState(Animation.State.STANCE);
            }
        }

        public static void IncrementAttackChain(Entity entity, CLNS.AttackBox attackBox) {
            ComboAttack.Chain attackChain = entity.GetDefaultAttackChain();

            if (attackChain != null) { 
                if (entity.GetAttackInfo().lastHitDirection == entity.GetDirX()) {
                    attackChain.IncrementMoveIndex(attackBox.GetComboStep());
                } else {
                    attackChain.ResetMove();
                }
            }
        }

        public static void ResetAttackChain(Entity entity) {
            if (entity.IsInAnimationAction(Animation.Action.ATTACKING) 
                    && entity.GetCurrentSprite().IsAnimationComplete()) {

                entity.GetAttackInfo().hasHit = false;
            }

            if (entity.GetAttackInfo().lastHitDirection != entity.GetDirX()) {
                if (entity.GetDefaultAttackChain() != null) {
                    entity.GetDefaultAttackChain().ResetMove();
                }
            } 
        }
    }
}
