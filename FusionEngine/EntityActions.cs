using Microsoft.Xna.Framework.Audio;
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
                GameManager.GetInstance().PlaySFX(target, target.GetLightPainGrabbedState(), "beat2");

            } else if (attackType == CLNS.AttackBox.AttackType.MEDIUM) {
                target.SetAnimationState(target.GetMediumPainGrabbedState());
                GameManager.GetInstance().PlaySFX(target, target.GetMediumPainGrabbedState(), "beat1");

            } else if (attackType == CLNS.AttackBox.AttackType.HEAVY) {
                target.SetAnimationState(target.GetHeavyPainGrabbedState());
                GameManager.GetInstance().PlaySFX(target, target.GetHeavyPainGrabbedState(), "beat2");
            }
        }

        public static void SetDefaultHitPain(Entity entity, Entity target, CLNS.AttackBox.AttackType attackType) {
            if (attackType == CLNS.AttackBox.AttackType.LIGHT) {
                target.SetAnimationState(target.GetLowPainState());
                GameManager.GetInstance().PlaySFX(target, target.GetLowPainState(), "beat2");

            } else if (attackType == CLNS.AttackBox.AttackType.MEDIUM) {
                target.SetAnimationState(target.GetMediumPainState());
                GameManager.GetInstance().PlaySFX(target, target.GetMediumPainState(), "beat1");

            } else if (attackType == CLNS.AttackBox.AttackType.HEAVY) {
                target.SetAnimationState(target.GetHeavyPainState());
                GameManager.GetInstance().PlaySFX(target, target.GetHeavyPainState(), "beat2");
            }
        }

        public static void SetPainState(Entity entity, Entity target, CLNS.AttackBox attackBox) {
            if (!target.IsInAnimationAction(Animation.Action.KNOCKED)) {
                if (target.IsGrabbed()) {
                    target.GetGrabInfo().grabbedTime += 15;
                    SetGrabbedHitPain(entity, target, attackBox.GetAttackType());
                } else {
                    SetDefaultHitPain(entity, target, attackBox.GetAttackType());
                }

                target.GetCurrentSprite().ResetAnimation();
            }
        }

        public static void FaceTarget(Entity target, Entity entity) {
            if (target is Character 
                    && !target.HasGrabbed() 
                    && !target.GetGrabInfo().inGrabHeight) { 

                if (entity.GetPosX() > target.GetPosX() + 20) {
                    target.SetIsLeft(false);

                } else if (entity.GetPosX() + 20 < target.GetPosX()){
                    target.SetIsLeft(true); 
                }
            }
        }

        public static void CheckMaxGrabHits(Entity entity, Entity target) {
            if (!entity.IsInAnimationAction(Animation.Action.ATTACKING)) {
                return ;
            }

            if (target.IsGrabbed()) { 
                if (!target.IsToss()) { 
                    target.GetGrabInfo().grabHitCount--;
                }

                if (target.GetGrabInfo().grabHitCount < 0) {
                    if (target.HasSprite(Animation.State.KNOCKED_DOWN1)) { 
                        target.SetAnimationState(Animation.State.KNOCKED_DOWN1);
                        target.SetCurrentKnockedState(Attributes.KnockedState.KNOCKED_DOWN);
                        target.Toss(entity.GetGrabInfo().throwHeight, entity.GetGrabInfo().throwVelX * target.GetGrabInfo().grabDirection, 1, 2);

                    } else {
                        if (target.HasSprite(Animation.State.THROW1)) { 
                            if (entity.GetCurrentAnimationState().ToString().Contains("GRAB")) {
                                ThrowTarget(entity, target);
                            }
                        }
                    }

                    Ungrab(entity, target);
                }
            }
        }

        public static void OnGrab(ref float newx, ref float x, ref float targetx, Entity entity, Entity target) {
            Entity obstacle = target.GetCollisionInfo().GetObstacle();
            int oWidth = (obstacle != null 
                                && (Math.Abs(obstacle.GetDepthBox().GetRect().Bottom - target.GetDepthBox().GetRect().Bottom) < 10)  
                                && target.GetCollisionInfo().GetCollideX() !=  Attributes.CollisionState.NO_COLLISION ? obstacle.GetBoundsBox().GetWidth() : 0);
            int ox = 0;

            if (obstacle != null) { 
                if (target.GetCollisionInfo().IsLeft()) {
                    ox = (int)(oWidth / (int)Math.Floor(entity.GetAbsoluteVelX() * GameManager.GAME_VELOCITY));
                } else if (target.GetCollisionInfo().IsRight()) {
                    ox = -(int)(oWidth / (int)Math.Floor(entity.GetAbsoluteVelX() * GameManager.GAME_VELOCITY));
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

        public static void SetGrabPosition(ref float newx, ref float newz, ref float x, ref float targetx, ref float targetz, Entity entity, Entity target) {
            Entity obstacle = target.GetCollisionInfo().GetObstacle();
            int oWidth = (obstacle != null 
                                && (Math.Abs(obstacle.GetDepthBox().GetRect().Bottom - target.GetDepthBox().GetRect().Bottom) < 10)  
                                && target.GetCollisionInfo().GetCollideX() !=  Attributes.CollisionState.NO_COLLISION ? obstacle.GetBoundsBox().GetWidth() : 0);
            int ox = 0;
            
            if (obstacle != null) { 
                if (target.GetCollisionInfo().IsLeft()) {
                    ox = (int)(oWidth / (int)Math.Floor(entity.GetAbsoluteVelX() * GameManager.GAME_VELOCITY));
                } else if (target.GetCollisionInfo().IsRight()) {
                    ox = -(int)(oWidth / (int)Math.Floor(entity.GetAbsoluteVelX() * GameManager.GAME_VELOCITY));
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
            
            target.StopMovement();        
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
            target.SetCurrentKnockedState(Attributes.KnockedState.THROWN);
            target.GetAttackInfo().lastJuggleState += 1;
            target.GetAttackInfo().attacker = entity;

            EntityActions.Ungrab(entity, target);
        }

        public static void SetGrabMovementLink(Entity entity, Entity target) {
            if (entity.GetDirX() > 0) {
                target.SetIsLeft(true);
            } else {
                target.SetIsLeft(false);
            }

            target.MoveX(entity.GetAccelX(), entity.GetDirX());
            target.MoveY(entity.GetAbsoluteVelY());
            target.MoveZ(entity.GetAccelZ(), entity.GetDirZ());

            target.SetAbsoluteVelX(entity.GetAbsoluteVelX());
            target.SetAbsoluteVelY(entity.GetAbsoluteVelY());
            target.SetAbsoluteVelZ(entity.GetAbsoluteVelZ());

            target.SetDirectionX(entity.GetDirX());
            target.SetDirectionY(entity.GetDirY());
            target.SetDirectionZ(entity.GetDirZ());
        }

        public static void LinkGrab(Entity entity, Entity target) {
            entity.SetAnimationState(Animation.State.GRAB_HOLD1);
            target.SetAnimationState(Animation.State.INGRAB1);

            target.GetGrabInfo().isGrabbed = true;
            target.GetGrabInfo().grabbedBy = entity;
            target.GetAttackInfo().attacker = entity;
            entity.GetGrabInfo().grabbed = target;
        }

        public static void SetGrabHeight(Entity entity, Entity target) {
            target.SetPosY((entity.GetGround() + entity.GetGrabInfo().grabHeight));
            target.SetGround((entity.GetGround() + entity.GetGrabInfo().grabHeight));
            target.GetGrabInfo().inGrabHeight = true;
        }

        public static void SetGrabGround(Entity entity, Entity target) {
            target.SetGround((entity.GetGround() + entity.GetGrabInfo().grabHeight));
        }

        public static void Ungrab(Entity entity, Entity target) {
            entity.GetGrabInfo().Reset();

            if (target != null) { 
                target.GetGrabInfo().Reset();
                target.GetAttackInfo().Reset();

                if (!target.IsInAnimationAction(Animation.Action.KNOCKED)) { 
                    target.SetAnimationState(Animation.State.FALL1);
                }

                if (!target.IsDying() && !target.IsInAnimationAction(Animation.Action.KNOCKED)) { 
                    if (target.InAir() || target.IsToss()) {
                        target.Toss(8);
                        target.SetGround(target.GetGroundBase());
                    }
                } else {
                     target.SetGround(target.GetGroundBase());
                }
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
            if (target.IsGrabbed() && (((distX > entity.GetGrabInfo().dist + 50) 
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

        public static void DefaultAttack(Entity entity) {
             if (!entity.InNegativeState()) {

                if (!entity.IsToss()) {
                    entity.ProcessAttackChainStep();

                } else {
                    if (!entity.IsInAnimationAction(Animation.Action.ATTACKING)
                            && !entity.IsInAnimationAction(Animation.Action.RECOVERY)
                            && entity.InAir()) {

                        if ((double)entity.GetTossInfo().velocity.X == 0.0) {
                            entity.SetAnimationState(Animation.State.JUMP_ATTACK1);
                        }
                        else {
                            entity.SetAnimationState(Animation.State.JUMP_TOWARD_ATTACK1);
                        }
                    }
                }
            }
        }

        public static void ThrowOrGrabAttack(Entity entity, bool isThrow) {
            if (entity.IsInAnimationAction(Animation.Action.GRABBING)
                    && !entity.IsInAnimationAction(Animation.Action.THROWING)
                    && isThrow) {

                    EntityActions.ThrowTarget(entity, entity.GetGrabInfo().grabbed);

            } else if (entity.IsInAnimationAction(Animation.Action.GRABBING)
                            && !entity.IsInAnimationAction(Animation.Action.ATTACKING)
                            && !entity.IsInAnimationAction(Animation.Action.THROWING)
                            && !isThrow) {

                   entity.SetAnimationState(Animation.State.GRAB_ATTACK1);
            }
        }

        private static void OnDeathStep1(Entity entity) {
            if ((entity.GetOldHealth() == 0 && entity.GetLives() <= 0) && entity.GetDeathStep() == 1) {

                if (!entity.IsToss() && entity.IsDeathMode(Entity.DeathType.IMMEDIATE_DIE)
                        && !entity.IsInAnimationState(Animation.State.DIE1)) {

                    entity.SetAnimationState(Animation.State.DIE1);
                }

                if (entity.IsInAnimationAction(Animation.Action.RISING) && entity.IsAnimationComplete()) {
                    entity.SetAnimationState(Animation.State.DIE1);
                }

                if (entity.IsInAnimationAction(Animation.Action.DYING)) {
                    if (entity.IsDeathMode(Entity.DeathType.FLASH) && !entity.IsFlash()) {
                        entity.Flash(GameManager.DEATH_FLASH_TIME);
                    }
                }

                if (entity.IsInAnimationAction(Animation.Action.DYING) && entity.IsAnimationComplete()) {
                    if (entity.IsDeathMode(Entity.DeathType.FLASH) && !entity.IsFlash()) {
                        entity.Flash(GameManager.DEATH_FLASH_TIME);
                    }

                    entity.SetAliveTime(entity.GetDieTime());
                    entity.SetDeathStep(2);
                }

                if (entity.IsInAnimationAction(Animation.Action.INPAIN) 
                        || entity.IsInAnimationAction(Animation.Action.FALLING)) {

                    entity.SetDeathStep(-1);
                }
            }
        }

        public static void OnDeath(Entity entity) {
            if ((entity.GetOldHealth() == 0 && entity.GetLives() <= 0) && entity.GetDeathStep() == -1) {
                entity.OnDeath();

                String defaultDieSFX = (entity is Drum ? "klunk" : (entity is PhoneBooth ? "glass" : "die3"));

                if (!entity.IsInAnimationAction(Animation.Action.DYING)) {
                    GameManager.GetInstance().PlaySFX(entity, Animation.State.DIE1, defaultDieSFX);
                }

                if (entity.IsDeathMode(Entity.DeathType.FLASH)) {
                    entity.Flash(GameManager.DEATH_FLASH_TIME);
                }

                if (entity.IsDeathMode(Entity.DeathType.IMMEDIATE_DIE)) {
                    if (!entity.InvalidDeathState()) {
                        entity.SetAnimationState(Animation.State.DIE1);
                    }

                    entity.SetDeathStep(1);

                } else if (entity.IsDeathMode(Entity.DeathType.DEFAULT)) {

                    if (!entity.InvalidDeathState()) {
                        if (entity.IsEntity(Entity.ObjectType.OBSTACLE) || entity is Obstacle) {
                            entity.SetAnimationState(Animation.State.DIE1);
                        } else {
                            entity.SetAnimationState(Animation.State.KNOCKED_DOWN1);
                            entity.SetCurrentKnockedState(Attributes.KnockedState.KNOCKED_DOWN);
                        }

                        float velX = (entity.GetAttackInfo().lastAttackDir > 0 ? -5 : 5);
                        entity.Toss(-17, velX, 1, 2); 
                        entity.SetTossGravity(0.7f);
                    }

                    entity.SetDeathStep(1);
                }

                entity.GetGrabInfo().Reset();
                entity.GetRumble().Reset(); 
            }

            OnDeathStep1(entity);
        }

        public static void OnRun(Entity entity) {
            SoundAction runSoundAction = entity.GetSoundAction(Animation.State.RUN);

            if (runSoundAction != null) { 
                if (entity.IsInAnimationAction(Animation.Action.RUNNING)) {
                    if (!runSoundAction.IsActive()) {
                        runSoundAction.StopSoundInstance(); 
                        entity.OnRun();

                        runSoundAction.SetSoundInstance(GameManager.GetInstance().PlaySFX(entity, Animation.State.RUN, "run2", 1, 0, 0, true));
                    }
                } else {
                    runSoundAction.Reset();
                }
            }
        }

        public static void OnAttacking(Entity entity, SoundAction soundAction) {
            if (soundAction != null) { 
                if (entity.IsInAnimationAction(Animation.Action.ATTACKING) 
                        && entity.GetCurrentSpriteFrame() > 0) {

                    if (!soundAction.IsActive()) { 
                        soundAction.StopSoundInstance();
                        soundAction.SetSoundInstance(GameManager.GetInstance().PlaySFX(entity, entity.GetCurrentAnimationState(), "punch1"));
                    }
                } else if (entity.IsLastAnimationState(soundAction.GetState())) {
                    soundAction.Reset();
                }
            }
        }
    }
}
