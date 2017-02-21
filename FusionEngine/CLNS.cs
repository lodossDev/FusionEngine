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
            protected Vector2 offset;
            protected Color color;
            protected BoxType type;
            protected int frame;
            protected float zDepth;

            public BoundingBox(BoxType type, int w, int h, int x, int y, int frame = -1) {
                SetSpriteColor(type);
                rect = new Rectangle(0, 0, w, h);
                offset = new Vector2(x, y);
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

                sprite = new Texture2D(System.graphicsDevice, 1, 1);
                Color[] colorData = new Color[1];
                colorData[0] = color;
                sprite.SetData<Color>(colorData);
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

            public void Update(GameTime gameTime, Entity entity) {
                
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

            public bool Intersects(CLNS.BoundingBox box) {
                return this.GetRect().Intersects(box.GetRect());
            }

            public Vector2 GetOffset() {
                return offset;
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
                    DrawStraightLine(new Vector2((int)rect.X, (int)rect.Y), new Vector2((int)rect.X + rect.Width, (int)rect.Y), sprite, color, THICKNESS); //top bar 
                    DrawStraightLine(new Vector2((int)rect.X, (int)rect.Y + rect.Height), new Vector2((int)rect.X + rect.Width + 1 * THICKNESS, (int)rect.Y + rect.Height), sprite, color, THICKNESS); //bottom bar 
                    DrawStraightLine(new Vector2((int)rect.X, (int)rect.Y), new Vector2((int)rect.X, (int)rect.Y + rect.Height), sprite, color, THICKNESS); //left bar 
                    DrawStraightLine(new Vector2((int)rect.X + rect.Width, (int)rect.Y), new Vector2((int)rect.X + rect.Width, (int)rect.Y + rect.Height), sprite, color, THICKNESS); //right bar 
                }
                
                if (drawType == DrawType.FILL) {
                    System.spriteBatch.Draw(sprite, new Vector2((float)rect.X, (float)rect.Y), rect, color * VISIBILITY, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.0f);
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

                System.spriteBatch.Draw(tex, rect, color);
            }
        }

        public class BoundsBox : BoundingBox {
            public BoundsBox(int w, int h, int x, int y, int depth) : base(BoxType.BOUNDS_BOX, w, h, x, y) {
                SetZdepth(depth);
            }
        }

        public class AttackBox : BoundingBox {
            public enum AttackPosition { STANDING, LOW, AIR, NONE };
            public enum BlockPosition { HI, LOW, AIR, NONE };
            public enum SettingType { ALL, FRAME, ONCE }

            private float hitPauseTime;
            private float painTime;
            private int hitDamage;
            private int hitPoints;
            private float hitStrength;
            private int comboStep;
            private int juggleCost;
            private AttackPosition attackPosition;
            private BlockPosition blockPosition;
            private SettingType settingType;
            private Effect.State sparkState;
            private Vector2 sparkOffset;

            public AttackBox(int w, int h, int x, int y, float zDepth = 30, float hitPauseTime = 1 / 60, 
                                        float painTime = 20 / 60, int hitDamage = 5,
                                        int hitPoints = 5, float hitStrength = 0.4f, int comboStep = 1,
                                        int juggleCost = 0, AttackPosition attackPosiiton = AttackPosition.NONE,
                                        BlockPosition blockPosition = BlockPosition.NONE,
                                        SettingType settingType = SettingType.ALL,
                                        Effect.State sparkState = Effect.State.NONE,
                                        float sparkX = 0, float sparkY = 0)
                                    : base(BoxType.HIT_BOX, w, h, x, y) {

                sparkOffset = Vector2.Zero;

                SetZdepth(zDepth);
                SetHitPauseTime(hitPauseTime);
                SetPainTime(painTime);
                SetHitDamage(hitDamage);
                SetHitPoints(hitPoints);
                SetHitStrength(hitStrength);
                SetComboStep(comboStep);
                SetJuggleCost(juggleCost);
                SetAttackPosition(attackPosition);
                SetSettingType(settingType);
                SetSparkState(sparkState);
                SetSparkOffset(sparkX, sparkY);
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

            public void SetAttackPosition(AttackPosition position) {
                attackPosition = position;
            }

            public void SetBlockPosition(BlockPosition position) {
                blockPosition = position;
            }

            public void SetSettingType(SettingType sparkRenderFrame) {
                this.settingType = sparkRenderFrame;
            }

            public void SetSparkState(Effect.State sparkState) {
                this.sparkState = sparkState;
            }

            public void SetSparkOffset(float x1, float y1) {
                sparkOffset.X = x1;
                sparkOffset.Y = y1;
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

            public AttackPosition GetAttackPosition() {
                return attackPosition;
            }

            public BlockPosition GetBlockPosition() {
                return blockPosition;
            }

            public SettingType GetSettingType() {
                return settingType;
            }

            public Effect.State GetSparkState() {
                return sparkState;
            }

            public Vector2 GetSparkOffset() {
                return sparkOffset;
            }
        }
    }
}
