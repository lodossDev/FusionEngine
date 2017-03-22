﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;

namespace FusionEngine
{
    public class CollisionManager : Manager {
        public static int hitCount = 0;
        public static long current_hit_id = 0;
        private SoundEffect hiteffect1;
        private SoundEffectInstance soundInstance, soundInstance2;
        private RenderManager renderManager;

        //Grab calculations.
        private Vector2 grabx1 = Vector2.Zero;
        private Vector2 grabx2 = Vector2.Zero;
        private Vector2 grabz1 = Vector2.Zero;
        private Vector2 grabz2 = Vector2.Zero;

        public CollisionManager(RenderManager renderManager) {
            hiteffect1 = System.contentManager.Load<SoundEffect>("Sounds//hit1");
            soundInstance = hiteffect1.CreateInstance();

            soundInstance2 = System.contentManager.Load<SoundEffect>("Sounds//test").CreateInstance();

            this.renderManager = renderManager;
        }

        public List<Entity> FindAbove(Entity entity) {
            List<Entity> found = new List<Entity>();

            CLNS.BoundsBox entityBox = entity.GetBoundsBox();
            CLNS.BoundingBox eDepthBox = entity.GetDepthBox();

            if (entityBox == null || eDepthBox == null) {
                return found;
            }
           
            int ePosY = (int)Math.Abs(Math.Round((double)entity.GetPosY()));
            int ePosZ = (int)Math.Abs(Math.Round((double)entity.GetPosZ()));
            int eDepth = (int)Math.Abs(Math.Round((double)entityBox.GetZdepth()));
            int eGround = (int)Math.Abs(Math.Round((double)entity.GetGround()));
            int eHeight = (int)(ePosY + (entityBox.GetHeight() - eDepth));

            float vx = Math.Abs(entity.GetAbsoluteVelX()) + 1 * 2;
            float vz = Math.Abs(entity.GetAbsoluteVelZ()) + 1 * 2; 

            foreach (Entity target in entities) {
                if (entity != target && (target.IsEntity(Entity.ObjectType.OBSTACLE) || entity.IsEntity(Entity.ObjectType.OBSTACLE))) {
                    CLNS.BoundsBox targetBox = target.GetBoundsBox();
                    CLNS.BoundingBox tDepthBox = target.GetDepthBox();

                    if (targetBox == null || tDepthBox == null) {
                        continue;
                    }
           
                    int tPosY = (int)Math.Abs(Math.Round((double)target.GetPosY()));
                    int tDepth = (int)Math.Abs(Math.Round((double)targetBox.GetZdepth()));
                    int tGround = (int)Math.Abs(Math.Round((double)target.GetGround()));
                    int tHeight = (int)(tPosY + (targetBox.GetHeight() - tDepth));

                    if (entityBox.Intersects(targetBox) && eDepthBox.Intersects(tDepthBox) 
                            && tHeight > ePosY && tPosY > ePosY && tPosY > eHeight
                            && entityBox.GetRect().Top < targetBox.GetRect().Bottom) {

                        bool isWithInBoundsX1 = (entity.HorizontalCollisionLeft(target, vx) == true && entity.HorizontalCollisionRight(target, vx) == true);
                        bool isWithInBoundsZ1 = (entity.VerticleCollisionTop(target, vz) == true && entity.VerticleCollisionBottom(target, vz) == true);

                        if (isWithInBoundsX1 && isWithInBoundsZ1) {
                            entity.GetCollisionInfo().Above();
                            found.Add(target);
                        }
                    }
                }
            }

            return found;
        }

