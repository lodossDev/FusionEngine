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

    public class Font {
        private Texture2D fontSprite;
        private Dictionary<char, FontItem> fontMap;
        private Dictionary<char, int> fontPosMap;

        public Font(String path) {
            fontMap = new Dictionary<char, FontItem>();
            fontPosMap = new Dictionary<char, int>();

            StreamReader file = new StreamReader(System.contentManager.RootDirectory + "/" + path);
            fontSprite = System.contentManager.Load<Texture2D>(path.Replace(".xFont", ""));

            string line;

            int i = 0;

            while ((line = file.ReadLine()) != null) {
                if (line.StartsWith(";") || line.StartsWith("[")) continue;
                Debug.WriteLine(line);

                List<String> items = line.Split(' ').ToList();
                FontItem font = new FontItem();
                
                font.character = items[0].Trim()[0];
                font.startX = int.Parse(items[1].Trim());
                font.width = int.Parse(items[2].Trim());

                fontMap.Add(font.character, font);
                fontPosMap.Add(font.character, i);
                i++;
            }

            file.Close();
        }

        public Texture2D GetTexture() {
            return fontSprite;
        }

        public Dictionary<char, FontItem> GetFontMap() {
            return fontMap;
        }

        public Dictionary<char, int> GetPosMap() {
            return fontPosMap;
        }

        public static void Draw(Font font, String text, Vector2 pos) {

            Debug.WriteLine("HAS H: " + font.GetFontMap().ContainsKey("H"[0]));

            foreach (char c in font.GetFontMap().Keys) {
                Debug.WriteLine("ITEMS: " + c);
            }

            foreach (char c in text) {
                Debug.WriteLine("CHAR: " + c);
                //if (!font.GetFontMap().ContainsKey(c)) continue;
                int index = font.GetPosMap()[c];
                Debug.WriteLine("INDEX: " + index);
                Vector2 ss = pos;
                ss.X += 50 * 3f;
                pos = ss;
                FontItem item = font.GetFontMap()[c]; 
                System.spriteBatch.Draw(font.GetTexture(), ss, new Rectangle(item.startX, 0, 8, 9), Color.White, 0, new Vector2(0, 0), 3f, SpriteEffects.None, 0f);
                //break;
            }
        }

        public class FontItem {
            public int startX;
            public int width;
            public char character;
        }
    }
}
