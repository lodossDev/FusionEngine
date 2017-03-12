using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public static class EntityActions {

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
            target.GetGrabInfo().grabHitCount++;

            if (target.GetGrabInfo().grabHitCount > target.GetGrabInfo().maxGrabHits) {
                entity.SetAnimationState(Animation.State.STANCE);
                target.Toss(entity.GetGrabInfo().throwHeight, entity.GetGrabInfo().throwVelX * target.GetGrabInfo().grabDirection, 1, 2);

                Ungrab(entity, target);
                target.GetGrabInfo().Reset();
            }
        }

        public static void SetGrabAnimation(Entity entity, Entity target) {
            if (!entity.IsInAnimationAction(Animation.Action.ATTACKING) 
                    && !entity.IsInAnimationAction(Animation.Action.GRABBING)
                    && !entity.IsInAnimationAction(Animation.Action.THROWING)) {

                entity.SetAnimationState(Animation.State.GRAB_HOLD1);
            }

            if (!target.IsInAnimationAction(Animation.Action.INPAIN)) { 
                target.SetAnimationState(Animation.State.INGRAB1);
            }
        }

        public static void SetGrabDirection(Entity entity, Entity target) {
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

        public static void Ungrab(Entity entity, Entity target) {
            target.GetGrabInfo().isGrabbed = false;
            target.GetGrabInfo().grabbedBy = null;
            entity.GetGrabInfo().grabbed = null;

            if (target.InAir() && target.IsToss()) {
                target.Toss(8);
            }
        }

        public static void CheckGrabTime(Entity entity, Entity target) {
            if (!entity.IsInAnimationAction(Animation.Action.ATTACKING)) {
                target.GetGrabInfo().grabbedTime--;
            }

            if (target.GetGrabInfo().grabbedTime <= 0) {

                if (!entity.IsInAnimationAction(Animation.Action.ATTACKING)) {
                    entity.SetAnimationState(Animation.State.STANCE);
                    target.GetGrabInfo().grabbedTime = 0;

                } else {
                    target.GetGrabInfo().grabbedTime = 500000;
                }
            }
        }

        public static void CheckUnGrabDistance(Entity entity, Entity target, float distX, float distZ) {

            if (target.GetGrabInfo().isGrabbed && (((distX > entity.GetGrabInfo().dist + 50) 
                    || distZ > (target.GetDepthBox().GetHeight() / 2) + 5))
                    || target.GetGrabInfo().grabbedTime <= 0) {

                Ungrab(entity, target);
            }

            if (entity.GetGrabInfo().grabbed == null && entity.IsInAnimationAction(Animation.Action.GRABBING)) {
                entity.SetAnimationState(Animation.State.STANCE);
            }
        }
    }
}