        public List<Entity> FindBelow(Entity entity){
            List<Entity> found = new List<Entity>();
            CLNS.BoundsBox entityBox = entity.GetBoundsBox();
            CLNS.BoundingBox eDepthBox = entity.GetDepthBox();

            if (entityBox == null || eDepthBox == null) {
                return  found;
            }
           
            int ePosY = (int)Math.Abs(Math.Round((double)entity.GetPosY()));
            int ePosZ = (int)Math.Abs(Math.Round((double)entity.GetPosZ()));
            int eDepth = (int)Math.Abs(Math.Round((double)entityBox.GetZdepth()));
            int eGround = (int)Math.Abs(Math.Round((double)entity.GetGround()));
            int eHeight = (int)(ePosY + (entityBox.GetHeight() - eDepth));

            float vx = Math.Abs(entity.GetAbsoluteVelX()) + 1 * 2;
            float vz = Math.Abs(entity.GetAbsoluteVelZ()) + 1 * 2; 

            foreach (Entity target in entities) {
                if (entity != target && (target.IsEntity(Entity.ObjectType.OBSTACLE) || entity.IsEntity(Entity.ObjectType.OBSTACLE))) {
                    CLNS.BoundsBox targetBox = target.GetBoundsBox();
                    CLNS.BoundingBox tDepthBox = target.GetDepthBox();

                    if (targetBox == null || tDepthBox == null) {
                        continue;
                    }
           
                    int tPosY = (int)Math.Abs(Math.Round((double)target.GetPosY()));
                    int tDepth = (int)Math.Abs(Math.Round((double)targetBox.GetZdepth()));
                    int tGround = (int)Math.Abs(Math.Round((double)target.GetGround()));
                    int tHeight = (int)(tPosY + (targetBox.GetHeight() - tDepth));

                    bool isWithInBoundsX1 = (entity.HorizontalCollisionLeft(target, vx) == true && entity.HorizontalCollisionRight(target, vx) == true);
                    bool isWithInBoundsZ1 = (entity.VerticleCollisionTop(target, vz) == true && entity.VerticleCollisionBottom(target, vz) == true);

                    if (entityBox.Intersects(targetBox) 
                            && eDepthBox.Intersects(tDepthBox) && ePosY >= tHeight - 10
                            && isWithInBoundsX1 && isWithInBoundsZ1) {

                        entity.GetCollisionInfo().Below();
                        found.Add(target);
                    }
                }
            }

            return found.Distinct().ToList();
        }

        private void CheckFall(Entity entity) {
            List<Entity> belowEntities = FindBelow(entity);

            if ((belowEntities.Count == 0 
                    && (int)entity.GetGround() != (int)entity.GetGroundBase()
                    && entity.HasLanded())

                    || (belowEntities.Count == 0 
                            && (int)entity.GetGround() != (int)entity.GetGroundBase()
                            && entity.GetCollisionInfo().IsOnTop())) {

                entity.SetGround(entity.GetGroundBase());
                entity.GetCollisionInfo().SetIsOnTop(false);
                entity.GetCollisionInfo().SetMovingObstacle(null);
                entity.GetCollisionInfo().SetObstacle(null);

                if (!entity.IsToss() && !entity.GetGrabInfo().isGrabbed) {
                    entity.SetAnimationState(Animation.State.FALL1);
                    entity.Toss(5);
                }
            }
        }

        private void CheckLand(Entity entity) {
            CLNS.BoundsBox entityBox = entity.GetBoundsBox();
            CLNS.BoundingBox eDepthBox = entity.GetDepthBox();

            if (entityBox == null || eDepthBox == null) {
                return;
            }
           
            int ePosY = (int)Math.Abs(Math.Round((double)entity.GetPosY()));
            int ePosZ = (int)Math.Abs(Math.Round((double)entity.GetPosZ()));
            int eDepth = (int)Math.Abs(Math.Round((double)entityBox.GetZdepth()));
            int eGround = (int)Math.Abs(Math.Round((double)entity.GetGround()));
            int eHeight = (int)(ePosY + (entityBox.GetHeight() - eDepth));

            float vx = Math.Abs(entity.GetAbsoluteVelX()) + 1 * 2;
            float vz = Math.Abs(entity.GetAbsoluteVelZ()) + 1 * 2; 
          
            foreach (Entity target in entities) {

                if (entity != target && target.IsEntity(Entity.ObjectType.OBSTACLE)) {

                    CLNS.BoundsBox targetBox = target.GetBoundsBox();
                    CLNS.BoundingBox tDepthBox = target.GetDepthBox();

                    if (targetBox == null || tDepthBox == null) {
                        continue;
                    }
           
                    int tPosY = (int)Math.Abs(Math.Round((double)target.GetPosY()));
                    int tDepth = (int)Math.Abs(Math.Round((double)targetBox.GetZdepth()));
                    int tGround = (int)Math.Abs(Math.Round((double)target.GetGround()));
                    int tHeight = (int)(tPosY + (targetBox.GetHeight() - tDepth));

                    if (entityBox.Intersects(targetBox) && eDepthBox.Intersects(tDepthBox)) {

                        bool isWithInBoundsX1 = (entity.HorizontalCollisionLeft(target, vx) == true && entity.HorizontalCollisionRight(target, vx) == true);
                        bool isWithInBoundsZ1 = (entity.VerticleCollisionTop(target, vz) == true && entity.VerticleCollisionBottom(target, vz) == true);

                        if (isWithInBoundsX1 && isWithInBoundsZ1
                                && target == entity.GetCollisionInfo().GetMovingObstacle() 
                                && entity.GetCollisionInfo().IsOnTop())  { 

                            entity.MoveY(target.GetAbsoluteVelY());
                            entity.SetGround(entity.GetPosY());
                        }

                        if (isWithInBoundsX1 && isWithInBoundsZ1 
                                && (double)entity.GetVelocity().Y > 0 && ePosY >= tHeight - 10) {

                            if (target.IsMovingY()) {
                                entity.GetCollisionInfo().SetMovingObstacle(target);
                            }

                            entity.GetCollisionInfo().SetIsOnTop(true);
                            entity.SetGround(-(tHeight + 5));
                        }
                    }
                }
            }
        }
        
