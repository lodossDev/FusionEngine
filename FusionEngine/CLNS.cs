using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FusionEngine
{
    public class CLNS {
        public static float VISIBILITY = 0.4f;
        public static int THICKNESS = 2;
        public enum BoxType { HIT_BOX, BODY_BOX, BOUNDS_BOX, RAY_BOX, DEPTH_BOX }
        public enum DrawType { LINES, FILL }

        public class BoundingBox {
            protected Texture2D sprite;
            protected Rectangle rect;
            private Rectangle baseRect;
            protected Vector2 offset;
            private Vector2 baseOffset;

            protected Color color;
            protected BoxType type;
            protected int frame;
            protected float zDepth;
            protected float visibility;

            public BoundingBox(BoxType type, int w, int h, int x, int y, int frame = -1) {
                SetSpriteColor(type);
                rect = baseRect = new Rectangle(0, 0, w, h);
                offset = baseOffset = new Vector2(x, y);
                visibility = 100f;

                if (frame == -1) {
                    this.frame = -1;
                } else {
                    this.frame = frame - 1;
                }
            }

            public void SetVisibility(float amount) {
                visibility = amount;
            }

            private void SetSpriteColor(BoxType type) {
                this.type = type;

                switch (this.type) {
                    case BoxType.BODY_BOX: { 
                        color = Color.Blue;
                        break;
                    }

                    case BoxType.HIT_BOX: { 
                        color = Color.Red;
                        break;
                    }

                    case BoxType.BOUNDS_BOX: { 
                        color = Color.Yellow;
                        break;
                    }

                    case BoxType.DEPTH_BOX: { 
                        color = Color.Orange;
                        break;
                    }

                    default: { 
                        color = Color.ForestGreen;
                        break;
                    }
                }

                sprite = new Texture2D(GameManager.GetGraphicsDevice(), 1, 1);
                Color[] colorData = new Color[1];
                colorData[0] = color;
                sprite.SetData<Color>(colorData);
            }

            public void SetBaseColor(Color color) {
                this.color = color;
            }

            public void SetFrame(int frame) {
                this.frame = frame - 1;
            }

            public void SetOffSet(float x, float y) {
                offset.X = x;
                offset.Y = y;
            }

            public void SetOffSetX(float x) {
                offset.X = x;
            }

            public void SetOffSetY(float y) {
                offset.Y = y;
            }

            public void SetZdepth(float depth) {
                zDepth = depth;
            }

            public void SetRectX(int x) {
                rect.X = x;
            }

            public void SetRectY(int y) {
                rect.Y = y;
            }

            public void SetRectWidth(int width) {
                rect.Width = width;
            }

            public void SetRectHeight(int height) {
                rect.Height = height;
            }

            public void SetBaseWidth(int w) {
                baseRect.Width = w;
            }

            public void SetBaseHeight(int h) {
                baseRect.Height = h;
            }

            public void SetBaseOffX(int x) {
                baseOffset.X = x;
            }

            public void SetBaseOffY(int y) {
                baseOffset.Y = y;
            }

            public void SetBase(int w, int h, int x, int y) {
                baseRect.Width = w;
                baseRect.Height = h;

                baseOffset.X = x;
                baseOffset.Y = y;
            }

            public void Update(GameTime gameTime, Entity entity) {
                float diffX = ((entity.GetScaleX() - entity.GetBaseScaleX()) / entity.GetBaseScaleX());
                float diffY = ((entity.GetScaleY() - entity.GetBaseScaleY()) / entity.GetBaseScaleY());

                rect.Width = baseRect.Width + (int)Math.Round(baseRect.Width * diffX);
                rect.Height = baseRect.Height + (int)Math.Round(baseRect.Height * diffY);

                offset.X = baseOffset.X + (baseOffset.X * diffX);
                offset.Y = baseOffset.Y + (baseOffset.Y * diffY);

                if (entity.IsLeft()) {
                    rect.X = (int)(entity.GetPosX() - (rect.Width + offset.X - 5));
                } else {
                    rect.X = (int)(entity.GetPosX() + offset.X);
                }

                if (type == BoxType.DEPTH_BOX) {
                    rect.Y = (int)(entity.GetPosZ() + offset.Y);
                } else {
                    rect.Y = (int)(entity.GetPosY() + entity.GetPosZ() + offset.Y);
                } 
            }

            public Texture2D GetSprite() {
                return sprite;
            }

            public Rectangle GetRect() {
                return rect;
            }

            public Rectangle GetBaseRect() {
                return baseRect;
            }

            public bool Intersects(CLNS.BoundingBox box) {
                return this.GetRect().Intersects(box.GetRect());
            }

            public Vector2 GetOffset() {
                return offset;
            }

            public Vector2 GetBaseOffset() {
                return baseOffset;
            }

            public Color GetColor() {
                return color;
            }

            public BoxType GetBoxType() {
                return type;
            }

            public int GetFrame() {
                return frame;
            }

            public int GetWidth() {
                return rect.Width;
            }

            public int GetHeight() {
                return rect.Height;
            }

            public float GetZdepth() {
                return zDepth;
            }

            public virtual void DrawRectangle(DrawType drawType) {
                if (drawType == DrawType.LINES || drawType == DrawType.FILL) {
                    DrawStraightLine(new Vector2((int)rect.X, (int)rect.Y), new Vector2((int)rect.X + rect.Width, (int)rect.Y), sprite, color * visibility, THICKNESS + 1); //top bar 
                    DrawStraightLine(new Vector2((int)rect.X, (int)rect.Y + rect.Height), new Vector2((int)rect.X + rect.Width + 1 * THICKNESS, (int)rect.Y + rect.Height), sprite, color * visibility, THICKNESS + 1); //bottom bar 
                    DrawStraightLine(new Vector2((int)rect.X, (int)rect.Y), new Vector2((int)rect.X, (int)rect.Y + rect.Height), sprite, color * visibility, THICKNESS + 1); //left bar 
                    DrawStraightLine(new Vector2((int)rect.X + rect.Width, (int)rect.Y), new Vector2((int)rect.X + rect.Width, (int)rect.Y + rect.Height), sprite, color * visibility, THICKNESS + 1); //right bar 
                }
                
                if (drawType == DrawType.FILL) {
                    GameManager.GetSpriteBatch().Draw(sprite, new Vector2((float)rect.X, (float)rect.Y), rect, color * VISIBILITY * visibility, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.0f);
                }
            }

            //draws a line (rectangle of thickness) from A to B.  A and B have make either horiz or vert line. 
            public static void DrawStraightLine(Vector2 A, Vector2 B, Texture2D tex, Color color, int thickness) {
                Rectangle rect;

                if (A.X < B.X) {
                    rect = new Rectangle((int)A.X, (int)A.Y, (int)(B.X - A.X), thickness);
                } else {
                    rect = new Rectangle((int)A.X, (int)A.Y, thickness, (int)(B.Y - A.Y));
                }

                GameManager.GetSpriteBatch().Draw(tex, rect, color);
            }
        }

        public class BoundsBox : BoundingBox {
            public BoundsBox(int w, int h, int x, int y, int depth) : base(BoxType.BOUNDS_BOX, w, h, x, y) {
                SetZdepth(depth);
            }
        }

        public class AttackBox : BoundingBox {
            public enum State { STANDING, LOW, AIR, NONE };
            public enum HitType { ALL, FRAME, ONCE }
            public enum AttackType { LIGHT, MEDIUM, HEAVY }

            private float hitPauseTime;
            private float painTime;
            private int hitDamage;
            private int hitPoints;
            private float hitStrength;
            private float tossHeight;
            private float moveX;
            private int comboStep;
            private int juggleCost;
            private State attackPosition;
            private State blockPosition;
            private AttackType attackType;
            private HitType hitType;
            private Effect.State sparkState;
            private Vector2 sparkOffset;

            public AttackBox(int w, int h, int x, int y, float zDepth = 30, float hitPauseTime = 1 / 60, 
                                        float painTime = 20 / 60, int hitDamage = 1, int hitPoints = 5, float hitStrength = 0.4f, 
                                        int comboStep = 1, int juggleCost = 0, AttackType attackType = AttackType.LIGHT,
                                        State attackPosiiton = State.NONE, State blockPosition = State.NONE,
                                        HitType hitType = HitType.ALL, Effect.State sparkState = Effect.State.NONE,
                                        float sparkX = 0, float sparkY = 0, float moveX = 5, float tossHeight = 0)

                                    : base(BoxType.HIT_BOX, w, h, x, y) {

                sparkOffset = Vector2.Zero;

                SetAttack(zDepth, hitPauseTime, painTime, hitDamage, hitPoints, hitStrength, comboStep, 
                                juggleCost, attackType, attackPosition, blockPosition, hitType, sparkState, sparkX, sparkY);
            }

            public void SetAttack(float zDepth = 30, float hitPauseTime = 1 / 60, 
                                        float painTime = 20 / 60, int hitDamage = 5,
                                        int hitPoints = 5, float hitStrength = 0.4f, int comboStep = 1,
                                        int juggleCost = 0, AttackType attackType = AttackType.LIGHT,
                                        State attackPosiiton = State.NONE, State blockPosition = State.NONE,
                                        HitType hitType = HitType.ALL, Effect.State sparkState = Effect.State.NONE,
                                        float sparkX = 0, float sparkY = 0, float moveX = 5, float tossHeight = 0) {

                SetZdepth(zDepth);
                SetHitPauseTime(hitPauseTime);
                SetPainTime(painTime);
                SetHitDamage(hitDamage);
                SetHitPoints(hitPoints);
                SetHitStrength(hitStrength);
                SetComboStep(comboStep);
                SetJuggleCost(juggleCost);
                SetAttackPosition(attackPosition);
                SetHitType(hitType);
                SetSparkState(sparkState);
                SetSparkOffset(sparkX, sparkY);
                SetMoveX(moveX);
                SetTossHeight(tossHeight);
            }

            public void SetHitPauseTime(float pauseTime) {
                hitPauseTime = pauseTime;
            }

            public void SetPainTime(float painTime) {
                this.painTime = painTime;
            }

            public void SetHitDamage(int damage) {
                hitDamage = damage;
            }

            public void SetHitPoints(int points) {
                hitPoints = points;
            }

            public void SetHitStrength(float strength) {
                hitStrength = strength;
            }

            public void SetComboStep(int step) {
                comboStep = step;
            }

            public void SetJuggleCost(int cost) {
                juggleCost = cost;
            }

            public void SetAttackType(AttackType attackType) {
                this.attackType = attackType;
            }

            public void SetAttackPosition(State position) {
                attackPosition = position;
            }

            public void SetBlockPosition(State position) {
                blockPosition = position;
            }

            public void SetHitType(HitType hitType) {
                this.hitType = hitType;
            }

            public void SetSparkState(Effect.State sparkState) {
                this.sparkState = sparkState;
            }

            public void SetSparkOffset(float x1, float y1) {
                sparkOffset.X = x1;
                sparkOffset.Y = y1;
            }

            public void SetTossHeight(float height) {
                tossHeight = height;
            }

             public void SetMoveX(float x) {
                moveX = x;
            }

            public AttackType GetAttackType() {
                return attackType;
            }

            public float GetHitPauseTime() {
                return hitPauseTime;
            }

            public float GetPainTime() {
                return painTime;
            }

            public int GetHitDamage() {
                return hitDamage;
            }

            public int GetHitPoints() {
                return hitPoints;
            }

            public float GetHitStrength() {
                return hitStrength;
            }

            public int GetComboStep() {
                return comboStep;
            }

            public int GetJuggleCost() {
                return juggleCost;
            }

            public State GetAttackPosition() {
                return attackPosition;
            }

            public State GetBlockPosition() {
                return blockPosition;
            }

            public HitType GetHitType() {
                return hitType;
            }

            public Effect.State GetSparkState() {
                return sparkState;
            }

            public Vector2 GetSparkOffset() {
                return sparkOffset;
            }

            public float GetTossHeight() {
                return tossHeight;
            }

            public float GetMoveX() {
                return moveX;
            }
        }
    }
}
