using System;
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

            float vx = Math.Abs(entity.GetAbsoluteVelX());
            float vz = Math.Abs(entity.GetAbsoluteVelZ());

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

            float vx = Math.Abs(entity.GetAbsoluteVelX());
            float vz = Math.Abs(entity.GetAbsoluteVelZ());

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

                if (!entity.IsToss()) {
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

            float vx = Math.Abs(entity.GetAbsoluteVelX());
            float vz = Math.Abs(entity.GetAbsoluteVelZ());
          
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
                                && entity.GetCollisionInfo().IsOnTop())  { 
                        
                            //if (target.IsMovingY()) { 
                                entity.MoveY(target.GetAbsoluteVelY());
                                entity.SetGround(entity.GetPosY());
                            //}
                        }

                        if (isWithInBoundsX1 && isWithInBoundsZ1 
                                && (double)entity.GetVelocity().Y > 0 && ePosY >= tHeight - 10) {

                            entity.GetCollisionInfo().SetIsOnTop(true);
                            entity.MoveY(target.GetAbsoluteVelY());
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

            float vx = Math.Abs(entity.GetAbsoluteVelX());
            float vz = Math.Abs(entity.GetAbsoluteVelZ()); 

            List<Entity> aboveEntities = FindAbove(entity);
            List<Entity> belowEntities = FindBelow(entity);
            
            foreach (Entity target in entities) {
                if (entity != target && (target.IsEntity(Entity.ObjectType.OBSTACLE) || entity.IsEntity(Entity.ObjectType.OBSTACLE))) {
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
                            && (aboveTarget != target && belowTarget != target)
                            /*&& !target.grabInfo.isGrabbed*/) {

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
                                entity.GetCollisionInfo().Bottom();

                            } else if (entity.GetDirZ() > 0 && entity.VerticleCollisionBottom(target, vz)) {
                                entity.MoveZ(depthZ);
                                entity.ResetZ();
                                entity.GetCollisionInfo().Top();
                            }
                        }
                        
                        if ((isWithInBoundsX1 || !isWithInBoundsX1 && !isWithInBoundsZ1) && isWithInBoundsZ2) {
                          
                            if (entity.GetDirX() < 0 && entity.HorizontalCollisionRight(target, vx)) {
                                entity.MoveX(depthX);
                                entity.ResetX();
                                entity.GetCollisionInfo().Left();

                            } else if (entity.GetDirX() > 0 && entity.HorizontalCollisionLeft(target, vx)) {
                                entity.MoveX(depthX);
                                entity.ResetX();
                                entity.GetCollisionInfo().Right();
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

        private void OnHit(Entity target, Entity entity, CLNS.AttackBox attackBox) {
            if (target != entity) {
                hitCount++;
                hiteffect1.CreateInstance().Play();

                target.Toss(-1.2f, 0, 200000000);
                //entity.GetAttackInfo().hitPauseTime = 2000f;
                //target.MoveY(-125 * attackBox.GetHitStrength());
            }
        }

        private void OnHit(Entity target, Entity entity) {
            if (target != entity) {
                hitCount++;
                hiteffect1.CreateInstance().Play();
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

                                        if (attackBox.GetSettingType() == CLNS.AttackBox.SettingType.ONCE) { 
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

                                        if (currentAttackHits > 0 && (attackBox.GetSettingType() == CLNS.AttackBox.SettingType.FRAME
                                                || attackBox.GetSettingType() == CLNS.AttackBox.SettingType.ONCE)) {

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
            if (!entity.HasSprite(Animation.State.GRAB_HOLD1)) {
                return;
            }

            CLNS.BoundsBox entityBox = entity.GetBoundsBox();
            CLNS.BoundingBox eDepthBox = entity.GetDepthBox();
            Attributes.GrabInfo eGrabInfo = entity.GetGrabInfo();
            float newx = 0, newz = 0, x = 0, targetx = 0, targetz = 0;

            foreach (Entity target in entities) {

                if (entity != target && target.IsEntity(Entity.ObjectType.ENEMY)) {
                    
                    CLNS.BoundsBox targetBox = target.GetBoundsBox();
                    CLNS.BoundingBox tDepthBox = target.GetDepthBox();
                    Attributes.GrabInfo tGrabInfo = target.GetGrabInfo();

                    Vector2 x1 = new Vector2(entityBox.GetRect().X, 0);
                    Vector2 x2 = new Vector2(targetBox.GetRect().X, 0);

                    Vector2 z1 = new Vector2(0, eDepthBox.GetRect().Bottom);
                    Vector2 z2 = new Vector2(0, tDepthBox.GetRect().Bottom);

                    float distX = Vector2.Distance(x1, x2);
                    float distZ = Vector2.Distance(z1, z2);
                    
                   
                    if ((distX < eGrabInfo.dist) && distZ <= (tDepthBox.GetHeight() / 2) 
                            && ((entity.GetDirX() > 0 && entity.GetPosX() < target.GetPosX())
                                    || (entity.GetDirX() < 0 && entity.GetPosX() > target.GetPosX()))
                            //Target must be on same ground level.
                            && target.GetPosY() == entity.GetGround()
                            && !target.IsToss()) {

                        if (eGrabInfo.grabIn == 1) {
                            newx = x = entity.GetPosX();
                            targetx = x + ((target.GetPosX() > entity.GetPosX()) ? (eGrabInfo.dist / 2) : -(eGrabInfo.dist / 2));
                        } else {
                            x = ((entity.GetPosX() + target.GetPosX()) / 2);
                            newx = x + ((entity.GetPosX() >= target.GetPosX()) ? (eGrabInfo.dist / 2) : -(eGrabInfo.dist / 2));
                            targetx = x + ((target.GetPosX() > entity.GetPosX()) ? (eGrabInfo.dist / 2) : -(eGrabInfo.dist / 2));
                        }

                        entity.SetPosX(newx);
                        target.SetPosX(targetx);
                        target.SetPosY(eGrabInfo.grabHeight);
                        tGrabInfo.isGrabbed = true;
                    }

                    if (tGrabInfo.isGrabbed && ((distX > eGrabInfo.dist + 5) || distZ > (tDepthBox.GetHeight() / 2) + 5)) {
                        tGrabInfo.isGrabbed = false;

                        if (target.InAir()) {
                            target.Toss(8);
                        }
                    }
                    
                    if (tGrabInfo.isGrabbed) {

                        if (entity.GetDirX() > 0) {
                            target.SetIsLeft(true);
                        } else {
                            target.SetIsLeft(false);
                        }

                        target.SetAnimationState(Animation.State.STANCE);

                        if (!entity.IsInAnimationAction(Animation.Action.ATTACKING) 
                                && !entity.IsInAnimationAction(Animation.Action.GRABBING)
                                && !entity.IsInAnimationAction(Animation.Action.THROWING)) {

                            entity.SetAnimationState(Animation.State.GRAB_HOLD1);
                        }

                        target.GetCurrentSprite().ResetAnimation();

                        if (eGrabInfo.grabIn == 1) {
                            newx = x = entity.GetPosX();
                            targetx = x + ((target.GetPosX() > entity.GetPosX()) ? (eGrabInfo.dist / 2) : -(eGrabInfo.dist / 2));
                        } else {
                            x = ((entity.GetPosX() + target.GetPosX()) / 2);
                            newx = x + ((entity.GetPosX() >= target.GetPosX()) ? (eGrabInfo.dist / 2) : -(eGrabInfo.dist / 2));
                            targetx = x + ((target.GetPosX() > entity.GetPosX()) ? (eGrabInfo.dist / 2) : -(eGrabInfo.dist / 2));
                        }

                        newz = targetz = entity.GetPosZ() - ((entity.GetPosZ() - target.GetPosZ()));

                        target.MoveX(entity.GetAccelX(), entity.GetDirX());
                        target.MoveZ(entity.GetAccelZ(), entity.GetDirZ());

                        target.SetAbsoluteVelX(entity.GetAbsoluteVelX());
                        target.SetAbsoluteVelZ(entity.GetAbsoluteVelZ());
                                               
                        if (entity.IsLeft()) {
                            targetx = x + -(eGrabInfo.dist / 2);
                        } else {
                            targetx = x + (eGrabInfo.dist / 2);
                        }

                        if (target.GetCollisionInfo().IsCollideX(Attributes.CollisionState.NO_COLLISION)) {
                            target.SetPosX(targetx);
                        }
                        
                        int zOffset = (eDepthBox.GetRect().Bottom - tDepthBox.GetRect().Bottom) + 2;

                        if (eGrabInfo.grabPos == -1) {
                            zOffset = (eDepthBox.GetRect().Bottom - tDepthBox.GetRect().Bottom) - 2;
                        }

                        if (target.GetCollisionInfo().IsCollideZ(Attributes.CollisionState.NO_COLLISION)) {
                            target.SetPosZ(newz + zOffset);
                        }

                        eGrabInfo.grabbed = target;

                        target.ResetX();
                        target.ResetZ();
                    } 
                }
            }
        }

        public void Update(GameTime gameTime) {
            foreach (Entity entity in entities) {
                entity.GetCollisionInfo().Reset();

                CheckGrab(entity);
                CheckAttack(entity);
                CheckBounds(entity);
                CheckLand(entity);
                CheckFall(entity);
            }
        }
    }
}