        private void CheckBounds(Entity entity) {
            CLNS.BoundsBox entityBox = entity.GetBoundsBox();
            CLNS.BoundingBox eDepthBox = entity.GetDepthBox();

            if (entityBox == null || eDepthBox == null) {
                return;
            }
           
            int ePosY = (int)Math.Abs(Math.Round((double)entity.GetPosY()));
            int ePosZ = (int)Math.Abs(Math.Round((double)entity.GetPosZ()));
            int eDepth = (int)Math.Abs(Math.Round((double)entityBox.GetZdepth()));
            int eGround = (int)Math.Abs(Math.Round((double)entity.GetGround()));
            int eHeight = (int)(ePosY + (entityBox.GetHeight() - eDepth));

            float vx = Math.Abs(entity.GetAbsoluteVelX()) + 1 * 2;
            float vz = Math.Abs(entity.GetAbsoluteVelZ()) + 1 * 2; 

            List<Entity> aboveEntities = FindAbove(entity);
            List<Entity> belowEntities = FindBelow(entity);
            
            foreach (Entity target in entities) {
                if (entity != target && (target.IsEntity(Entity.ObjectType.OBSTACLE) 
                        || (entity.IsEntity(Entity.ObjectType.OBSTACLE) && target.IsEntity(Entity.ObjectType.OBSTACLE)))) {

                    Entity aboveTarget = aboveEntities.Find(item => item == target);
                    Entity belowTarget = belowEntities.Find(item => item == target);

                    CLNS.BoundsBox targetBox = target.GetBoundsBox();
                    CLNS.BoundingBox tDepthBox = target.GetDepthBox();

                    if (targetBox == null || tDepthBox == null) {
                        continue;
                    }
           
                    int tPosY = (int)Math.Abs(Math.Round((double)target.GetPosY()));
                    int tDepth = (int)Math.Abs(Math.Round((double)targetBox.GetZdepth()));
                    int tGround = (int)Math.Abs(Math.Round((double)target.GetGround()));
                    int tHeight = (int)(tPosY + (targetBox.GetHeight() - tDepth));

                    int xw = 5;
                    int pw = Math.Abs(entityBox.GetWidth() - targetBox.GetWidth()) + 5;

                    if (entity.GetPosX() < targetBox.GetRect().X + (targetBox.GetWidth() / 2)) {
                        xw = pw;
                    }
                                 
                    if (Math.Abs(eDepthBox.GetRect().X - tDepthBox.GetRect().X) < tDepthBox.GetWidth() + (xw + vx) 
                            && entity.DepthCollision(target, vz)
                            && ePosY <= tHeight - 10 && eHeight >= tPosY 
                            && (aboveTarget != target && belowTarget != target)) {

                        bool isWithInBoundsX1 = ((entity.HorizontalCollisionLeft(target, vx) == true && entity.HorizontalCollisionRight(target, vx) == false
                                                    || entity.HorizontalCollisionLeft(target, vx) == false && entity.HorizontalCollisionRight(target, vx) == true));

                        bool isWithInBoundsZ1 = (entity.VerticleCollisionTop(target, vz) == false && entity.VerticleCollisionBottom(target, vz) == true
                                                    || entity.VerticleCollisionTop(target, vz) == true && entity.VerticleCollisionBottom(target, vz) == false);

                        bool isWithInBoundsZ2 = (entity.VerticleCollisionTop(target, vz) == true && entity.VerticleCollisionBottom(target, vz) == true);

                        float depthX = entityBox.GetRect().GetHorizontalIntersectionDepth(targetBox.GetRect());
                        float depthZ = eDepthBox.GetRect().GetVerticalIntersectionDepth(tDepthBox.GetRect());

                        if (isWithInBoundsZ1 && !isWithInBoundsX1) { 

                            if (entity.GetDirZ() < 0 && entity.VerticleCollisionTop(target, vz)) {

                                entity.MoveZ(depthZ);
                                entity.ResetZ();
                                target.ResetZ();

                                entity.GetCollisionInfo().Bottom();
                                entity.GetCollisionInfo().SetObstacle(target);
                                target.GetCollisionInfo().SetObstacle(entity);

                            } else if (entity.GetDirZ() > 0 && entity.VerticleCollisionBottom(target, vz)) {

                                entity.MoveZ(depthZ);
                                entity.ResetZ();
                                target.ResetZ();

                                entity.GetCollisionInfo().Top();
                                entity.GetCollisionInfo().SetObstacle(target);
                                target.GetCollisionInfo().SetObstacle(entity);
                            }
                        }
                        
                        if ((isWithInBoundsX1 || !isWithInBoundsX1 && !isWithInBoundsZ1) && isWithInBoundsZ2) {

                            if (entity.GetDirX() < 0 && entity.HorizontalCollisionRight(target, vx)) {

                                entity.MoveX(depthX);
                                entity.ResetX();
                                target.ResetX();

                                entity.GetCollisionInfo().Left();
                                entity.GetCollisionInfo().SetObstacle(target);
                                target.GetCollisionInfo().SetObstacle(entity);

                            } else if (entity.GetDirX() > 0 && entity.HorizontalCollisionLeft(target, vx)) {

                                entity.MoveX(depthX);
                                entity.ResetX();
                                target.ResetX();

                                entity.GetCollisionInfo().Right();
                                entity.GetCollisionInfo().SetObstacle(target);
                                target.GetCollisionInfo().SetObstacle(entity);
                            }
                        }
                    }
                }
            }

            if (aboveEntities.Count > 0 && entity.InAir()) {
                entity.VelY(0f);
                entity.GetTossInfo().velocity.Y = entity.GetTossInfo().maxVelocity.Y;
            }
        }
        
