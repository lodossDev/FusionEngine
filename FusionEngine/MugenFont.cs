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
        private Vector2 startPos;
        private int height;
        private int lineHeight;

        public MugenFont(String path) {
            fontMap = new Dictionary<char, FontItem>();
            StreamReader file = new StreamReader(System.contentManager.RootDirectory + "/" + path);
            fontSprite = System.contentManager.Load<Texture2D>(path.Replace(".xFont", ""));
            height = fontSprite.Height;
            lineHeight = 8;

            string line;

            while ((line = file.ReadLine()) != null) {
                if (line.StartsWith(";") || line.StartsWith("[")) continue;
                Debug.WriteLine(line);

                List<String> items = line.Split(' ').ToList();
                FontItem font = new FontItem();
                
                font.character = items[0].Trim()[0];
                font.startX = int.Parse(items[1].Trim());
                font.width = int.Parse(items[2].Trim());

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

        public void Draw(String text, Vector2 pos, float scale = 1f) {
            if (startPos == Vector2.Zero) {
                startPos = pos;
            }

            Vector2 nextPos = pos;
            Debug.WriteLine("SPACE: " + "\n"[0]);

            foreach (char c in fontMap.Keys) {
                Debug.WriteLine("ITEMS: " + c);
            }

            int lastX = 0;

            foreach (char c in text) {
                Debug.WriteLine("CURRENT: " + c);

                if (c != ' ' && c != '\n') { 
                    FontItem item = GetFontMap()[c]; 
                    System.spriteBatch.Draw(fontSprite, nextPos, new Rectangle(item.startX, 0, item.width, height), Color.White, 0, new Vector2(0, 0), scale, SpriteEffects.None, 0f);
                    nextPos.X += (item.width + 1) * scale;
                } else if (c == '\n') {

                    nextPos.X = startPos.X;
                    nextPos.Y += (height + lineHeight) * scale;

                } else if (c == ' ') {
                    lastX = (int)nextPos.X;
                    nextPos.X += 10 * scale;
                    Debug.WriteLine("LAST SPACE X: " + (nextPos.X - lastX));
                }

                pos = nextPos;
            }
        }

        public class FontItem {
            public int startX;
            public int width;
            public char character;
        }
    }
}
