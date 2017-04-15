using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FusionEngine {

    public class Attributes {

        public enum AttackState {
            NO_ATTACK_ID = -1
        }

        public enum CollisionState {
            NO_COLLISION = -1,
            RIGHT = 0,
            LEFT = 1,
            TOP = 2,
            BOTTOM = 3
        }

        public class AnimationConfig {
            public Animation.State? lowPainState;
            public Animation.State? mediumPainState;
            public Animation.State? heavyPainState;
            public Animation.State? grabbedState;
            public Animation.State? grabHoldState;
            public Animation.State? throwState;
            public Animation.State? lowPainGrabbedState;
            public Animation.State? mediumPainGrabbedState;
            public Animation.State? heavyPainGrabbedState;


            public AnimationConfig() { }
        }

        public class Rumble {
            public float time;
            public float maxTime;
            public float forceTime;
            public float maxForceTime;
            public float lastDir;
            public float dir;
            public float force;
            public float lx;
            public bool isRumble;
            public int count;


            public Rumble() {
                lx = 0;
                isRumble = false;
                time = 0;
                maxTime = 100;
                forceTime = 0;
                maxForceTime = 25;
                force = 2.5f;
                dir = lastDir = -1;
                count = 0;
            }

            public void Reset() {
                lx = 0;
                isRumble = false;
                time = 0;
                maxTime = 100;
                forceTime = 0;
                maxForceTime = 25;
                force = 2.5f;
                dir = lastDir = -1;
                count = 0;
            }
        }

        public class CollisionInfo {
            private CollisionState collide_x;
            private CollisionState collide_y;
            private CollisionState collide_z;
            private Entity movingObstacle;
            private Entity obstacle;
            private Entity item;
            private bool onTop;


            public CollisionInfo() {
                Reset();
                onTop = false;
            }

            public void Reset() {
                collide_x = CollisionState.NO_COLLISION;
                collide_y = CollisionState.NO_COLLISION;
                collide_z = CollisionState.NO_COLLISION;
            }

            public void SetMovingObstacle(Entity entity) {
                movingObstacle = entity;
            }

            public void SetObstacle(Entity entity) {
                obstacle = entity;
            }

            public void SetItem(Entity entity) {
                item = entity;
            }

            public bool IsCollideX(CollisionState state) {
                return collide_x == state;
            }

            public bool IsCollideY(CollisionState state) {
                return collide_y == state;
            }

            public bool IsCollideZ(CollisionState state) {
                return collide_z == state;
            }

            public void Right() {
                collide_x = CollisionState.RIGHT;
            }

            public void Left() {
                collide_x = CollisionState.LEFT;
            }

            public void Top() {
                collide_z = CollisionState.TOP;
            }

            public void Bottom() {
                collide_z = CollisionState.BOTTOM;
            }

            public void Above() {
                collide_y = CollisionState.TOP;
            }

            public void Below() {
                collide_y = CollisionState.BOTTOM;
            }

            public bool IsRight() {
                return collide_x == CollisionState.RIGHT;
            }

            public bool IsLeft() {
                return collide_x == CollisionState.LEFT;
            }

            public bool IsTop() {
                return collide_z == CollisionState.TOP;
            }

            public bool IsBottom() {
                return collide_z == CollisionState.BOTTOM;
            }

            public bool IsAbove() {
                return collide_y == CollisionState.TOP;
            }

            public bool IsBelow() {
                return collide_y == CollisionState.BOTTOM;
            }

            public bool IsOnTop() {
                return onTop;
            }

            public void SetIsOnTop(bool status) {
                onTop = status;
            }

            public CollisionState GetCollideX() {
                return collide_x;
            }

            public CollisionState GetCollideY() {
                return collide_y;
            }

            public CollisionState GetCollideZ() {
                return collide_z;
            }

            public Entity GetMovingObstacle() {
                return movingObstacle;
            }

            public Entity GetObstacle() {
                return obstacle;
            }

            public Entity GetItem() {
                return item;
            }
        }

        public class GrabInfo {
            public int grabIn;           //Should the target be brought in close or at distance
            public int grabPos;          //(1 infront of attacker, -1 behind attacker)
            public int dist;             //Distance in x needed for grab to work
            public int grabHeight;
            public bool grabWalk;
            public bool isGrabbed;
            public Entity grabbed;       //The target grabbed
            public Entity grabbedBy;     //The attacker/grabber
            public int grabDirection;
            public int grabbedTime;
            public int maxGrabbedTime;
            public float throwVelX;
            public float throwHeight;
            public int grabHitCount;
            public int maxGrabHits;
            public bool grabbable;
            public int grabResistance;
            public int maxGrabResistance;


            public GrabInfo() {
                grabIn = -1;            
                grabPos = 1;           
                dist = 140;              
                grabHeight = -100;
                grabWalk = false;
                isGrabbed = false;
                grabbed = grabbedBy = null;       
                grabDirection = 0;
                maxGrabbedTime = 100;
                grabbedTime = maxGrabbedTime;
                throwVelX = 6;
                throwHeight = -15;
                maxGrabHits = 5;
                grabHitCount = maxGrabHits;
                grabbable = true;
                maxGrabResistance = 0;
                grabResistance = maxGrabResistance;
            }

            public void Reset() {
                grabWalk = false;
                isGrabbed = false;
                grabbed = grabbedBy = null;
                grabDirection = 0;
                grabbedTime = maxGrabbedTime;
                grabHitCount = maxGrabHits;
                grabResistance = maxGrabResistance;
            }
        }

        public class AttackInfo {
            public long hitByAttackId;
            public Animation.State? lastAttackState;
            public int lastAttackFrame;
            public int lastHitDirection;
            public int hitPauseTime;
            public bool hasHit;
            public bool isHit;
            public Entity attacker;
            public int lastAttackDir;
            public int blockResistance;
            public int maxBlockResistance;
            public int juggleHits;
            public int maxJuggleHits;


            public AttackInfo() {
                Reset();
                lastHitDirection = lastAttackDir = 0;
            }

            public void Reset() {
                attacker = null;
                lastAttackFrame = -1;
                lastAttackState = Animation.State.NONE;
                hitPauseTime = 0;
                maxBlockResistance = 100;
                blockResistance = maxBlockResistance;
                maxJuggleHits = 4;
                juggleHits = maxJuggleHits;
                hasHit = isHit = false;
            }
        }

        public class FrameInfo {
            public enum FrameState { NO_FRAME = -1 }
            private int startFrame;
            private int endFrame;


            public FrameInfo(int startFrame, int endFrame) {
                this.startFrame = startFrame;
                this.endFrame = endFrame;
            }

            public FrameInfo(int startFrame) {
                this.startFrame = startFrame;
                this.endFrame = (int)FrameState.NO_FRAME;
            }

            public int GetStartFrame() {
                return startFrame;
            }

            public int GetEndFrame() {
                return endFrame;
            }

            public void SetStartFrame(int sx) {
                startFrame = sx;
            }

            public void SetEndFrame(int ex) {
                endFrame = ex;
            }

            public bool IsInFrame(int currentFrame) {
                return (currentFrame >= startFrame && currentFrame <= endFrame);
            }
        }

        public class TossInfo {
            public Vector3 velocity;
            public Vector3 maxVelocity;
            public bool isToss;
            public bool inTossFrame;
            public float gravity;
            public float height;
            public float tempHeight;
            public int maxHitGround;
            public int hitGoundCount;
            public int tossCount;
            public int maxTossCount;


            public TossInfo() {
                height = tempHeight = 0f;
                velocity = Vector3.Zero;
                maxVelocity = new Vector3(10 * GameManager.GAME_VELOCITY, 13 * GameManager.GAME_VELOCITY, 10 * GameManager.GAME_VELOCITY);
                gravity = 0.48f * GameManager.GAME_VELOCITY;
                inTossFrame = false;
                isToss = false;
                hitGoundCount = 0;
                tossCount = 0;
                maxHitGround = 3;
                maxTossCount = 1;
            }

            public void Reset() {
                height = tempHeight = 0f;
                velocity = Vector3.Zero;
                maxVelocity.X = 10 * GameManager.GAME_VELOCITY;
                maxVelocity.Y = 13 * GameManager.GAME_VELOCITY;
                maxVelocity.Z = 10 * GameManager.GAME_VELOCITY;
                gravity = 0.48f * GameManager.GAME_VELOCITY;
                inTossFrame = false;
                isToss = false;
                hitGoundCount = 0;
                tossCount = 0;
                maxHitGround = 3;
                maxTossCount = 1;
            }
        }

        public class ColourInfo {
            public float alpha;
            public float fadeFrequency;
            public float r, g, b;
            public float currentFadeTime;
            public float maxFadeTime;
            public bool isFlash;
            public bool expired;
            public float originalFreq;
            

            public ColourInfo() {
                r = 255;
                g = 255;
                b = 255;
                alpha = 255;
                fadeFrequency = 3f;
                originalFreq = 3f;
                currentFadeTime = 0f;
                maxFadeTime = 100f;
                isFlash = false;
                expired = false;
            }
            
            public Color GetColor() {
                return new Color((byte)r, (byte)g, (byte)b, (byte)MathHelper.Clamp(alpha, 0, 255));
            } 
        }
    }
}
