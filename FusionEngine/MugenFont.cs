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
        private int height;
        private int lineHeight;
        private int lineWidth;
        private int lineSpace;
        private float scale;


        public MugenFont(String path, Vector2 position, int lineHeight = 1, int lineWidth = 1, float scale = 1f) {
            fontMap = new Dictionary<char, FontItem>();
            StreamReader file = new StreamReader(System.contentManager.RootDirectory + "/" + path);
            fontSprite = System.contentManager.Load<Texture2D>(path.Replace(".xFont", ""));

            height = fontSprite.Height;
            this.lineHeight = lineHeight;
            this.lineWidth = lineWidth;
            lineSpace = height;
            this.scale = scale;

            this.position = position;
            string line;

            while ((line = file.ReadLine()) != null) {
                if (line.StartsWith(";") || line.StartsWith("[")) continue;

                List<String> items = line.Split(' ').ToList();
                FontItem font = new FontItem();
                
                font.character = items[0].Trim()[0];
                font.startX = int.Parse(items[1].Trim());
                font.width = int.Parse(items[2].Trim());
                font.rect = new Rectangle(font.startX, 0, font.width, height);

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

        public void Draw(String text) {
            Vector2 nextPos = position;

            foreach (char c in text) {

                if (c != ' ' && c != '\n') {
                    if (fontMap.ContainsKey(c)) {  
                        FontItem item = fontMap[c]; 
                        System.spriteBatch.Draw(fontSprite, nextPos, item.rect, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                        nextPos.X += (item.width + lineWidth) * scale;
                    }
                } else if (c == '\n') {

                    nextPos.X = position.X;
                    nextPos.Y += (height + lineHeight) * scale;

                } else if (c == ' ') {
                    
                    nextPos.X += lineSpace * scale;
                }

                //startPos
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