        private float TargetBodyX(Entity target, Entity entity, CLNS.AttackBox attack) {
            int x1 = entity.GetBoundsBox().GetWidth();
            int x2 = target.GetBoundsBox().GetWidth();

            float v1 = ((target.GetPosX() / 2) + (entity.GetPosX() / 2));

            if (entity.GetPosX() >= target.GetPosX() + (x2 / 2)) {
                v1 = ((target.GetPosX() / 2) + (entity.GetPosX() / 2));
            } else if (entity.GetPosX() <= target.GetPosX() + (x2 / 2)) {
                v1 = ((target.GetPosX() / 2) + (entity.GetPosX() / 2));
            }

            if (entity.IsLeft()) {
                v1 -= attack.GetOffset().X;
            } else {
                v1 += attack.GetOffset().X;
            }

            return v1;
        }

        private float TargetBodyY(Entity target, Entity entity, CLNS.AttackBox attack) {
            return (int)-attack.GetRect().Height + (int)Math.Round(attack.GetOffset().Y + entity.GetPosY());
        }

        private void OnAttack(Entity entity, Entity target, CLNS.AttackBox attackBox) {
            if (entity != target) {
                ComboAttack.Chain attackChain = entity.GetDefaultAttackChain();

                if (attackChain != null) { 
                    if (entity.GetAttackInfo().lastHitDirection == entity.GetDirX()) {
                        attackChain.IncrementMoveIndex(attackBox.GetComboStep());
                    } else {
                        attackChain.ResetMove();
                    }
                }
            }
        }

        private void OnHit(Entity target, Entity entity, CLNS.AttackBox attackInfo) {
            if (target != entity) {
                hitCount++;
                hiteffect1.CreateInstance().Play();
                //target.Toss(-5.2f, 0, 200000000);
                float dir = (entity.IsLeft() ? -1 : 1);

                EntityActions.SetPainState(entity, target, attackInfo);

                target.GetCurrentSprite().ResetAnimation();
                target.SetPainTime(80);
                target.SetRumble(dir, 2.8f);
                EntityActions.FaceTarget(target, entity);

                EntityActions.CheckMaxGrabHits(entity, target);
                target.SetHitPauseTime(100);
                //target.MoveY(-125 * attackBox.GetHitStrength());
            }
        }

