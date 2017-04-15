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


        public MugenFont(String path, Vector2 position, int lineHeight = 0, int characterSpacing = 0, float scale = 1f) 
                :this(path, position, Vector2.Zero, lineHeight, characterSpacing, scale) {
            
        }

        public MugenFont(String path, Vector2 position, Vector2 offset, int lineHeight = 0, int characterSpacing = 0, float scale = 1f) {
            fontMap = new Dictionary<char, FontItem>();
            StreamReader file = new StreamReader(GameManager.GetContentManager().RootDirectory + "/" + path);
            fontSprite = GameManager.GetContentManager().Load<Texture2D>(path.Replace(".xFont", ""));

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

        public void Draw(String text) {
            Draw(text, this.position);
        }

        public void Draw(String text, Vector2 otherPosition) {
            position = otherPosition;
            position.X = position.X + offset.X;
            position.Y = position.Y + offset.Y;
            Vector2 nextPos = position;

            foreach (char c in text) {
                if (c != ' ' && c != '\n') {
                    if (fontMap.ContainsKey(c)) {  
                        FontItem item = fontMap[c]; 
                        GameManager.GetSpriteBatch().Draw(fontSprite, nextPos, item.rect, Color.White * alpha, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                        nextPos.X += (item.width + characterSpacing) * scale;
                    }
                } else if (c == '\n') {
                    nextPos.X = this.position.X;
                    nextPos.Y += (fontSprite.Height + lineHeight) * scale;
                } else if (c == ' ') {
                    nextPos.X += newSpacing * scale;
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
