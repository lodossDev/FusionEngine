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
        private static long current_hit_id = 0;

        //Grab calculations.
        private Vector2 grabx1;
        private Vector2 grabx2;
        private Vector2 grabz1;
        private Vector2 grabz2;
        private Vector2 itemPos;
        private Random rnd;


        public CollisionManager() : base() {
            grabx1 = Vector2.Zero;
            grabx2 = Vector2.Zero;
            grabz1 = Vector2.Zero;
            grabz2 = Vector2.Zero;
            itemPos = Vector2.Zero;
            rnd = new Random();
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

            for (int i = 0; i < entities.Count; i++) {
                Entity target = entities[i];

                if (entity != target && (target.IsCollidable() || target.IsPlatform())) {

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

                        bool isWithInBoundsX1 = entity.IsWithinBoundsX1(target, vx);
                        bool isWithInBoundsZ1 =  entity.IsWithinBoundsZ1(target, vz);

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

            for (int i = 0; i < entities.Count; i++) {
                Entity target = entities[i];

                if (entity != target && (target.IsCollidable() || target.IsPlatform())) {

                    CLNS.BoundsBox targetBox = target.GetBoundsBox();
                    CLNS.BoundingBox tDepthBox = target.GetDepthBox();

                    if (targetBox == null || tDepthBox == null) {
                        continue;
                    }
           
                    int tPosY = (int)Math.Abs(Math.Round((double)target.GetPosY()));
                    int tDepth = (int)Math.Abs(Math.Round((double)targetBox.GetZdepth()));
                    int tGround = (int)Math.Abs(Math.Round((double)target.GetGround()));
                    int tHeight = (int)(tPosY + (targetBox.GetHeight() - tDepth));

                    bool isWithInBoundsX1 = entity.IsWithinBoundsX1(target, vx);
                    bool isWithInBoundsZ1 =  entity.IsWithinBoundsZ1(target, vz);

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

                if (!entity.IsToss() && !entity.IsGrabbed()) {
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
          
            for (int i = 0; i < entities.Count; i++) {
                Entity target = entities[i];

                if (entity != target && target.IsPlatform()) {

                    CLNS.BoundsBox targetBox = target.GetBoundsBox();
                    CLNS.BoundingBox tDepthBox = target.GetDepthBox();

                    if (targetBox == null || tDepthBox == null) {
                        continue;
                    }
           
                    int tPosY = (int)Math.Abs(Math.Round((double)target.GetPosY()));
                    int tDepth = (int)Math.Abs(Math.Round((double)targetBox.GetZdepth()));
                    int tGround = (int)Math.Abs(Math.Round((double)target.GetGround()));
                    int tHeight = (int)(tPosY + (targetBox.GetHeight() - tDepth));

                    if (entityBox.Intersects(targetBox) 
                            && eDepthBox.Intersects(tDepthBox)
                            && !target.IsDying()) {

                        bool isWithInBoundsX1 = entity.IsWithinBoundsX1(target, vx);
                        bool isWithInBoundsZ1 = entity.IsWithinBoundsZ1(target, vz);

                        if (isWithInBoundsX1 && isWithInBoundsZ1
                                && target == entity.GetCollisionInfo().GetMovingObstacle() 
                                && entity.GetCollisionInfo().IsOnTop())  { 

                            entity.MoveY(target.GetAbsoluteVelY());
                            entity.SetGround(entity.GetPosY());
                        }

                        if (isWithInBoundsX1 && isWithInBoundsZ1 
                                && (double)entity.GetVelocity().Y > 0 
                                && ePosY >= tHeight - 10) {

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

        public bool FindObstacle(Entity entity) {
            CLNS.BoundsBox entityBox = entity.GetBoundsBox();
            CLNS.BoundingBox eDepthBox = entity.GetDepthBox();

            if (entityBox == null || eDepthBox == null) {
                return false;
            }
           
            int ePosY = (int)Math.Abs(Math.Round((double)entity.GetPosY()));
            int ePosZ = (int)Math.Abs(Math.Round((double)entity.GetPosZ()));
            int eDepth = (int)Math.Abs(Math.Round((double)entityBox.GetZdepth()));
            int eGround = (int)Math.Abs(Math.Round((double)entity.GetGround()));
            int eHeight = (int)(ePosY + (entityBox.GetHeight() - eDepth));

            float vx = Math.Abs(entity.GetAbsoluteVelX()) + 1 * 2;
            float vz = Math.Abs(entity.GetAbsoluteVelZ()) + 1 * 2; 

            for (int i = 0; i < entities.Count; i++) {
                Entity target = entities[i];

                if (entity != target && (target is Obstacle || target is Wall)
                        && (target.IsCollidable() || target.IsPlatform())
                        && !target.IsInAnimationAction(Animation.Action.KNOCKED)
                        && !target.IsDying()) {

                    CLNS.BoundsBox targetBox = target.GetBoundsBox();
                    CLNS.BoundingBox tDepthBox = target.GetDepthBox();

                    if (targetBox == null || tDepthBox == null) {
                        continue;
                    }
           
                    int tPosY = (int)Math.Abs(Math.Round((double)target.GetPosY()));
                    int tDepth = (int)Math.Abs(Math.Round((double)targetBox.GetZdepth()));
                    int tGround = (int)Math.Abs(Math.Round((double)target.GetGround()));
                    int tHeight = (int)(tPosY + (targetBox.GetHeight() - tDepth));

                    int ox = (int)-(((double)(targetBox.GetRect().Width) / 8) + vx);

                    if (entityBox.GetRect().X < targetBox.GetRect().X) {
                        ox = (int)(((double)(targetBox.GetRect().Width) / 8) + vx);
                    }

                    Rectangle rect1 = new Rectangle(entityBox.GetRect().X + ox, entityBox.GetRect().Y, entityBox.GetRect().Width, entityBox.GetRect().Height);
                   
                    if (rect1.Intersects(targetBox.GetRect())
                            && entity.DepthCollision(target, vz)
                            && ePosY <= tHeight - 10) {

                        return true;                        
                    }
                }
            }

            return false;
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
            
            for (int i = 0; i < entities.Count; i++) {
                Entity target = entities[i];

                if (entity != target && target.GetCollisionInfo().IsCollidable()
                        && !target.IsInAnimationAction(Animation.Action.KNOCKED)
                        && !target.IsDying()) {

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

                    int ox = (int)-(((double)(targetBox.GetRect().Width) / 8) + vx);

                    if (entityBox.GetRect().X < targetBox.GetRect().X) {
                        ox = (int)(((double)(targetBox.GetRect().Width) / 8) + vx);
                    }

                    Rectangle rect1 = new Rectangle(entityBox.GetRect().X + ox, entityBox.GetRect().Y, entityBox.GetRect().Width, entityBox.GetRect().Height);
                   
                    if (rect1.Intersects(targetBox.GetRect())
                            && entity.DepthCollision(target, vz)
                            && ePosY <= tHeight - 10
                            && (aboveTarget != target && belowTarget != target)) {

                        bool isWithInBoundsX1 = entity.IsWithinBoundsX2(target, vx);
                        bool isWithInBoundsZ1 = entity.IsWithinBoundsZ2(target, vz);
                        bool isWithInBoundsZ2 = entity.IsWithinBoundsZ1(target, vz);

                        float depthX = (int)Math.Round(entityBox.GetRect().GetHorizontalIntersectionDepth(targetBox.GetRect()));
                        float depthZ = (int)Math.Round(eDepthBox.GetRect().GetVerticalIntersectionDepth(tDepthBox.GetRect()));

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
                        
                        if ((isWithInBoundsX1 || (!isWithInBoundsX1 && !isWithInBoundsZ1)) && isWithInBoundsZ2) {

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

         private void CheckKnocked(Entity entity) {
            if (!entity.IsInAnimationAction(Animation.Action.KNOCKED)) {
                return;
            }

            if (entity.CanKnockOtherEntity()) {
                List<CLNS.BoundingBox> entityBoxes = entity.GetCurrentBoxes(CLNS.BoxType.BODY_BOX);
                entityBoxes.Add(entity.GetBodyBox());

                CLNS.BoundsBox entityBox = entity.GetBoundsBox();
                CLNS.BoundingBox eDepthBox = entity.GetDepthBox();

                for (int i = 0; i < entities.Count; i++) {
                    Entity target = entities[i];

                    if (entity != target && entityBoxes != null 
                            && entityBoxes.Count > 0 && !target.IsDying()
                            && !(target is Wall)) {

                        //Get all body boxes for collision with attack boxes.
                        List<CLNS.BoundingBox> targetBoxes = target.GetCurrentBoxes(CLNS.BoxType.BODY_BOX);
                        //Add global body box if exists.
                        targetBoxes.Add(target.GetBodyBox());
                        
                        Attributes.AttackInfo targetAttackInfo = target.GetAttackInfo();
                        CLNS.BoundsBox targetBox = target.GetBoundsBox();
                        CLNS.BoundingBox tDepthBox = target.GetDepthBox();
                        bool targetHit = false;
                       
                        if (targetBoxes != null && targetBoxes.Count > 0 
                                && Math.Abs(eDepthBox.GetRect().Bottom - tDepthBox.GetRect().Bottom) < tDepthBox.GetHeight() + 5) {

                            if (!target.IsInAnimationAction(Animation.Action.KNOCKED) 
                                    && target != entity.GetAttackInfo().attacker
                                    && target.GetKnockedFromKnockedEntityState() == 1
                                    && target.InAllowedKnockedState(entity.GetCurrentKnockedState())) {

                                foreach (CLNS.BoundingBox eBodyNode in entityBoxes) {

                                    foreach (CLNS.BoundingBox tBodyNode in targetBoxes) {

                                        if (eBodyNode.Intersects(tBodyNode)) {

                                           if (!targetHit) {
                                                GameManager.GetInstance().PlaySFX("beat2");
                                                CollisionActions.AddSpark(entity, target, tBodyNode, CLNS.AttackBox.AttackType.MEDIUM, Effect.Type.HIT_SPARK);
                                                target.SetAnimationState(Animation.State.KNOCKED_DOWN1);
                                                target.SetCurrentKnockedState(Attributes.KnockedState.KNOCKED_DOWN);

                                                float velX = entity.GetTossInfo().velocity.X / 1.2f;
                                                target.Toss(-15, velX, 1, 2); 

                                                if (target is Obstacle || target.IsEntity(Entity.ObjectType.OBSTACLE)) {
                                                    target.DecreaseHealth(100);
                                                } else {
                                                    target.DecreaseHealth(10);
                                                }

                                                target.GetAttackInfo().attacker = entity.GetAttackInfo().attacker;
                                                targetHit = true;
                                           }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        private void CheckAttack(Entity entity) {
            if (!entity.IsInAnimationAction(Animation.Action.ATTACKING)) {
                return;
            }

            //Get all frame attack boxes.
            List<CLNS.AttackBox> attackBoxes = entity.GetCurrentBoxes(CLNS.BoxType.HIT_BOX).Cast<CLNS.AttackBox>().ToList();
            List<CLNS.AttackBox> attackBoxesHitInFrame = new List<CLNS.AttackBox>();
            CLNS.BoundingBox eDepthBox = entity.GetDepthBox();

            EntityActions.ResetAttackChain(entity);

            if (attackBoxes != null && attackBoxes.Count > 0) {

                for (int i = 0; i < entities.Count; i++) {
                    Entity target = entities[i];

                    if (entity != target && target.IsHittable() 
                            && (((entity is Player && target is Enemy || entity is Enemy && target is Player) 
                                        || entity is Enemy && target is Enemy && entity.CanHurtOthers()))) {

                        //Get all body boxes for collision with attack boxes.
                        List<CLNS.BoundingBox> targetBoxes = target.GetCurrentBoxes(CLNS.BoxType.BODY_BOX);
                        //Add global body box if exists.
                        targetBoxes.Add(target.GetBodyBox());
                        
                        Attributes.AttackInfo targetAttackInfo = target.GetAttackInfo();
                        CLNS.BoundingBox tDepthBox = target.GetDepthBox();
                        CLNS.BoundingBox tBodyBox = null;
                        int currentAttackHits = 0;
                        bool targetHit = false;

                        if (Math.Abs(eDepthBox.GetRect().Bottom - tDepthBox.GetRect().Bottom) < tDepthBox.GetHeight() + 5 
                                && targetBoxes.Count > 0) {

                            //Get all attackboxes for this one frame, you can only hit once in each attack frame.
                            foreach (CLNS.AttackBox attackBox in attackBoxes) {

                                foreach (CLNS.BoundingBox bodyBox in targetBoxes) {

                                    if (attackBox.Intersects(bodyBox)) {
                                        attackBoxesHitInFrame.Add(attackBox);
                                        tBodyBox = bodyBox;

                                        CollisionActions.CheckAttack(entity, target, attackBox);
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

                                        CollisionActions.SetTargetHit(entity, target, attackBox, ref targetHit);
                                        currentAttackHits++;
                                    }
                                }

                                targetAttackInfo.hitByAttackId = current_hit_id;
                            }
                        }
                    }

                    target.GetAttackInfo().lastJuggleState = -1;
                }
            }
        }

        private void CheckGrabItem(Entity entity) {
            CLNS.BoundingBox eDepthBox = entity.GetDepthBox();

            for (int i = 0; i < entities.Count; i++) {
                Entity target = entities[i];
                
                if (entity != target && (target is Collectable || target.IsEntity(Entity.ObjectType.COLLECTABLE))) {
                    var collectable = target as Collectable;
                    bool isCollected = false;

                    CLNS.BoundingBox tDepthBox = collectable.GetDepthBox();
                    itemPos.X = (float)(tDepthBox.GetRect().X + (tDepthBox.GetRect().Width / 2));
                    itemPos.Y = tDepthBox.GetRect().Y;
  
                    if (eDepthBox.GetRect().Contains(itemPos) 
                            && collectable.GetPosY() == entity.GetPosY()
                            && collectable.GetGround() == entity.GetGround()) {

                        CollisionActions.ProcessGrabItem(entity, collectable, ref isCollected);
                    }
                }
            }
        }

        private void CheckGrab(Entity entity) {
            CLNS.BoundsBox entityBox = entity.GetBoundsBox();
            CLNS.BoundingBox eDepthBox = entity.GetDepthBox();

            float newx = 0;
            float newz = 0;
            float x = 0;
            float targetx = 0;
            float targetz = 0;
            List<Entity> targets = new List<Entity>();

            for (int i = 0; i < entities.Count; i++) {
                Entity target = entities[i];

                if (entity != target && entity.IsEntity(Entity.ObjectType.PLAYER) 
                        && target.IsEntity(Entity.ObjectType.ENEMY)
                        && target.IsGrabbable()) {

                    CLNS.BoundsBox targetBox = target.GetBoundsBox();
                    CLNS.BoundingBox tDepthBox = target.GetDepthBox();

                    grabx1.X = entityBox.GetRect().X;
                    grabx2.X = targetBox.GetRect().X;

                    grabz1.Y = eDepthBox.GetRect().Bottom;
                    grabz2.Y = tDepthBox.GetRect().Bottom;

                    float distX = Vector2.Distance(grabx1, grabx2);
                    float distZ = Vector2.Distance(grabz1, grabz2);

                    if ((distX < entity.GetGrabInfo().dist + 120) && distZ <= (tDepthBox.GetHeight() + 10)
                            && !target.InvalidGrabbedState()
                            && target.GetPosY() == entity.GetPosY()
                            && target.GetGround() == entity.GetGround()
                            && (entity.IsMoving() || entity.IsInAnimationAction(Animation.Action.ATTACKING))
                            && !entity.IsInAnimationAction(Animation.Action.KNOCKED)
                            && !entity.IsInAnimationAction(Animation.Action.INPAIN)
                            && !entity.IsDying()) {

                         target.DecreaseGrabResistance();
                    }
                    
                    if ((distX < entity.GetGrabInfo().dist) && distZ <= (tDepthBox.GetHeight() / 1.2) 
                            && ((entity.GetDirX() > 0 && entity.GetPosX() < target.GetPosX())
                                    || (entity.GetDirX() < 0 && entity.GetPosX() > target.GetPosX()))

                            //Entity must be moving forward to grab?
                            && entity.IsMoving()
                            //Target must be on same ground level.
                            && target.GetPosY() == entity.GetPosY()
                            && target.GetGround() == entity.GetGround()
                            && !target.InvalidGrabbedState()
                            //Must not be in these action states.
                            && !entity.IsInAnimationAction(Animation.Action.ATTACKING)
                            && !entity.IsInAnimationAction(Animation.Action.KNOCKED)
                            && !entity.IsInAnimationAction(Animation.Action.INPAIN)
                            && !entity.IsDying()
                            && entity.GetGrabInfo().grabbed == null) {

                        targets.Add(target);
                    }

                    EntityActions.CheckUnGrabDistance(entity, target, distX, distZ);
                }
            }

            if (targets.Count > 0) {
                List<Entity> nearest = targets.Where(item => item.GetGrabResistance() <= 0).OrderBy(item => item.GetDepthBox().GetRect().Bottom > entity.GetDepthBox().GetRect().Bottom).ToList();

                if (nearest.Count > 0) { 
                    Entity target = nearest[0];

                    if (target.GetGrabResistance() <= 0) { 
                        EntityActions.OnGrab(ref newx, ref x, ref targetx, entity, target);
                    }
                }
            }

            if (entity.GetGrabInfo().grabbed != null) {
                if (entity.GetGrabInfo().grabbed.IsDying() 
                        || entity.GetGrabInfo().grabbed.IsInAnimationAction(Animation.Action.KNOCKED)) {

                    entity.GetGrabInfo().grabbed.Toss(8);
                    entity.GetGrabInfo().grabbed.SetGround(entity.GetGrabInfo().grabbed.GetGroundBase());
                    entity.GetGrabInfo().grabbed.GetGrabInfo().Reset();
                    entity.GetGrabInfo().grabbed.GetAttackInfo().Reset();
                    entity.GetAttackInfo().Reset();
                    entity.GetGrabInfo().Reset();

                } else { 
                    if (entity.GetGrabInfo().grabbed.GetGrabInfo().isGrabbed && entity.GetGrabInfo().grabbed.GetGrabInfo().grabbedBy == entity) {
                        EntityActions.SetGrabPosition(ref newx, ref newz, ref x, ref targetx, ref targetz, entity, entity.GetGrabInfo().grabbed);
                        EntityActions.SetGrabGround(entity, entity.GetGrabInfo().grabbed);
                        EntityActions.ThrowIfNoGrab(entity, entity.GetGrabInfo().grabbed);
                        EntityActions.SetGrabAnimation(entity, entity.GetGrabInfo().grabbed);
                        EntityActions.CheckGrabTime(entity, entity.GetGrabInfo().grabbed);
                    }
                }
            }
        }

        public void BeforeUpdate(GameTime gameTime) {
            for (int i = 0; i < entities.Count; i++) {
                Entity entity = entities[i];
                entity.GetCollisionInfo().SetItem(null);

                CheckGrabItem(entity);
                CheckGrab(entity);
                CheckKnocked(entity);
                CheckAttack(entity);
            }
        }

        public void AfterUpdate(GameTime gameTime) {
            for (int i = 0; i < entities.Count; i++) {
                Entity entity = entities[i];
                entity.GetCollisionInfo().Reset();

                if (!entity.IsEntity(Entity.ObjectType.OBSTACLE)) {
                    entity.SetAbsoluteVelY(0);
                }
                
                CheckBounds(entity);
                CheckLand(entity);
                CheckFall(entity);
            }
        }

        public static void CreateHitId() {
            current_hit_id ++;
        }
    }
}