        private void CheckAttack(Entity entity) {
            List<CLNS.AttackBox> attackBoxes = entity.GetCurrentBoxes(CLNS.BoxType.HIT_BOX).Cast<CLNS.AttackBox>().ToList();
            List<CLNS.AttackBox> attackBoxesHitInFrame = new List<CLNS.AttackBox>();

            Attributes.AttackInfo entityAttackInfo = entity.GetAttackInfo();
            CLNS.BoundingBox eDepthBox = entity.GetDepthBox();

            if (entity.GetAttackInfo().lastHitDirection != entity.GetDirX()) {
                if (entity.GetDefaultAttackChain() != null) {
                    entity.GetDefaultAttackChain().ResetMove();
                }
            } 

            if (attackBoxes != null && attackBoxes.Count > 0) {

                foreach (Entity target in entities) {

                    if (entity != target) {
                        //Get all body boxes for collision with attack boxes
                        List<CLNS.BoundingBox> targetBoxes = target.GetCurrentBoxes(CLNS.BoxType.BODY_BOX);
                        targetBoxes.Add(target.GetBodyBox());
                        
                        Attributes.AttackInfo targetAttackInfo = target.GetAttackInfo();
                        CLNS.BoundingBox tDepthBox = target.GetDepthBox();
                        CLNS.BoundingBox tBodyBox = null;
                        int currentAttackHits = 0;
                        bool targetHit = false;

                        if (Math.Abs(eDepthBox.GetRect().Bottom - tDepthBox.GetRect().Bottom) < tDepthBox.GetZdepth() + 10
                                && entity.IsInAnimationAction(Animation.Action.ATTACKING) 
                                && attackBoxes.Count > 0 && targetBoxes.Count > 0) {

                            //Get all attackboxes for this one frame, you can only hit once in each attack frame.
                            foreach (CLNS.AttackBox attackBox in attackBoxes) {

                                foreach (CLNS.BoundingBox bodyBox in targetBoxes) {

                                    if (attackBox.Intersects(bodyBox)) {
                                        attackBoxesHitInFrame.Add(attackBox);
                                        tBodyBox = bodyBox;

                                        if (attackBox.GetHitType() == CLNS.AttackBox.HitType.ONCE) { 
                                            if (entityAttackInfo.lastAttackState != entity.GetCurrentAnimationState()) {
                                                current_hit_id++;

                                                OnAttack(entity, target, attackBox);
                                                entity.OnAttack(target, attackBox);

                                                entity.GetAttackInfo().lastHitDirection = entity.GetDirX();
                                                entityAttackInfo.lastAttackState = entity.GetCurrentAnimationState();                                               
                                            }
                                        } else { 
                                            if (entityAttackInfo.lastAttackState != entity.GetCurrentAnimationState()) {
                                                OnAttack(entity, target, attackBox);
                                                entity.OnAttack(target, attackBox);

                                                entity.GetAttackInfo().lastHitDirection = entity.GetDirX();
                                                entityAttackInfo.lastAttackState = entity.GetCurrentAnimationState();                                               
                                            }

                                            if (entityAttackInfo.lastAttackFrame != entity.GetCurrentSprite().GetCurrentFrame()) {
                                                current_hit_id++;
                                                entityAttackInfo.lastAttackFrame = entity.GetCurrentSprite().GetCurrentFrame();
                                            }
                                        }    
                                    }
                                }
                            }

                            attackBoxesHitInFrame = attackBoxesHitInFrame.Distinct().ToList();
                            //Debug.WriteLine("AttackBoxes: " + attackBoxesHitInFrame.Count);

                            if (tBodyBox != null && attackBoxesHitInFrame.Count > 0 
                                    && targetAttackInfo.hitByAttackId != current_hit_id) {

                                foreach (CLNS.AttackBox attackBox in attackBoxesHitInFrame) {
                                    
                                    if (attackBox.Intersects(tBodyBox)) {
                                        //Debug.WriteLine("currentAttackHits: " + currentAttackHits);

                                        if (currentAttackHits > 0 && (attackBox.GetHitType() == CLNS.AttackBox.HitType.FRAME
                                                || attackBox.GetHitType() == CLNS.AttackBox.HitType.ONCE)) {

                                            break;
                                        }

                                        if (!targetHit) {
                                            OnHit(target, entity, attackBox);
                                            target.OnHit(entity, attackBox);
                                            targetHit = true;
                                        }

                                        float x1 = TargetBodyX(target, entity, attackBox);
                                        float y1 = TargetBodyY(target, entity, attackBox);

                                        Entity hitSpark1 = new Entity(Entity.ObjectType.HIT_FLASH, "SPARK1");
                                        hitSpark1.AddSprite(Animation.State.STANCE, new Sprite("Sprites/Actors/Ryo/Hitflash1", Animation.Type.ONCE));
                                        hitSpark1.SetAnimationState(Animation.State.STANCE);
                                        hitSpark1.SetFrameDelay(Animation.State.STANCE, 2);
                                        //hitSpark1.SetFrameDelay(Animation.State.STANCE, 1, 5);
                                        hitSpark1.SetScale(1.8f, 1.5f);
                                        //hitSpark1.AddBoundsBox(160, 340, -60, 15, 50);

                                        Debug.WriteLine("Y HITSPAK: " + y1);
                                        hitSpark1.SetPostion(x1 , y1, (entity.GetPosZ() + target.GetBoundsBox().GetZdepth()) + 5);
                                        hitSpark1.SetLayerPos(tDepthBox.GetRect().Bottom + 15);
                                        //hitSpark1.SetFade(225);

                                        renderManager.AddEntity(hitSpark1);
                                        currentAttackHits++;
                                    }
                                }

                                targetAttackInfo.hitByAttackId = current_hit_id;
                            }
                         
                            //Debug.WriteLine("SparkCount: " + renderManager.entities.FindAll(item => item.IsEntity(Entity.EntityType.HIT_FLASH)).ToList().Count);
                        }
                    }
                }
            }
        }

