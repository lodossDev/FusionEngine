using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FusionEngine {

    public class MugenFont {
        private Texture2D fontSprite;
        private Dictionary<char, FontItem> fontMap;
        private Vector2 position;
        private Vector2 offset;
        private int lineHeight;
        private int characterSpacing;
        private int newSpacing;
        private float scale;
        private float alpha;

        public bool isTransForward;
        public bool isTransBack;
        private Vector2 transVel;
        private Vector2 transPos;
        public int transTime;
        private int maxTransTime;


        public MugenFont(String path, int lineHeight = 0, int characterSpacing = 0, float scale = 1f) 
                :this(path, Vector2.Zero, Vector2.Zero, lineHeight, characterSpacing, scale) {
            
        }

        public MugenFont(String path, Vector2 position, int lineHeight = 0, int characterSpacing = 0, float scale = 1f) 
                :this(path, position, Vector2.Zero, lineHeight, characterSpacing, scale) {
            
        }

        public MugenFont(String path, Vector2 position, Vector2 offset, int lineHeight = 0, int characterSpacing = 0, float scale = 1f) {
            isTransForward = false;
            isTransBack = false;
            transTime = 0;
            maxTransTime = 100;
            transPos = Vector2.Zero;
            transVel = Vector2.Zero;

            fontMap = new Dictionary<char, FontItem>();
            StreamReader file = new StreamReader(GameManager.ContentManager.RootDirectory + "/" + path);
            fontSprite = GameManager.ContentManager.Load<Texture2D>(path.Replace(".xFont", ""));

            this.lineHeight = lineHeight;
            this.characterSpacing = characterSpacing;
            this.newSpacing = fontSprite.Height / 2;
            this.scale = scale;
            this.alpha = 1f;

            this.position = position;
            this.offset = offset;
            string line;

            while ((line = file.ReadLine()) != null) {
                if (line.StartsWith(";") || line.StartsWith("[")) continue;

                List<String> items = line.Split(' ').ToList();
                FontItem font = new FontItem();
                
                font.character = items[0].Trim()[0];
                font.startX = int.Parse(items[1].Trim());
                font.width = int.Parse(items[2].Trim());
                font.rect = new Rectangle(font.startX, 0, font.width, fontSprite.Height);

                fontMap.Add(font.character, font);
            }

            file.Close();
        }

        public Texture2D GetTexture() {
            return fontSprite;
        }

        public Dictionary<char, FontItem> GetFontMap() {
            return fontMap;
        }

        public int GetLineHeight() {
            return lineHeight;
        }

        public int GetCharacterSpacing() {
            return characterSpacing;
        }

        public int GetNewSpacing() {
            return newSpacing;
        }

        public float GetScale() {
            return scale;
        }

        public void SetScale(float scale) {
            this.scale = scale;
        }

        public void SetLineHeight(int height) {
            lineHeight = height;
        }

        public void SetCharacterSpacing(int spacing) {
            characterSpacing = spacing;
        }

        public void SetNewSpacing(int space) {
            newSpacing = space;
        }

        public float GetPosX() {
            return position.X;
        }

        public float GetPosY() {
            return position.Y;
        }

        public float GetOffsetX() {
            return offset.X;
        }

        public float GetOffsetY() {
            return offset.Y;
        }

        public void SetPosX(float x) {
            position.X = x;
        }

        public void SetPosY(float y) {
            position.Y = y;
        }

        public void MoveX(float vel) {
            position.X += vel;
        }

        public void MoveY(float vel) {
            position.Y += vel;
        }

        public void SetAlpha(float a) {
            alpha = a;
        }

        public void Translate(float x, float y, float vx, float vy, int time) {
            isTransForward = true;
            isTransBack = false;

            transPos.X = x;
            transPos.Y = y;
            transVel.X = vx;
            transVel.Y = vy;
            maxTransTime = time;
            transTime = 0;
        }

        public void Flash(GameTime gameTime, float a) {
            alpha -= a * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (alpha < 0.0) {
                alpha = 1f;
            }
        }

        public void Draw(String text) {
            Draw(text, this.position);
        }
        
        public void Draw(String text, Vector2 otherPosition) {
            position = otherPosition;
            position.X = position.X + offset.X;
            position.Y = position.Y + offset.Y;
            Vector2 nextPos = position;

            if (isTransForward) {
                transTime ++;

                if (transTime > maxTransTime) {
                    transTime = 0;
                    isTransForward = false;
                    isTransBack = true;
                }

                if (nextPos.X < transPos.X) {
                    MoveX(transVel.X);
                }
            }

            if (isTransBack) {
                if (nextPos.X > -400) {
                     MoveX(-transVel.X);
                } else {
                    isTransBack = false;
                }
            }

            foreach (char c in text) {
                if (c != ' ' && c != '\n') {
                    if (fontMap.ContainsKey(c)) {  
                        FontItem item = fontMap[c]; 
                        GameManager.SpriteBatch.Draw(fontSprite, nextPos, item.rect, Color.White * alpha, 0f, Vector2.Zero, this.scale, SpriteEffects.None, 0f);
                        nextPos.X += (item.width + characterSpacing) * this.scale;
                    }
                } else if (c == '\n') {
                    nextPos.X = this.position.X;
                    nextPos.Y += (fontSprite.Height + lineHeight) * this.scale;
                } else if (c == ' ') {
                    nextPos.X += newSpacing * this.scale;
                }
            }
        }

        public class FontItem {
            public int startX;
            public int width;
            public char character;
            public Rectangle rect;
        }
    }
}
