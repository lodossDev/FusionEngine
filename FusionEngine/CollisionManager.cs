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
        public static SoundEffect hiteffect1;
        public static SoundEffectInstance soundInstance, soundInstance2;

        //Grab calculations.
        private Vector2 grabx1;
        private Vector2 grabx2;
        private Vector2 grabz1;
        private Vector2 grabz2;
        private Vector2 itemPos;
        private Random rnd;


        public CollisionManager() {
            grabx1 = Vector2.Zero;
            grabx2 = Vector2.Zero;
            grabz1 = Vector2.Zero;
            grabz2 = Vector2.Zero;
            itemPos = Vector2.Zero;
            rnd = new Random();

            hiteffect1 = Globals.contentManager.Load<SoundEffect>("Sounds//hit1");
            soundInstance = hiteffect1.CreateInstance();
            soundInstance2 = Globals.contentManager.Load<SoundEffect>("Sounds//test").CreateInstance();
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

            for (int i = 0; i < entities.Count; i++) {
                Entity target = entities[i];

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
          
            for (int i = 0; i < entities.Count; i++) {
                Entity target = entities[i];

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
            
            for (int i = 0; i < entities.Count; i++) {
                Entity target = entities[i];

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

                        float depthX = (int)Math.Round(entityBox.GetRect().GetHorizontalIntersectionDepth(targetBox.GetRect()));
                        float depthZ = (int)Math.Round(eDepthBox.GetRect().GetVerticalIntersectionDepth(tDepthBox.GetRect()));

                        if (entity.IsEntity(Entity.ObjectType.ENEMY) && !entity.IsGrabbed()) {
                            int agg = rnd.Next(1, 100);

                            if (agg > 75) {
                                int jump = rnd.Next(1, 4);

                                if (jump == 2) {
                                    entity.Toss(-18, entity.GetDirX() * 2.3f);
                                }
                            }
                        }

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
        
        private void CheckAttack(Entity entity) {
            //Get all frame attack boxes.
            List<CLNS.AttackBox> attackBoxes = entity.GetCurrentBoxes(CLNS.BoxType.HIT_BOX).Cast<CLNS.AttackBox>().ToList();
            List<CLNS.AttackBox> attackBoxesHitInFrame = new List<CLNS.AttackBox>();

            Attributes.AttackInfo entityAttackInfo = entity.GetAttackInfo();
            CLNS.BoundingBox eDepthBox = entity.GetDepthBox();
            EntityActions.ResetAttackChain(entity);

            if (attackBoxes != null && attackBoxes.Count > 0) {

                for (int i = 0; i < entities.Count; i++) {
                    Entity target = entities[i];

                    if (entity != target) {
                        //Get all body boxes for collision with attack boxes.
                        List<CLNS.BoundingBox> targetBoxes = target.GetCurrentBoxes(CLNS.BoxType.BODY_BOX);
                        //Add global body box if exists.
                        targetBoxes.Add(target.GetBodyBox());
                        
                        Attributes.AttackInfo targetAttackInfo = target.GetAttackInfo();
                        CLNS.BoundingBox tDepthBox = target.GetDepthBox();
                        CLNS.BoundingBox tBodyBox = null;
                        int currentAttackHits = 0;
                        bool targetHit = false;

                        if (CollisionActions.IsInAttackRange(entity, target, targetBoxes.Count)) {

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

                                        if (CollisionActions.IsHitFrameProcessed(entity, target, attackBox, currentAttackHits)) {
                                            break;
                                        }

                                        CollisionActions.SetTargetHit(entity, target, attackBox, ref targetHit);
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

        private void CheckGrabItem(Entity entity) {
            CLNS.BoundingBox eDepthBox = entity.GetDepthBox();

            for (int i = 0; i < entities.Count; i++) {
                Entity target = entities[i];

                bool isValidGrabFrame = (entity.IsInAnimationAction(Animation.Action.PICKING_UP) && entity.IsAnimationComplete());

                if (entity.GetGrabItemFrameInfo() != null) {
                    isValidGrabFrame = (entity.IsInAnimationState(entity.GetGrabItemAnimationState()) 
                                               && entity.GetGrabItemFrameInfo().IsInFrame(entity.GetCurrentSpriteFrame())); 
                }

                if (entity != target && (target is Collectable || target.IsEntity(Entity.ObjectType.COLLECTABLE))) {
                    CLNS.BoundingBox tDepthBox = target.GetDepthBox();
                    itemPos.X = (float)(tDepthBox.GetRect().X + (tDepthBox.GetRect().Width / 2));
                    itemPos.Y = tDepthBox.GetRect().Y;
  
                    if (eDepthBox.GetRect().Contains(itemPos) 
                            && target.GetPosY() == entity.GetPosY()
                            && target.GetGround() == entity.GetGround()) {

                        entity.GetCollisionInfo().SetItem(target);

                        if (isValidGrabFrame) { 
                            soundInstance2.Play();
                            GameManager.GetInstance().RemoveEntity(target);
                        }
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
                        && target.IsEntity(Entity.ObjectType.ENEMY)) {

                    CLNS.BoundsBox targetBox = target.GetBoundsBox();
                    CLNS.BoundingBox tDepthBox = target.GetDepthBox();

                    grabx1.X = entityBox.GetRect().X;
                    grabx2.X = targetBox.GetRect().X;

                    grabz1.Y = eDepthBox.GetRect().Bottom;
                    grabz2.Y = tDepthBox.GetRect().Bottom;

                    float distX = Vector2.Distance(grabx1, grabx2);
                    float distZ = Vector2.Distance(grabz1, grabz2);
                    
                    if ((distX < entity.GetGrabInfo().dist) && distZ <= (tDepthBox.GetHeight() / 1.2) 
                            && ((entity.GetDirX() > 0 && entity.GetPosX() < target.GetPosX())
                                    || (entity.GetDirX() < 0 && entity.GetPosX() > target.GetPosX()))

                            //Target must be on same ground level.
                            && target.GetPosY() == entity.GetPosY()
                            && target.GetGround() == entity.GetGround()
                            && !target.InvalidGrabbedState()
                            //Entity must be moving forward to grab?
                            && entity.IsMoving()
                            && !entity.IsInAnimationAction(Animation.Action.ATTACKING)
                            && entity.GetGrabInfo().grabbed == null) {

                        targets.Add(target);
                    }

                    EntityActions.CheckUnGrabDistance(entity, target, distX, distZ);
                }
            }

            if (targets.Count > 0) {
                List<Entity> nearest = targets.OrderBy(item => item.GetDepthBox().GetRect().Bottom > entity.GetDepthBox().GetRect().Bottom).ToList();
                Entity target = nearest[0];

                EntityActions.OnGrab(ref newx, ref x, ref targetx, entity, target);
            }

            if (entity.GetGrabInfo().grabbed != null) {
                if (entity.GetGrabInfo().grabbed.GetGrabInfo().isGrabbed && entity.GetGrabInfo().grabbed.GetGrabInfo().grabbedBy == entity) {
                    EntityActions.SetGrabPosition(ref newx, ref newz, ref x, ref targetx, ref targetz, entity, entity.GetGrabInfo().grabbed);
                    EntityActions.SetGrabGround(entity, entity.GetGrabInfo().grabbed);
                    EntityActions.ThrowIfNoGrab(entity, entity.GetGrabInfo().grabbed);
                    EntityActions.SetGrabAnimation(entity, entity.GetGrabInfo().grabbed);
                    EntityActions.CheckGrabTime(entity, entity.GetGrabInfo().grabbed);
                }
            }
        }

        public void BeforeUpdate(GameTime gameTime) {
            for (int i = 0; i < entities.Count; i++) {
                Entity entity = entities[i];

                entity.GetCollisionInfo().SetItem(null);

                CheckGrabItem(entity);
                CheckGrab(entity);
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