        private void CheckGrab(Entity entity) {
            CLNS.BoundsBox entityBox = entity.GetBoundsBox();
            CLNS.BoundingBox eDepthBox = entity.GetDepthBox();
            Attributes.GrabInfo eGrabInfo = entity.GetGrabInfo();

            float newx = 0;
            float newz = 0;
            float x = 0;
            float targetx = 0;
            float targetz = 0;

            foreach (Entity target in entities) {

                if (entity != target && target.IsEntity(Entity.ObjectType.ENEMY) && entity.GetName().Contains("RYO")) {
                    CLNS.BoundsBox targetBox = target.GetBoundsBox();
                    CLNS.BoundingBox tDepthBox = target.GetDepthBox();
                    Attributes.GrabInfo tGrabInfo = target.GetGrabInfo();

                    grabx1.X = entityBox.GetRect().X;
                    grabx2.X = targetBox.GetRect().X;

                    grabz1.Y = eDepthBox.GetRect().Bottom;
                    grabz2.Y = tDepthBox.GetRect().Bottom;

                    float distX = Vector2.Distance(grabx1, grabx2);
                    float distZ = Vector2.Distance(grabz1, grabz2);
                    
                    if ((distX < eGrabInfo.dist) && distZ <= (tDepthBox.GetHeight() / 2) 
                            && ((entity.GetDirX() > 0 && entity.GetPosX() < target.GetPosX())
                                    || (entity.GetDirX() < 0 && entity.GetPosX() > target.GetPosX()))
                            && tGrabInfo.grabbedTime > 0
                            //Target must be on same ground level.
                            && target.GetPosY() == entity.GetPosY()
                            //Entity must be moving forward to grab?
                            && (double)entity.GetVelX() != 0.0
                            && !entity.IsInAnimationAction(Animation.Action.ATTACKING)
                            && !target.IsToss()
                            && eGrabInfo.grabbed == null) {

                        EntityActions.OnGrab(out newx, out x, out targetx, entity, target);
                    }

                    if (tGrabInfo.isGrabbed && tGrabInfo.grabbedBy == entity) {
                        EntityActions.SetGrabPosition(out newx, out newz, out x, out targetx, out targetz, entity, target);

                        target.StopMovement();
                        EntityActions.SetGrabGround(entity, target);
                        EntityActions.ThrowIfNoGrab(entity, target);
                        EntityActions.SetGrabAnimation(entity, target);
                        EntityActions.CheckGrabTime(entity, target);
                    }

                    EntityActions.CheckUnGrabDistance(entity, target, distX, distZ);
                }
            }
        }

        public void BeforeUpdate(GameTime gameTime) {
            foreach (Entity entity in entities) {
                CheckGrab(entity);
                CheckAttack(entity);
            }
        }

        public void AfterUpdate(GameTime gameTime) {
            foreach (Entity entity in entities) {
                entity.GetCollisionInfo().Reset();
                
                CheckBounds(entity);
                CheckLand(entity);
                CheckFall(entity);
            }
        }
    }
}
